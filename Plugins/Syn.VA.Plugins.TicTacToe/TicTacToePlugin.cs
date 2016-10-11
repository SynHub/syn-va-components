using System;
using Syn.Bot.Siml;
using Syn.Utility.Extensions;
using Syn.VA.Libraries.Game.TicTacToe;
using Syn.VA.Libraries.Language.Locale;
using Syn.VA.Plugins.TicTacToe.Adapter;
using Syn.VA.Plugins.TicTacToe.View;
using Syn.VA.Plugins.TicTacToe.ViewModel;
using Syn.VA.Utility.Extensions;

namespace Syn.VA.Plugins.TicTacToe
{
    public class TicTacToePlugin : Plugin
    {
        public TicTacToePlugin()
        {
            try
            {
                var bot = VA.Components.Get<SimlBot>();
                bot.Adapters.Add(new TicTacToeAdapter(this));
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }
        }

        public override string Name => "TicTacToe";

        public override string DisplayName => StringResource.TicTacToePlugin_DisplayName;

        public override T GetPanel<T>(params object[] parameters)
        {
            try
            {
                var ticTacToePanel = new TicTacToePanel();
                var ticTacToeContext = ticTacToePanel.DataContext as TicTacToeContext;

                if (ticTacToeContext != null)
                {
                    ticTacToeContext.PlayerTypes.Add(PlayerType.Computer.ToString());
                    ticTacToeContext.PlayerTypes.Add(PlayerType.Human.ToString());
                    ticTacToeContext.DifficultyTypes.AddRange(Enum.GetNames(typeof(DifficultyType)));
                }

                ticTacToePanel.FirstPlayerComboBox.SelectedItem = Settings["First-Player"].Value;
                ticTacToePanel.GameDifficultyComboBox.SelectedItem = Settings["Game-Difficulty"].Value;

                ticTacToePanel.FirstPlayerComboBox.SelectionChanged += delegate
                {
                    Settings["First-Player"].Value = ticTacToePanel.FirstPlayerComboBox.SelectedItem.ToString();
                    VA.SettingsManager.Save(Settings);
                };

                ticTacToePanel.GameDifficultyComboBox.SelectionChanged += delegate
                {
                    Settings["Game-Difficulty"].Value = ticTacToePanel.GameDifficultyComboBox.SelectedItem.ToString();
                    VA.SettingsManager.Save(Settings);
                };

                return ticTacToePanel as T;
            }
            catch (Exception exception)
            {
                VA.Logger.Error(exception);
            }

            return null;
        }
    }
}