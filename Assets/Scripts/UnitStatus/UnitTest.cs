using UnityEngine;

/// <summary>
/// 유닛 테스트를 위한 스크립트
/// </summary>
public class UnitTest : MonoBehaviour
{
    [SerializeField] private float damageAmount = 10f;    // 데미지량
    [SerializeField] private float healAmount = 20f;      // 회복량
    [SerializeField] private LayerMask unitLayer;         // 유닛 레이어

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 마우스 위치에서 레이캐스트
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity, unitLayer);

        if (hit.collider != null)
        {
            UnitStatus targetUnit = hit.collider.GetComponent<UnitStatus>();
            
            if (targetUnit != null)
            {
                // Space 키를 누르면 데미지
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    float actualDamage = targetUnit.TakeDamage(damageAmount);
                    Debug.Log($"Dealt {actualDamage} damage. Current Health: {targetUnit.CurrentHealth}");
                }
                
                // R 키를 누르면 회복
                if (Input.GetKeyDown(KeyCode.R))
                {
                    targetUnit.Heal(healAmount);
                    Debug.Log($"Healed {healAmount}. Current Health: {targetUnit.CurrentHealth}");
                }
            }
        }
    }
}
