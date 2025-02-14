using UnityEngine;
using System.Collections.Generic;
using System;

public class LineController : MonoBehaviour
{
    #region Singleton
    public static LineController Instance;
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

    public GameObject player;

    [SerializeField] private LayerMask wrapLayerMask;

    private EdgeCollider2D _edgeCollider;
    private LineRenderer _lineRenderer;

    private RaycastHit2D _rayToClosestPivotPoint;
    private Vector3 _anchoredPos;
    private Color _startColor;
    private Color _endColor;

    private bool _isInitialized = false;
    private int _pivotsAdded = 0;

    private float _pushOutIncrement;
    private float _oldAngle;
    private float _currentAngle;

    private List<bool> _pivotSwingList = new List<bool>();
   
    private void Start()
    {
        _edgeCollider = GetComponent<EdgeCollider2D>();
        _lineRenderer = GetComponent<LineRenderer>();

        _startColor = _lineRenderer.startColor;
        _endColor = _lineRenderer.endColor;
        

        _pushOutIncrement = _lineRenderer.startWidth / 10f;
        _anchoredPos = player.transform.position;

        _lineRenderer.positionCount = 2;
        _lineRenderer.SetPosition(0, player.transform.position);
        _lineRenderer.SetPosition(1, _anchoredPos);

        _isInitialized = true;
    }

    private void Update()
    {
        if (!_isInitialized) return; 

        if (!IsLineOfSiteToClosestPivotClear())
        {
            WrapTheRope();
        }
        else if (IsLineOfSightTo2ndClosestPivotClear())
        {
            
            ClearClosestPivotAndSwingFromList();
        }
        UpdateCollider();
        SetRopeEndPoints();
    }


    /// <summary>
    ///  свободна ли линия обзора к ближайшей точке поворота
    /// </summary>
    /// <returns></returns>
    bool IsLineOfSiteToClosestPivotClear()
    {
        _rayToClosestPivotPoint = SendRayToClosestPivotPoint();
        return _rayToClosestPivotPoint.collider == null;
    }
    /// <summary>
    /// Отправляет луч от игрока к ближайшей точке поворота
    /// </summary>
    /// <returns></returns>
    RaycastHit2D SendRayToClosestPivotPoint()
    {
        Vector2 playerPosition = (Vector2)player.transform.position;
        Vector2 closestPivotToPlayer = (Vector2)_lineRenderer.GetPosition(1);
        Vector2 rayDirection = closestPivotToPlayer - playerPosition;
        float ropeDistance = Vector2.Distance(closestPivotToPlayer, playerPosition);

        RaycastHit2D[] hits = Physics2D.RaycastAll(playerPosition, rayDirection, ropeDistance, wrapLayerMask);
        foreach (var hit in hits)
        {
            if (hit.collider != null && hit.collider.gameObject != player)
            {
                return hit;
            }
        }

        return new RaycastHit2D(); 
    }

