using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using LAIR.Collections.Generic;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Interfaces;
using Syn.Utility;
using Syn.VA.Extensions;
using Syn.VA.Libraries.WordNet;

namespace Syn.VA.Plugins.Dictionary.Adapter
{
    public class DictionaryAdapter : IAdapter
    {
        readonly WordNetEngine _wordNetEngine;
        readonly DictionaryPlugin _plugin;

        public DictionaryAdapter(DictionaryPlugin plugin)
        {
            try
            {
                _plugin = plugin;
                var resourcesDirectory = VA.SettingsManager["VA"]["Resources-Directory"].Value;
                var loadPath = Path.Combine(resourcesDirectory, "WordNet");
                _wordNetEngine = new WordNetEngine(loadPath, false);
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public bool IsRecursive => true;
        public VirtualAssistant VA => VirtualAssistant.Instance;
        public XName TagName => SimlSpecification.Namespace.O + "Dictionary";

        public string Evaluate(Context context)
        {
            try
            {
                var commonMode = _plugin.Settings["Use-Common"].GetValue<bool>();
                var toSearch = context.Element.Value;
                Set<SynSet> synSetsList = null;
                var definitionList = new List<string>();
                if (commonMode)
                {
                    synSetsList = _wordNetEngine.GetSynSets(toSearch);
                }
                else
                {
                    var synset = _wordNetEngine.GetMostCommonSynSet(toSearch, WordNetEngine.POS.Noun);
                    if (synset != null)
                    {
                        synSetsList = new Set<SynSet>(new[] { synset });
                    }
                }

                if (synSetsList != null)
                {
                    foreach (var synSet in synSetsList)
                    {
                        var types = SynUtility.Text.GetFormattedSentence(synSet.Words);
                        types = "( " + types.Replace("_", " ") + " ) ";
                        var glossString = "";

                        foreach (var sentence in SynUtility.Text.GetWords(synSet.Gloss, ";"))
                        {
                            if (sentence.StartsWith(" \""))
                            {
                                glossString += ". Example sentence: " + sentence;
                            }
                            else
                            {
                                glossString += "Definition: " + sentence;
                            }
                        }
                        definitionList.Add(types + "\n" + glossString);
                    }

                    var toShow = "";
                    foreach (var sentence in definitionList)
                    {
                        toShow += sentence + "\n";
                    }

                    return toShow;
                }
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}