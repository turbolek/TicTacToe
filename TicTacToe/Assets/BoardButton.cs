using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardButton : MonoBehaviour
{
    [SerializeField]
    private Button _button;
    public int Index { get; private set; }

    public void Initialize(int index)
    {
        Index = index;
        _button.onClick.AddListener(OnButtonClicked);
    }

    private void OnButtonClicked()
    {
        Debug.Log(Index);
    }
}