    void WrapTheRope()
    {
        Vector2 polygonVertexPoint = GetClosestColliderPointFromRaycastHit(_rayToClosestPivotPoint, _rayToClosestPivotPoint.collider.gameObject.GetComponent<PolygonCollider2D>());

        AddSwingDirectionForNewPivot(polygonVertexPoint);
        AddLineRenderPivotPoint(polygonVertexPoint);
        PushPivotPointOutwards(_rayToClosestPivotPoint.collider.gameObject.GetComponent<Rigidbody2D>());
    }
    /// <summary>
    /// свободна ли линия обзора ко второй ближайшей точке поворота, а также: Убедится, что угол увеличивается.Проверяет, совпадает ли направление поворота с ожидаемым.
    /// </summary>
    /// <returns></returns>
    bool IsLineOfSightTo2ndClosestPivotClear()
    {
        return _pivotsAdded > 0 && SendRayTo2ndClosestPivotHit() &&
           _lineRenderer.positionCount > 2 &&
           IsAngleGettingLarger() && IsPivotAngleOnCounterSwingDirection();

    }
    /// <summary>
    /// Отправляет луч от игрока ко второй ближайшей точке поворота
    /// </summary>
    /// <returns></returns>
    bool SendRayTo2ndClosestPivotHit()
    {
        bool isLineToSecondPivotClear = true;
        float ropeDistance = Vector2.Distance((Vector2)_lineRenderer.GetPosition(2), (Vector2)player.transform.position);
        Vector2 rayDirection = (Vector2)_lineRenderer.GetPosition(2) - (Vector2)player.transform.position;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)player.transform.position, rayDirection, ropeDistance * 0.95f, wrapLayerMask);

        if (hit.collider != null)
            isLineToSecondPivotClear = false;

        return isLineToSecondPivotClear;
    }
    /// <summary>
    /// Находит ближайшую точку на полигональном коллайдере к месту столкновения луча
    /// </summary>
    /// <param name="hit"></param>
    /// <param name="polyCollider"></param>
    /// <returns></returns>
    private Vector2 GetClosestColliderPointFromRaycastHit(RaycastHit2D hit, PolygonCollider2D polyCollider)
    {
        Vector2 closestPoint = Vector2.zero;
        float minDistance = float.MaxValue;

        foreach (Vector2 point in polyCollider.points)
        {
            Vector2 worldPoint = polyCollider.transform.TransformPoint(point);
            float distance = Vector2.Distance(hit.point, worldPoint);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestPoint = worldPoint;
            }
        }

        return closestPoint;
    }
    /// <summary>
    /// Добавляет новую точку поворота в LineRenderer
    /// </summary>
    /// <param name="polygonHitPoint"></param>
    void AddLineRenderPivotPoint(Vector2 polygonHitPoint)
    {
        _pivotsAdded++;
        _lineRenderer.positionCount++;
        Vector3[] positions = new Vector3[_lineRenderer.positionCount];
        _lineRenderer.GetPositions(positions);
        Array.Copy(positions, 1, positions, 2, positions.Length - 2);
        positions[1] = polygonHitPoint;
        _lineRenderer.SetPositions(positions);
    }
    /// <summary>
    /// Определяет направление вращения 
    /// </summary>
    /// <param name="polygonHitPoint"></param>
    void AddSwingDirectionForNewPivot(Vector2 polygonHitPoint)
    {
        bool isSwingClockWise = CheckSwingDirectionByPlayerPositon(polygonHitPoint);

        _pivotSwingList.Add(isSwingClockWise);
    }

    /// <summary>
    /// Смещает новую точку поворота наружу от центра
    /// </summary>
    /// <param name="rgbdWrapped"></param>
    void PushPivotPointOutwards(Rigidbody2D rgbdWrapped)
    {
        Vector3 pointToPush = _lineRenderer.GetPosition(1);
        Vector2 pushVector = pointToPush - (Vector3)rgbdWrapped.worldCenterOfMass;
        pushVector = Vector2.ClampMagnitude(pushVector, _pushOutIncrement * 5f);    
        pointToPush += (Vector3)pushVector;
        _lineRenderer.SetPosition(1, pointToPush);
    }

    /// <summary>
    /// направление вращения игрока относительно точки поворота
    /// </summary>
    /// <param name="pivotPosition"></param>
    /// <returns></returns>
    private bool CheckSwingDirectionByPlayerPositon(Vector2 pivot)
    {
        Vector3 cross = Vector3.Cross((Vector2)_lineRenderer.GetPosition(1) - pivot, (Vector2)player.transform.position - pivot);
        return cross.z > 0;
    }

    /// <summary>
    /// совпадает ли направление поворота с ожидаемым
    /// </summary>
    /// <returns></returns>
    bool IsPivotAngleOnCounterSwingDirection()
    {
        bool isPivotVsPlayerClockWise = false;
        int closestPivotIndex = 1;
        Vector2 pivotPoint = (Vector2)_lineRenderer.GetPosition(closestPivotIndex);
        Vector2 playerOldPoint = (Vector2)_lineRenderer.GetPosition(closestPivotIndex + 1);
        Vector2 playerNewPoint = player.transform.position;
        Vector2 firstVector = playerOldPoint - pivotPoint;
        Vector2 secondVector = playerNewPoint - pivotPoint;
        Vector3 leftHandRuleVector = Vector3.Cross(firstVector, secondVector);

        if (leftHandRuleVector.z < 0)
            isPivotVsPlayerClockWise = true;

        bool result = isPivotVsPlayerClockWise == _pivotSwingList[_pivotSwingList.Count - 1];
        return result;
    }
    /// <summary>
    /// Удаляет ближайшую точку поворота и связанное с ней направление вращения
    /// </summary>
    void ClearClosestPivotAndSwingFromList()
    {
        _pivotsAdded--;
        DeleteLastLineRenderBendPoint();
    }
    /// <summary>
    /// Удаляет последний узел в LineRenderer
    /// </summary>
    void DeleteLastLineRenderBendPoint()
    {
        _pivotSwingList.RemoveAt(_pivotSwingList.Count - 1);

        Vector2[] tempPoints = new Vector2[_lineRenderer.positionCount - 1];
        tempPoints[0] = _lineRenderer.GetPosition(0);

        for (int i = 1; i < _lineRenderer.positionCount - 1; i++)
            tempPoints[i] = _lineRenderer.GetPosition(i + 1);

        _lineRenderer.positionCount--;

        for (int i = 0; i < tempPoints.Length; i++)
            _lineRenderer.SetPosition(i, (Vector3)tempPoints[i]);
    }
    /// <summary>
    /// Вычисляет угол между тремя точками
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    /// <returns></returns>
    float GetAngleBetweenPoints(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector2 ab = b - a;
        Vector2 ac = c - a;
        float angle = Vector2.SignedAngle(ab, ac);
        if (angle < 0) angle += 360;
        return angle;
    }

    /// <summary>
    /// увеличивается ли угол между игроком, ближайшей и второй ближайшей точками поворота
    /// </summary>
    /// <returns></returns>
    bool IsAngleGettingLarger()
    {
        return GetAngleBetweenPoints(player.transform.position, _lineRenderer.GetPosition(1), _lineRenderer.GetPosition(2)) > _oldAngle; ;
    }

    private void SetRopeEndPoints()
    {
        _lineRenderer.SetPosition(0, player.transform.position);    
        _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _anchoredPos);     
    }

    private void UpdateCollider()
    {
        if (_lineRenderer.positionCount < 2) return;

        Vector2[] colliderPoints = new Vector2[_lineRenderer.positionCount];
        for (int i = 0; i < _lineRenderer.positionCount; i++)
        {
            Vector3 worldPoint = _lineRenderer.GetPosition(i);
            colliderPoints[i] = worldPoint;
        }
        _edgeCollider.points = colliderPoints;
    }

    public void SetColor(Color startColor,Color endColor)
    {
        if (_lineRenderer == null) return;
        _lineRenderer.startColor = startColor;
        _lineRenderer.endColor = endColor;
    }

    public void ResetColor()
    {
        if ( _lineRenderer == null ) return;
        _lineRenderer.startColor = _startColor;
        _lineRenderer.endColor= _endColor;
    }

    public Vector2[] GetColliderPoints()
    {
        return _edgeCollider.points;
    }
}



