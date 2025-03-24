using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SliderController : MonoBehaviour
{
    public Slider slider;
    public float fillDuration = 3f; // add 1 per 3 second
    private int maxValue = 10;
    [SerializeField] private TextMeshProUGUI sliderValueText; // 슬라이더 값을 표시할 텍스트
    // ---------------------------------------------------------
    public TextMeshProUGUI timerText;
    public int startMinutes = 10; // start minute
    public float timeRemaining; // remain second
    private bool isRunning = false;
    // ---------------------------------------------------------
    
    // BaseStatusData 참조 추가
    [SerializeField] private BaseStatusData baseStatusDataA;
    [SerializeField] private BaseStatusData baseStatusDataB;
    [SerializeField] private BaseStatusData baseStatusDataC;

    public Button buttonA;
    public Button buttonB;
    public Button buttonC;

    // ---------------------------------------------------------
    void Start()
    {
        StartCoroutine(FillSlider());

        timeRemaining = startMinutes * 60; // 10분을 초로 변환
        StartCoroutine(TimerCountdown());
        
        // 초기 슬라이더 값 표시
        UpdateSliderValueText();

        // 버튼 클릭 이벤트 등록
        buttonA.onClick.AddListener(() => UseSkill(baseStatusDataA));
        buttonB.onClick.AddListener(() => UseSkill(baseStatusDataB));
        buttonC.onClick.AddListener(() => UseSkill(baseStatusDataC));
    }

    // 슬라이더 값을 텍스트로 표시하는 메서드
    private void UpdateSliderValueText()
    {
        if (sliderValueText != null)
        {
            sliderValueText.text = Mathf.FloorToInt(slider.value).ToString();
        }
    }

    IEnumerator FillSlider()
    {
        while (true)
        {
            float startValue = slider.value;
            float endValue = Mathf.Min(startValue + 1, maxValue); // 최대값 초과 방지
            float elapsedTime = 0f;

            while (elapsedTime < fillDuration)
            {
                slider.value = Mathf.Lerp(startValue, endValue, elapsedTime / fillDuration);
                UpdateSliderValueText(); // 슬라이더 값 업데이트
                elapsedTime += Time.deltaTime;

                yield return null;
            }

            slider.value = endValue; // 보정
            UpdateSliderValueText(); // 최종 값 업데이트

            yield return new WaitForSeconds(0.1f); // delay
        }
    }

    IEnumerator TimerCountdown() // timer coroutine
    {
        isRunning = true;

        while (timeRemaining > 0)
        {
            UpdateTimerText();
            yield return new WaitForSeconds(1f);
            timeRemaining--;
        }

        timeRemaining = 0;
        UpdateTimerText(); // 00:00 표시
        isRunning = false;
    }

    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(timeRemaining / 60);
        int seconds = Mathf.FloorToInt(timeRemaining % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // 버튼 클릭 시 호출할 메서드
    private void UseSkill(BaseStatusData skillData)
    {
        if (skillData != null && slider.value >= skillData.cost)
        {
            slider.value -= skillData.cost;
            Debug.Log($"스킬 사용: 슬라이더 값 {skillData.cost} 감소");
            UpdateSliderValueText();
        }
    }
}
