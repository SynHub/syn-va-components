using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Libraries.MovieInfo;

namespace Syn.VA.Plugins.MovieInfo
{
    public class MovieInfoAdapter : IAdapter
    {
        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "MovieInfo";

        public string Evaluate(Context context)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var omdbApi = new OmdbApi();
                    var result = omdbApi.Search(context.Element.Value);
                    if (result.Success)
                    {
                        var getAttribute = context.Element.Attribute(Tag.GetAttribute);
                        var toReturn = string.Empty;
                        if (getAttribute != null)
                        {
                            var getValue = getAttribute.Value.ToLower();
                            if (getValue == "rating")
                            {
                                toReturn = result.ImdbRating.ToString(CultureInfo.InvariantCulture);
                            }
                            if (getValue == "language")
                            {
                                toReturn = result.Language;
                            }
                            if (getValue == "genre")
                            {
                                toReturn = result.Genre;
                            }
                            if (getValue == "votes")
                            {
                                toReturn = result.ImdbVotes.ToString(CultureInfo.InvariantCulture);
                            }
                            if (getValue == "writer")
                            {
                                toReturn = result.Writer;
                            }
                            if (getValue == "country")
                            {
                                toReturn = result.Country;
                            }
                            if (getValue == "plot")
                            {
                                toReturn = result.Plot;
                            }
                            if (getValue == "year")
                            {
                                toReturn = result.Year.ToString(CultureInfo.InvariantCulture);
                            }
                            if (getValue == "awards")
                            {
                                toReturn = result.Awards;
                            }
                            if (getValue == "actors")
                            {
                                toReturn = result.Actors;
                            }
                            if (getValue == "director")
                            {
                                toReturn = result.Director;
                            }
                            var dispatcher = VirtualAssistant.Instance.Components.Get<Dispatcher>();
                            if (dispatcher != null && !string.IsNullOrEmpty(toReturn))
                            {
                                dispatcher.Invoke(() =>
                                {
                                    context.User.Vars["mediainfo-movie"].Value = context.Element.Value;
                                    context.User.Vars["mediainfo-type"].Value = getValue;
                                    context.User.Vars["mediainfo-value"].Value = toReturn;
                                    context.Bot.Trigger("mediainfo-query");
                                });
                            }
                            else
                            {
                                context.Bot.Trigger("mediainfo-query-error");
                            }
                        }

                    }
                });
            }
            catch (Exception exception)
            {
                VirtualAssistant.Instance.Logger.Error(exception);
            }

            return string.Empty;
        }
    }
}
