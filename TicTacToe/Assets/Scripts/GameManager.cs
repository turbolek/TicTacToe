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
        _boardSpawner.BoardStateChanged += OnBoardStateChanged;
        _startGameButton.onClick.AddListener(StartGame);
    }

    private void OnDestroy()
    {
        _boardSpawner.BoardStateChanged -= OnBoardStateChanged;
    }

    private void StartGame()
    {
        _player1 = new Player("Player 1", PlayerType.HumanPlayer);
        _player2 = new Player("Player 2", PlayerType.CPU);

        _startingPlayer = GetStartingPlayer();
        _startingPlayer.Mark = "X";
        GetNextPlayer(_startingPlayer).Mark = "O";

        _startGameButton.gameObject.SetActive(false);
        CurrentGameState = GameState.Setup;
        _boardSpawner.Clear();
        _boardSpawner.Init(this);
        CurrentGameState = GameState.Gameplay;

        StartTurn();
    }

    private Player GetStartingPlayer()
    {
        float diceRoll = Random.Range(0f, 1f);
        return diceRoll < 0.5f ? _player1 : _player2;
    }

    private Player GetNextPlayer(Player currentPlayer)
    {
        return currentPlayer == _player1 ? _player2 : _player1;
    }

    private void OnBoardStateChanged(BoardSpawner board)
    {
        switch (CurrentGameState)
        {
            case GameState.Gameplay:
                {
                    if (board.LongestSequence >= _requiredSequenceLength)
                    {
                        FinishGame(ActivePlayer);
                    }
                    else if (!board.HasEmptyField())
                    {
                        FinishGame(null);
                    }
                    else
                    {
                        StartTurn();
                    }
                    break;
                }
        }
    }

    private async void StartTurn()
    {
        _timeotCancellationTokenSource?.Cancel();
        _timeotCancellationTokenSource = new CancellationTokenSource();

        ActivateNextPlayer();

        var playerTask = ActivePlayer.GetField(_boardSpawner, _timeotCancellationTokenSource.Token);
        var timeouteTask = StartTurnCountdown(_timeotCancellationTokenSource.Token);
        Debug.Log(ActivePlayer.Name + "'s turn");

        await Task.WhenAny(playerTask, timeouteTask);
        _timeotCancellationTokenSource.Cancel();

        BoardButton selectedField = playerTask.Result;

        if (selectedField != null)
        {
            selectedField.SetOwner(ActivePlayer);
        }
        else
        {
            FinishGame(GetNextPlayer(ActivePlayer));
        }
    }

    private void ActivateNextPlayer()
    {
        if (ActivePlayer == null)
        {
            _startingPlayer.Activate();
        }
        else
        {
            Player activePlayer = ActivePlayer;
            activePlayer?.Deactivate();
            GetNextPlayer(activePlayer).Activate();
        }
    }

    private async Task StartTurnCountdown(CancellationToken cancellationToken)
    {
        float timer = _timeLimit;
        while (timer > 0 && !cancellationToken.IsCancellationRequested)
        {
            timer -= Time.deltaTime;
            await Task.Yield();
            //Debug.Log(timer);
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
}
