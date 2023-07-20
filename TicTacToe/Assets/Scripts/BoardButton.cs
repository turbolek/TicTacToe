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

    private GameManager _gameManager;
    public Player Owner { get; private set; }

    public void Initialize(int index, GameManager gameManager)
    {
        _gameManager = gameManager;
        Index = index;
        _button.onClick.AddListener(OnButtonClicked);
        SetOwner(null);
    }

    private void OnButtonClicked()
    {
        switch (_gameManager.CurrentGameState)
        {
            case GameState.Gameplay:
                {
                    SetOwner(_gameManager.ActivePlayer);
                    break;
                }
        }
    }

    public void SetOwner(Player owner)
    {
        Owner = owner;
        if (owner != null)
        {
            _text.text = owner.Mark;
        }
        else
        {
            _text.text = "";
        }

        ButtonStateChanged?.Invoke(this);
    }
}
