using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player" && other.GetComponent<Player>().isAlive)
        {
            if (FindObjectOfType<GameSettings>().practice)
            {
                int current1 = SceneManager.GetActiveScene().buildIndex + 1;
                if (current1 > 8)
                {
                    current1 = 8;
                }
                if (current1 > PlayerPrefs.GetInt("PracticeLevel", 1))
                {
                    PlayerPrefs.SetInt("PracticeLevel", current1);
                    PlayerPrefs.Save();
                }
                StartCoroutine(LoadNextLevel());
                Time.timeScale = 0;
                return;
            }
            int current = SceneManager.GetActiveScene().buildIndex + 1;
            if (current > 12)
            {
                current--;
            }
            if (current > PlayerPrefs.GetInt("Level", 1))
            {
                PlayerPrefs.SetInt("Level", current);
                PlayerPrefs.Save();
            }
            StartCoroutine(LoadNextLevel());
            Time.timeScale = 0;
        }
    }

    IEnumerator LoadNextLevel()
    {
        if (GameObject.FindGameObjectsWithTag("Checkpoint").Length > 0)
        {
            Destroy(GameObject.FindGameObjectsWithTag("Checkpoint")[0]);
        }
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex == 9 && FindObjectOfType<GameSettings>().practice)
        {
            nextSceneIndex = 13;
        }

        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            nextSceneIndex = 0;
        }

        FindObjectOfType<ScenePersist>().ResetScenePersist();
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneIndex);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
