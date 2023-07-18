using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BoardSpawner _boardSpawner;

    public GameState CurrentGameState { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        CurrentGameState = GameState.Setup;
        _boardSpawner.BoardStateChanged += OnBoardStateChanged;
        _boardSpawner.Init(this);
        CurrentGameState = GameState.Player1Turn;
    }



    private void OnBoardStateChanged(BoardSpawner board)
    {
        switch (CurrentGameState)
        {
            case GameState.Player1Turn:
                {
                    CurrentGameState = GameState.Player2Turn;
                    break;
                }
            case GameState.Player2Turn:
                {
                    CurrentGameState = GameState.Player1Turn;
                    break;
                }
        }
    }
}
