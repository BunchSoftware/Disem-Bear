using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UGCPointGUI : MonoBehaviour
{
    [SerializeField] private Button checkMark;
    [SerializeField] private Button checkMarkNull;
    [SerializeField] private Button editButton;
    [SerializeField] private Text header;
    [SerializeField] private Image avatar;

    private UGCPoint ugcPoint;

    private bool isCheck = false;
    public void Init(UGCPoint ugcPoint, Sprite avatar, string header, Action onClickEdit)
    {
        this.ugcPoint = ugcPoint;
        this.avatar.sprite = avatar;
        this.header.text = header;

        editButton.onClick.AddListener(() =>
        {
            onClickEdit?.Invoke();
        });

        checkMark.onClick.AddListener(() =>
        {
            checkMarkNull.gameObject.SetActive(true);
            checkMark.gameObject.SetActive(false);
            isCheck = false;
        });

        checkMarkNull.onClick.AddListener(() =>
        {
            checkMark.gameObject.SetActive(true);
            checkMarkNull.gameObject.SetActive(false);
            isCheck = true;
        });
    }

    private void UpdateData(UGCPoint ugcPoint)
    {
        this.ugcPoint = ugcPoint;
    }
}
