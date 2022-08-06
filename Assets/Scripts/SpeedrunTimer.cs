using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedrunTimer : MonoBehaviour
{
    private TextMeshProUGUI textMesh;
    private float time = 0;
    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        time += Time.deltaTime;
        float minutes = Mathf.Floor(time / 60);
        float seconds = Mathf.RoundToInt(time % 60);
        string min = minutes.ToString();
        string secs = seconds.ToString();
        if (minutes < 10)
        {
            min = "0" + minutes.ToString();
        }
        if (seconds < 10)
        {
            secs = "0" + seconds.ToString();
        }
        textMesh.text = min + ":" + secs;
    }
}
