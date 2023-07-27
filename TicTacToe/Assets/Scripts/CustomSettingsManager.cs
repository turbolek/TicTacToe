using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomSettingsManager : MonoBehaviour
{
    [SerializeField]
    private Button _acceptButton;
    [SerializeField]
    private Button _cancelButton;

    [SerializeField]
    private TMP_Dropdown _player1TypeDropdown;
    [SerializeField]
    private TMP_Dropdown _player2TypeDropdown;

    [SerializeField]
    private Slider _timeLimitSlider;
    [SerializeField]
    private TMP_Text _timerLabel;
    [SerializeField]
    private Slider _boardWidth;
    [SerializeField]
    private TMP_Text _widthLabel;
    [SerializeField]
    private Slider _boardHeight;
    [SerializeField]
    private TMP_Text _heightLabel;
    [SerializeField]
    private Slider _requiredSequence;
    [SerializeField]
    private TMP_Text _sequenceLabel;

    private bool _accepted = false;
    private bool _canceled = false;

    private void Start()
    {
        _acceptButton.onClick.AddListener(Accept);
        _cancelButton.onClick.AddListener(Cancel);
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public async Task<GameSettings> GetCustomSettings()
    {
        Show();
        _accepted = false;
        _canceled = false;

        while (!_canceled && !_accepted)
        {
            await Task.Yield();
        }

        GameSettings settings = null;
        if (_accepted)
        {
            settings = new GameSettings();
            settings.BoardHeight = (int)_boardHeight.value;
            settings.BoardWidth = (int)_boardWidth.value;
            settings.RequiredSequenceLength = (int)_requiredSequence.value;
            settings.Player1Type = _player1TypeDropdown.value == 0 ? PlayerType.HumanPlayer : PlayerType.CPU;
            settings.Player2Type = _player2TypeDropdown.value == 0 ? PlayerType.HumanPlayer : PlayerType.CPU;
            settings.TimeLimit = _timeLimitSlider.value;
        }
        Hide();
        return settings;
    }

    private void Accept()
    {
        _accepted = true;
    }

    private void Cancel()
    {
        _canceled = false;
    }

    private void Update()
    {
        _timerLabel.text = "Time limit:" + _timeLimitSlider.value;
        _widthLabel.text = "Board width:" + _boardWidth.value;
        _heightLabel.text = "Board height:" + _boardHeight.value;
        _sequenceLabel.text = "Required sequence:" + _requiredSequence.value;
    }

}
