using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager _instance;
    [SerializeField] private TextMeshProUGUI ToolTipText;
    private Image ToolTipImage;
    [Tooltip("Time Fade ToolTip")]
    [SerializeField] private float ToolTipTime = 0.5f;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        ToolTipImage = GetComponent<Image>();
    }
    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void ToolTipOn(string message)
    {
        ToolTipText.text = message;
        ToolTipImage.DOFade(1f, ToolTipTime);
        ToolTipText.DOFade(1f, ToolTipTime);
    }

    public void ToolTipOff()
    {
        ToolTipImage.DOFade(0f, ToolTipTime);
        ToolTipText.DOFade(0f, ToolTipTime);
        ToolTipText.text = string.Empty;
    }

}
