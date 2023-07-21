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
    private BoardSpawner _boardSpawner;

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
    }

    private void OnDestroy()
    {
    }

    private void StartGame()
    {
        _boardStates = new Stack<BoardState>();
        _player1 = new Player("Player 1", PlayerType.HumanPlayer, FieldOwnerType.Player1);
        _player2 = new Player("Player 2", PlayerType.CPU, FieldOwnerType.Player2);

        _startingPlayer = GetStartingPlayer();
        _startingPlayer.Mark = "X";
        GetNextPlayer(_startingPlayer).Mark = "O";

        _startGameButton.gameObject.SetActive(false);
        CurrentGameState = GameState.Setup;
        _boardSpawner.Clear();
        _boardSpawner.Init(this);
        CurrentGameState = GameState.Gameplay;

        _turnCancellationTokenSource = new CancellationTokenSource();

        _boardStates.Push(_boardSpawner.GetCurrentBoardState());

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

    private void CheckBoard()
    {
        switch (CurrentGameState)
        {
            case GameState.Gameplay:
                {
                    if (_boardSpawner.LongestSequence >= _requiredSequenceLength)
                    {
                        FinishGame(ActivePlayer);
                    }
                    else if (!_boardSpawner.HasEmptyField())
                    {
                        FinishGame(null);
                    }
                    else
                    {
                        _boardStates.Push(_boardSpawner.GetCurrentBoardState());

                        StartTurn(GetNextPlayer(ActivePlayer), _turnCancellationTokenSource.Token);
                    }
                    break;
                }
        }
    }

    private async void StartTurn(Player playerToActivate, CancellationToken cancellationToken)
    {
        _timeotCancellationTokenSource?.Cancel();
        _timeotCancellationTokenSource = new CancellationTokenSource();

        _player1.Deactivate();
        _player2.Deactivate();

        playerToActivate.Activate();

        var playerTask = ActivePlayer.GetField(_boardSpawner, _timeotCancellationTokenSource.Token);
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
                BoardButton selectedField = playerTask.Result;
                selectedField.SetOwner(ActivePlayer);
                CheckBoard();
            }

            else if (timeoutTask.Status == TaskStatus.RanToCompletion)
            {
                FinishGame(GetNextPlayer(ActivePlayer));
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

    private void FinishGame(Player winner)
    {
        CurrentGameState = GameState.GameOver;
        if (winner != null)
        {
            Debug.Log(winner.Name + " wins!");
        }
        else
        {
            Debug.Log("Draw!");
        }
        _startGameButton.gameObject.SetActive(true);
    }

    private void Undo()
    {
        BoardState previousBoardState = GetUndoTargetState();
        if (previousBoardState != null)
        {
            CancelCurrentTurn();
            _boardSpawner.LoadBoardState(previousBoardState);
            Player playerToActivate = GetNextPlayer(GetPlayerByFieldOwnerType(previousBoardState.ActivePlayer));
            _boardStates.Push(previousBoardState);
            StartTurn(playerToActivate, _turnCancellationTokenSource.Token);
        }
    }

    private void CancelCurrentTurn()
    {
        _turnCancellationTokenSource.Cancel();
        _turnCancellationTokenSource = new CancellationTokenSource();
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
            Player activePlayer = GetPlayerByFieldOwnerType(targetState.ActivePlayer);
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
}
