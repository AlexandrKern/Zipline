using UnityEngine;

public class MovableCircularSaw : BaseCircularSaw
{
    public enum MoveDirection
    {
        Horizontal,
        Vertical
    }

    [Header("Move settings")]
    [SerializeField] private MoveDirection moveDirection;
    [SerializeField] private float distance = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Color gizmoColor = Color.cyan;

    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool movingToEnd = true;

    public override void Start()
    {
        base.Start();
        startPosition = transform.position;
        endPosition = GetEndPosition();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        Vector2 target = movingToEnd ? endPosition : startPosition;
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector2.Distance(transform.position, target) < 0.1f)
        {
            movingToEnd = !movingToEnd;
        }
    }

    private Vector2 GetEndPosition()
    {
        switch (moveDirection)
        {
            case MoveDirection.Horizontal:
                return new Vector2(startPosition.x + distance, startPosition.y);

            case MoveDirection.Vertical:
                return new Vector2(startPosition.x, startPosition.y + distance);

            default:
                return startPosition;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector2 start = transform.position;
        Vector2 end = moveDirection == MoveDirection.Horizontal ?
            new Vector2(start.x + distance, start.y) :
            new Vector2(start.x, start.y + distance);

        Gizmos.DrawLine(start, end);
        Gizmos.DrawSphere(end, 0.2f);

        Vector2 endOpposite = moveDirection == MoveDirection.Horizontal ?
            new Vector2(start.x - distance, start.y) :
            new Vector2(start.x, start.y - distance);

        Gizmos.DrawLine(start, endOpposite);
        Gizmos.DrawSphere(endOpposite, 0.2f);
    }
}
