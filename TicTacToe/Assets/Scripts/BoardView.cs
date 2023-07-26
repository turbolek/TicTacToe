using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class BoardView : MonoBehaviour, ISkinable
{
    [SerializeField]
    private Transform _boardParent;
    private RectTransform _boardRect;

    [SerializeField]
    private BoardButton _boardButtonPrefab;
    [SerializeField]
    private GameObject _boardRowPrefab;
    [SerializeField]
    private Image _verticalSeparatorPrefab;
    [SerializeField]
    private Image _horizontalSeparatorPrefab;
    [SerializeField]
    private Image _background;

    private BoardController _boardController;
    private GameManager _gameManager;

    public BoardButton[] BoardButtons { get; private set; }
    private RectTransform[] _rows;

    private List<Image> _separators = new List<Image>();

    public void Init(GameManager gameManager, BoardController boardController)
    {
        _gameManager = gameManager;
        _boardController = boardController;
        _boardRect = _boardParent.GetComponent<RectTransform>();

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
            if (boardState.FieldOwners != null && boardButton.Index < boardState.FieldOwners.Length)
            {
                FieldOwnerType fieldOwnerType = boardState.FieldOwners[boardButton.Index];
                boardButton.SetOwner(_gameManager.GetPlayerByFieldOwnerType(fieldOwnerType));
            }
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

        _rows = new RectTransform[boardHeight];
        BoardButtons = new BoardButton[boardHeight * boardWidth];
        int buttonIndex = 0;
        for (int i = 0; i < boardHeight; i++)
        {
            RectTransform row = Instantiate(_boardRowPrefab, _boardParent).GetComponent<RectTransform>();
            _rows[i] = row;
            for (int j = 0; j < boardWidth; j++)
            {
                BoardButton button = Instantiate(_boardButtonPrefab, row.transform);
                button.Initialize(buttonIndex);
                BoardButtons[buttonIndex] = button;
                buttonIndex++;
            }
        }

        Canvas.ForceUpdateCanvases();

        _separators = new List<Image>();
        SpawnVerticalSeparators(boardWidth);
        SpawnHorizontalSeparators();
    }

    private void SpawnVerticalSeparators(int width)
    {
        if (width <= 1)
        {
            return;
        }

        for (int i = 1; i < width; i++)
        {
            BoardButton boardButton = BoardButtons[i];
            BoardButton previousButton = BoardButtons[i - 1];

            Image separator = Instantiate(_verticalSeparatorPrefab, _boardParent);
            _separators.Add(separator);
            RectTransform rectTransform = separator.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 0f);
            rectTransform.anchorMax = new Vector2(0f, 1f);

            float xPosition = (boardButton.RectTransform.anchoredPosition.x + previousButton.RectTransform.anchoredPosition.x) / 2f;

            rectTransform.anchoredPosition = new Vector2(xPosition, 0f);
            rectTransform.sizeDelta = new Vector2(_boardRect.rect.height, _boardRect.rect.width);
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, 0f);
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, 0f);

        }
    }

    private void SpawnHorizontalSeparators()
    {
        if (_rows.Length <= 1)
        {
            return;
        }

        for (int i = 1; i < _rows.Length; i++)
        {
            RectTransform row = _rows[i];
            RectTransform previousRow = _rows[i - 1];

            Image separator = Instantiate(_horizontalSeparatorPrefab, _boardParent);
            RectTransform rectTransform = separator.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0f, 1f);
            rectTransform.anchorMax = new Vector2(1f, 1f);

            float yPosition = (row.anchoredPosition.y + previousRow.anchoredPosition.y) / 2f;

            rectTransform.anchoredPosition = new Vector2(0f, yPosition);
            rectTransform.sizeDelta = new Vector2(_boardRect.rect.width, _boardRect.rect.height);
            rectTransform.offsetMin = new Vector2(0f, rectTransform.offsetMin.y);
            rectTransform.offsetMax = new Vector2(0f, rectTransform.offsetMax.y);
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

        if (_separators != null)
        {
            for (int i = _separators.Count - 1; i >= 0; i--)
            {
                Destroy(_separators[i].gameObject);
            }
        }

        _separators = null;
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

    public void ApplySkin(Skin skin)
    {
        _background.sprite = skin.Background;
    }
}
