using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public bool StartScreen = true;
    public PlayerParent player;
    public GameObject TapToStart;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(StartScreen == true)
        {
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                player.speed = 15;
                TapToStart.SetActive(false);
                StartScreen = false;
            }
        }
    }
}
