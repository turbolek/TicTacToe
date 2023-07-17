using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardSpawner : MonoBehaviour
{
    [SerializeField]
    private int _boardHeight;
    [SerializeField]
    private int _boardWidth;
    [SerializeField]
    private Transform _boardParent;
    [SerializeField]
    private BoardButton _boardButtonPrefab;
    [SerializeField]
    private GameObject _boardRowPrefab;

    public void Init()
    {
        int buttonIndex = 0;
        for (int i = 0; i < _boardHeight; i++)
        {
            GameObject row = Instantiate(_boardRowPrefab, _boardParent);
            for (int j = 0; j < _boardWidth; j++)
            {
                BoardButton button = Instantiate(_boardButtonPrefab, row.transform);
                button.Initialize(buttonIndex);
                buttonIndex++;
            }
        }
    }
}
