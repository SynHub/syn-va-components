using System;
using System.Windows;
using System.Xml.Linq;
using Syn.Bot.Siml;
using Syn.Bot.Siml.Helper;
using Syn.Bot.Siml.Interfaces;
using Syn.VA.Libraries.Game.TicTacToe;
using Syn.VA.Plugins.TicTacToe.View;

namespace Syn.VA.Plugins.TicTacToe.Adapter
{
    public class TicTacToeAdapter : IAdapter
    {
        private readonly Plugin _botPlugin;

        public TicTacToeAdapter(Plugin botPlugin)
        {
            _botPlugin = botPlugin;
        }

        public bool GameOn => Window !=null && PresentationSource.FromVisual(Window) != null;

        public TicTacToeWindow Window { get; set; }

        public bool IsRecursive => true;

        public XName TagName => SimlSpecification.Namespace.O + "TicTacToe";

        public string Evaluate(Context context)
        {
            try
            {
                var taskAttribute = context.Element.Attribute(Tag.TaskAttribute);
                if (taskAttribute != null)
                {
                    var taskValue = taskAttribute.Value.ToLower();
                    if (taskValue == "start")
                    {
                        if (GameOn)
                        {
                            Window.Close();
                        }
                        Window = new TicTacToeWindow(_botPlugin);
                        Window.Show();
                    }
                    else if (taskValue == "stop")
                    {
                        if (GameOn)
                        {
                            Window.Close();
                        }
                    }
                    else if (taskValue == "mark")
                    {
                        if (GameOn)
                        {
                            int outValue;
                            if (int.TryParse(context.Element.Value, out outValue))
                            {
                                Window.SetMove(outValue, PlayerType.Human);
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
