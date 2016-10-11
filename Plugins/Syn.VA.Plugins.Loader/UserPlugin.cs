using System;
using System.Windows.Forms;
using Syn.Bot.Siml;
using Syn.Utility;
using Syn.Utility.Extensions;
using Syn.VA.Controls;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Loader.View;
using Syn.VA.Plugins.Loader.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.Loader
{
    /// <summary>
    /// A UserPlugin class to store and interact with user settings.
    /// </summary>
    /// <seealso cref="Plugin" />
    /// <remarks>This class is internal to prevent autoload.</remarks>
    internal class UserPlugin : Plugin
    {
        public UserPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                VA.Resetting += (sender, args) =>
                {
                    try
                    {
                        var mainUserSettings = bot.MainUser.Settings;

                        mainUserSettings["Name"].Value = "User";
                        mainUserSettings["First-Name"].Value = string.Empty;
                        mainUserSettings["Last-Name"].Value = string.Empty;
                        mainUserSettings["Middle-Name"].Value = string.Empty;
                        mainUserSettings["Gender"].Value = "Male";
                        mainUserSettings["Country"].Value = "United States";
                        mainUserSettings["Email"].Value = "you@example.com";

                        LoaderPlugin.SimlFileManager.SaveDocument(LoaderPlugin.UserSettingsFileId, mainUserSettings.GetDocument());
                    }
                    catch (Exception exception)
                    {
                        VA.Logger.Error(exception);
                    }
                };

                Settings.Bind(bot.MainUser.Settings);
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "User";

        public override string DisplayName => StringResource.UserPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var userPanel = new UserPanel();
                var context = userPanel.DataContext as UserContext;
                var bot = VA.Components.Get<SimlBot>();
                if (context != null)
                {
                    var mainUserSettings = bot.MainUser.Settings;

                    mainUserSettings.ApplyToProperties(context, "Name", "First-Name", "Last-Name", "Middle-Name", "Email");

                    foreach (var item in Enum.GetValues(typeof(Gender)))
                    {
                        context.GenderList.Add(item.ToString());
                    }

                    //Get all countries from Country Set found in the main Bot Set
                    foreach (var item in bot.Sets["country"].GetValues())
                    {
                        context.CountryList.Add(item);
                    }

                    userPanel.GenderComboBox.SelectedItem = mainUserSettings["Gender"].Value;
                    userPanel.CountryComboBox.SelectedItem = mainUserSettings["Country"].Value;

                    //var imagePath = generalSettings["user-avatar-file"].Value;
                    var userImagePath = SynUtility.Computer.GetUserTileImagePath(SystemInformation.UserName);
                    if (!string.IsNullOrEmpty(userImagePath)) userPanel.UserImage.SetSource(userImagePath);


                    context.SaveAction = () =>
                    {
                        try
                        {
                            var settingsWindow = VA.Components.Get<SettingsWindow>();
                            mainUserSettings["Name"].Value = context.Name;
                            mainUserSettings["First-Name"].Value = context.FirstName;
                            mainUserSettings["Last-Name"].Value = context.LastName;
                            mainUserSettings["Middle-Name"].Value = context.MiddleName;
                            mainUserSettings["Gender"].Value = userPanel.GenderComboBox.SelectedItem.ToString();
                            mainUserSettings["Country"].Value = userPanel.CountryComboBox.SelectedItem.ToString();
                            mainUserSettings["Email"].Value = context.Email;
                            settingsWindow.DisplayMessage(Name, StringResource.UserPlugin_SettingsSavedMessage);
                            LoaderPlugin.SimlFileManager.SaveDocument(LoaderPlugin.UserSettingsFileId, mainUserSettings.GetDocument());
                        }
                        catch (Exception exception)
                        {
                            VA.Logger.Error(exception);
                        }
                    };
                }

                return userPanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
            return null;
        }
    }
}