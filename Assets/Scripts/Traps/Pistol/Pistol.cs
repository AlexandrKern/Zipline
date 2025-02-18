using UnityEngine;

public class Pistol : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;
    [SerializeField] private float fireRate = 0.5f;

    private BulletManager _bulletManager;


    private void Start()
    {
        _bulletManager = BulletManager.Instance;
        InvokeRepeating(nameof(Shoot), 0f, fireRate);
    }


    private void Shoot()
    {
        GameObject bulletObject = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Bullet bullet = bulletObject.GetComponent<Bullet>();
        bullet.Init(bulletSpeed, firePoint.right);
        _bulletManager.AddBullet(bullet);
        
    }
}
