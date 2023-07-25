using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class AIFieldGetter : FieldGetter
{
    public override async Task<int> GetFieldIndex(BoardState boardState, CancellationToken cancellationToken)
    {
        int targetFieldIndex = -1;
        List<int> emptyBoardButtonsIndices = new List<int>();
        for (int i = 0; i < boardState.FieldOwners.Length; i++)
        {
            if (boardState.FieldOwners[i] == FieldOwnerType.Empty)
            {
                emptyBoardButtonsIndices.Add(i);
            }
        }

        if (emptyBoardButtonsIndices.Count > 0)
        {
            targetFieldIndex = emptyBoardButtonsIndices[Random.Range(0, emptyBoardButtonsIndices.Count)];
        }

        return targetFieldIndex;
    }
}