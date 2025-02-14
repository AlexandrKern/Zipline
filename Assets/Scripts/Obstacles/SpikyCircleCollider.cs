using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class SpikyCircleCollider : MonoBehaviour
{
    [SerializeField] private int numberOfSpikes = 8;
    [SerializeField] private float radius = 2f;
    [SerializeField] private float spikeLength = 0.5f;

    private void Start()
    {
        PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();

        Vector2[] points = new Vector2[numberOfSpikes * 2];

        for (int i = 0; i < numberOfSpikes; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfSpikes;
            float spikeAngle = angle + Mathf.PI / numberOfSpikes;

            points[i * 2] = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            points[i * 2 + 1] = new Vector2(Mathf.Cos(spikeAngle), Mathf.Sin(spikeAngle)) * (radius + spikeLength);
        }

        polygonCollider.pathCount = 1;
        polygonCollider.SetPath(0, points);
    }
}
