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

    public GameState CurrentGameState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        _boardSpawner.BoardStateChanged += OnBoardStateChanged;
        _startGameButton.onClick.AddListener(StartGame);
    }

    private void StartGame()
    {
        _startGameButton.gameObject.SetActive(false);
        CurrentGameState = GameState.Setup;
        _boardSpawner.Clear();
        _boardSpawner.Init(this);
        CurrentGameState = GameState.Player1Turn;
    }

    private void OnBoardStateChanged(BoardSpawner board)
    {
        switch (CurrentGameState)
        {
            case GameState.Player1Turn:
                {
                    if (board.LongestSequence >= _requiredSequenceLength)
                    {
                        CurrentGameState = GameState.GameOver;
                        Debug.Log("Player 1 wins!");
                        _startGameButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        CurrentGameState = GameState.Player2Turn;
                    }
                    break;
                }
            case GameState.Player2Turn:
                {
                    if (board.LongestSequence >= _requiredSequenceLength)
                    {
                        CurrentGameState = GameState.GameOver;
                        Debug.Log("Player 2 wins!");
                        _startGameButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        CurrentGameState = GameState.Player1Turn;
                    }
                    break;
                }
        }
    }
}
