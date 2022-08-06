using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndgameManager : MonoBehaviour
{
    void Start()
    {
        Destroy(FindObjectOfType<GameSession>().gameObject);
        Destroy(FindObjectOfType<GameSettings>().gameObject);
        Destroy(FindObjectOfType<MusicManager>().gameObject);
        if (GameObject.FindGameObjectsWithTag("Checkpoint").Length > 0)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Checkpoint")[0]);
        }
    }
    public void PlayAgain()
    {
        SceneManager.LoadScene(0);
    }
}
