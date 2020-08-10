using System;
using System.Linq;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Extensions;
using Syn.VA.Plugins.Contact.Sets;

namespace Syn.VA.Plugins.Contact.Adapter
{
    class ContactAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "Contact";

        public string Evaluate(Context context)
        {
            try
            {
                var getAttribute = context.Element.Attribute(Tag.GetAttribute);
                if (getAttribute != null)
                {
                    var getValue = getAttribute.Value;
                    var contactSet = context.Bot.Sets["contact"] as ContactSet;
                    if (contactSet != null)
                    {
                        var contact = contactSet.ContactsTable.Get(context.Element.Value);
                        if (getValue == "number")
                        {

                            if (contact.PhoneNumbers.Any())
                            {
                                return contact.PhoneNumbers[0].Number;
                            }
                        }
                        else if (getValue == "email")
                        {
                            if (contact.EmailAddresses.Any())
                            {
                                return contact.EmailAddresses[0].Address;
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }
            
            return string.Empty;
        }
    }
}