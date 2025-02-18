using UnityEngine;

public class CircularSaw : MonoBehaviour
{
    private PathToEndPoint _pathToEndPoint;

    private void Start()
    {
        _pathToEndPoint = PathToEndPoint.Instance;
    }

    [System.Obsolete]
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cat cat = collision.GetComponent<Cat>();
        if (cat != null)
        {
            _pathToEndPoint.StopCatOnPath(cat);
        }
        else
        {
            Debug.Log("Это явно не кот");
        }

    }
}
