using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 유닛의 이동과 선택을 관리하는 컴포넌트
/// RTS 게임에서 사용되는 기본적인 유닛 제어 시스템을 구현
/// </summary>
public class UnitMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f; // 유닛의 이동 속도
    [SerializeField, Range(0.5f, 5f), Tooltip("유닛들 사이의 간격을 조절합니다. 값이 클수록 유닛들이 더 넓게 배치됩니다.")]
    private float formationSpacing = 0.5f; // 포메이션에서 유닛 간 간격
    [SerializeField] private float unitRadius = 0.5f; // 유닛의 충돌 반경
    [SerializeField] private LayerMask unitLayer; // 유닛 레이어

    [Header("Selection Settings")]
    [SerializeField] private float doubleClickTimeThreshold = 0.3f; // 더블 클릭 감지를 위한 시간 간격 (초)

    [Header("Patrol Settings")]
    [SerializeField] private float patrolWaitTime = 1f; // 순찰 지점 도달 시 대기 시간

    // 유닛 상태 변수
    private bool isSelected; // 현재 유닛이 선택되었는지 여부
    private Vector2 targetPosition; // 이동할 목표 위치
    private bool isMoving; // 현재 이동 중인지 여부
    private float lastClickTime; // 마지막 클릭 시간 (더블 클릭 감지용)
    private Coroutine moveCoroutine; // 이동 코루틴 참조
    private Vector2 originalPosition; // 순찰 시작 위치
    private bool isPatrolling; // 순찰 중인지 여부
    private Coroutine patrolCoroutine; // 순찰 코루틴 참조
    private Vector2 patrolTarget; // 순찰 목표 위치

    // 성능 최적화를 위한 컴포넌트 캐싱
    private Camera mainCamera; // 메인 카메라 캐싱
    private Transform cachedTransform; // Transform 컴포넌트 캐싱
    
    // 드래그 선택 관련 변수
    private Vector2 dragStartPos; // 드래그 시작 위치
    private Vector2 dragEndPos; // 드래그 끝 위치
    private bool isDragging; // 현재 드래그 중인지 여부
    private Rect dragRect; // 드래그 영역을 나타내는 사각형
    
    // 선택된 유닛들을 관리하는 정적 리스트
    private static readonly List<UnitMovement> selectedUnits = new List<UnitMovement>();

    /// <summary>
    /// 컴포넌트 초기화 시 호출되는 메서드
    /// 자주 사용되는 컴포넌트들을 캐싱
    /// </summary>
    private void Awake()
    {
        mainCamera = Camera.main;
        cachedTransform = transform;
    }

    /// <summary>
    /// 컴포넌트가 활성화될 때 호출되는 메서드
    /// 선택된 유닛 목록을 초기화하여 메모리 누수 방지
    /// </summary>
    private void OnEnable()
    {
        selectedUnits.Clear();
    }

    /// <summary>
    /// 매 프레임마다 호출되는 메서드
    /// 유닛의 선택, 이동, 드래그 선택 등을 처리
    /// </summary>
    void Update()
    {
        HandleDragSelection();
        HandleUnitSelection();
        HandleMovementCommand();
        
        // 'S' 키를 눌러 모든 선택된 유닛의 이동을 중지
        if (Input.GetKeyDown(KeyCode.S))
        {
            StopMoving();
        }

        // 'P' 키를 눌러 선택된 유닛들의 순찰 시작
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (isSelected)
            {
                Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
                StartPatrol(mousePosition);
            }
        }
    }

    /// <summary>
    /// 드래그 선택 기능을 처리하는 메서드
    /// 마우스 드래그로 여러 유닛을 한 번에 선택할 수 있음
    /// </summary>
    private void HandleDragSelection()
    {
        // 드래그 시작
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            isDragging = true;
        }

        // 드래그 중 - 선택 영역 업데이트
        if (isDragging && Input.GetMouseButton(0))
        {
            dragEndPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            dragRect = new Rect(
                Mathf.Min(dragStartPos.x, dragEndPos.x),
                Mathf.Min(dragStartPos.y, dragEndPos.y),
                Mathf.Abs(dragStartPos.x - dragEndPos.x),
                Mathf.Abs(dragStartPos.y - dragEndPos.y)
            );
        }

        // 드래그 종료 - 선택된 유닛들 처리
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            SelectUnitsInDragArea();
        }
    }

    /// <summary>
    /// 유닛 선택 기능을 처리하는 메서드
    /// 단일 클릭과 더블 클릭을 구분하여 처리
    /// </summary>
    private void HandleUnitSelection()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        
        float timeSinceLastClick = Time.time - lastClickTime;
        
        // 더블 클릭 감지 및 처리
        if (timeSinceLastClick <= doubleClickTimeThreshold && hit != null)
        {
            HandleDoubleClick(hit);
        }
        else
        {
            HandleSingleClick(hit);
        }
        
        lastClickTime = Time.time;
    }

    /// <summary>
    /// 더블 클릭 처리 메서드
    /// 같은 태그를 가진 모든 유닛을 선택
    /// </summary>
    private void HandleDoubleClick(Collider2D hit)
    {
        string targetTag = hit.gameObject.tag;
        SelectAllUnitsWithSameTag(targetTag);
        Debug.Log($"Double clicked on: {hit.gameObject.name}");
    }

    /// <summary>
    /// 단일 클릭 처리 메서드
    /// Shift 키와 함께 사용하면 다중 선택 가능
    /// </summary>
    private void HandleSingleClick(Collider2D hit)
    {
        bool isShiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        
        if (hit != null && hit.transform == cachedTransform)
        {
            isSelected = true;
            
            // Shift 키가 눌리지 않은 경우 기존 선택 해제
            if (!isShiftHeld)
            {
                DeselectAllUnits();
            }
            
            // 선택 리스트에 추가
            if (!selectedUnits.Contains(this))
            {
                selectedUnits.Add(this);
            }
            
            Debug.Log("유닛 선택됨!");
        }
        else if (!isShiftHeld)
        {
            isSelected = false;
            selectedUnits.Remove(this);
        }
    }

    /// <summary>
    /// 유닛 이동 명령을 처리하는 메서드
    /// 마우스 오른쪽 클릭으로 이동 명령 실행
    /// </summary>
    private void HandleMovementCommand()
    {
        if (!Input.GetMouseButtonDown(1)) return;

        Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        
        // 선택된 유닛이 한 마리인 경우 충돌을 피해 이동
        if (selectedUnits.Count == 1)
        {
            var unit = selectedUnits[0];
            if (unit != null)
            {
                Vector2 targetPos = FindNearestEmptyPosition(mousePosition);
                unit.StopCurrentMovement();
                unit.moveCoroutine = unit.StartCoroutine(unit.MoveToTarget(targetPos));
                unit.isMoving = true;
                Debug.Log($"단일 유닛 이동 명령: {targetPos}");
            }
            return;
        }
        
        // 여러 유닛이 선택된 경우 포메이션으로 이동
        if (selectedUnits.Count > 1)
        {
            MoveSelectedUnits(mousePosition);
        }
    }

    /// <summary>
    /// 현재 진행 중인 이동을 중지하는 메서드
    /// </summary>
    private void StopCurrentMovement()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        
        if (isPatrolling)
        {
            StopPatrol();
        }
    }

    /// <summary>
    /// 드래그 영역 내의 모든 유닛을 선택하는 메서드
    /// </summary>
    private void SelectUnitsInDragArea()
    {
        UnitMovement[] allUnits = FindObjectsOfType<UnitMovement>();
        int selectedCount = 0;

        foreach (UnitMovement unit in allUnits)
        {
            if (dragRect.Contains((Vector2)unit.cachedTransform.position))
            {
                if (!selectedUnits.Contains(unit))
                {
                    unit.isSelected = true;
                    selectedUnits.Add(unit);
                    selectedCount++;
                }
            }
            else
            {
                unit.isSelected = false;
            }
        }
        
        if (selectedCount > 0)
        {
            Debug.Log($"선택된 유닛 수: {selectedCount}");
        }
    }

    /// <summary>
    /// 모든 선택된 유닛의 이동을 중지하는 메서드
    /// </summary>
    private void StopMoving()
    {
        StopCurrentMovement();
        isMoving = false;
        Debug.Log("이동 중지!");

        foreach (UnitMovement unit in selectedUnits)
        {
            unit.StopCurrentMovement();
            unit.isMoving = false;
        }
    }

    /// <summary>
    /// 목표 지점까지 유닛을 이동시키는 코루틴
    /// </summary>
    /// <param name="target">목표 위치</param>
    private IEnumerator MoveToTarget(Vector2 target)
    {
        Vector2 currentPos = cachedTransform.position;
        while (Vector2.Distance(currentPos, target) > 0.1f)
        {
            currentPos = cachedTransform.position;
            cachedTransform.position = Vector2.MoveTowards(currentPos, target, speed * Time.deltaTime);
            yield return null;
        }
        
        cachedTransform.position = target;
        isMoving = false;
        Debug.Log("목표 지점에 도달!");
    }
    
    /// <summary>
    /// 같은 태그를 가진 모든 유닛을 선택하는 메서드
    /// </summary>
    /// <param name="tag">선택할 유닛들의 태그</param>
    private void SelectAllUnitsWithSameTag(string tag)
    {
        DeselectAllUnits();
        
        var allUnits = FindObjectsOfType<UnitMovement>();
        int selectedCount = 0;
        
        foreach (var unit in allUnits)
        {
            if (unit.CompareTag(tag))
            {
                unit.isSelected = true;
                selectedUnits.Add(unit);
                selectedCount++;
            }
        }
        
        if (selectedCount > 0)
        {
            Debug.Log($"같은 태그({tag})를 가진 {selectedCount}개의 유닛이 선택되었습니다.");
        }
    }

    /// <summary>
    /// 모든 선택된 유닛의 선택을 해제하는 메서드
    /// </summary>
    private void DeselectAllUnits()
    {
        foreach (var unit in selectedUnits)
        {
            if (unit != null)
            {
                unit.isSelected = false;
            }
        }
        selectedUnits.Clear();
    }

    /// <summary>
    /// 선택된 모든 유닛을 지정된 위치로 이동시키는 메서드
    /// 각 유닛은 가장 가까운 포메이션 위치로 이동
    /// </summary>
    /// <param name="target">목표 위치</param>
    private void MoveSelectedUnits(Vector2 target)
    {
        int unitCount = selectedUnits.Count;
        if (unitCount == 0) return;

        // 원형 포메이션 위치 계산
        Vector2[] formationPositions = CalculateCircularFormation(target, unitCount);

        // 각 유닛의 현재 위치와 가능한 목표 위치 간의 거리를 계산하여 최적의 위치 할당
        AssignOptimalPositions(formationPositions);
    }

    /// <summary>
    /// 각 유닛에게 가장 가까운 포메이션 위치를 할당하는 메서드
    /// </summary>
    /// <param name="formationPositions">포메이션의 목표 위치들</param>
    private void AssignOptimalPositions(Vector2[] formationPositions)
    {
        int unitCount = selectedUnits.Count;
        bool[] assignedPositions = new bool[unitCount];
        Dictionary<int, float> distanceCache = new Dictionary<int, float>();

        // 각 유닛에 대해 처리
        for (int unitIndex = 0; unitIndex < unitCount; unitIndex++)
        {
            var unit = selectedUnits[unitIndex];
            if (unit == null) continue;

            float shortestDistance = float.MaxValue;
            int bestPositionIndex = -1;

            // 각 남은 포메이션 위치에 대해 거리 계산
            for (int posIndex = 0; posIndex < formationPositions.Length; posIndex++)
            {
                if (assignedPositions[posIndex]) continue;

                // 현재 유닛과 포메이션 위치 간의 거리 계산
                float distance = Vector2.Distance(
                    (Vector2)unit.transform.position,
                    formationPositions[posIndex]
                );

                // 더 가까운 위치를 찾으면 업데이트
                if (distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestPositionIndex = posIndex;
                }
            }

            // 가장 가까운 위치로 유닛 이동
            if (bestPositionIndex != -1)
            {
                assignedPositions[bestPositionIndex] = true;
                unit.StopCurrentMovement();
                unit.moveCoroutine = unit.StartCoroutine(unit.MoveToTarget(formationPositions[bestPositionIndex]));
                unit.isMoving = true;
            }
        }
    }

    /// <summary>
    /// 원형 포메이션의 각 위치를 계산하는 메서드
    /// </summary>
    /// <param name="center">포메이션의 중심 위치</param>
    /// <param name="unitCount">배치할 유닛의 수</param>
    /// <returns>각 유닛의 목표 위치 배열</returns>
    private Vector2[] CalculateCircularFormation(Vector2 center, int unitCount)
    {
        Vector2[] positions = new Vector2[unitCount];
        
        // 원의 반지름 계산 (유닛 수에 따라 조정)
        float radius = formationSpacing * Mathf.Sqrt(unitCount);
        
        // 각 유닛의 위치 계산
        float angleStep = 360f / unitCount;
        for (int i = 0; i < unitCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad; // 각도를 라디안으로 변환
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            positions[i] = new Vector2(x, y);
        }
        
        return positions;
    }

    /// <summary>
    /// 순찰을 시작하는 메서드
    /// </summary>
    /// <param name="target">순찰 목표 위치</param>
    private void StartPatrol(Vector2 target)
    {
        StopCurrentMovement(); // 현재 진행 중인 이동 중지
        originalPosition = cachedTransform.position;
        patrolTarget = target;
        isPatrolling = true;
        patrolCoroutine = StartCoroutine(PatrolCoroutine());
        Debug.Log("순찰 시작!");
    }

    /// <summary>
    /// 순찰을 중지하는 메서드
    /// </summary>
    private void StopPatrol()
    {
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
        isPatrolling = false;
        Debug.Log("순찰 중지!");
    }

    /// <summary>
    /// 순찰 동작을 처리하는 코루틴
    /// 원래 위치와 목표 위치를 왕복하며 순찰
    /// </summary>
    private IEnumerator PatrolCoroutine()
    {
        bool movingToTarget = true;

        while (isPatrolling)
        {
            Vector2 currentTarget = movingToTarget ? patrolTarget : originalPosition;
            Vector2 currentPos = cachedTransform.position;

            while (Vector2.Distance(currentPos, currentTarget) > 0.1f)
            {
                if (!isPatrolling) yield break;
                
                currentPos = cachedTransform.position;
                cachedTransform.position = Vector2.MoveTowards(currentPos, currentTarget, speed * Time.deltaTime);
                yield return null;
            }

            // 목표 지점에 도달하면 잠시 대기
            yield return new WaitForSeconds(patrolWaitTime);
            
            // 다음 목표 지점으로 전환
            movingToTarget = !movingToTarget;
        }
    }

    /// <summary>
    /// 주변에서 가장 가까운 빈 위치를 찾는 메서드
    /// </summary>
    /// <param name="targetPosition">목표 위치</param>
    /// <returns>다른 유닛과 겹치지 않는 가장 가까운 위치</returns>
    private Vector2 FindNearestEmptyPosition(Vector2 targetPosition)
    {
        // 목표 위치에 다른 유닛이 없으면 그대로 반환
        if (!IsPositionOccupied(targetPosition))
        {
            return targetPosition;
        }

        float checkRadius = unitRadius * 2; // 검사 반경
        int maxRings = 8; // 최대 검사 링 수
        int pointsPerRing = 8; // 각 링당 검사할 점의 수

        // 나선형으로 주변 위치 검사
        for (int ring = 1; ring <= maxRings; ring++)
        {
            float ringRadius = ring * checkRadius;
            
            // 각 링에서 여러 방향의 점 검사
            for (int i = 0; i < pointsPerRing; i++)
            {
                float angle = (i * 360f / pointsPerRing) * Mathf.Deg2Rad;
                Vector2 offset = new Vector2(
                    Mathf.Cos(angle) * ringRadius,
                    Mathf.Sin(angle) * ringRadius
                );
                
                Vector2 testPosition = targetPosition + offset;
                
                // 위치가 비어있으면 반환
                if (!IsPositionOccupied(testPosition))
                {
                    return testPosition;
                }
            }
        }

        // 빈 위치를 찾지 못한 경우 원래 위치 반환
        return targetPosition;
    }

    /// <summary>
    /// 지정된 위치에 다른 유닛이 있는지 확인하는 메서드
    /// </summary>
    /// <param name="position">확인할 위치</param>
    /// <returns>유닛이 있으면 true, 없으면 false</returns>
    private bool IsPositionOccupied(Vector2 position)
    {
        // 주변의 유닛 검사
        Collider2D[] colliders = Physics2D.OverlapCircleAll(
            position,
            unitRadius,
            unitLayer
        );

        // 자신을 제외한 다른 유닛이 있는지 확인
        foreach (var collider in colliders)
        {
            if (collider.gameObject != gameObject)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 선택된 유닛을 시각적으로 표시하는 Gizmo
    /// </summary>
    void OnDrawGizmos()
    {
        if (isSelected)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, 0.5f);
        }
    }
}