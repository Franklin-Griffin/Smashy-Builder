using UnityEngine;

public class Fog : MonoBehaviour
{
    public float spinSpeed = 1;
    public Vector3 moveSpeed;
    public bool breakable = false;
    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + Time.deltaTime * 360 * spinSpeed);
        transform.position += moveSpeed * Time.deltaTime;
    }
    public void OnTriggerEnter2D(Collider2D other)
    {
        if ((other.name == "BattleHammer" || other.name == "SledgeHammer") && breakable)
        {
            Destroy(gameObject);
        }
    }
}
