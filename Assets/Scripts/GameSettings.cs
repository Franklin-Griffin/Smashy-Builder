using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public bool speedrun = false;
    public bool tutorial = true;
    public bool coin = true;
    public bool practice = false;
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void TutorialToggle()
    {
        tutorial = !tutorial;
    }
    public void CoinToggle()
    {
        coin = !coin;
    }
}
