using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class Boss : MonoBehaviour
{
    public GameObject eyes;
    public GameObject minion;
    public GameObject exit;
    public GameObject hp;
    Image miniBox;
    Slider _hp;
    [SerializeField] float spinSpeed = 0;
    [SerializeField] Vector3 moveSpeed;
    [SerializeField] bool vulnerable;
    [SerializeField] bool frozen;
    [SerializeField] bool hit;
    [SerializeField] int health = 10;
    SpriteRenderer render;
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.color = new Color(0, 0, 0, 0);
        StartCoroutine("BossLoop");
    }
    void Update()
    {
        if (_hp)
        {
            _hp.value = health * 0.1f;
        }
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + Time.deltaTime * 360 * spinSpeed);
        transform.position += moveSpeed * Time.deltaTime;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.name == "BattleHammer" || other.name == "SledgeHammer") && _hp)
        {
            if (vulnerable)
            {
                health--;
                vulnerable = false;
                hit = true;
            }
            else if (!frozen && !hit)
            {
                StartCoroutine("VulnerableWaiter");
            }
        }
    }
    IEnumerator BossLoop()
    {
        do
        {
            float val = Mathf.Clamp(render.color.r + 0.2f * Time.deltaTime, 0, 1);
            render.color = new Color(val, 0, 0, val);
            spinSpeed = val * 3;
            transform.localScale = new Vector3(val * 25, val * 25, 1);
            yield return new WaitForEndOfFrame();
        } while (render.color.r < 1);
        for (int i = 1; i < 5; i++)
        {
            spinSpeed = -spinSpeed;
            yield return new WaitForSeconds(1.5f / i);
        }
        spinSpeed = 0;
        Instantiate(eyes, transform);
        _hp = Instantiate(hp, FindObjectOfType<GameSession>().transform.GetChild(0)).GetComponent<Slider>();
        miniBox = _hp.transform.GetChild(2).GetChild(0).GetComponent<Image>();
        miniBox.color = Color.gray;
        yield return new WaitForSeconds(1);
        StartCoroutine("Pulse");
        do
        {
            while (vulnerable || frozen || hit)
            {
                yield return new WaitForEndOfFrame();
            }
            Fog newMinion = Instantiate(minion, transform.position, Quaternion.identity).GetComponent<Fog>();
            newMinion.spinSpeed = Random.Range(0, 30 / health);
            float move = Random.Range(1, 10 / health);
            newMinion.transform.localScale = new Vector3(1 + 10 / health, 1 + 10 / health, 1 + 10 / health);
            switch (Mathf.RoundToInt(Random.Range(0, 7)))
            {
                case 0:
                    newMinion.moveSpeed = new Vector3(-move, move * Random.Range(0, 2.0f), 0);
                    break;
                case 1:
                    newMinion.moveSpeed = new Vector3(move, -move * Random.Range(0, 2.0f), 0);
                    break;
                case 2:
                    newMinion.moveSpeed = new Vector3(-move, -move * Random.Range(0, 2.0f), 0);
                    break;
                case 3:
                    newMinion.moveSpeed = new Vector3(move, move * Random.Range(0, 2.0f), 0);
                    break;
                case 4:
                    newMinion.moveSpeed = new Vector3(-move, 0 * Random.Range(0, 2.0f), 0);
                    break;
                case 5:
                    newMinion.moveSpeed = new Vector3(move, 0 * Random.Range(0, 2.0f), 0);
                    break;
                case 6:
                    newMinion.moveSpeed = new Vector3(0, -move * Random.Range(0, 2.0f), 0);
                    break;
                case 7:
                    newMinion.moveSpeed = new Vector3(0, move * Random.Range(0, 2.0f), 0);
                    break;
            }
            if (Random.Range(0, 10 / health) > 5)
            {
                newMinion.breakable = false;
                newMinion.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
            }
            yield return new WaitForSeconds(health / 40 + 0.5f);
        } while (health > 0);
    }
    IEnumerator Pulse()
    {
        do
        {
            while (vulnerable || frozen || hit)
            {
                yield return new WaitForEndOfFrame();
            }
            spinSpeed = 10 / health * Random.Range(0.75f, 1.25f);
            if (Random.Range(0, 100) > 50)
            {
                spinSpeed = -spinSpeed;
            }
            float scale = (health * 1.25f + 12.5f) * Random.Range(0.75f, 1.25f);
            transform.localScale = new Vector3(scale, scale, 1);
            yield return new WaitForSeconds(health * Random.Range(0.3f, 0.4f));
        } while (health > 0);
    }
    IEnumerator VulnerableWaiter()
    {
        frozen = true;
        float time = health * 2;
        render.color = Color.black;
        miniBox.color = Color.black;
        spinSpeed = 0;
        do
        {
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        } while (time > health * 1.75f);
        frozen = false;
        vulnerable = true;
        render.color = Color.white;
        miniBox.color = Color.white;
        while (!hit && time > 0)
        {
            time -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        if (hit)
        {
            if (health == 0)
            {
                render.color = Color.magenta;
                miniBox.color = Color.magenta;
                yield return new WaitForSeconds(1);
                Instantiate(exit, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
            render.color = Color.green;
            miniBox.color = Color.green;
            yield return new WaitForSeconds(1);
        }
        vulnerable = false;
        frozen = false;
        hit = false;
        render.color = Color.red;
        miniBox.color = Color.gray;
    }
}