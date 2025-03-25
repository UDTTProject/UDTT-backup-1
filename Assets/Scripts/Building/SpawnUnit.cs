using UnityEngine;
using UnityEngine.UI;  // UI 관련 기능 사용

public class SpawnUnit : MonoBehaviour
{
    public GameObject unitPrefab1;  // 프리팹 1
    public GameObject unitPrefab2;  // 프리팹 2
    public GameObject unitPrefab3;  // 프리팹 3
    public Button buttonA;  // 버튼 A
    public Button buttonB;  // 버튼 B
    public Button buttonC;  // 버튼 C

    private void Start()
    {
        // 각 버튼에 리스너 추가 (코드로 연결)
        buttonA.onClick.AddListener(() => SpawnUnitAtPosition(unitPrefab1));
        buttonB.onClick.AddListener(() => SpawnUnitAtPosition(unitPrefab2));
        buttonC.onClick.AddListener(() => SpawnUnitAtPosition(unitPrefab3));
    }

    // 버튼 클릭 시 호출되는 메서드
    private void SpawnUnitAtPosition(GameObject prefab)
    {
        // (0, 0, 0) 위치에 프리팹을 소환
        if(prefab != null)
        {
            Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            Debug.LogError("프리팹이 연결되지 않았습니다!");
        }
    }
}
