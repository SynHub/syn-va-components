using System;
using Syn.Bot.Siml;
using Syn.VA.Plugins.Contact.Adapter;
using Syn.VA.Plugins.Contact.Sets;

namespace Syn.VA.Plugins.Contact
{
    public class ContactPlugin : Plugin
    {
        public ContactPlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new ContactAdapter());
                bot.Sets.Add(new ContactSet());
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override T GetPanel<T>(params object[] parameters)
        {
            return null;
        }
    }
}