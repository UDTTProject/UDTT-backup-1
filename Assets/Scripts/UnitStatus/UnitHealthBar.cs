using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유닛의 체력바를 관리하는 컴포넌트
/// </summary>
public class UnitHealthBar : MonoBehaviour
{
    [Header("Health Bar Settings")]
    [SerializeField] private Vector3 offset = new Vector3(0, 1.25f, 0); // 유닛 위의 오프셋 위치

    [Header("Health Bar Colors")]
    [SerializeField] private Color fullHealthColor = Color.green;   // 체력이 가득 찼을 때 색상
    [SerializeField] private Color lowHealthColor = Color.red;      // 체력이 낮을 때 색상
    [SerializeField] private float lowHealthThreshold = 0.3f;       // 낮은 체력 기준점 (30%)

    private UnitStatus unitStatus;                     // 유닛 상태 참조
    private Camera mainCamera;                         // 메인 카메라 참조
    private Image fillImage;                           // 체력바 이미지
    private GameObject healthBarObject;                // 체력바 게임오브젝트
    private Slider healthSlider;                       // 체력바 슬라이더

    private void Start()
    {
        mainCamera = Camera.main;
        unitStatus = GetComponent<UnitStatus>();
        
        // 체력바 생성
        CreateHealthBar();
        
        // 유닛 상태 컴포넌트가 없으면 경고
        if (unitStatus == null)
        {
            Debug.LogError("UnitHealthBar requires UnitStatus component!");
            enabled = false;
            return;
        }

        // 초기 체력바 설정
        healthSlider.maxValue = unitStatus.MaxHealth;
        healthSlider.value = unitStatus.CurrentHealth;
        UpdateHealthBarColor(unitStatus.CurrentHealth / unitStatus.MaxHealth);

        // 체력 변경 이벤트 구독
        unitStatus.onHealthChanged.AddListener(UpdateHealthBarColor);
    }

    private void CreateHealthBar()
    {
        // HealthBarManager를 통해 체력바 생성
        healthBarObject = HealthBarManager.Instance.CreateHealthBar();
        
        if (healthBarObject != null)
        {
            healthSlider = healthBarObject.GetComponent<Slider>();
            fillImage = healthSlider.fillRect.GetComponent<Image>();
        }
    }

    private void OnDestroy()
    {
        if (unitStatus != null)
        {
            unitStatus.onHealthChanged.RemoveListener(UpdateHealthBarColor);
        }

        if (healthBarObject != null)
        {
            Destroy(healthBarObject);
        }
    }

    private void LateUpdate()
    {
        if (healthBarObject != null && mainCamera != null)
        {
            // 체력바가 항상 카메라를 향하도록 설정
            healthBarObject.transform.rotation = mainCamera.transform.rotation;
            
            // 체력바 위치 업데이트
            healthBarObject.transform.position = transform.position + offset;
            
            // 체력바 값 업데이트
            healthSlider.value = unitStatus.CurrentHealth;
        }
    }

    private void UpdateHealthBarColor(float healthPercent)
    {
        if (fillImage != null)
        {
            fillImage.color = Color.Lerp(lowHealthColor, fullHealthColor, healthPercent);
        }
    }

    public void SetHealthBarVisible(bool visible)
    {
        if (healthBarObject != null)
        {
            healthBarObject.SetActive(visible);
        }
    }
}
