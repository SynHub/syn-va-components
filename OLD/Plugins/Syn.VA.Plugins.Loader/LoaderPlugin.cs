using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using Syn.Bot.Siml;
using Syn.Bot.Utility;
using Syn.EmotionML;
using Syn.Log;
using Syn.Utility;
using Syn.VA.Collections;
using Syn.VA.Controls;
using Syn.VA.Interaction;
using Syn.VA.Interfaces;
using Syn.VA.Plugins.Loader.Adapter;
using Syn.VA.Plugins.Loader.View;
using Syn.VA.Plugins.Loader.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Loader
{
    //This is the only Plugin that is loaded by Syn Virtual Assistant.
    //This Plugin is responsible for loading all other plugins and to provide a Bot architecture to the Virtual Assistant.
    public class LoaderPlugin : Plugin
    {
        public static SimlFileManager SimlFileManager { get; set; }
        public static string UserSettingsFileId => "e9f4b72b-a052-4e08-a4d5-8c4ecaeac89c";
        public static string BotSettingsFileId => "814a14f9-98f6-478f-a1ee-5c41315fffbc";

        public string LearnedFilePath => Path.Combine(VA.SettingsManager["VA"]["Bot-Directory"].Value, "Assistant", "Settings", "Learned.siml");
        public string MemorizedFilePath => Path.Combine(VA.SettingsManager["VA"]["Bot-Directory"].Value, "Assistant", "Settings", "Memorized.siml");

        public LoaderPlugin()
        {
            try
            {
                Bot = SimlBot.Instance;
                VA.Components.Add(Bot);

                //Perform tick based on Virtual Assistant timer.
                VA.Timer.Tick += (sender, args) =>
                {
                    Bot.Timer.PerformTick();
                };

                SimlBot.Logger.LogReceived += (sender, args) =>
                {
                    VirtualAssistant.Instance.Logger.Log(args.Level, args.Message);
                };

                var vaSettings = VA.SettingsManager["VA"];

                var simlSettingsPath = Path.Combine(vaSettings["Bot-Directory"].Value, "Commands", "Settings");
                SimlFileManager = new SimlFileManager(simlSettingsPath);

                //When the VA is closing, save both MainUser and Bot settings to files.
                VA.Closing += (sender, eventArgs) =>
                {
                    SimlFileManager.SaveDocument(UserSettingsFileId, Bot.MainUser.Settings.GetDocument());
                    SimlFileManager.SaveDocument(BotSettingsFileId, Bot.Settings.GetDocument());
                };

                Bot.Learning += (sender, learningEventArgs) =>
                {
                    SaveLearned(LearnedFilePath, learningEventArgs.Document);
                };

                Bot.Memorizing += (sender, memorizingEventArgs) =>
                {
                    SaveLearned(MemorizedFilePath, memorizingEventArgs.Document);
                };

                VirtualAssistant.Instance.Logger.LogReceived += (sender, args) =>
                {
                    //Trigger the 'Error-Logged' event only if the level is greater than or equal to 'Warn'
                    if (args.Level >= LogLevel.Warn)
                    {
                        Bot.Trigger("Error-Logged");
                    }
                };

                VA.Architecture = "SIML";
                //Whenever the VA receives a message, the Bot generates a response.
                VA.Interaction.MessageReceived += (sender, args) =>
                {
                    if (args.Handled) return;

                    //Message type is UserMessage
                    if (args.Message.Type == MessageType.UserMessage)
                    {
                        ChatResult result;
                        if (args.Message.User.IsMain)
                        {
                            result = Bot.Chat(args.Message.Text);
                        }
                        else
                        {
                            result = Bot.Chat(args.Message.Text, Bot.CreateUser(args.Message.User.ID));
                        }
                        var response = new Response
                        {
                            Text = result.BotMessage,
                            Hint = result.Hint,
                            Rank = result.LastResponse.Rank,
                            User = args.Message.User
                        };

                        args.Handled = true;
                        VA.Interaction.Respond(response);
                    }
                    //Message is an Event message
                    else if (args.Message.Type == MessageType.EventMessage)
                    {
                        if (args.Message.User.IsMain)
                        {
                            Bot.Trigger(args.Message.Text);
                        }
                        else
                        {
                            Bot.Trigger(args.Message.Text, Bot.CreateUser(args.Message.User.ID));
                        }
                        args.Handled = true;
                    }
                };

                //Whenever the VA receives a textual response, use speech synthesis.
                VA.Interaction.ResponseReceived += (sender, args) =>
                {
                    if (!args.Response.User.IsMain) return;
                    var rank = Enum.Parse(typeof(ResponseRank), VA.SettingsManager["Speech"]["Speech-Rank"].Value);
                    if (args.Response.Rank >= (ResponseRank)rank)
                    {
                        VA.Speech.Synthesizers.Current?.Speak(args.Response.Text);
                    }
                };

                //Whenever the MainUser receives an Event Message, send a response to the VA.
                Bot.MainUser.ResponseReceived += (sender, args) =>
                {
                    var response = new Response
                    {
                        Text = args.Result.BotMessage,
                        Hint = args.Result.Hint,
                        Rank = args.Result.LastResponse.Rank
                    };
                    VA.Interaction.Respond(response);
                };


                Emotion currentEmotion = null;

                //When the Bot emotion changes set the currentEmotion.
                Bot.EmotionChanged += (sender, args) => { currentEmotion = args.Current; };

                //Whenver the current SpeechRecognizer is changed.
                VA.Speech.Recognizers.RecognizerChanged += (sender, args) =>
                {
                    args.Recognizer.SpeechRecognized += (o, eventArgs) =>
                    {
                        var defaultConfidence = 0.5d;
                        if (VA.SettingsManager.Contains("Speech"))
                        {
                            var speechSettings = VA.SettingsManager["Speech"];
                            if (speechSettings.Contains("Speech-Confidence"))
                            {
                                var confidence = speechSettings["Speech-Confidence"].Value;
                                double outValue;
                                if (double.TryParse(confidence, out outValue))
                                {
                                    defaultConfidence = outValue;
                                }
                            }
                        }
                        if (eventArgs.Confidence >= defaultConfidence)
                        {
                            VA.Interaction.SendMessage(eventArgs.Text);
                        }
                    };
                };

                //Whenever the Current Speech Synthesizer is changed.
                VA.Speech.Synthesizers.SynthesizerChanged += (sender, args) =>
                {
                    args.Synthesizer.VisemeReached += (sender1, visemeArrivedEventArgs) =>
                    {
                        VA.Avatar.Morph(visemeArrivedEventArgs.Viseme.ToString(), visemeArrivedEventArgs.Duration);
                    };

                    //If currentEmotion is not null then at the end of speech switch to expression.
                    VA.Speech.Synthesizers.Current.SpeakCompleted += (sender1, args1) =>
                    {
                        try
                        {
                            if (currentEmotion == null) return;
                            var emotionElement = SynUtility.Xml.RemoveAllNamespaces(currentEmotion.GetElement());
                            var durationAttribute = emotionElement.Attribute("duration");
                            var infoElement = emotionElement.Element("info");
                            var duration = 2000;
                            if (durationAttribute != null)
                            {
                                duration = Convert.ToInt32(durationAttribute.Value);
                            }
                            var simlEmotion = infoElement?.Element("Emotion");
                            var expressionAttribute = simlEmotion?.Attribute("Expression");
                            if (expressionAttribute != null)
                            {
                                VA.Avatar.Morph(expressionAttribute.Value, TimeSpan.FromMilliseconds(duration));
                            }
                            //Reset currentEmotion to null.
                            currentEmotion = null;
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };
                };

                Project = new ProjectManager();

                //Path to the functional Commands knowledge-base.
                var commandsSimlProject = Path.Combine(vaSettings["Bot-Directory"].Value, "Commands.simlproj");

                //Path to the General chat knowledge-base.
                var generalSimlProject = Path.Combine(vaSettings["Bot-Directory"].Value, "General.simlproj");

                if (File.Exists(commandsSimlProject)) { Project.LoadFromFile(commandsSimlProject); }
                if (File.Exists(generalSimlProject)) { Project.LoadFromFile(generalSimlProject); }

                var externalBotDirectory = vaSettings["External-Bot-Directory"].Value;
                var externalPluginsDirectory = vaSettings["External-Plugins-Directory"].Value;

                //if External-Bot-Directory is set then load external Bot Project.
                if (!string.IsNullOrEmpty(externalBotDirectory))
                {
                    LoadProjectFromDirectory(externalBotDirectory);
                }

                // Load Knowledge-Base into Bot.
                //All projects are loaded at this point.
                Project.LoadIntoBot(Bot);

                //Add the 'MainUser' Settings so the VA can work with User variables.
                VA.SettingsManager.Add(Bot.MainUser.Settings);

                //Create textual choices.
                foreach (var item in Bot.Examples)
                {
                    VA.Interaction.Choices.Add(new ChoiceItem("Siml-Bot", item.Value));
                }

                //Load Important Plugins first.
                AddPlugin(typeof(BotPlugin));
                AddPlugin(typeof(UserPlugin));

                //Now load external Plugins.
                LoadPlugins(vaSettings["Plugins-Directory"].Value);

                //If 'External-Plugins-Directory' is set then load plugins from directory
                if (!string.IsNullOrEmpty(externalPluginsDirectory))
                {
                    LoadPlugins(externalPluginsDirectory);
                }

                VA.SettingsManager.SettingsSaved += (sender, args) =>
                {
                    var settingsWindow = VA.Components.Get<SettingsWindow>();
                    var settingsSavedMessage = !string.IsNullOrEmpty(args.Settings["Settings-Saved-Message"].Value) ? args.Settings["Settings-saved-message"].Value : Settings["Settings-Saved"].Value;
                    settingsWindow.DisplayMessage(args.Settings.Name, settingsSavedMessage);
                };

                VA.Timer.Tick += (sender, args) =>
                {
                    Bot.Timer.PerformTick();
                };

                //Trigger Greet-User event when the VA is ready.
                VA.Loaded += (sender, args) =>
                {
                    Bot.Trigger("Load-Time-Event");
                    Bot.Trigger("Greet-User");
                };

                //Reset all plugin settings if the VA is reset.
                VA.Resetting += (sender, args) =>
                {
                    ResetAllPlugins();
                };

                Bot.Adapters.Add(new CreditAdapter());
                Bot.Adapters.Add(new AgentAdapter());
                Bot.Adapters.Add(new TextWindowAdapter());
                Bot.Adapters.Add(new HelpWindowAdapter());

                AddWelcomeWidget();
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public void LoadProjectFromDirectory(string directory)
        {
            foreach (var projFile in Directory.GetFiles(directory, "*.simlproj"))
            {
                Project.LoadFromFile(projFile);
                //Load just first Project file found.
                break;
            }
        }

        public void SaveLearned(string filePath, XDocument currentDocument)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var parentDocument = XDocument.Load(filePath);
                    var parentRoot = parentDocument.Root;
                    var childRoot = currentDocument.Root;
                    if (parentRoot == null || childRoot == null) return;

                    var childElementList = childRoot.Elements();

                    foreach (var element in childElementList)
                    {
                        parentRoot.Add(element);
                    }
                    parentDocument.Save(filePath);
                }
                else
                {
                    currentDocument.Save(filePath);
                }
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Loader";

        public void LoadPlugins(string pluginDirectory)
        {
            if (!Directory.Exists(pluginDirectory)) return;
            //Search for Link libraries that begin with "Syn.VA.Plugins"
            VA.Logger.Info($"Searching directory '{pluginDirectory}' for Plugin files.");
            var pluginFiles = Directory.GetFiles(pluginDirectory, "Syn.VA.Plugins*.dll", SearchOption.AllDirectories);
            VA.Logger.Info($"Found \"{pluginFiles.Count()}\" Plugin files.");
            foreach (var filePath in pluginFiles)
            {
                var fileName = Path.GetFileName(filePath);
                if (fileName == null)
                {
                    VA.Logger.Info($"File name returned null for path: '{filePath}'.");
                    continue;
                }

                //Avoid loading Self.
                var skipName = $"Syn.VA.Plugins.{Name}";
                if (fileName.Contains(skipName))
                {
                    VA.Logger.Info($"Skipped loading: '{skipName}'.");
                    continue;
                }

                try
                {
                    VA.Logger.Info($"Loading Plugin assembly from path: '{filePath}'.");
                    var plugins = Assembly.LoadFrom(filePath);
                    foreach (var type in plugins.GetTypes())
                    {
                        if (type.GetInterfaces().Contains(typeof(IPlugin)) && type.IsPublic)
                        {
                            AddPlugin(type);
                        }
                    }
                }
                catch (Exception exception)
                {
                    VA.Logger.Error(exception);
                }
            }
        }

        public void AddWelcomeWidget()
        {
            var userControl = new WelcomePanelChart();
            var context = userControl.DataContext as WelcomePanelChartContext;
            if (context == null) return;

            context.SeriesCollection = new SeriesCollection
            {
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Bot.Stats.Vocabulary.Count) },
                    DataLabels = true,
                    Title = "Vocabulary"

                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Bot.Stats.ModelCount) },
                    DataLabels = true,
                    Title = "Models"
                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Bot.Stats.ConceptCount) },
                    DataLabels = true,
                    Title = "Concepts"
                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Bot.Stats.PublicConceptCount) },
                    DataLabels = true,
                    Title = "Public"
                },
                new PieSeries
                {
                    Values = new ChartValues<ObservableValue> { new ObservableValue(Bot.Stats.PrivateConceptCount) },
                    DataLabels = true,
                    Title = "Private"
                }
            };
            VA.Components.Add("Welcome-Widget", userControl);
        }

        public void AddPlugin(Type type)
        {
            VA.Logger.Info($"Loading Plugin class: '{type.Name}'");
            if (VA.SettingsManager["Components"].Contains("Disabled-Plugins"))
            {
                if (VA.SettingsManager["Components"]["Disabled-Plugins"].Contains(type.FullName))
                {
                    VA.Logger.Info($"Skipping disabled plugin: \"{type.Name}\"");
                    return;
                }
            }

            var plugin = Activator.CreateInstance(type) as IPlugin;
            VA.Plugins.Add(plugin);
            CheckPluginSettings(plugin);
            VA.Logger.Info($"Successfully loaded Plugin class: '{type.Name}'");
        }

        private void ResetAllPlugins()
        {
            try
            {
                foreach (var plugin in VA.Plugins)
                {
                    var settingsDirectory = VA.SettingsManager["VA"]["Settings-Directory"].Value;
                    var filePath = Path.Combine(settingsDirectory, $"{plugin.Name}.xml");
                    var settingsString = GetSettingsResource(plugin);
                    if (!string.IsNullOrEmpty(settingsString))
                    {
                        File.WriteAllText(filePath, settingsString);
                        VA.SettingsManager.LoadFromFile(filePath);
                    }
                }
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }  
        }

        private void CheckPluginSettings(IPlugin plugin)
        {
            try
            {
                var settingsDirectory = VA.SettingsManager["VA"]["Settings-Directory"].Value;
                var filePath = Path.Combine(settingsDirectory, $"{plugin.Name}.xml");
                if (!File.Exists(filePath))
                {
                    var settingsString = GetSettingsResource(plugin);
                    if (!string.IsNullOrEmpty(settingsString))
                    {
                        VA.Logger.Info($"Extracting and Loading Settings file of Plugin: '{plugin.Name}'");
                        File.WriteAllText(filePath, settingsString);
                        VA.SettingsManager.LoadFromFile(filePath);
                    }
                }
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        private static string GetSettingsResource(IPlugin plugin)
        {
            var result = string.Empty;
            var assembly = plugin.GetType().Assembly;
            var defaultNamespace = plugin.GetType().Namespace;
            var resourceName = $"{defaultNamespace}.{plugin.Name}Settings.xml";
            VirtualAssistant.Instance.Logger.Info($"Searching for embedded resource: '{resourceName}'");
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    VirtualAssistant.Instance.Logger.Info($"Embedded resource '{resourceName}' not available.");
                    return result;
                }
                using (var streamReader = new StreamReader(stream))
                {
                    VirtualAssistant.Instance.Logger.Info($"Embedded resource '{resourceName}' is available.");
                    result = streamReader.ReadToEnd();
                }
            }
            return result;
        }

        public ProjectManager Project { get; set; }

        public SimlBot Bot { get; }

        public override T GetPanel<T>(params object[] parameters)
        {
            return null;
        }
    }
}