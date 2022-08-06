using System.Collections;
using UnityEngine;
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 1f;
    Rigidbody2D myRigidbody;
    public bool Boxed = false;
    bool Bump = false;
    bool Stunned = false;

    void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!Stunned)
        {
            myRigidbody.velocity = new Vector2(moveSpeed, 0f);
            if (moveSpeed >= 0)
            {
                transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }
        }
        else
        {
            Debug.Log("Stunned");
            myRigidbody.velocity = Vector2.zero;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer != 8)
        {
            if (Boxed)
            {
                if (Bump)
                {
                    Bump = false;
                    return;
                }
                moveSpeed = -moveSpeed;
            }
            else
            {
                moveSpeed = -moveSpeed;

            }
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "BattleHammer")
            StartCoroutine("Stun");
        if (Boxed || (other.gameObject.layer != 6 && other.gameObject.layer != 8))
        {
            moveSpeed = -moveSpeed;
            Bump = true;
        }
    }

    IEnumerator Stun()
    {
        Stunned = true;
        Debug.Log(Stunned);
        Debug.Log(Mathf.Abs(3 / moveSpeed));
        yield return new WaitForSeconds(Mathf.Abs(3 / moveSpeed));
        Stunned = false;
        Debug.Log(Stunned);
    }
}
