using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerPoint : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    #region Singleton
    public static PlayerPoint Instance;
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

    private Rigidbody2D _rb;
    private SpriteRenderer _spriteRenderer;
    private LineController _ropeController;

    private IInput _input;
    private Camera _mainCamera;

    private Vector3 _minScreenBounds;
    private Vector3 _maxScreenBounds;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteColor = _spriteRenderer.color;
        _rb = GetComponent<Rigidbody2D>();
        _ropeController = LineController.Instance;
        _input = InputController.Instance.input;
        _mainCamera = Camera.main;

        _minScreenBounds = _mainCamera.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z - _mainCamera.transform.position.z));
        _maxScreenBounds = _mainCamera.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z - _mainCamera.transform.position.z));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPathReady = false;
        Vector3 mousePosition = _mainCamera.ScreenToWorldPoint(_input.InputPosition());
        _offset = transform.position - mousePosition;
        _offset.z = 0;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isFixed) return;

        Vector3 targetPosition = _mainCamera.ScreenToWorldPoint(_input.InputPosition()) + _offset;
        targetPosition.z = transform.position.z;

        targetPosition.x = Mathf.Clamp(targetPosition.x, _minScreenBounds.x, _maxScreenBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, _minScreenBounds.y, _maxScreenBounds.y);

        Vector2 newPosition = Vector2.MoveTowards(_rb.position, targetPosition, _maxSpeed * Time.deltaTime);
        _rb.MovePosition(newPosition);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isEndPoint) isPathReady = true;
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


