using System.Threading;
using System.Threading.Tasks;

public class ButtonFieldGetter : FieldGetter
{
    private BoardButton _clickedButton;

    public override async Task<BoardButton> GetField(BoardSpawner board, CancellationToken cancellationToken)
    {
        _clickedButton = null;
        BoardButton.ButtonClicked += OnButtonClicked;

        while (_clickedButton == null)
        {
            await Task.Yield();
        }

        BoardButton.ButtonClicked -= OnButtonClicked;
        return _clickedButton;
    }

    private void OnButtonClicked(BoardButton clickedButton)
    {
        if (clickedButton.Owner == null)
        {
            _clickedButton = clickedButton;
        }
    }
}
