using UnityEngine;

public class UnscaledTimeWrapper : MonoBehaviour
{
    [SerializeField] private CustomRenderTexture CustomRenderTexture;

    private void Update()
    {
        CustomRenderTexture.material.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}