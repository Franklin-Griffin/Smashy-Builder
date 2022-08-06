using UnityEngine;

public class Spin : MonoBehaviour
{
    public float speed = 1;
    float timePassed = 0;
    void Update()
    {
        transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + Time.deltaTime * 360 * speed);
        timePassed += Time.deltaTime;
        if (timePassed >= 1 / speed)
        {
            Destroy(gameObject);
        }
    }
}
