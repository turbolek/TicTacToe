using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BoardButton : MonoBehaviour
{
    public static event Action<BoardButton> ButtonStateChanged;
    public static event Action<BoardButton> ButtonClicked;

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
        ButtonClicked?.Invoke(this);
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
