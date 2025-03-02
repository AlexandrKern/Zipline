using UnityEngine;

public class MovingPillowDamageDiller : MonoBehaviour
{
    private PathToEndPoint _pathToEndPoint;

    public virtual void Start()
    {
        _pathToEndPoint = PathToEndPoint.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Cat cat = collision.GetComponent<Cat>();
        if (cat != null)
        {
            _pathToEndPoint.StopCatOnPath(cat);
        }
        else
        {
            Debug.Log("��� ���� �� ���");
        }

    }
}
