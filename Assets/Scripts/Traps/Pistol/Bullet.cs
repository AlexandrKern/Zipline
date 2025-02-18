using UnityEngine;

public class Bullet : MonoBehaviour
{
    private PathToEndPoint _pathToEndPoint;
    private BulletManager _bulletManager;

    private float speed;
    private Vector2 direction;

    public virtual void Start()
    {
        _pathToEndPoint = PathToEndPoint.Instance;
        _bulletManager = BulletManager.Instance;
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
            Debug.Log("Это явно не кот");
        }
        _bulletManager.RemoveBullet(this);
        Destroy(gameObject);
    }

    public void Init(float bulletSpeed, Vector2 shootDirection)
    {
        speed = bulletSpeed;
        direction = shootDirection;
    }

    public void Move()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

}
