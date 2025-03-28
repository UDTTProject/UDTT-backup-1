using UnityEngine;

public class AttackRangeCircle : MonoBehaviour
{
    [SerializeField] private BaseStatusData baseStatusData;
    public float attackRange => baseStatusData.attackRange;
    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 101; // 원을 완전히 닫기 위해 101개의 점 사용
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;

        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // 라인이 연속적으로 보이도록 설정
        lineRenderer.loop = true; // 중요: 선을 닫힌 루프로 만듦
        lineRenderer.sortingOrder = 10;
    }

    void Update()
    {
        if (IsMouseOverUnit())
        {
            DrawAttackRange();
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private bool IsMouseOverUnit()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        return Vector3.Distance(mousePos, transform.position) <= attackRange;
    }

    private void DrawAttackRange()
    {
        lineRenderer.enabled = true;
        float angleStep = 360f / (lineRenderer.positionCount - 1); // -1을 해서 완전한 원 생성
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * attackRange,
                Mathf.Sin(angle) * attackRange,
                0
            );
            lineRenderer.SetPosition(i, transform.position + position);
        }
    }
}