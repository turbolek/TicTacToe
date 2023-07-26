using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : SerializedMonoBehaviour, ISkinable
{
    [SerializeField]
    private Button _startGameButton;
    [SerializeField]
    private Button _undoButton;
    [SerializeField]
    private Button _hintButton;
    [SerializeField]
    private Button _mainMenuButton;
    [SerializeField]
    private GameObject _contentPanel;
    [SerializeField]
    private TMP_Text _playerLabel;
    [SerializeField]
    private TMP_Text _timerLabel;
    [SerializeField]
    private GameObject _endgamePanel;
    [SerializeField]
    private TMP_Text _winnerLabel;

    private MainMenuRequester _menuRequester;

    [SerializeField]
    private BoardView _boardView;
    private BoardController _boardController;

    private CancellationTokenSource _timeotCancellationTokenSource;
    private CancellationTokenSource _turnCancellationTokenSource;

    private Stack<BoardState> _boardStates;

    private Player _player1;
    private Player _player2;

    private GameSettings _gameSettings;
    private Skin _skin;

    private string _timeSeparatorString = ":";
    private string _twoDigitFormatString = "00";

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
        _menuRequester = new MainMenuRequester();
        _startGameButton.onClick.AddListener(delegate { StartGame(_gameSettings); });
        _undoButton.onClick.AddListener(Undo);
        _hintButton.onClick.AddListener(OnHintButtonClicked);
        _mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);

        _boardController = new BoardController();
        _contentPanel.SetActive(false);
    }

    private void OnDestroy()
    {
    }

    public void StartGame(GameSettings settings)
    {
        _contentPanel.SetActive(true);
        _endgamePanel.SetActive(false);

        _gameSettings = settings;

        CancelCurrentTurn();
        _boardStates = new Stack<BoardState>();
        _player1 = new Player("Player 1", settings.Player1Type, FieldOwnerType.Player1);
        _player2 = new Player("Player 2", settings.Player2Type, FieldOwnerType.Player2);

        _startingPlayer = GetStartingPlayer();

        _startingPlayer.Mark = _skin.XIcon;
        GetNextPlayer(_startingPlayer).Mark = _skin.OIcon;
        _boardView.ApplySkin(_skin);

        CurrentGameState = GameState.Setup;

        _boardController.Init(settings);
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
        _playerLabel.text = playerToActivate.Name;
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
        float timer = _gameSettings.TimeLimit;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            timer = Mathf.Clamp(timer, 0f, _gameSettings.TimeLimit);
            _timerLabel.text = FormatTimeString(timer);
            await Task.Yield();
        }
    }

    private string FormatTimeString(float time)
    {
        int minutes = (int)(time / 60);
        time -= minutes;
        int seconds = (int)time;
        time -= seconds;
        int miliseconds = (int)(time * 1000);

        return string.Concat(minutes.ToString(_twoDigitFormatString), _timeSeparatorString, seconds.ToString(_twoDigitFormatString), _timeSeparatorString, miliseconds.ToString(_twoDigitFormatString));
    }

    private void FinishGame(WinnerState winnerState)
    {
        CurrentGameState = GameState.GameOver;

        switch (winnerState)
        {
            case WinnerState.Player1Wins:
                {
                    _winnerLabel.text = _player1.Name + " wins!";
                    break;
                }
            case WinnerState.Player2Wins:
                {
                    _winnerLabel.text = _player2.Name + " wins!";
                    break;
                }
            case WinnerState.Draw:
                {
                    _winnerLabel.text = "It's a draw!";
                    break;
                }
        }

        _endgamePanel.SetActive(true);
    }

    private void Undo()
    {
        BoardState previousBoardState = GetUndoTargetState();
        if (previousBoardState != null)
        {
            CancelCurrentTurn();
            _boardController.LoadBoardState(previousBoardState);
            Player playerToActivate = GetNextPlayer(GetPlayerByFieldOwnerType(previousBoardState.LastActivePlayer));
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

    private void OnMainMenuButtonClicked()
    {
        _menuRequester.RequestMainMenu();
        _contentPanel.SetActive(false);
    }

    public void ApplySkin(Skin skin)
    {
        _skin = skin;
    }
}
