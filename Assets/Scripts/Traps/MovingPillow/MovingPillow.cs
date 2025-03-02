using UnityEngine;

public class MovingPillow : MonoBehaviour
{
    public enum MoveDirection { Left, Right, Up, Down }

    [SerializeField] private Transform anchorPoint;
    [SerializeField] private Transform movingPoint;
    [SerializeField] private LineRenderer _lineRenderer;

    [SerializeField] private MoveDirection moveDirection = MoveDirection.Right;
    [SerializeField] private float moveDistance = 3f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float returnSpeed = 2f;

    private Vector3 _startPos;
    private Vector3 _targetPos;
    private bool _movingForward = true;
    private float _currentSpeed = 0f;

    private void Start()
    {
        _startPos = movingPoint.position;
        _targetPos = _startPos + GetMoveVector() * moveDistance;
    }

    private void Update()
    {
        UpdateLine();
        MovePillow();
    }

    private void UpdateLine()
    {
        _lineRenderer.SetPosition(0, anchorPoint.position);
        _lineRenderer.SetPosition(1, movingPoint.position);
    }

    private void MovePillow()
    {
        if (_movingForward)
        {
            _currentSpeed = Mathf.MoveTowards(_currentSpeed, moveSpeed, acceleration * Time.deltaTime);
            movingPoint.position = Vector3.MoveTowards(movingPoint.position, _targetPos, _currentSpeed * Time.deltaTime);

            if (Vector3.Distance(movingPoint.position, _targetPos) < 0.1f)
                _movingForward = false;
        }
        else
        {
            movingPoint.position = Vector3.MoveTowards(movingPoint.position, _startPos, returnSpeed * Time.deltaTime);

            if (Vector3.Distance(movingPoint.position, _startPos) < 0.1f)
            {
                _movingForward = true;
                _currentSpeed = 0f;
            }
        }
    }

    private Vector3 GetMoveVector()
    {
        return moveDirection switch
        {
            MoveDirection.Left => Vector3.left,
            MoveDirection.Right => Vector3.right,
            MoveDirection.Up => Vector3.up,
            MoveDirection.Down => Vector3.down,
            _ => Vector3.zero
        };
    }
}


