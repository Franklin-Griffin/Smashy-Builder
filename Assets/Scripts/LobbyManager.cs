using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
public class LobbyManager : MonoBehaviour
{
    public Button play;
    public Button speedrun;
    public GameObject home;
    public GameObject lvlSelect;
    public Button[] buttons = new Button[8];
    int level = 1;
    int levelsUnlocked = 0;
    int practiceLevelsUnlocked = 0;
    void Awake()
    {
        if (PlayerPrefs.HasKey("Level"))
        {
            levelsUnlocked = PlayerPrefs.GetInt("Level", 1);
            practiceLevelsUnlocked = PlayerPrefs.GetInt("PracticeLevel", 1);
        }
        else
        {
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("PracticeLevel", 1);
            PlayerPrefs.Save();
        }
    }
    public void Play()
    {
        StartCoroutine("LoadLevel");
    }
    public void Speedrun()
    {
        FindObjectOfType<GameSettings>().speedrun = true;
        FindObjectOfType<GameSettings>().tutorial = false;
        StartCoroutine("LoadLevel");
    }
    IEnumerator LoadLevel()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(level);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    public void LevelSelect()
    {
        lvlSelect.SetActive(true);
        home.SetActive(false);
        for (int i = 1; i < levelsUnlocked; i++)
        {
            buttons[i].interactable = true;
        }
    }
    public void Back()
    {
        lvlSelect.SetActive(false);
        home.SetActive(true);
        FindObjectOfType<GameSettings>().practice = false;
        for (int i = 1; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
    }
    public void LevelChosen(int newLevel)
    {
        level = newLevel;
        StartCoroutine("LoadLevel");
    }
    public void ClearData()
    {
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("PracticeLevel", 1);
        for (int i = 1; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        PlayerPrefs.Save();
    }
    public void Practice()
    {
        lvlSelect.SetActive(true);
        home.SetActive(false);
        FindObjectOfType<GameSettings>().practice = true;
        for (int i = 1; i < practiceLevelsUnlocked; i++)
        {
            buttons[i].interactable = true;
        }
    }
}
