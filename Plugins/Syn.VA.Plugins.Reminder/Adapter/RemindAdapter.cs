using System;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;
using Syn.VA.Extensions;

namespace Syn.VA.Plugins.Reminder.Adapter
{
    class RemindAdapter : IAdapter
    {
        public bool IsRecursive => true;
        public XName TagName => SimlSpecification.Namespace.O + "Remind";

        public string Evaluate(Context context)
        {
            var va = VirtualAssistant.Instance;
            try
            {
                var remindText = context.Element.Value;
                var timeAttribute = context.Element.Attribute("Time");
                var minuteAttribute = context.Element.Attribute("Minute");
                var hourAttribute = context.Element.Attribute("Hour");
                var dayAttribute = context.Element.Attribute("Day");
                if (timeAttribute != null )
                {
                    var timeValue = timeAttribute.Value;  
                    if (dayAttribute == null)
                    {
                        var dateTime = SynUtility.Date.GetDateTime(timeValue);
                        dateTime = NextDate(dateTime);
                        AddRemind(context, remindText, dateTime);
                        return string.Empty;
                    }


                    var dayValue = dayAttribute.Value.ToLower();
                    if (dayValue == "next")
                    {
                        var dateTime = SynUtility.Date.GetDateTime(timeValue, DateTime.Now.AddDays(1).DayOfWeek);
                        AddRemind(context, remindText, dateTime);
                        return string.Empty;
                    }

                    DayOfWeek dayOfWeek;
                    if (Enum.TryParse(dayValue, true, out dayOfWeek))
                    {
                        var dateTime = SynUtility.Date.GetDateTime(timeValue, dayOfWeek);
                        AddRemind(context, remindText, dateTime);
                        return string.Empty;
                    }
                }

                if (minuteAttribute != null)
                {
                    var minuteValue = minuteAttribute.Value;
                    double outValue;
                    if (double.TryParse(minuteValue, out outValue))
                    {
                        var dateTime = DateTime.Now.AddMinutes(outValue);
                        AddRemind(context, remindText, dateTime);
                        return string.Empty;
                    }
                }

                if (hourAttribute != null)
                {
                    var hourValue = hourAttribute.Value;
                    double outValue;
                    if (double.TryParse(hourValue, out outValue))
                    {
                        var dateTime = DateTime.Now.AddHours(outValue);
                        AddRemind(context, remindText, dateTime);
                        return string.Empty;
                    }
                }
                
                context.Bot.Trigger("Reminder-Invalid", context.User);
            }
            catch (Exception exception)
            {
                va.Logger.Error(exception);
            }

            return string.Empty;
        }

        private static DateTime NextDate(DateTime dateTime)
        {
            var afterHour = DateTime.Now.Hour >= 12;

            if (dateTime < DateTime.Now)
            {
                if (afterHour)
                {
                    dateTime = dateTime.AddHours(12);
                }
            }

            return dateTime;
        }

        private static void AddRemind(Context context, string content, DateTime dateTime)
        {
            
            var va = VirtualAssistant.Instance;

            var reminderItem = new ReminderItem
            {
                Content = content,
                Date = dateTime
            };

            var plugin = va.Plugins.GetPluginByType<ReminderPlugin>();
            plugin.Reminders.Add(reminderItem);
            plugin.SaveReminders();
            context.Bot.Trigger("Reminder-Added", context.User);
        }
    }
}