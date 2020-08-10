using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using NHunspell;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;

namespace Syn.VA.Plugins.Dictionary.Adapter
{
    public class ThesaurusAdapter : IAdapter
    {
        static Hunspell _hunspell;
        static MyThes _thes;
        static Hyphen _hyphen;

        public ThesaurusAdapter()
        {
            try
            {
                var resourceDirectory = VA.SettingsManager["VA"]["resources-directory"].Value;
                var spellDirectory = Path.Combine(resourceDirectory, "Spell");
                _hunspell = new Hunspell(Path.Combine(spellDirectory, "en_us.aff"), Path.Combine(spellDirectory, "en_us.dic"));
                _thes = new MyThes(Path.Combine(spellDirectory, "th_en_US_new.dat"));
                _hyphen = new Hyphen(Path.Combine(spellDirectory, "hyph_en_US.dic"));
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public VirtualAssistant VA => VirtualAssistant.Instance;

        public bool IsRecursive => true;
        public XName TagName => SimlSpecification.Namespace.O + "Thesaurus";

        public string Evaluate(Context context)
        {
            try
            {
                //Elements has Get attribute
                var getAttribute = context.Element.Attribute("Get");
                var searchWord = context.Element.Value;
                if (getAttribute != null)
                {
                    #region Synonym
                    if (getAttribute.Value.Equals("synonym", StringComparison.InvariantCultureIgnoreCase))
                    {

                        var thesResult = _thes.Lookup(searchWord);

                        var foundList = new List<string>();
                        foreach (var meaning in thesResult.Meanings)
                        {
                            foreach (var synonym in meaning.Synonyms)
                            {
                                if (foundList.Contains(synonym) == false)
                                {
                                    foundList.Add(synonym);
                                }
                            }
                        }

                        if (foundList.Any())
                        {
                            var speakList = ReducedList(foundList, 4);//Todo: implement a speaking interface
                            var toReturn = SynUtility.Text.GetFormattedSentence(speakList);
                            context.User.Vars["synonym"].Value = toReturn;
                        }
                    }
                    #endregion
                    #region Spell
                    else if (getAttribute.Value.Equals("spell", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var foundList = new List<string>();

                        foreach (var suggest in _hunspell.Suggest(searchWord))
                        {
                            if (foundList.Contains(suggest) == false)
                            {
                                foundList.Add(suggest);
                            }
                        }

                        if (foundList.Any())
                        {
                            var speakList = ReducedList(foundList, 4);//Todo: implement a speaking interface
                            var toReturn = SynUtility.Text.GetFormattedSentence(speakList);
                            context.User.Vars["spell"].Value = toReturn;
                        }
                    }
                    #endregion
                    #region Hyphenate
                    if (getAttribute.Value.Equals("hyphenate", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var hyphenated = _hyphen.Hyphenate(searchWord);
                        context.User.Vars["Hyphenate"].Value = hyphenated.HyphenatedWord;
                    }
                    #endregion
                }
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return string.Empty;
        }

        public static List<string> ReducedList(List<string> theList, int reduction)
        {
            if (theList.Count <= reduction)
            {
                return theList;
            }
            var toReturn = new List<string>();
            var counter = 0;
            foreach (var toAdd in theList)
            {
                if (counter < reduction)
                {
                    counter = counter + 1;
                    toReturn.Add(toAdd);
                }
            }

            return toReturn;
        }
    }
}