using UnityEngine;
using UnityEngine.Events;

public class BuildingSpawner : MonoBehaviour
{
    [Header("Spawner Settings")]
    [SerializeField] private Transform spawnPoint;        // 유닛이 생성될 위치
    [SerializeField] private float spawnRadius = 2f;      // 생성 반경

    [Header("Cost Settings")]
    [SerializeField] private ResourceManager resourceManager;  // 자원 관리자

    [Header("Unit Prefabs")]
    [SerializeField] private UnitData[] availableUnits;   // 생성 가능한 유닛 목록

    private SpawnUI spawnUI;                              // 소환 UI

    private void Start()
    {
        // 컴포넌트 확인
        if (availableUnits == null || availableUnits.Length == 0)
        {
            Debug.LogError("No available units set in BuildingSpawner");
            return;
        }

        // 유닛 데이터 유효성 검사
        for (int i = 0; i < availableUnits.Length; i++)
        {
            if (availableUnits[i] == null)
            {
                Debug.LogError($"Unit Data at index {i} is null");
                return;
            }
            if (availableUnits[i].UnitPrefab == null)
            {
                Debug.LogError($"Unit Prefab is missing for {availableUnits[i].UnitName}");
                return;
            }
            if (availableUnits[i].Icon == null)
            {
                Debug.LogError($"Icon is missing for {availableUnits[i].UnitName}");
                return;
            }
        }

        if (resourceManager == null)
        {
            Debug.LogWarning("Resource Manager not set in BuildingSpawner");
        }

        if (spawnPoint == null)
        {
            spawnPoint = transform;
            Debug.Log("Using building transform as spawn point");
        }

        // SpawnUI 컴포넌트 가져오기
        spawnUI = GetComponent<SpawnUI>();
        if (spawnUI == null)
        {
            Debug.LogError("SpawnUI component not found on BuildingSpawner");
            return;
        }

        // SpawnUI 초기화
        Debug.Log($"Initializing SpawnUI with {availableUnits.Length} units");
        spawnUI.Initialize(availableUnits, this);
    }

    private void OnMouseDown()
    {
        if (spawnUI != null)
        {
            Debug.Log("Building clicked, toggling UI");
            spawnUI.ToggleUI();
        }
        else
        {
            Debug.LogError("SpawnUI is null in BuildingSpawner");
        }
    }

    // 유닛 소환 시도
    public bool TrySpawnUnit(UnitData unitData)
    {
        if (unitData == null)
        {
            Debug.LogError("Unit data is null in TrySpawnUnit");
            return false;
        }

        Debug.Log($"Attempting to spawn unit: {unitData.UnitName}, Cost: {unitData.Cost}");

        // 자원 체크
        if (resourceManager != null)
        {
            if (!resourceManager.CanSpendResources(unitData.Cost))
            {
                Debug.Log("Not enough resources to spawn unit");
                return false;
            }
            resourceManager.SpendResources(unitData.Cost);
        }

        // 랜덤한 위치에 유닛 생성
        Vector2 randomOffset = Random.insideUnitCircle * spawnRadius;
        Vector3 spawnPosition = spawnPoint.position + new Vector3(randomOffset.x, randomOffset.y, 0);
        
        GameObject newUnit = Instantiate(unitData.UnitPrefab, spawnPosition, Quaternion.identity);
        
        if (newUnit != null)
        {
            Debug.Log($"Successfully spawned unit: {unitData.UnitName}");
            return true;
        }
        
        Debug.LogError("Failed to spawn unit");
        return false;
    }
}
