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
    private Button _customSettingsButton;

    [SerializeField]
    private Button _loadSkinButton;
    [SerializeField]
    private TMP_InputField _skinNameTextField;

    [SerializeField]
    private CustomSettingsManager _customSettingsManager;

    private void Start()
    {
        MainMenuRequester.MainMenuRequested += OnMainMenuRequested;

        foreach (var button in _startGameButtons)
        {
            button.Init(OnStartGameButtonClicked);
        }

        _loadSkinButton.onClick.AddListener(OnSkinButtonClicked);
        _customSettingsButton.onClick.AddListener(OnCustomSettingsButtonClicked);

        _skinManager.Init();
        Open();
    }

    private void OnDestroy()
    {
        MainMenuRequester.MainMenuRequested -= OnMainMenuRequested;
    }

    private void OnStartGameButtonClicked(StartGameButton button)
    {
        StartGame(button.GameSettings);
    }

    private void StartGame(GameSettings settings)
    {
        Close();
        _skinManager.ApplyCurrentSkin();
        _gameManager.StartGame(settings);
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

    private async void OnCustomSettingsButtonClicked()
    {
        Close();
        GameSettings customSettings = await _customSettingsManager.GetCustomSettings();
        if (_customSettingsButton != null)
        {
            StartGame(customSettings);
        }
        else
        {
            Open();
        }
    }
}

