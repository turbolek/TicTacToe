using System.Threading;
using System.Threading.Tasks;

public abstract class FieldGetter
{
    public abstract Task<int> GetFieldIndex(BoardState boardState, CancellationToken cancellationToken);
}