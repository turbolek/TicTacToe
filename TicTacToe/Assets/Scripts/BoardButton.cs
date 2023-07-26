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
    private Image _icon;
    public int Index { get; private set; }
    public Player Owner { get; private set; }

    private Color _originalColor;
    private Color _highlightColor;

    public void Initialize(int index)
    {
        Index = index;
        _button.onClick.AddListener(OnButtonClicked);
        SetOwner(null);

        _originalColor = _icon.color;
        _highlightColor = new Color(_originalColor.r, _originalColor.g, _originalColor.b, _originalColor.a / 2f);
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
            _icon.sprite = owner.Mark;
            _icon.enabled = true;
        }
        else
        {
            _icon.sprite = null;
        }

        ButtonStateChanged?.Invoke(this);
    }

    public void Highlight(Player player)
    {
        if (Owner == null)
        {
            _icon.color = _highlightColor;
            _icon.sprite = player.Mark;
            _icon.enabled = true;
        }
    }

    public void ClearHiglight()
    {
        _icon.color = _originalColor;

        if (Owner == null)
        {
            _icon.sprite = null;
            _icon.enabled = false;

        }
    }
}
