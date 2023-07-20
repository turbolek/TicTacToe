using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Button _startGameButton;

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

    private Player _player1;
    private Player _player2;
    public Player ActivePlayer { get; private set; }

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
        _player1 = new Player("Player 1");
        _player2 = new Player("Player 2");
        ActivePlayer = GetStartingPlayer();

        ActivePlayer.Mark = "X";
        GetNextPlayer().Mark = "O";

        _startGameButton.gameObject.SetActive(false);
        CurrentGameState = GameState.Setup;
        _boardSpawner.Clear();
        _boardSpawner.Init(this);
        CurrentGameState = GameState.Gameplay;
    }

    private Player GetStartingPlayer()
    {
        float diceRoll = Random.Range(0f, 1f);
        return diceRoll < 0.5f ? _player1 : _player2;
    }

    private Player GetNextPlayer()
    {
        return ActivePlayer == _player1 ? _player2 : _player1;
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
                        ActivePlayer = GetNextPlayer();
                        Debug.Log(ActivePlayer.Name + "'s turn");
                    }
                    break;
                }
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
