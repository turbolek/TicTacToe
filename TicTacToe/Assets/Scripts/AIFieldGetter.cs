using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AIFieldGetter : FieldGetter
{
    public override async Task<BoardButton> GetField(BoardSpawner board, CancellationToken cancellationToken)
    {
        List<BoardButton> emptyBoardButtons = new List<BoardButton>();
        foreach (BoardButton boardButton in board.BoardButtons)
        {
            if (boardButton.Owner == null)
            {
                emptyBoardButtons.Add(boardButton); 
            }
        }

        if (emptyBoardButtons.Count > 0)
        {
            return emptyBoardButtons[Random.Range(0, emptyBoardButtons.Count)];
        }
        else
        {
            return null;
        }
    }
}