using UnityEngine;

public class DragBox : MonoBehaviour
{
    private Vector2 startPosition;
    private Vector2 endPosition;
    private Rect selectionRect;
    private bool isSelecting = false;

    void OnGUI()
    {
        // 드래그 중일 때만 박스 그리기
        if (isSelecting)
        {
            // 드래그 방향에 상관없이 올바른 사각형 그리기
            var rect = GetScreenRect(startPosition, endPosition);

            // 선택 박스 스타일 설정
            GUI.color = new Color(0.5f, 0.8f, 1f, 0.3f);
            GUI.Box(rect, "");
            GUI.color = Color.white;
        }
    }

    void Update()
    {
        // 마우스 왼쪽 버튼 눌렀을 때
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            isSelecting = true;
        }

        // 마우스 버튼 누르고 있을 때 끝점 업데이트
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;
        }

        // 마우스 버튼 놓았을 때
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
        }
    }

    // 드래그 방향과 상관없이 올바른 사각형 생성
    Rect GetScreenRect(Vector2 screenStart, Vector2 screenEnd)
    {
        // 좌측 상단에서 우측 하단으로 드래그할 때와
        // 우측 상단에서 좌측 하단으로 드래그할 때 모두 처리
        screenStart.y = Screen.height - screenStart.y;
        screenEnd.y = Screen.height - screenEnd.y;

        Vector2 topLeft = Vector2.Min(screenStart, screenEnd);
        Vector2 bottomRight = Vector2.Max(screenStart, screenEnd);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}