using System.Collections;
using System.Linq;
using UnityEngine;
public class PathToEndPoint : MonoBehaviour
{
    private LineController _lineController;
    private PlayerPoint _playerPoint;

    [SerializeField] private Cats [] _cats;
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
        ChecksPeopleAreOnWay();
    }


    private void ChecksPeopleAreOnWay()
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

    private void Move()
    {
        foreach (var human in _cats)
        {
            
            if (human.startedPath && !human.isFinished)
            {
                
                if (human.currentPointIndex < _pathPoints.Length)
                {
                    
                    Vector2 targetPoint = _pathPoints[human.currentPointIndex];

                   
                    human.cat.transform.position = Vector2.MoveTowards(
                        human.cat.transform.position,
                        targetPoint,
                        _speed * Time.deltaTime
                    );

                   
                    if (Vector2.Distance(human.cat.transform.position, targetPoint) < 0.1f)
                    {
                        human.currentPointIndex++;
                    }
                }
                else
                {
                    human.isFinished = true;
                    human.startedPath = false;
                }
            }
        }
    }

    private void FollowPath()
    {
        SetPathPoints();
        if (_cuerrentCatIndex >= _cats.Length)
        {
            Debug.Log("Все люди спасены");
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

    [System.Serializable]
    public class Cats
    {
        [HideInInspector] public int currentPointIndex;
        [HideInInspector] public bool startedPath;
        [HideInInspector] public bool isFinished;
        public GameObject cat;
    }



}

