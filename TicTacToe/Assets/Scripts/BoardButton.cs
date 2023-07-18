using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardButton : MonoBehaviour
{
    public event Action<BoardButton> ButtonStateChanged;

    [SerializeField]
    private Button _button;
    [SerializeField]
    private TMP_Text _text;
    public int Index { get; private set; }
    public BoardFieldState State { get; private set; }

    private GameManager _gameManager;

    public void Initialize(int index, GameManager gameManager)
    {
        _gameManager = gameManager;
        Index = index;
        _button.onClick.AddListener(OnButtonClicked);
        SetState(BoardFieldState.Empty);
    }

    private void OnButtonClicked()
    {
        switch (_gameManager.CurrentGameState)
        {
            case GameState.Player1Turn:
                {
                    SetState(BoardFieldState.Player1);
                    break;
                }
            case GameState.Player2Turn:
                {
                    SetState(BoardFieldState.Player2);
                    break;
                }
        }
    }

    public void SetState(BoardFieldState state)
    {
        State = state;
        _button.interactable = State == BoardFieldState.Empty;

        switch (state)
        {
            case BoardFieldState.Player1:
                {
                    _text.text = "X";
                    break;
                }
            case BoardFieldState.Player2:
                {
                    _text.text = "O";
                    break;
                }
            case BoardFieldState.Empty:
                {
                    _text.text = "";
                    break;
                }
        }

        ButtonStateChanged?.Invoke(this);
    }
}
