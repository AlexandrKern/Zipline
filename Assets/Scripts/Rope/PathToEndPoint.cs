using System.Collections;
using System.Linq;
using UnityEngine;
public class PathToEndPoint : MonoBehaviour
{
    #region Singleton
    public static PathToEndPoint Instance {  get; private set; }

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

    private LineController _lineController;
    private PlayerPoint _playerPoint;

    [SerializeField] private Cat [] _cats;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _moveInterval = 0.5f;
    private bool _isPathActive = false;

    private Vector2[] _pathPoints;

    private int _cuerrentCatIndex = 0;

    [SerializeField] private Color _assignedColor;

    private Color _currentColor;

    private IInput _input;



    private void Start()
    {
        _lineController = LineController.Instance;
        _playerPoint = PlayerPoint.Instance;
        _input = InputController.Instance.input;
    }

    private void Update()
    {
        if (_input.MoveCats() && _playerPoint.isPathReady&&!_isPathActive)
        {
            StartCoroutine(StartingFollowPath());
        }
        Move();
        ChecksCatAreOnWay();
    }


    private void ChecksCatAreOnWay()
    {
        bool isAnyoneAtStart = _cats.Any(human => human.startedPath);

        if (_playerPoint.isFixed != isAnyoneAtStart)
        {
            _playerPoint.isFixed = isAnyoneAtStart;

            if (isAnyoneAtStart)
            {
                _currentColor = _playerPoint.GetColor();
                _playerPoint.SetColor(_assignedColor);
                _lineController.SetColor(_assignedColor, _assignedColor);
            }
            else
            {
                _playerPoint.SetColor(_currentColor);
                _lineController.SetColor(_currentColor, _currentColor);
            }
        }
    }

    private void SetPathPoints()
    {
        _pathPoints = _lineController.GetColliderPoints();
        System.Array.Reverse(_pathPoints);
    }

    [System.Obsolete]
    public void StopCatOnPath(Cat cat)
    {
        cat.startedPath = false;
        cat.isFinished = true;

        cat.currentPointIndex = 0;

        Rigidbody2D rb = cat.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.linearVelocity = Vector2.zero; 
            rb.gravityScale = 1f; 
        }
    }
    private void Move()
    {
        foreach (var cat in _cats)
        {
            
            if (cat.startedPath && !cat.isFinished)
            {
                
                if (cat.currentPointIndex < _pathPoints.Length)
                {
                    
                    Vector2 targetPoint = _pathPoints[cat.currentPointIndex];

                   
                    cat.transform.position = Vector2.MoveTowards(
                        cat.transform.position,
                        targetPoint,
                        _speed * Time.deltaTime
                    );

                   
                    if (Vector2.Distance(cat.transform.position, targetPoint) < 0.1f)
                    {
                        cat.currentPointIndex++;
                    }
                }
                else
                {
                    cat.isFinished = true;
                    cat.startedPath = false;
                }
            }
        }
    }

    private void FollowPath()
    {
        SetPathPoints();
        if (_cuerrentCatIndex >= _cats.Length)
        {
            Debug.Log("��� ���� �������");
            return;
        }

        if (!_cats[_cuerrentCatIndex].isFinished && !_cats[_cuerrentCatIndex].startedPath)
        {
            _cats[_cuerrentCatIndex].startedPath = true;
        }

        if (_cats[_cuerrentCatIndex].startedPath)
        {
            _cuerrentCatIndex++;
        }
    }

    private IEnumerator StartingFollowPath()
    {
        _isPathActive = true;

        while (_input.MoveCats()) 
        {
            FollowPath();
            yield return new WaitForSeconds(_moveInterval);
        }

        _isPathActive = false;
    }

  



}

