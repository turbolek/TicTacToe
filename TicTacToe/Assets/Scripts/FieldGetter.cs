using System.Threading;
using System.Threading.Tasks;

public abstract class FieldGetter
{
    public abstract Task<BoardButton> GetField(BoardSpawner board, CancellationToken cancellationToken);
}