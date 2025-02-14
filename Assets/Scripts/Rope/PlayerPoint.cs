using UnityEngine;

public class PlayerPoint : MonoBehaviour
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

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteColor = _spriteRenderer.color;
        _rb = GetComponent<Rigidbody2D>();
        _ropeController = LineController.Instance;

    
    }

    private void OnMouseDown()
    {
        isPathReady = false;
        _offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _offset.z = 0;
    }

    private void OnMouseDrag()
    {
        if (isFixed) return;

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + _offset;
        targetPosition.z = transform.position.z;

        Vector3 minScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, transform.position.z - Camera.main.transform.position.z));
        Vector3 maxScreenBounds = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, transform.position.z - Camera.main.transform.position.z));

        targetPosition.x = Mathf.Clamp(targetPosition.x, minScreenBounds.x, maxScreenBounds.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minScreenBounds.y, maxScreenBounds.y);

        Vector2 newPosition = Vector2.MoveTowards(_rb.position, targetPosition, _maxSpeed * Time.deltaTime);
        _rb.MovePosition(newPosition);
    }

    private void OnMouseUp()
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
