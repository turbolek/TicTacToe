using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Button _startGameButton;

    [SerializeField]
    private Button _undoButton;

    [SerializeField]
    private Button _hintButton;

    [SerializeField]
    private float _timeLimit = 5f;
    [SerializeField]
    private int _requiredSequenceLength = 3;
    [SerializeField]
    private int _boardHeight;
    public int BoardHeight => _boardHeight;

    [SerializeField]
    private int _boardWidth;
    public int BoardWidth => _boardWidth;

    [SerializeField]
    private BoardView _boardView;
    private BoardController _boardController;

    private CancellationTokenSource _timeotCancellationTokenSource;
    private CancellationTokenSource _turnCancellationTokenSource;

    private Stack<BoardState> _boardStates;

    private Player _player1;
    private Player _player2;
    public Player ActivePlayer
    {
        get
        {
            if (_player1.IsActive)
            {
                return _player1;
            }
            else if (_player2.IsActive)
            {
                return _player2;
            }
            else
            {
                return null;
            }
        }
    }
    private Player _startingPlayer;

    public GameState CurrentGameState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _startGameButton.onClick.AddListener(StartGame);
        _undoButton.onClick.AddListener(Undo);
        _hintButton.onClick.AddListener(OnHintButtonClicked);

        _boardController = new BoardController();

    }

    private void OnDestroy()
    {
    }

    private void StartGame()
    {
        CancelCurrentTurn();
        _boardStates = new Stack<BoardState>();
        _player1 = new Player("Player 1", PlayerType.HumanPlayer, FieldOwnerType.Player1);
        _player2 = new Player("Player 2", PlayerType.CPU, FieldOwnerType.Player2);

        _startingPlayer = GetStartingPlayer();
        _startingPlayer.Mark = "X";
        GetNextPlayer(_startingPlayer).Mark = "O";

        CurrentGameState = GameState.Setup;

        _boardController.Init(_boardWidth, _boardHeight, _requiredSequenceLength);
        _boardView.Init(this, _boardController);
        CurrentGameState = GameState.Gameplay;

        _turnCancellationTokenSource = new CancellationTokenSource();



        StartTurn(_startingPlayer, _turnCancellationTokenSource.Token);
    }

    private Player GetStartingPlayer()
    {
        float diceRoll = Random.Range(0f, 1f);
        return diceRoll < 0.5f ? _player1 : _player2;
    }

    private Player GetNextPlayer(Player currentPlayer)
    {
        if (currentPlayer == _player1)
        {
            return _player2;
        }

        if (currentPlayer == _player2)
        {
            return _player1;
        }

        return _startingPlayer;
    }

    private void CheckBoard(int changedFieldIndex)
    {
        WinnerState winnerState = _boardController.GetWinnerState(changedFieldIndex);

        if (winnerState == WinnerState.NotConcluded)
        {
            StartTurn(GetNextPlayer(ActivePlayer), _turnCancellationTokenSource.Token);
        }
        else
        {
            FinishGame(winnerState);
        }
    }

    private async void StartTurn(Player playerToActivate, CancellationToken cancellationToken)
    {
        _boardStates.Push(_boardController.BoardState.Copy());

        _timeotCancellationTokenSource?.Cancel();
        _timeotCancellationTokenSource = new CancellationTokenSource();

        _player1.Deactivate();
        _player2.Deactivate();

        playerToActivate.Activate();

        _hintButton.gameObject.SetActive(ActivePlayer.Type == PlayerType.HumanPlayer && GetNextPlayer(ActivePlayer).Type == PlayerType.CPU);
        _undoButton.gameObject.SetActive(ActivePlayer.Type == PlayerType.HumanPlayer && GetNextPlayer(ActivePlayer).Type == PlayerType.CPU);

        var playerTask = ActivePlayer.GetFieldIndex(_boardController.BoardState, _timeotCancellationTokenSource.Token);
        var timeoutTask = StartTurnCountdown(_timeotCancellationTokenSource.Token);

        Debug.Log(ActivePlayer.Name + "'s turn");

        while (!(playerTask.IsCompleted || timeoutTask.IsCompleted || cancellationToken.IsCancellationRequested))
        {
            await Task.Yield();
        }

        _timeotCancellationTokenSource.Cancel();
        if (!cancellationToken.IsCancellationRequested)
        {

            if (playerTask.Status == TaskStatus.RanToCompletion)
            {
                int selectedFieldIndex = playerTask.Result;
                _boardController.SetFieldState(selectedFieldIndex, ActivePlayer.FieldOwnerType);
                CheckBoard(selectedFieldIndex);
            }

            else if (timeoutTask.Status == TaskStatus.RanToCompletion)
            {
                WinnerState winnerState = ActivePlayer == _player1 ? WinnerState.Player2Wins : WinnerState.Player1Wins;
                FinishGame(winnerState);
            }
        }
    }

    private async Task StartTurnCountdown(CancellationToken cancellationToken)
    {
        float timer = _timeLimit;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            await Task.Yield();
        }
    }

    private void FinishGame(WinnerState winnerState)
    {
        CurrentGameState = GameState.GameOver;

        switch (winnerState)
        {
            case WinnerState.Player1Wins:
                {
                    Debug.Log(_player1.Name + " wins!");
                    break;
                }
            case WinnerState.Player2Wins:
                {
                    Debug.Log(_player2.Name + " wins!");
                    break;
                }
            case WinnerState.Draw:
                {
                    Debug.Log("Draw!");
                    break;
                }
        }
    }

    private void Undo()
    {
        BoardState previousBoardState = GetUndoTargetState();
        if (previousBoardState != null)
        {
            CancelCurrentTurn();
            _boardController.LoadBoardState(previousBoardState);
            Player playerToActivate = GetNextPlayer(GetPlayerByFieldOwnerType(previousBoardState.LastActivePlayer));
            _boardStates.Push(previousBoardState);
            StartTurn(playerToActivate, _turnCancellationTokenSource.Token);
        }
    }

    private void CancelCurrentTurn()
    {
        if (_turnCancellationTokenSource != null)
        {
            _turnCancellationTokenSource.Cancel();
            _turnCancellationTokenSource = new CancellationTokenSource();
        }
    }

    private BoardState GetUndoTargetState()
    {
        BoardState targetState = null;

        if (_boardStates.Count > 0)
        {
            targetState = _boardStates.Pop();
        }

        while (_boardStates.Count > 0)
        {
            targetState = _boardStates.Pop();
            Player activePlayer = GetPlayerByFieldOwnerType(targetState.LastActivePlayer);
            if (activePlayer == null || activePlayer.Type == PlayerType.CPU)
            {
                break;
            }
        }

        return targetState;
    }

    public Player GetPlayerByFieldOwnerType(FieldOwnerType fieldOwnerType)
    {
        if (_player1.FieldOwnerType == fieldOwnerType)
        {
            return _player1;
        }

        if (_player2.FieldOwnerType == fieldOwnerType)
        {
            return _player2;
        }

        return null;
    }

    public FieldOwnerType GetActivePlayerFieldOwnerType()
    {
        return ActivePlayer != null ? ActivePlayer.FieldOwnerType : FieldOwnerType.Empty;
    }

    private void OnHintButtonClicked()
    {
        _boardView.ShowHintForPlayer(ActivePlayer);
    }
}
