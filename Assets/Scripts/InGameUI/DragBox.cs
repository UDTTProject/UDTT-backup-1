using UnityEngine;

public class DragBox : MonoBehaviour
{
    private Vector2 startPosition;
    private Vector2 endPosition;
    private bool isSelecting = false;
    private float dragThreshold = 5f; // �巡�� ���� �ּ� �Ÿ�

    void OnGUI()
    {
        if (isSelecting)
        {
            // �ùٸ� �簢�� ����
            var rect = GetScreenRect(startPosition, endPosition);

            // ���� �ڽ� ��Ÿ��
            GUI.color = new Color(0.5f, 0.8f, 1f, 0.3f);
            GUI.Box(rect, "");
            GUI.color = Color.white;
        }
    }

    void Update()
    {
        // ���콺 ���� ��ư�� ������ ��
        if (Input.GetMouseButtonDown(0))
        {
            startPosition = Input.mousePosition;
            isSelecting = false; // ó���� �巡�װ� �ƴ�
        }

        // ���콺�� �����̸� �巡�� ���� Ȯ��
        if (Input.GetMouseButton(0))
        {
            endPosition = Input.mousePosition;

            // Ŭ������ �巡������ Ȯ��
            if (!isSelecting && Vector2.Distance(startPosition, endPosition) > dragThreshold)
            {
                isSelecting = true; // ���� �Ÿ� �̻� �����̸� �巡�� ����
            }
        }

        // ���콺 ��ư�� ������ ��
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false; // �巡�� ����
        }
    }

    // �巡�� ����� ������� �ùٸ� �簢�� ����
    Rect GetScreenRect(Vector2 screenStart, Vector2 screenEnd)
    {
        screenStart.y = Screen.height - screenStart.y;
        screenEnd.y = Screen.height - screenEnd.y;

        Vector2 topLeft = Vector2.Min(screenStart, screenEnd);
        Vector2 bottomRight = Vector2.Max(screenStart, screenEnd);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }
}
