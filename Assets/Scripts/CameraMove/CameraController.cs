using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed = 10f; // 카메라 이동 속도
    public float edgeThreshold = 10f; // 화면 가장자리 감지 범위 (픽셀 단위)
    public float zoomSpeed = 5f; // 줌 속도
    public float minZoom = 2f; // 최소 줌 값
    public float maxZoom = 10f; // 최대 줌 값

    private Vector2 screenSize;
    private Camera cam;

    void Start()
    {
        screenSize = new Vector2(Screen.width, Screen.height);
        cam = Camera.main;
    }

    void Update()
    {
        Vector3 direction = Vector3.zero;
        Vector3 mousePosition = Input.mousePosition;

        if (mousePosition.x <= edgeThreshold) // 왼쪽 가장자리
        {
            direction.x = -1;
        }
        else if (mousePosition.x >= screenSize.x - edgeThreshold) // 오른쪽 가장자리
        {
            direction.x = 1;
        }

        if (mousePosition.y <= edgeThreshold) // 아래쪽 가장자리
        {
            direction.y = -1;
        }
        else if (mousePosition.y >= screenSize.y - edgeThreshold) // 위쪽 가장자리
        {
            direction.y = 1;
        }

        transform.position += direction * moveSpeed * Time.deltaTime;

        // 마우스 휠 줌 기능
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            cam.orthographicSize -= scroll * zoomSpeed;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, minZoom, maxZoom);
        }
    }
}
