using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.SceneManagement;
public class Player : MonoBehaviour
{
    float runSpeed = 7f;
    float jumpSpeed = 20f;
    float climbSpeed = 5f;
    float deathKickMax = 20f;
    [SerializeField] Transform HammerHand;
    public AudioClip DeathSFX;

    Vector2 moveInput;
    Rigidbody2D myRigidbody;
    Animator myAnimator;
    CapsuleCollider2D myBodyCollider;
    BoxCollider2D myFeetCollider;
    float gravityScaleAtStart;

    public bool isAlive = true;
    bool attack = false;
    bool mine = false;
    public GameObject BattleHammer;
    public GameObject LadderHammer;
    public GameObject SledgeHammer;
    public Tilemap ladderMap;
    public Tilemap platformMap;
    public TileBase ladder;
    bool super = false;
    public bool superRing = false;
    public bool teleRing = false;
    Vector3 offset;
    public GameObject Checkpoint;
    GameObject checkpoint;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider = GetComponent<CapsuleCollider2D>();
        myFeetCollider = GetComponent<BoxCollider2D>();
        gravityScaleAtStart = myRigidbody.gravityScale;
        if (GameObject.FindGameObjectsWithTag("Checkpoint").Length > 0)
        {
            transform.position = GameObject.FindGameObjectsWithTag("Checkpoint")[0].transform.position;
            checkpoint = GameObject.FindGameObjectsWithTag("Checkpoint")[0];
        }
    }

    void Update()
    {
        if (!isAlive) return;
        Run();
        FlipSprite();
        ClimbLadder();
        Die();
        CheckInput();
    }

    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, myRigidbody.velocity.y);
        myRigidbody.velocity = playerVelocity;

        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);

    }

    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidbody.velocity.x), 1f);
        }
    }

    void ClimbLadder()
    {
        if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Climbing")))
        {
            myRigidbody.gravityScale = gravityScaleAtStart;
            myAnimator.SetBool("isClimbing", false);
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, 1 * climbSpeed);
            myRigidbody.velocity = climbVelocity;
            myRigidbody.gravityScale = 0f;

            myAnimator.SetBool("isClimbing", true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, -1 * climbSpeed);
            myRigidbody.velocity = climbVelocity;
            myRigidbody.gravityScale = 0f;

            myAnimator.SetBool("isClimbing", true);
        }
        else
        {
            Vector2 climbVelocity = new Vector2(myRigidbody.velocity.x, 0);
            myRigidbody.velocity = climbVelocity;
            myRigidbody.gravityScale = 0f;

            myAnimator.SetBool("isClimbing", false);
        }
    }

    void Die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            if (SceneManager.GetActiveScene().buildIndex == 12)
            {
                if (FindObjectOfType<Canvas>().transform.childCount == 4)
                    Destroy(FindObjectOfType<Canvas>().transform.GetChild(3).gameObject);
                else
                    Destroy(FindObjectOfType<Canvas>().transform.GetChild(2).gameObject);
            }
            isAlive = false;
            myAnimator.SetTrigger("Dying");
            myRigidbody.velocity = new Vector2(Random.Range(-deathKickMax, deathKickMax), Random.Range(0, deathKickMax));
            FindObjectOfType<GameSession>().ProcessPlayerDeath();
            AudioSource.PlayClipAtPoint(DeathSFX, Camera.main.transform.position);
            GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.None;
        }
    }

    void CheckInput() {
        if (teleRing)
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                offset.y = 1;
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                offset.y = -1;
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                offset.x = transform.localScale.x == 1 ? 1 : -1;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                offset.x = transform.localScale.x == 1 ? -1 : 1;
            }
        }
        if (Input.GetKeyDown(KeyCode.Space) || (Input.GetKey(KeyCode.Space) && super))
        {
            if (attack && BattleHammer)
            {
                GameObject hammer = Instantiate(BattleHammer, HammerHand);
                hammer.transform.Translate(offset);
                hammer.name = "BattleHammer";
            }
            else if (mine && SledgeHammer)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
                {
                    platformMap.SetTile(new Vector3Int
                    (Mathf.RoundToInt(transform.position.x - 0.5f + offset.x),
                        Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0), null);
                    GameObject hammer = Instantiate(SledgeHammer, HammerHand);
                    hammer.transform.Translate(offset);
                    hammer.name = "SledgeHammer";
                }
                else if (transform.localScale.x == 1)
                {
                    platformMap.SetTile(new Vector3Int
                        (Mathf.RoundToInt(transform.position.x + offset.x),
                        Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0), null);
                    GameObject hammer = Instantiate(SledgeHammer, HammerHand);
                    hammer.transform.Translate(offset);
                    hammer.name = "SledgeHammer";
                }
                else
                {
                    platformMap.SetTile(new Vector3Int
                        (Mathf.RoundToInt(transform.position.x - 1 + offset.x),
                    Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0), null);
                    GameObject hammer = Instantiate(SledgeHammer, HammerHand);
                    hammer.transform.Translate(offset);
                    hammer.name = "SledgeHammer";
                }
            }
            else if (LadderHammer)
            {
                if (Input.GetKey(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
                {
                    if (!ladderMap.HasTile(new Vector3Int
                    (Mathf.RoundToInt(transform.position.x - 0.5f + offset.x),
                    Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0)))
                        ladderMap.SetTile(new Vector3Int
                        (Mathf.RoundToInt(transform.position.x - 0.5f + offset.x),
                        Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0), ladder);
                    GameObject hammer = Instantiate(LadderHammer, HammerHand);
                    hammer.transform.Translate(offset);
                    hammer.name = "LadderHammer";
                }
                else if (transform.localScale.x == 1)
                {
                    if (!ladderMap.HasTile(new Vector3Int
                    (Mathf.RoundToInt(transform.position.x + offset.x),
                    Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0)))
                        ladderMap.SetTile(new Vector3Int
                        (Mathf.RoundToInt(transform.position.x + offset.x),
                        Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0), ladder);
                    GameObject hammer = Instantiate(LadderHammer, HammerHand);
                    hammer.transform.Translate(offset);
                    hammer.name = "LadderHammer";
                }
                else
                {
                    if (!ladderMap.HasTile(new Vector3Int
                    (Mathf.RoundToInt(transform.position.x - 1 + offset.x),
                    Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0)))
                        ladderMap.SetTile(new Vector3Int
                        (Mathf.RoundToInt(transform.position.x - 1 + offset.x),
                        Mathf.RoundToInt(transform.position.y - 0.5f + offset.y), 0), ladder);
                    GameObject hammer = Instantiate(LadderHammer, HammerHand);
                    hammer.transform.Translate(offset);
                    hammer.name = "LadderHammer";
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.P) && FindObjectOfType<GameSettings>().practice)
        {
            Destroy(checkpoint);
            checkpoint = Instantiate(Checkpoint, transform);
            checkpoint.transform.parent = null;
            DontDestroyOnLoad(checkpoint);
        }
        if (Input.GetKeyDown(KeyCode.R) && checkpoint)
        {
            Destroy(checkpoint);
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (!isAlive) { return; }
            moveInput = new Vector2(1, 0);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            if (!isAlive) { return; }
            moveInput = new Vector2(-1, 0);
        }
        else
        {
            if (!isAlive) { return; }
            moveInput = new Vector2(0, 0);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveInput *= 0.5f;
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                moveInput *= 0.5f;
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (!isAlive) { return; }
            if (!myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"))) { return; }

            myRigidbody.velocity += new Vector2(0f, jumpSpeed);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (BattleHammer)
            {
                attack = !attack;
                mine = false;
                if (attack)
                {
                    if (super)
                        GetComponent<SpriteRenderer>().color = Color.red;
                    else
                        GetComponent<SpriteRenderer>().color = Color.magenta;
                }
                else
                {
                    if (super)
                        GetComponent<SpriteRenderer>().color = Color.green;
                    else
                        GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            if (SledgeHammer)
            {
                mine = !mine;
                attack = false;
                if (mine)
                {
                    if (super)
                        GetComponent<SpriteRenderer>().color = Color.black;
                    else
                        GetComponent<SpriteRenderer>().color = Color.gray;
                }
                else
                {
                    if (super)
                        GetComponent<SpriteRenderer>().color = Color.green;
                    else
                        GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (superRing)
            {
                super = !super;
                if (attack)
                {
                    if (super)
                        GetComponent<SpriteRenderer>().color = Color.red;
                    else
                        GetComponent<SpriteRenderer>().color = Color.magenta;
                }
                else if (mine)
                {
                    if (super)
                        GetComponent<SpriteRenderer>().color = Color.black;
                    else
                        GetComponent<SpriteRenderer>().color = Color.gray;
                }
                else
                {
                    if (super)
                        GetComponent<SpriteRenderer>().color = Color.green;
                    else
                        GetComponent<SpriteRenderer>().color = Color.white;
                }
            }
        }
        offset = Vector3.zero;
    }
}