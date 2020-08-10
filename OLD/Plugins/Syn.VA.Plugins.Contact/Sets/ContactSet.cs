using System;
using System.Collections.Generic;
using Microsoft.Communications.Contacts;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Extensions;

namespace Syn.VA.Plugins.Contact.Sets
{
    class ContactSet : ISet
    {
        internal readonly Dictionary<string, Microsoft.Communications.Contacts.Contact> ContactsTable;

        public ContactSet()
        {
            try
            {
                ContactsTable = new Dictionary<string, Microsoft.Communications.Contacts.Contact>(StringComparer.InvariantCultureIgnoreCase);
                var contactManager = new ContactManager();
                foreach (var contact in contactManager.GetContactCollection())
                {
                    ContactsTable.Put(contact.Names[0].FormattedName, contact);
                }
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
        }
        public string Name => "Contact";

        public IEnumerable<string> GetValues(string parameter = "")
        {
            return ContactsTable.Keys;
        } 
        public bool Contains(string item, string parameter)
        {
            return ContactsTable.ContainsKey(item);
        }
    }
}