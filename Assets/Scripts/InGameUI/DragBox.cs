using UnityEngine;

public class DragBox : MonoBehaviour
{
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isSelecting = false;
    private float dragThreshold = 5f; // 드래그 감지 최소 거리

    void OnGUI()
    {
        if (isSelecting)
        {
            // 올바른 사각형 생성
            var rect = GetScreenRect(startPosition, endPosition);

            // 선택 박스 스타일
            GUI.color = new Color(0.5f, 0.8f, 1f, 0.3f);
            GUI.Box(rect, "");
            GUI.color = Color.white;
        }
    }

    void Update()
    {
        // 마우스 왼쪽 버튼을 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            isSelecting = false; // 처음엔 드래그가 아님
        }

        // 마우스를 움직이면 드래그 여부 확인
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;

            // 클릭인지 드래그인지 확인
            if (!isSelecting && Vector2.Distance(startPosition, endPosition) > dragThreshold)
            {
                isSelecting = true; // 일정 거리 이상 움직이면 드래그 시작
            }
        }

        // 마우스 버튼을 놓았을 때
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false; // 드래그 종료
        }
    }

    // 드래그 방향과 상관없이 올바른 사각형 생성
    Rect GetScreenRect(Vector2 screenStart, Vector2 screenEnd)
    {
        screenStart.y = Screen.height - screenStart.y;
        screenEnd.y = Screen.height - screenEnd.y;

        Vector2 topLeft = Vector2.Min(screenStart, screenEnd);
        Vector2 bottomRight = Vector2.Max(screenStart, screenEnd);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
