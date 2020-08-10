using System;
using System.Collections.Generic;
using System.Diagnostics;
using Syn.Bot.Siml;
using Syn.VA.Libraries.Language.Locale;

namespace Syn.VA.Plugins.Interaction
{
    public class InteractionPlugin : Plugin
    {
        public Dictionary<string, DateTime> Detected { get; }

        public InteractionPlugin()
        {
            try
            {
                Detected = new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
                var bot = VA.Components.Get<SimlBot>();
                var interactionService = new Service("Interaction-Monitoring-Service", () =>
                {
                    var knownProcesses = Settings["Known-Processes"].List;
                    foreach (var processName in knownProcesses)
                    {
                        var processes = Process.GetProcessesByName(processName);
                        if (processes.Length > 0 && !Detected.ContainsKey(processName))
                        {
                            Detected.Add(processName, DateTime.Now);
                            bot.Trigger($"Interaction-Detected {processName}");
                        }
                        else if (processes.Length > 0 && Detected.ContainsKey(processName))
                        {
                            var processStartTime = Detected[processName];
                            var elapsed = DateTime.Now - processStartTime;
                            var busyLevelStart = Convert.ToInt32(Settings["Busy-Level-Start"].Value);
                            var busyLevelEnd = Convert.ToInt32(Settings["Busy-Level-End"].Value);
                            if (elapsed.TotalSeconds > busyLevelStart)
                            {
                                bot.Trigger($"Interaction-Busy-Detected {processName}");
                                Detected[processName] = Detected[processName].AddSeconds(busyLevelEnd);
                            }
                        }
                        else if (processes.Length == 0 && Detected.ContainsKey(processName))
                        {
                            Detected.Remove(processName);
                        }
                    }
                });

                interactionService.Interval = TimeSpan.FromSeconds(1);
                interactionService.Description = StringResource.InteractionPlugin_ServiceDescription;
                VA.Services.Add(interactionService);
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "Interaction";

        public override string DisplayName => StringResource.InteractionPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            return null;
        }
    }
}