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
    void Start()
    {
        StartCoroutine(FillSlider());

        timeRemaining = startMinutes * 60; // 10분을 초로 변환
        StartCoroutine(TimerCountdown());
        
        // 초기 슬라이더 값 표시
        UpdateSliderValueText();
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

                if (Input.GetKeyDown(KeyCode.A) && slider.value >= 3)
                {
                    slider.value -= 3;
                    Debug.Log("A키 감지: 슬라이더 값 3 감소");

                    // 감소 후 다시 증가하도록 변수 재설정
                    startValue = slider.value;
                    endValue = Mathf.Min(startValue + 1, maxValue);
                    elapsedTime = 0f;
                    
                    UpdateSliderValueText(); // 슬라이더 값 감소 후 업데이트
                }

                if (timeRemaining == 590)
                {
                    fillDuration = 1.5f;
                }

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
}
