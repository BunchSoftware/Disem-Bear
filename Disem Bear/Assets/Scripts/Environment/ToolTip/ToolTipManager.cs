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
    [Tooltip("Time Fade ToolTip")]
    [SerializeField] private float timeOfAppearanceToolTip = 0.5f;
    [SerializeField] private float timeOfDissaperenceToolTip = 0.5f;

    private Image ToolTipImage;

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
        ToolTipImage.DOFade(1f, timeOfAppearanceToolTip);
        ToolTipText.DOFade(1f, timeOfAppearanceToolTip);
    }

    public void ToolTipOn(string message, float timeOfAppearanceToolTip)
    {
        ToolTipText.text = message;
        ToolTipImage.DOFade(1f, timeOfAppearanceToolTip);
        ToolTipText.DOFade(1f, timeOfAppearanceToolTip);
    }

    public void ToolTipOff(float timeOfDissaperenceToolTip)
    {
        ToolTipImage.DOFade(0f, timeOfDissaperenceToolTip);
        ToolTipText.DOFade(0f, timeOfDissaperenceToolTip);
        ToolTipText.text = string.Empty;
    }

    public void ToolTipOff()
    {
        ToolTipImage.DOFade(0f, timeOfDissaperenceToolTip);
        ToolTipText.DOFade(0f, timeOfDissaperenceToolTip);
        ToolTipText.text = string.Empty;
    }
}
