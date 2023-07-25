using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Player
{
    public string Name { get; private set; }
    public string Mark;
    public bool IsActive { get; private set; }

    public FieldOwnerType FieldOwnerType { get; private set; }
    public PlayerType Type { get; private set; }
    private FieldGetter _fieldGetter;

    public Player(string name, PlayerType type, FieldOwnerType fieldOwnerType)
    {
        Type = type;
        Name = name;

        _fieldGetter = GetFieldGetter(type);
        FieldOwnerType = fieldOwnerType;
    }

    public void Activate()
    {
        IsActive = true;
    }

    public async Task<int> GetFieldIndex(BoardState board, CancellationToken cancellationToken)
    {
        int fieldIndex = await _fieldGetter?.GetFieldIndex(board, cancellationToken);
        return fieldIndex;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    private FieldGetter GetFieldGetter(PlayerType playerType)
    {
        switch (playerType)
        {
            case PlayerType.HumanPlayer:
                {
                    return new ButtonFieldGetter();
                }
            case PlayerType.CPU:
                {
                    return new AIFieldGetter();
                }
        }

        return null;
    }
}
