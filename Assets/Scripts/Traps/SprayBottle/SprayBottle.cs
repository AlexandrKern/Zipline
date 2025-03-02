using System.Collections;
using UnityEngine;

public class SprayBottle : MonoBehaviour
{
    [SerializeField] private float _workingTime = 2;
    [SerializeField] private float _cooldownTime = 3f;
    [SerializeField] private ParticleSystem _spray;

    private Coroutine _waterRoutine;

    private PathToEndPoint _pathToEndPoint;

    private void Start()
    {
        _waterRoutine = StartCoroutine(WaterRoutineLoop());
        _pathToEndPoint = PathToEndPoint.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!_spray.isPlaying) return;

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

    private IEnumerator WaterRoutineLoop()
    {
        while (true)
        {
            SprayOn();
            yield return new WaitForSeconds(_workingTime);

            SprayOff();
            yield return new WaitForSeconds(_cooldownTime);
        }
    }

    private void SprayOn()
    {
        _spray.Play();
    }

    private void SprayOff()
    {
        _spray.Stop();
    }
}
