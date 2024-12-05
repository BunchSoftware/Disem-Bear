using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UscaledTimeWrapper : MonoBehaviour
{
    [SerializeField] private CustomRenderTexture CustomRenderTexture;

    private void Update()
    {
       CustomRenderTexture.material.SetFloat("_UnscaledTime", Time.unscaledTime);
    }
}
