using UnityEngine;
using UnityEngine.EventSystems;

public class CursorManage : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static Texture2D hand;
    private static Texture2D original;

    void Start()
    {
        // 한 번만 로드 (모든 UI 요소에서 공유)
        if (hand == null)
            hand = Resources.Load<Texture2D>("hand");

        if (original == null)
            original = Resources.Load<Texture2D>("original");

        // 기본 커서 설정
        if (original != null)
            Cursor.SetCursor(original, Vector2.zero, CursorMode.Auto);
    }

    // UI 요소에 마우스 진입 시 호출됨
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hand != null)
            Cursor.SetCursor(hand, new Vector2(hand.width / 3, 0), CursorMode.Auto);
    }

    // UI 요소에서 마우스 나갈 때 호출됨
    public void OnPointerExit(PointerEventData eventData)
    {
        if (original != null)
            Cursor.SetCursor(original, Vector2.zero, CursorMode.Auto);
    }

    // 2D 오브젝트용 (Collider 필요)
    void OnMouseOver()
    {
        if (hand != null)
            Cursor.SetCursor(hand, new Vector2(hand.width / 3, 0), CursorMode.Auto);
    }

    void OnMouseExit()
    {
        if (original != null)
            Cursor.SetCursor(original, Vector2.zero, CursorMode.Auto);
    }
}
