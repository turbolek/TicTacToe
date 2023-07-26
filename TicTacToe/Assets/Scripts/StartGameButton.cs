using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StartGameButton : SerializedMonoBehaviour
{
    [SerializeField]
    private GameSettings _gameSettings;
    public GameSettings GameSettings => _gameSettings;

    private Action<StartGameButton> _onClickCallback;
    private Button _button;

    public void Init(Action<StartGameButton> onClickCallback)
    {
        _button = GetComponent<Button>();
        _onClickCallback = onClickCallback;
        _button.onClick.AddListener(OnClicked);
    }

    private void OnClicked()
    {
        _onClickCallback?.Invoke(this);
    }
}
