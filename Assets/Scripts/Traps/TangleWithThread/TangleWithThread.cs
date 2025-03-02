using UnityEngine;

public class TangleWithThread : MonoBehaviour
{
    public float speed = 2f;
    public float maxAngle = 45f;

    private float time;

    void Update()
    {
        time += Time.deltaTime * speed;
        float angle = maxAngle * Mathf.Sin(time);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}
