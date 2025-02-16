using UnityEngine;

public class PlayerPoint : MonoBehaviour
{
    #region Singleton
    public static PlayerPoint Instance { get; private set; }
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

    [SerializeField] private float _maxSpeed = 10f;

    [HideInInspector] public bool isEndPoint = false;
    [HideInInspector] public bool isPathReady = false;
    [HideInInspector] public bool isFixed = false;

    private Vector3 _offset;
    private Color _spriteColor;

    private Collider2D _collider;
    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private LineController _ropeController;

    private IInput _input;
    private Camera _mainCamera;

    private Vector3 _minScreenBounds;
    private Vector3 _maxScreenBounds;

    private bool _isDesktop;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteColor = _spriteRenderer.color;
        _rb = GetComponent<Rigidbody2D>();
        _ropeController = LineController.Instance;
        _input = InputController.Instance.input;
        _collider = GetComponent<Collider2D>();
        _mainCamera = Camera.main;
        _isDesktop = InputController.Instance.IsDesktop;

        _minScreenBounds = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        _maxScreenBounds = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, 0));
    }

    private void Update()
    {
        if (_isDesktop) return;

        if (_input.MoveCats())
        {
            Vector3 touchPosition = _mainCamera.ScreenToWorldPoint(_input.InputPosition());
            Vector2 touchPosition2D = new Vector2(touchPosition.x, touchPosition.y);

            if (_collider.OverlapPoint(touchPosition2D))
            {
                isPathReady = false;
                GetOffset();
            }

            if (!isFixed)
            {
                Move();
            }
            else if (isEndPoint)
            {
                isPathReady = true;
            }
        }
    }

    private void OnMouseDown()
    {
        isPathReady = false;
        GetOffset();
    }

    private void OnMouseDrag()
    {
        if (isFixed) return;
        Move();
    }

    private void OnMouseUp()
    {
        if (isEndPoint) isPathReady = true;
    }

    private void Move()
    {
        Vector3 targetPosition = _mainCamera.ScreenToWorldPoint(_input.InputPosition()) + _offset;
        targetPosition.z = transform.position.z;

        targetPosition = new Vector3(
            Mathf.Clamp(targetPosition.x, _minScreenBounds.x, _maxScreenBounds.x),
            Mathf.Clamp(targetPosition.y, _minScreenBounds.y, _maxScreenBounds.y),
            0
        );

        Vector2 newPosition = Vector2.MoveTowards(_rb.position, targetPosition, _maxSpeed * Time.deltaTime);
        _rb.MovePosition(newPosition);
    }

    private void GetOffset()
    {
        Vector3 position = _mainCamera.ScreenToWorldPoint(_input.InputPosition());
        _offset = transform.position - position;
        _offset.z = 0;
    }

    public void SetColor(Color color)
    {
        _spriteRenderer.color = color;
    }

    public void ResetColor()
    {
        _spriteRenderer.color = _spriteColor;
    }

    public Color GetColor()
    {
        return _spriteRenderer.color;
    }
}






