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
    public Player Owner { get; private set; }

    public void Initialize(int index)
    {
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

    public void Highlight(Player player)
    {
        if (Owner == null)
        {
            _text.color = Color.red;
            _text.text = player.Mark;
        }
    }

    public void ClearHiglight()
    {
        _text.color = Color.black;

        if (Owner == null)
        {
            _text.text = "";
        }
    }
}
