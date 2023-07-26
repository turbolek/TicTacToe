using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameManager _gameManager;

    [SerializeField]
    private StartGameButton[] _startGameButtons;

    private void Start()
    {
        MainMenuRequester.MainMenuRequested += OnMainMenuRequested;

        foreach (var button in _startGameButtons)
        {
            button.Init(OnStartGameButtonClicked);
        }

        Open();
    }

    private void OnDestroy()
    {
        MainMenuRequester.MainMenuRequested -= OnMainMenuRequested;
    }

    private void OnStartGameButtonClicked(StartGameButton button)
    {
        Close();
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
}

