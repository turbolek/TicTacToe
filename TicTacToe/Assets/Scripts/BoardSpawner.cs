using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpawner : MonoBehaviour
{
    public Action<BoardSpawner> BoardStateChanged;

    [SerializeField]
    private int _boardHeight;
    [SerializeField]
    private int _boardWidth;
    [SerializeField]
    private Transform _boardParent;
    [SerializeField]
    private BoardButton _boardButtonPrefab;
    [SerializeField]
    private GameObject _boardRowPrefab;

    private GameManager _gameManager;
    private BoardButton[] _boardButtons;

    public void Init(GameManager gameManager)
    {
        _boardButtons = new BoardButton[_boardHeight * _boardWidth];
        _gameManager = gameManager;
        int buttonIndex = 0;
        for (int i = 0; i < _boardHeight; i++)
        {
            GameObject row = Instantiate(_boardRowPrefab, _boardParent);
            for (int j = 0; j < _boardWidth; j++)
            {
                BoardButton button = Instantiate(_boardButtonPrefab, row.transform);
                button.ButtonStateChanged += OnFieldStateChanged;
                button.Initialize(buttonIndex, gameManager);
                _boardButtons[buttonIndex] = button;
                buttonIndex++;
            }
        }
    }

    public void Clear()
    {
        for (int i = _boardButtons.Length - 1; i >= 0; i--)
        {
            _boardButtons[i].ButtonStateChanged -= OnFieldStateChanged;
            Destroy(_boardButtons[i]);
        }

        _boardButtons = null;
    }

    private void OnFieldStateChanged(BoardButton button)
    {
        BoardStateChanged?.Invoke(this);
    }
}
