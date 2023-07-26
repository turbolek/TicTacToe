using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;
    [SerializeField]
    private SkinManager _skinManager;

    [SerializeField]
    private StartGameButton[] _startGameButtons;

    [SerializeField]
    private Button _loadSkinButton;
    [SerializeField]
    private TMP_InputField _skinNameTextField;

    private void Start()
    {
        MainMenuRequester.MainMenuRequested += OnMainMenuRequested;

        foreach (var button in _startGameButtons)
        {
            button.Init(OnStartGameButtonClicked);
        }

        _loadSkinButton.onClick.AddListener(OnSkinButtonClicked);

        _skinManager.Init();
        Open();
    }

    private void OnDestroy()
    {
        MainMenuRequester.MainMenuRequested -= OnMainMenuRequested;
    }

    private void OnStartGameButtonClicked(StartGameButton button)
    {
        Close();
        _skinManager.ApplyCurrentSkin();
        _gameManager.StartGame(button.GameSettings);
    }

    private void OnMainMenuRequested()
    {
        Open();
    }

    private void Open()
    {
        gameObject.SetActive(true);
    }

    private void Close()
    {
        gameObject.SetActive(false);
    }

    private async void OnSkinButtonClicked()
    {
        _loadSkinButton.interactable = false;
        await _skinManager.LoadSkin(_skinNameTextField.text);
        _loadSkinButton.interactable = true;
    }
}

