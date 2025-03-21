using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpawnUI : MonoBehaviour
{
    [Header("UI References")]
    private GameObject uiPanel;          // UI 패널
    private Transform buttonContainer;   // 버튼들이 들어갈 컨테이너
    private GameObject unitButtonPrefab; // 유닛 버튼 프리팹

    [Header("UI Settings")]
    [SerializeField] private Vector2 panelSize = new Vector2(150, 200);     // 패널 크기
    [SerializeField] private Vector2 containerSize = new Vector2(130, 180); // 컨테이너 크기
    [SerializeField] private Vector2 buttonSize = new Vector2(60, 60);      // 버튼 크기
    [SerializeField] private Vector2 buttonSpacing = new Vector2(5, 5);     // 버튼 간격

    [Header("Screen Position")]
    [SerializeField] private Vector2 screenOffset = new Vector2(100, 100);  // 화면 기준 오프셋
    [SerializeField] private ScreenAnchor screenAnchor = ScreenAnchor.TopRight;  // 화면 기준점

    public enum ScreenAnchor
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        Center
    }

    private BuildingSpawner spawner;
    private UnitData[] availableUnits;
    private Canvas canvas;
    private RectTransform panelRect;

    private void Awake()
    {
        CreateUI();
    }

    public void Initialize(UnitData[] units, BuildingSpawner buildingSpawner)
    {
        if (units == null || units.Length == 0)
        {
            Debug.LogError("No units provided to SpawnUI");
            return;
        }

        Debug.Log($"Initializing SpawnUI with {units.Length} units");
        availableUnits = units;
        spawner = buildingSpawner;

        // 기본 Unit Button Prefab 생성
        if (unitButtonPrefab == null)
        {
            CreateDefaultUnitButton();
        }

        // 유닛 버튼 생성
        if (buttonContainer != null)
        {
            CreateUnitButtons();
        }
        else
        {
            Debug.LogError("Button container is null after initialization");
        }

        // 초기에는 UI 숨기기
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("UI Panel is null after initialization");
        }
    }

    private void CreateDefaultUnitButton()
    {
        // 기본 버튼 프리팹 생성
        GameObject buttonObj = new GameObject("UnitButton");
        
        // 버튼 컴포넌트 추가
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = Color.white;

        // 유닛 아이콘 이미지 추가
        GameObject iconObj = new GameObject("Icon");
        iconObj.transform.SetParent(buttonObj.transform, false);
        Image iconImage = iconObj.AddComponent<Image>();
        RectTransform iconRect = iconObj.GetComponent<RectTransform>();
        iconRect.anchorMin = new Vector2(0.1f, 0.1f);
        iconRect.anchorMax = new Vector2(0.9f, 0.9f);
        iconRect.sizeDelta = Vector2.zero;

        // 코스트 텍스트 추가
        GameObject costObj = new GameObject("CostText", typeof(RectTransform));
        costObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI costText = costObj.AddComponent<TextMeshProUGUI>();
        costText.alignment = TextAlignmentOptions.Bottom;
        costText.fontSize = 12;
        
        RectTransform costRect = costObj.GetComponent<RectTransform>();
        costRect.anchorMin = new Vector2(0, 0);
        costRect.anchorMax = new Vector2(1, 0.3f);
        costRect.sizeDelta = Vector2.zero;

        // UnitButton 스크립트 추가
        UnitButton unitButton = buttonObj.AddComponent<UnitButton>();
        unitButton.SetReferences(iconImage, costText, button);

        // 프리팹 설정
        unitButtonPrefab = buttonObj;
    }

    private void CreateUI()
    {
        Debug.Log("Creating UI elements");

        // 기존 UI 제거
        if (canvas != null)
        {
            Destroy(canvas.gameObject);
        }
        
        // UI 캔버스 생성
        GameObject canvasObj = new GameObject("SpawnCanvas");
        canvasObj.transform.SetParent(transform);
        canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // UI 패널 생성
        GameObject panelObj = new GameObject("SpawnPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        uiPanel = panelObj;
        
        Image panelImage = panelObj.AddComponent<Image>();
        panelImage.color = new Color(0, 0, 0, 0.8f);
        panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.sizeDelta = panelSize;

        // 버튼 컨테이너 생성
        GameObject containerObj = new GameObject("ButtonContainer");
        containerObj.transform.SetParent(panelObj.transform, false);
        
        RectTransform containerRect = containerObj.AddComponent<RectTransform>();
        containerRect.anchorMin = Vector2.zero;
        containerRect.anchorMax = Vector2.one;
        containerRect.sizeDelta = containerSize;
        containerRect.anchoredPosition = Vector2.zero;
        
        // 그리드 레이아웃 추가
        GridLayoutGroup grid = containerObj.AddComponent<GridLayoutGroup>();
        grid.cellSize = buttonSize;
        grid.spacing = buttonSpacing;
        grid.padding = new RectOffset(5, 5, 5, 5);

        // 버튼 컨테이너 참조 저장
        buttonContainer = containerObj.transform;

        UpdatePanelPosition();
        
        Debug.Log($"UI creation completed. Panel: {uiPanel != null}, Container: {buttonContainer != null}");
    }

    private void CreateUnitButtons()
    {
        if (buttonContainer == null)
        {
            Debug.LogError("Button container is null");
            return;
        }

        if (availableUnits == null || availableUnits.Length == 0)
        {
            Debug.LogError("Available units array is null or empty");
            return;
        }

        Debug.Log($"Creating {availableUnits.Length} unit buttons");

        // 기존 버튼들 제거
        foreach (Transform child in buttonContainer)
        {
            Destroy(child.gameObject);
        }

        // 각 유닛에 대한 버튼 생성
        foreach (UnitData unit in availableUnits)
        {
            if (unit == null)
            {
                Debug.LogError("Unit data is null");
                continue;
            }

            GameObject buttonObj = Instantiate(unitButtonPrefab, buttonContainer);
            UnitButton unitButton = buttonObj.GetComponent<UnitButton>();
            
            if (unitButton != null)
            {
                unitButton.Initialize(unit, spawner);
                Debug.Log($"Created button for unit: {unit.UnitName}");
            }
            else
            {
                Debug.LogError("UnitButton component not found on instantiated button");
            }
        }
    }

    public void ToggleUI()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
            if (uiPanel.activeSelf)
            {
                UpdatePanelPosition();
            }
        }
        else
        {
            Debug.LogError("UI Panel is null in ToggleUI");
        }
    }

    private void UpdatePanelPosition()
    {
        if (panelRect == null) return;

        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 anchorPosition = Vector2.zero;
        
        switch (screenAnchor)
        {
            case ScreenAnchor.TopLeft:
                panelRect.pivot = new Vector2(0, 1);
                anchorPosition = new Vector2(0, screenSize.y);
                break;
            case ScreenAnchor.TopRight:
                panelRect.pivot = new Vector2(1, 1);
                anchorPosition = new Vector2(screenSize.x, screenSize.y);
                break;
            case ScreenAnchor.BottomLeft:
                panelRect.pivot = new Vector2(0, 0);
                anchorPosition = Vector2.zero;
                break;
            case ScreenAnchor.BottomRight:
                panelRect.pivot = new Vector2(1, 0);
                anchorPosition = new Vector2(screenSize.x, 0);
                break;
            case ScreenAnchor.Center:
                panelRect.pivot = new Vector2(0.5f, 0.5f);
                anchorPosition = screenSize * 0.5f;
                break;
        }

        panelRect.position = anchorPosition + screenOffset;
    }

    private void OnRectTransformDimensionsChange()
    {
        UpdatePanelPosition();
    }
}
