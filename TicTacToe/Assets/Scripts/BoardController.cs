using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardController
{
    public Action<BoardState> BoardStateChanged;

    private int _boardHeight, _boardWidth;

    private BoardState _boardState;
    public BoardState BoardState
    {
        get
        {
            return _boardState;
        }
        private set
        {
            _boardState = value;
            BoardStateChanged?.Invoke(_boardState);
        }
    }

    public void Init(GameManager gameManager)
    {
        _boardHeight = gameManager.BoardHeight;
        _boardWidth = gameManager.BoardWidth;

        BoardState = new BoardState();
        BoardState.Width = _boardWidth;
        BoardState.Height = _boardHeight;
        BoardState.FieldOwners = new FieldOwnerType[_boardWidth * _boardHeight];
        BoardState.LastActivePlayer = FieldOwnerType.Empty;
    }

    public void SetFieldState(int fieldIndex, FieldOwnerType fieldState)
    {
        BoardState.FieldOwners[fieldIndex] = fieldState;
        BoardState.LastActivePlayer = fieldState;

        BoardStateChanged?.Invoke(BoardState);
    }

    public void SetBoardState(BoardState boardState)
    {
        BoardState = boardState;
        BoardStateChanged?.Invoke(BoardState);
    }

    public bool HasEmptyField()
    {
        foreach (FieldOwnerType field in BoardState.FieldOwners)
        {
            if (field == FieldOwnerType.Empty)
            {
                return true;
            }
        }
        return false;
    }

    public int GetBestMoveForPlayer(Player player)
    {
        int bestButtonIndex = -1;
        List<int> emptyButtonsIndices = new List<int>();

        for (int i = 0; i < BoardState.FieldOwners.Length; i++)
        {
            FieldOwnerType field = BoardState.FieldOwners[i];

            if (field == FieldOwnerType.Empty)
            {
                emptyButtonsIndices.Add(i);
            }
        }

        if (emptyButtonsIndices.Count > 0)
        {
            bestButtonIndex = emptyButtonsIndices[UnityEngine.Random.Range(0, emptyButtonsIndices.Count)];
        }

        return bestButtonIndex;
    }

    public int GetLongestSequence(int sourceIndex)
    {
        int longestSequence = GetHorizontalSequence(sourceIndex);

        int verticalSequence = GetVerticalSequence(sourceIndex);
        if (verticalSequence > longestSequence)
        {
            longestSequence = verticalSequence;
        }

        int diagonalAscendingSequence = GetDiagonalAscendingSequence(sourceIndex);
        if (diagonalAscendingSequence > longestSequence)
        {
            longestSequence = diagonalAscendingSequence;
        }

        int diagonalDescendingSequence = GetDiagonalDescendingSequence(sourceIndex);
        if (diagonalDescendingSequence > longestSequence)

        {
            longestSequence = diagonalDescendingSequence;
        }

        return longestSequence;
    }

    private int GetHorizontalSequence(int sourceIndex)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceIndex, GetLeftNeighbourIndex);
        sequence += GetSingleDirectionSequence(sourceIndex, GetRightNeighbourIndex);
        return sequence;
    }

    private int GetVerticalSequence(int sourceIndex)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceIndex, GetUpNeighbourIndex);
        sequence += GetSingleDirectionSequence(sourceIndex, GetDownNeighbourIndex);
        return sequence;
    }

    private int GetDiagonalDescendingSequence(int sourceIndex)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceIndex, GetUpLeftNeighbourIndex);
        sequence += GetSingleDirectionSequence(sourceIndex, GetDownRightNeighbourIndex);
        return sequence;
    }

    private int GetDiagonalAscendingSequence(int sourceIndex)
    {
        int sequence = 1;
        sequence += GetSingleDirectionSequence(sourceIndex, GetDownLeftNeighbourIndex);
        sequence += GetSingleDirectionSequence(sourceIndex, GetUpRightNeighbourIndex);
        return sequence;
    }

    private int GetSingleDirectionSequence(int sourceIndex, Func<int, int> firstGetNeighbourMethod)
    {
        int sequence = 0;

        int currentIndex = sourceIndex;
        int neighbourIndex = firstGetNeighbourMethod.Invoke(currentIndex);

        while (neighbourIndex >= 0 && BoardState.FieldOwners[neighbourIndex] == BoardState.FieldOwners[currentIndex])
        {
            sequence++;
            currentIndex = neighbourIndex;
            neighbourIndex = firstGetNeighbourMethod.Invoke(currentIndex);
        }

        return sequence;
    }

    private int GetLeftNeighbourIndex(int sourceIndex)
    {
        int sourceIndexInRow = sourceIndex % _boardWidth;
        int targetIndex = -1;

        if (sourceIndexInRow > 0)
        {
            targetIndex = sourceIndex - 1;
        }

        return targetIndex;
    }

    private int GetRightNeighbourIndex(int sourceIndex)
    {
        int sourceIndexInRow = sourceIndex % _boardWidth;

        int tergetIndex = -1;

        if (sourceIndexInRow < _boardWidth - 1)
        {
            tergetIndex = sourceIndex + 1;
        }

        return tergetIndex;
    }

    private int GetUpNeighbourIndex(int sourceIndex)
    {
        int sourceColumnIndex = sourceIndex / _boardWidth;

        int targetIndex = -1;

        if (sourceColumnIndex > 0)
        {
            targetIndex = sourceIndex - _boardWidth;
        }

        return targetIndex;
    }

    private int GetDownNeighbourIndex(int sourceIndex)
    {
        int sourceColumnIndex = sourceIndex / _boardWidth;

        int targetIndex = -1;

        if (sourceColumnIndex < _boardHeight - 1)
        {
            targetIndex = sourceIndex + _boardWidth;
        }

        return targetIndex;
    }

    private int GetUpLeftNeighbourIndex(int sourceIndex)
    {
        int leftNeighbourIndex = GetLeftNeighbourIndex(sourceIndex);
        int leftUpNeighbourIndex = leftNeighbourIndex >= 0 ? GetUpNeighbourIndex(leftNeighbourIndex) : -1;

        return leftUpNeighbourIndex;
    }

    private int GetDownRightNeighbourIndex(int sourceIndex)
    {
        int rightNeighbourIndex = GetRightNeighbourIndex(sourceIndex);
        int rightDownNeighbourIndex = rightNeighbourIndex >= 0 ? GetDownNeighbourIndex(rightNeighbourIndex) : -1;

        return rightDownNeighbourIndex;
    }

    private int GetDownLeftNeighbourIndex(int sourceIndex)
    {
        int leftNeighbourIndex = GetLeftNeighbourIndex(sourceIndex);
        int leftDownNeighbourIndex = leftNeighbourIndex >= 0 ? GetDownNeighbourIndex(leftNeighbourIndex) : -1;

        return leftDownNeighbourIndex;
    }

    private int GetUpRightNeighbourIndex(int sourceButtonIndex)
    {
        int rightNeighbourIndex = GetRightNeighbourIndex(sourceButtonIndex);
        int rightUpNeighbourIndex = rightNeighbourIndex >= 0 ? GetUpNeighbourIndex(rightNeighbourIndex) : -1;

        return rightUpNeighbourIndex;
    }

    public void LoadBoardState(BoardState boardState)
    {
        BoardState = boardState;
    }


}
