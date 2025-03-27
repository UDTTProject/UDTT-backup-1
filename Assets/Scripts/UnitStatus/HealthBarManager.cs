using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 모든 유닛의 체력바를 관리하는 매니저 클래스
/// </summary>
public class HealthBarManager : MonoBehaviour
{
    private static HealthBarManager instance;
    public static HealthBarManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<HealthBarManager>();
                
                if (instance == null)
                {
                    GameObject go = new GameObject("HealthBarManager");
                    instance = go.AddComponent<HealthBarManager>();
                    instance.Initialize();
                }
            }
            return instance;
        }
    }

    [Header("Required References")]
    [SerializeField] private GameObject healthBarPrefab;
    private Canvas worldSpaceCanvas;

    // 시작함수
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 초기화
    private void Initialize()
    {
        GameObject canvasObj = new GameObject("WorldSpaceCanvas");
        canvasObj.transform.SetParent(transform);

        worldSpaceCanvas = canvasObj.AddComponent<Canvas>();
        worldSpaceCanvas.renderMode = RenderMode.WorldSpace;
        worldSpaceCanvas.worldCamera = Camera.main;

        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.scaleFactor = 1;
        scaler.dynamicPixelsPerUnit = 100;

        canvasObj.AddComponent<GraphicRaycaster>();
    }

    // 체력바 생성
    public GameObject CreateHealthBar()
    {
        if (healthBarPrefab == null || worldSpaceCanvas == null)
        {
            return null;
        }

        GameObject healthBar = Instantiate(healthBarPrefab, worldSpaceCanvas.transform);
        healthBar.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        
        Slider slider = healthBar.GetComponent<Slider>();
        if (slider != null)
        {
            slider.minValue = 0;
            slider.maxValue = 1;
            slider.value = 1;
        }

        return healthBar;
    }
}
