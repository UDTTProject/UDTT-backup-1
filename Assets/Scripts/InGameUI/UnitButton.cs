using UnityEngine;
using UnityEngine.UI;

public class UnitButton : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject buttonPanel; // 버튼들을 포함하는 패널

    [Header("Target Object")]
    public GameObject spawnPoint; // 클릭할 대상 오브젝트

    private bool isVisible = false; // 버튼의 현재 활성화 상태

    void Start()
    {
        buttonPanel.SetActive(isVisible); // 시작 시 버튼 숨김
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // 마우스 왼쪽 버튼 클릭 감지
        {
            CheckObjectClick();
        }
    }

    void CheckObjectClick()
    {
        // 2D Raycast 사용
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 마우스 위치를 2D 월드 좌표로 변환
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero); // 2D Raycast 쏘기

        if (hit.collider != null) // 충돌한 오브젝트가 있을 경우
        {
            if (hit.collider.gameObject == spawnPoint) // 클릭한 오브젝트가 특정 오브젝트라면
            {
                ToggleButtons();
                Debug.Log("클릭됨");
            }
        }
    }

    void ToggleButtons()
    {
        isVisible = !isVisible; // 상태 반전
        buttonPanel.SetActive(isVisible); // 버튼 패널 보이기/숨기기
    }
}
