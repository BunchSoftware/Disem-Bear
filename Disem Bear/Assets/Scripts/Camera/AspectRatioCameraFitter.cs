using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteAlways]
public class AspectRatioCameraFitter : MonoBehaviour
{
    private readonly Vector2 targetAspectRatio = new Vector2(16, 9);
    private readonly Vector2 rectCenter = new Vector2(0.5f, 0.5f);

    private Vector2 lastResolution;

    [SerializeField] private Camera m_camera;

    public void OnValidate()
    {
        m_camera = GetComponent<Camera>();
    }

    public void LateUpdate()
    {
        Vector2 currentScreenResolution = new Vector2(Screen.width, Screen.height);
        if (lastResolution != currentScreenResolution)
        {
            CalculateCameraRect(currentScreenResolution);
        }

        lastResolution = currentScreenResolution;
    }

    private void CalculateCameraRect(Vector2 currentScreenResolution)
    {
        Vector2 normalizedAspectRatio = targetAspectRatio / currentScreenResolution;
        Vector2 size = normalizedAspectRatio / Mathf.Max(normalizedAspectRatio.x, normalizedAspectRatio.y);
        m_camera.rect = new Rect(default, size) { center = rectCenter };
    }
}
