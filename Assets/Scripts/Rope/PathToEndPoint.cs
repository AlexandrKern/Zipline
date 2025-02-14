using System.Collections;
using System.Linq;
using UnityEngine;
public class PathToEndPoint : MonoBehaviour
{
    private LineController _lineController;
    private PlayerPoint _playerPoint;

    [SerializeField] private People [] _people;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private float _moveInterval = 0.5f;
    private bool _isPathActive = false;

    private Vector2[] _pathPoints;

    private int _cuerrentHumanIndex = 0;

    [SerializeField] private Color _assignedColor;

    private Color _currentColor;



    private void Start()
    {
        _lineController = LineController.Instance;
        _playerPoint = PlayerPoint.Instance;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && _playerPoint.isPathReady&&!_isPathActive)
        {
            StartCoroutine(StartingFollowPath());
        }
        Move();
        ChecksPeopleAreOnWay();
    }


    private void ChecksPeopleAreOnWay()
    {
        bool isAnyoneAtStart = _people.Any(human => human.startedPath);

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
        foreach (var human in _people)
        {
            
            if (human.startedPath && !human.isFinished)
            {
                
                if (human.currentPointIndex < _pathPoints.Length)
                {
                    
                    Vector2 targetPoint = _pathPoints[human.currentPointIndex];

                   
                    human.human.transform.position = Vector2.MoveTowards(
                        human.human.transform.position,
                        targetPoint,
                        _speed * Time.deltaTime
                    );

                   
                    if (Vector2.Distance(human.human.transform.position, targetPoint) < 0.1f)
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
        if (_cuerrentHumanIndex >= _people.Length)
        {
            Debug.Log("Все люди спасены");
            return;
        }

        if (!_people[_cuerrentHumanIndex].isFinished && !_people[_cuerrentHumanIndex].startedPath)
        {
            _people[_cuerrentHumanIndex].startedPath = true;
        }

        if (_people[_cuerrentHumanIndex].startedPath)
        {
            _cuerrentHumanIndex++;
        }



    }

    private IEnumerator StartingFollowPath()
    {
        _isPathActive = true;

        while (Input.GetMouseButton(0)) 
        {
            FollowPath();
            yield return new WaitForSeconds(_moveInterval);
        }

        _isPathActive = false;
    }

    [System.Serializable]
    public class People
    {
        [HideInInspector] public int currentPointIndex;
        [HideInInspector] public bool startedPath;
        [HideInInspector] public bool isFinished;
        public GameObject human;
    }



}

