using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UnitButton : MonoBehaviour
{
    private Image icon;
    private TextMeshProUGUI costText;
    private Button button;
    private UnitData unitData;
    private BuildingSpawner spawner;

    public void SetReferences(Image icon, TextMeshProUGUI cost, Button btn)
    {
        this.icon = icon;
        costText = cost;
        button = btn;
    }

    public void Initialize(UnitData data, BuildingSpawner buildingSpawner)
    {
        if (data == null)
        {
            Debug.LogError("Unit data is null in UnitButton Initialize");
            return;
        }

        Debug.Log($"Initializing UnitButton for unit: {data.UnitName}");
        unitData = data;
        spawner = buildingSpawner;

        // 컴포넌트가 없다면 찾기
        if (icon == null) icon = transform.Find("Icon")?.GetComponent<Image>();
        if (costText == null) costText = transform.Find("CostText")?.GetComponent<TextMeshProUGUI>();
        if (button == null) button = GetComponent<Button>();

        if (icon == null || costText == null || button == null)
        {
            Debug.LogError("Missing required components in UnitButton");
            return;
        }

        // UI 설정
        icon.sprite = data.Icon;
        costText.text = data.Cost.ToString();
        
        // 버튼 이벤트 설정
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => OnButtonClick());
    }

    private void OnButtonClick()
    {
        if (spawner != null && unitData != null)
        {
            Debug.Log($"Attempting to spawn unit: {unitData.UnitName}");
            spawner.TrySpawnUnit(unitData);
        }
        else
        {
            Debug.LogError($"Spawner or UnitData is null. Spawner: {spawner}, UnitData: {unitData}");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
