using UnityEngine;
using UnityEngine.Events;

public class ResourceManager : MonoBehaviour
{
    [SerializeField] private SliderController costSlider;  // 코스트 슬라이더 참조
    private int currentResources;

    public UnityEvent<int> onResourcesChanged;

    private void Awake()
    {
        onResourcesChanged = new UnityEvent<int>();
    }

    private void Start()
    {
        if (costSlider == null)
        {
            costSlider = FindObjectOfType<SliderController>();
        }
    }

    public bool CanSpendResources(int amount)
    {
        return costSlider != null && costSlider.slider.value >= amount;
    }

    public bool SpendResources(int amount)
    {
        if (!CanSpendResources(amount)) return false;

        costSlider.slider.value -= amount;
        onResourcesChanged.Invoke((int)costSlider.slider.value);
        return true;
    }

    public int GetCurrentResources()
    {
        return costSlider != null ? (int)costSlider.slider.value : 0;
    }
}
