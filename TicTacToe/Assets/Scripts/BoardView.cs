using System.Collections;
using UnityEngine;

public class BoardView : MonoBehaviour
{
    [SerializeField]
    private Transform _boardParent;
    [SerializeField]
    private BoardButton _boardButtonPrefab;
    [SerializeField]
    private GameObject _boardRowPrefab;

    private BoardController _boardController;
    private GameManager _gameManager;

    public BoardButton[] BoardButtons { get; private set; }
    private GameObject[] _rows;

    public void Init(GameManager gameManager, BoardController boardController)
    {
        _gameManager = gameManager;
        _boardController = boardController;

        Clear();

        _boardController.BoardStateChanged += DisplayBoardState;
        SpawnButtons(_boardController.BoardState.Width, _boardController.BoardState.Height);
        DisplayBoardState(_boardController.BoardState);
    }

    private void DisplayBoardState(BoardState boardState)
    {
        ClearBoardHiglight();

        foreach (BoardButton boardButton in BoardButtons)
        {
            FieldOwnerType fieldOwnerType = boardState.FieldOwners[boardButton.Index];
            boardButton.SetOwner(_gameManager.GetPlayerByFieldOwnerType(fieldOwnerType));
        }
    }

    public void ShowHintForPlayer(Player player)
    {
        int hintButtonIndex = _boardController.GetBestMoveForPlayer(player);
        foreach (BoardButton button in BoardButtons)
        {
            if (button.Index == hintButtonIndex)
            {
                button.Highlight(player);
            }
        }
    }

    private void SpawnButtons(int boardWidth, int boardHeight)
    {

        _rows = new GameObject[boardHeight];
        BoardButtons = new BoardButton[boardHeight * boardWidth];
        int buttonIndex = 0;
        for (int i = 0; i < boardHeight; i++)
        {
            GameObject row = Instantiate(_boardRowPrefab, _boardParent);
            _rows[i] = row;
            for (int j = 0; j < boardWidth; j++)
            {
                BoardButton button = Instantiate(_boardButtonPrefab, row.transform);
                button.Initialize(buttonIndex);
                BoardButtons[buttonIndex] = button;
                buttonIndex++;
            }
        }
    }
    private void ClearBoard()
    {
        if (_rows != null)
        {
            for (int i = _rows.Length - 1; i >= 0; i--)
            {
                Destroy(_rows[i].gameObject);
            }
        }

        _rows = null;
        BoardButtons = null;
    }

    public void Clear()
    {
        ClearBoard();
        _boardController.BoardStateChanged -= DisplayBoardState;

    }

    private void ClearBoardHiglight()
    {
        if (BoardButtons != null)
        {
            foreach (BoardButton b in BoardButtons)
            {
                b?.ClearHiglight();
            }
        }
    }
}
