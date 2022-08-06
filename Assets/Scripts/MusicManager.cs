using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    AudioSource source;
    public AudioClip[] clips;
    public int curNum = 1;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = clips[0];
        source.Play();
    }

    void Update()
    {
        transform.position = Camera.main.transform.position;
        if (Input.GetKeyDown(KeyCode.M))
        {
            if (curNum == clips.Length)
            {
                source.clip = clips[0];
                curNum = 1;
                source.Play();
            }
            else
            {
                source.clip = clips[curNum];
                curNum++;
                source.Play();
            }
        }
    }
}
