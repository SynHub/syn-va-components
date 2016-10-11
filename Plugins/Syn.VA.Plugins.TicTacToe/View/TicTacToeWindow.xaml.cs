using System;
using System.Windows.Threading;
using Syn.Bot.Siml;
using Syn.Utility;
using Syn.VA.Libraries.Game.TicTacToe;

namespace Syn.VA.Plugins.TicTacToe.View
{
    /// <summary>
    /// Interaction logic for TicTacToeWindow.xaml
    /// </summary>
    public partial class TicTacToeWindow
    {
        public TicTacToeWindow(Plugin botPlugin)
        {
            InitializeComponent();

            Plugin = botPlugin;

            Reset();

            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(2) };
            timer.Tick += delegate
            {
                if (Game.NextPlayer == PlayerType.Computer)
                {
                    var moveResult = Game.Computer.AutoMove();
                    if (moveResult.LastMove != null)
                    {
                        SetMove(moveResult.LastMove.Value, PlayerType.Computer);
                        CheckWinner(moveResult);
                    }

                }
            };
            timer.Start();
        }

        private  Plugin Plugin { get; }

        private void CheckWinner(MoveResult moveResult)
        {
            var bot = VirtualAssistant.Instance.Components.Get<SimlBot>();
            if (moveResult.GameOver)
            {
                if (moveResult.Winner == PlayerType.None)
                {
                    bot.Trigger("game-over");
                }
                else if (moveResult.Winner == PlayerType.Human)
                {
                    bot.Trigger("player-won");
                }
                else if (moveResult.Winner == PlayerType.Computer)
                {
                    bot.Trigger("computer-won");
                }
            }
        }

        public TicTacToeGame Game { get; private set; }

        public void Reset()
        {
            Game = new TicTacToeGame {NextPlayer = PlayerType.Human};
            PlayerType outValue;

            if (Enum.TryParse(Plugin.Settings["First-Player"].Value, true, out outValue))
            {
                Game.NextPlayer = outValue;
            }

            Image1.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number1);
            Image2.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number2);
            Image3.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number3);
            Image4.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number4);
            Image5.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number5);
            Image6.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number6);
            Image7.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number7);
            Image8.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number8);
            Image9.Source = SynUtility.Image.CreateBitmapSource(Properties.Resources.Number9);
        }

        public void SetMove(int move, PlayerType playerType)
        {
            if (playerType == PlayerType.Human)
            {
                if (!Game.Human.CanMoveTo(move)) return;
                CheckWinner(Game.Human.MakeMove(move));
            }

            switch (move)
            {
                case 1:
                    Image1.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 2:
                    Image2.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 3:
                    Image3.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 4:
                    Image4.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 5:
                    Image5.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 6:
                    Image6.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 7:
                    Image7.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 8:
                    Image8.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
                case 9:
                    Image9.Source = SynUtility.Image.CreateBitmapSource(playerType == PlayerType.Computer ? Properties.Resources.Circle : Properties.Resources.Cross);
                    break;
            }
        }
    }
}
