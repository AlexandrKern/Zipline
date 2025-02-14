using UnityEngine;

public class EndPoint : MonoBehaviour
{
    private PlayerPoint _playerPoint;
    private LineController _wrapController;

    [SerializeField] private Color _assignedColor;

    private void Start()
    {
       _playerPoint = PlayerPoint.Instance;
        _wrapController = LineController.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("StartPoint"))
        {
           _playerPoint.isEndPoint = true;
            _wrapController.SetColor(_assignedColor,_assignedColor);
            _playerPoint.SetColor(_assignedColor);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("StartPoint"))
        {
            _playerPoint.isEndPoint = false;
            _wrapController.ResetColor();
            _playerPoint.ResetColor();
        }
    }

  
}
