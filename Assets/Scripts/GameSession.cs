using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameSession : MonoBehaviour
{
    int playerLives = 25;
    [SerializeField] int score = 0;
    [SerializeField] TextMeshProUGUI livesText;
    [SerializeField] TextMeshProUGUI scoreText;
    public GameObject speedrunTimer;

    void Awake()
    {
        if (FindObjectOfType<GameSettings>() && FindObjectOfType<GameSettings>().speedrun && !FindObjectOfType<GameSettings>().coin)
        {
            foreach (CoinPickup coin in FindObjectsOfType<CoinPickup>())
            {
                Destroy(coin.gameObject);
            }
        }
        if (FindObjectOfType<GameSettings>() && !FindObjectOfType<GameSettings>().tutorial)
        {
            Destroy(transform.GetChild(0).GetChild(2).gameObject);
        }
        Time.timeScale = 1;
        int numGameSessions = FindObjectsOfType<GameSession>().Length;
        if (numGameSessions > 1)
        {
            if (FindObjectsOfType<GameSession>()[0] == this)
            {
                if (FindObjectsOfType<GameSession>()[1].transform.GetChild(0).childCount == 3
                && FindObjectsOfType<GameSession>()[1].transform.GetChild(0).GetChild(2).name != "Speedrun Timer")
                    Destroy(FindObjectsOfType<GameSession>()[1].transform.GetChild(0).GetChild(2).gameObject);
                if (FindObjectOfType<GameSettings>() && FindObjectOfType<GameSettings>().tutorial)
                    transform.GetChild(0).GetChild(2).SetParent(FindObjectsOfType<GameSession>()[1].transform.GetChild(0));
            }
            else
            {
                if (FindObjectsOfType<GameSession>()[0].transform.GetChild(0).childCount == 3
                && FindObjectsOfType<GameSession>()[0].transform.GetChild(0).GetChild(2).name != "Speedrun Timer")
                    Destroy(FindObjectsOfType<GameSession>()[0].transform.GetChild(0).GetChild(2).gameObject);
                if (FindObjectOfType<GameSettings>() && FindObjectOfType<GameSettings>().tutorial)
                    transform.GetChild(0).GetChild(2).SetParent(FindObjectsOfType<GameSession>()[0].transform.GetChild(0));
            }
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
            if (FindObjectOfType<GameSettings>() && FindObjectOfType<GameSettings>().speedrun)
            {
                Instantiate(speedrunTimer, transform.GetChild(0), false).name = "Speedrun Timer";
            }
        }
    }


    void Update()
    {
        livesText.text = playerLives.ToString();
        scoreText.text = score.ToString();
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(FindObjectOfType<GameSession>().gameObject);
            Destroy(FindObjectOfType<GameSettings>().gameObject);
            Destroy(FindObjectOfType<MusicManager>().gameObject);
            if (FindObjectOfType<ScenePersist>())
                Destroy(FindObjectOfType<ScenePersist>().gameObject);
            SceneManager.LoadScene(0);
        }
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            StartCoroutine(TakeLife());
        }
        else
        {
            StartCoroutine(ResetGameSession());
        }
    }

    public void AddToScore(int pointsToAdd)
    {
        score += pointsToAdd;
        scoreText.text = score.ToString();
    }

    IEnumerator TakeLife()
    {
        playerLives--;
        livesText.text = playerLives.ToString();
        yield return new WaitForSeconds(2);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }

    IEnumerator ResetGameSession()
    {
        playerLives--;
        livesText.text = playerLives.ToString();
        yield return new WaitForSeconds(2);
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        SceneManager.LoadScene(0);
        Destroy(gameObject);
    }
}
