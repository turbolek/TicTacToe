using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpawner : MonoBehaviour
{
    public Action<BoardSpawner> BoardStateChanged;


    [SerializeField]
    private Transform _boardParent;
    [SerializeField]
    private BoardButton _boardButtonPrefab;
    [SerializeField]
    private GameObject _boardRowPrefab;

    private GameManager _gameManager;
    private BoardButton[] _boardButtons;
    private GameObject[] _rows;

    public int LongestSequence { get; private set; } = 0;

    private int _boardHeight, _boardWidth;

    public void Init(GameManager gameManager)
    {
        _boardHeight = gameManager.BoardHeight;
        _boardWidth = gameManager.BoardWidth;

        _rows = new GameObject[_boardHeight];
        _boardButtons = new BoardButton[_boardHeight * _boardWidth];
        _gameManager = gameManager;
        int buttonIndex = 0;
        for (int i = 0; i < _boardHeight; i++)
        {
            GameObject row = Instantiate(_boardRowPrefab, _boardParent);
            _rows[i] = row;
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
        if (_boardButtons != null)
        {
            for (int i = _boardButtons.Length - 1; i >= 0; i--)
            {
                _boardButtons[i].ButtonStateChanged -= OnFieldStateChanged;
            }
        }
        _boardButtons = null;

        if (_rows != null)
        {
            for (int i = _rows.Length - 1; i >= 0; i--)
            {
                Destroy(_rows[i].gameObject);
            }
        }
        LongestSequence = 0;
    }

    private void OnFieldStateChanged(BoardButton button)
    {
        LongestSequence = GetLongestSequence(button);
        BoardStateChanged?.Invoke(this);
    }

    private int GetLongestSequence(BoardButton sourceButton)
    {
        int longestSequence = GetHorizaontalSequence(sourceButton);

        int verticalSequence = GetVerticalSequence(sourceButton);
        if (verticalSequence > longestSequence)
        {
            longestSequence = verticalSequence;
        }

        int diagonalAscendingSequence = GetDiagonalAscendingSequence(sourceButton);
        if (diagonalAscendingSequence > longestSequence)
        {
            longestSequence = diagonalAscendingSequence;
        }

        int diagonalDescendingSequence = GetDiagonalDescendingSequence(sourceButton);
        if (diagonalDescendingSequence > longestSequence)

        {
            longestSequence = diagonalDescendingSequence;
        }

        return longestSequence;
    }

    private int GetHorizaontalSequence(BoardButton sourceButton)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceButton, GetLeftNeighbour);
        sequence += GetSingleDirectionSequence(sourceButton, GetRightNeighbour);
        return sequence;
    }

    private int GetVerticalSequence(BoardButton sourceButton)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceButton, GetUpNeighbour);
        sequence += GetSingleDirectionSequence(sourceButton, GetDownNeighbour);
        return sequence;
    }

    private int GetDiagonalDescendingSequence(BoardButton sourceButton)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceButton, GetUpLeftNeighbour);
        sequence += GetSingleDirectionSequence(sourceButton, GetDownRightNeighbour);
        return sequence;
    }

    private int GetDiagonalAscendingSequence(BoardButton sourceButton)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceButton, GetDownLeftNeighbour);
        sequence += GetSingleDirectionSequence(sourceButton, GetUpRightNeighbour);
        return sequence;
    }

    private int GetSingleDirectionSequence(BoardButton sourceButton, Func<BoardButton, BoardButton> firstGetNeighbourMethod)
    {
        int sequence = 0;

        BoardButton currentButton = sourceButton;
        BoardButton neighbour = firstGetNeighbourMethod.Invoke(currentButton);

        while (neighbour != null && neighbour.Owner == currentButton.Owner)
        {
            sequence++;
            currentButton = neighbour;
            neighbour = firstGetNeighbourMethod.Invoke(currentButton);
        }

        return sequence;
    }

    private BoardButton GetLeftNeighbour(BoardButton sourceButton)
    {
        int sourceIndex = sourceButton.Index;
        int sourceIndexInRow = sourceIndex % _boardWidth;

        BoardButton targetButton = null;

        if (sourceIndexInRow > 0)
        {
            targetButton = _boardButtons[sourceIndex - 1];
        }

        return targetButton;
    }

    private BoardButton GetRightNeighbour(BoardButton sourceButton)
    {
        int sourceIndex = sourceButton.Index;
        int sourceIndexInRow = sourceIndex % _boardWidth;

        BoardButton targetButton = null;

        if (sourceIndexInRow < _boardWidth - 1)
        {
            targetButton = _boardButtons[sourceIndex + 1];
        }

        return targetButton;
    }

    private BoardButton GetUpNeighbour(BoardButton sourceButton)
    {
        int sourceIndex = sourceButton.Index;
        int sourceColumnIndex = sourceIndex / _boardWidth;

        BoardButton targetButton = null;

        if (sourceColumnIndex > 0)
        {
            targetButton = _boardButtons[sourceIndex - _boardWidth];
        }

        return targetButton;
    }

    private BoardButton GetDownNeighbour(BoardButton sourceButton)
    {
        int sourceIndex = sourceButton.Index;
        int sourceColumnIndex = sourceIndex / _boardWidth;

        BoardButton targetButton = null;

        if (sourceColumnIndex < _boardHeight - 1)
        {
            targetButton = _boardButtons[sourceIndex + _boardWidth];
        }

        return targetButton;
    }

    private BoardButton GetUpLeftNeighbour(BoardButton sourceButton)
    {
        BoardButton leftNeighbour = GetLeftNeighbour(sourceButton);
        BoardButton leftUpNeighbour = leftNeighbour != null ? GetUpNeighbour(leftNeighbour) : null;

        return leftUpNeighbour;
    }

    private BoardButton GetDownRightNeighbour(BoardButton sourceButton)
    {
        BoardButton rightNeighbour = GetRightNeighbour(sourceButton);
        BoardButton rightDownNeighbour = rightNeighbour != null ? GetDownNeighbour(rightNeighbour) : null;

        return rightDownNeighbour;
    }

    private BoardButton GetDownLeftNeighbour(BoardButton sourceButton)
    {
        BoardButton leftNeighbour = GetLeftNeighbour(sourceButton);
        BoardButton leftDownNeighbour = leftNeighbour != null ? GetDownNeighbour(leftNeighbour) : null;

        return leftDownNeighbour;
    }

    private BoardButton GetUpRightNeighbour(BoardButton sourceButton)
    {
        BoardButton rightNeighbour = GetRightNeighbour(sourceButton);
        BoardButton rightUpNeighbour = rightNeighbour != null ? GetUpNeighbour(rightNeighbour) : null;

        return rightUpNeighbour;
    }
}
