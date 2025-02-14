using UnityEngine;

public class TestDragObject : MonoBehaviour
{
    private Vector3 offset;
    private Rigidbody2D rb;
    private LineController ropeController;

    private Vector3 _lastMousePosition;
    public float MouseSpeed { get; private set; }

    private bool _stop;

    public float speedToStop = 1000f;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ropeController = LineController.Instance;
    }

    void Update()
    {
        Vector3 currentMousePosition = Input.mousePosition;
        float distance = Vector3.Distance(currentMousePosition, _lastMousePosition);
        MouseSpeed = distance / Time.deltaTime;
        _lastMousePosition = currentMousePosition;
        if(MouseSpeed>speedToStop) _stop = true;
    }

    private void OnMouseDown()
    {
        _stop = false;
        offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset.z = 0;
    }

    private void OnMouseDrag()
    {
        if(_stop) return;
        Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
        newPosition.z = transform.position.z;

        rb.MovePosition(newPosition);
    }
}
