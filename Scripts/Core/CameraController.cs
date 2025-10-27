using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    public float panSpeed = 20f;
    public float zoomSpeed = 10f;
    public float minOrtho = 3f;
    public float maxOrtho = 40f;

    public Transform worldTarget; // assign WorldQuad transform to clamp within bounds

    Camera cam;

    void Awake() { cam = GetComponent<Camera>(); if (!cam.orthographic) cam.orthographic = true; }

    void Update()
    {
        // Mouse pan (right button) / WASD
        Vector3 delta = Vector3.zero;
        if (Input.GetMouseButton(1))
        {
            delta.x -= Input.GetAxis("Mouse X") * panSpeed * Time.deltaTime;
            delta.z -= Input.GetAxis("Mouse Y") * panSpeed * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.A)) delta.x -= panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.D)) delta.x += panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.S)) delta.z -= panSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.W)) delta.z += panSpeed * Time.deltaTime;

        transform.position += delta;

        // Zoom
        float scroll = Input.mouseScrollDelta.y;
        if (Mathf.Abs(scroll) > 0.001f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - scroll * zoomSpeed * Time.deltaTime * 10f, minOrtho, maxOrtho);
        }

        // (Optional) clamp to world bounds
        if (worldTarget)
        {
            var b = worldTarget.GetComponent<Renderer>().bounds;
            var p = transform.position;
            p.x = Mathf.Clamp(p.x, b.min.x, b.max.x);
            p.z = Mathf.Clamp(p.z, b.min.z, b.max.z);
            transform.position = p;
        }
    }
}
