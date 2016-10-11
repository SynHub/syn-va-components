using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Syn.Bot.Siml;
using Syn.Utility.Extensions;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.Reminder.Adapter;
using Syn.VA.Plugins.Reminder.View;
using Syn.VA.Plugins.Reminder.ViewModel;

namespace Syn.VA.Plugins.Reminder
{
    public class ReminderPlugin : Plugin
    {
        private readonly string _reminderPath;

        public ReminderPlugin()
        {
            try
            {
                Reminders = new List<ReminderItem>();
                Reminders.Add(new ReminderItem() {Content = "Something", Date = DateTime.Now});
                _reminderPath = Path.Combine(VA.SettingsManager["VA"]["Resources-Directory"].Value, "Reminder", "Settings.json");
                Bot.Adapters.Add(new RemindAdapter());

                var reminderService = new Service("Reminder-Service", () =>
                {
                    foreach (var reminder in Reminders.ToList())
                    {
                        if (reminder.Date <= DateTime.Now)
                        {
                            Bot.MainUser.Vars["Reminder"].Value = reminder.Content;
                            Bot.Trigger("Reminder-Event");
                            Reminders.Remove(reminder);
                        }
                    }
                });

                reminderService.Description = StringResource.ReminderPlugin_ReminderServiceDescription;
                VA.Services.Add(reminderService);

                VA.Closing += (sender, args) =>
                {
                    SaveReminders();
                };

                if (File.Exists(_reminderPath))
                {
                    var reminderString = File.ReadAllText(_reminderPath);
                    var deserializedObject = JsonConvert.DeserializeObject(reminderString, typeof(List<ReminderItem>)) as List<ReminderItem>;
                    Reminders = deserializedObject;
                }
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        internal void SaveReminders()
        {
            var serializedObject = JsonConvert.SerializeObject(Reminders, Formatting.Indented);
            File.WriteAllText(_reminderPath, serializedObject);
        }

        public List<ReminderItem> Reminders { get; }

        private SimlBot Bot => VA.Components.Get<SimlBot>();

        public override string Name => "Reminder";

        public override string DisplayName => StringResource.ReminderPlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var reminderPanel = new ReminderPanel();
                var context = reminderPanel.DataContext as ReminderPanelContext;
                if (context != null)
                {
                    context.ReminderList.AddRange(Reminders);
                    context.RemoveItemAction = () =>
                    {
                        Reminders.Remove(context.SelectedItem); //Position critical.
                        context.ReminderList.Remove(context.SelectedItem);
                        SaveReminders();
                    };
                }
                return reminderPanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}