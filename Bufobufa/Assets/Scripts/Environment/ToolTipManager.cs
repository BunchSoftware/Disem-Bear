using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ToolTipManager : MonoBehaviour
{
    public static ToolTipManager _instance;
    [SerializeField] private TextMeshProUGUI ToolTipText;

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
        gameObject.SetActive(false);
    }
    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void ToolTipOn(string message)
    {
        ToolTipText.text = message;
        gameObject.SetActive(true);
    }

    public void ToolTipOff()
    {
        gameObject.SetActive(false);
        ToolTipText.text = string.Empty;
    }

}
