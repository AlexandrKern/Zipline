
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{
    #region Singleton
    public static BulletManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    private List<Bullet> _bullets = new List<Bullet>();

    private void Update()
    {
        foreach (Bullet bullet in _bullets)
        {
            bullet.Move();
        }
    }

    public void AddBullet(Bullet bullet)
    {
        _bullets.Add(bullet);
    }
    public void RemoveBullet(Bullet bullet)
    {
        _bullets.Remove(bullet);
    }
}
