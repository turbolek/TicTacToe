using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private BoardSpawner _boardSpawner;

    // Start is called before the first frame update
    void Start()
    {
        _boardSpawner.Init();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
