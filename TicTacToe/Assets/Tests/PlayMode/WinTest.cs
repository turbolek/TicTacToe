using NUnit.Framework;

namespace Tests
{
    public class WinTest
    {
        /*
         Field indices in 3x3 board
         
        _0_|_1_|_2_
        _3_|_4_|_5_
        _6_|_7_|_8_        
         */

        [Test]
        public void ClearBoard()
        {
            BoardController boardController = new BoardController();

            GameSettings settings = new GameSettings();
            settings.BoardHeight = 3;
            settings.BoardWidth = 3;
            settings.RequiredSequenceLength = 3;

            boardController.Init(settings);

            WinnerState winnerState = boardController.GetWinnerState();

            Assert.AreEqual(WinnerState.NotConcluded, winnerState);
        }

        [Test]
        public void VerticalWin()
        {
            BoardController boardController = new BoardController();

            GameSettings settings = new GameSettings();
            settings.BoardHeight = 3;
            settings.BoardWidth = 3;
            settings.RequiredSequenceLength = 3;

            boardController.Init(settings);

            boardController.SetFieldState(0, FieldOwnerType.Player1);
            boardController.SetFieldState(3, FieldOwnerType.Player1);
            boardController.SetFieldState(6, FieldOwnerType.Player1);

            WinnerState winnerState = boardController.GetWinnerState();

            Assert.AreEqual(WinnerState.Player1Wins, winnerState);
        }

        [Test]
        public void HorizontalWin()
        {
            BoardController boardController = new BoardController();

            GameSettings settings = new GameSettings();
            settings.BoardHeight = 3;
            settings.BoardWidth = 3;
            settings.RequiredSequenceLength = 3;

            boardController.Init(settings);

            boardController.SetFieldState(0, FieldOwnerType.Player1);
            boardController.SetFieldState(1, FieldOwnerType.Player1);
            boardController.SetFieldState(2, FieldOwnerType.Player1);

            WinnerState winnerState = boardController.GetWinnerState();

            Assert.AreEqual(WinnerState.Player1Wins, winnerState);
        }

        [Test]
        public void DiagonalDescendingWin()
        {
            BoardController boardController = new BoardController();

            GameSettings settings = new GameSettings();
            settings.BoardHeight = 3;
            settings.BoardWidth = 3;
            settings.RequiredSequenceLength = 3;

            boardController.Init(settings);

            boardController.SetFieldState(0, FieldOwnerType.Player1);
            boardController.SetFieldState(4, FieldOwnerType.Player1);
            boardController.SetFieldState(8, FieldOwnerType.Player1);

            WinnerState winnerState = boardController.GetWinnerState();

            Assert.AreEqual(WinnerState.Player1Wins, winnerState);
        }

        [Test]
        public void DiagonalAscendingWin()
        {
            BoardController boardController = new BoardController();

            GameSettings settings = new GameSettings();
            settings.BoardHeight = 3;
            settings.BoardWidth = 3;
            settings.RequiredSequenceLength = 3;

            boardController.Init(settings);

            boardController.SetFieldState(6, FieldOwnerType.Player1);
            boardController.SetFieldState(4, FieldOwnerType.Player1);
            boardController.SetFieldState(2, FieldOwnerType.Player1);

            WinnerState winnerState = boardController.GetWinnerState();

            Assert.AreEqual(WinnerState.Player1Wins, winnerState);
        }

        [Test]
        public void OpponentWin()
        {
            BoardController boardController = new BoardController();

            GameSettings settings = new GameSettings();
            settings.BoardHeight = 3;
            settings.BoardWidth = 3;
            settings.RequiredSequenceLength = 3;

            boardController.Init(settings);

            boardController.SetFieldState(0, FieldOwnerType.Player2);
            boardController.SetFieldState(1, FieldOwnerType.Player2);
            boardController.SetFieldState(2, FieldOwnerType.Player2);

            WinnerState winnerState = boardController.GetWinnerState();

            Assert.AreEqual(WinnerState.Player2Wins, winnerState);
        }

        [Test]
        public void Draw()
        {
            BoardController boardController = new BoardController();

            GameSettings settings = new GameSettings();
            settings.BoardHeight = 3;
            settings.BoardWidth = 3;
            settings.RequiredSequenceLength = 3;

            boardController.Init(settings);

            boardController.SetFieldState(1, FieldOwnerType.Player1);
            boardController.SetFieldState(3, FieldOwnerType.Player1);
            boardController.SetFieldState(5, FieldOwnerType.Player1);
            boardController.SetFieldState(6, FieldOwnerType.Player1);
            boardController.SetFieldState(8, FieldOwnerType.Player1);

            boardController.SetFieldState(0, FieldOwnerType.Player2);
            boardController.SetFieldState(2, FieldOwnerType.Player2);
            boardController.SetFieldState(4, FieldOwnerType.Player2);
            boardController.SetFieldState(7, FieldOwnerType.Player2);

            WinnerState winnerState = boardController.GetWinnerState();

            Assert.AreEqual(WinnerState.Draw, winnerState);
        }

        //TODO add timeout tests
        //TODO add undo tests
        //TODO get hint tests
    }
}
