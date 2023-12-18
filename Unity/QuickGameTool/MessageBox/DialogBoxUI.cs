using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
#region
//保持UTF-8
#endregion
public class DialogBoxUI : MonoBehaviour
{
    [SerializeField]
    TMP_Text titleText;
    [SerializeField]
    TMP_Text contentText;
    [SerializeField]
    TMP_Text confirmText;
    [SerializeField]
    Button confirmBtn;

    public string titleTextName="TitleText_N";
    public string contentTextName="ContentText_N";
    public string confirmTextName="ConfirmText_N";
    public string confirmBtnName="ConfirmBtn_N";

    public TMP_Text TitleText { get => titleText; }
    public TMP_Text ContentText { get => contentText; }
    public TMP_Text ConfirmText { get => confirmText;  }
    public Button ConfirmBtn { get => confirmBtn;  }


    // UISubManager subMgr = null;
    private void Awake()
    {
        if (titleText == null | confirmText == null | contentText == null | confirmBtn == null)
        {
            var subMgr = gameObject.AddComponent<UISubManager>();
            titleText=subMgr.GetUIBehavior(titleTextName).GetComponent<TMP_Text>();
            confirmText=subMgr.GetUIBehavior(confirmTextName).GetComponent< TMP_Text>();
            contentText=subMgr.GetUIBehavior(contentTextName).GetComponent<TMP_Text>();
            confirmBtn = subMgr.GetUIBehavior(confirmBtnName).GetComponent<Button>();
        }
    }

    public void Open(string title,string content,string confirm,Action action)
    {
        titleText.text=title;
        contentText.text=content;
        if (string.IsNullOrEmpty(confirm))
            confirm = "Confirm";
        confirmText.text=confirm;
        confirmBtn.onClick.RemoveAllListeners();
        confirmBtn.onClick.AddListener(Close);
        if(action!=null)
            confirmBtn.onClick.AddListener(()=>action());
        transform.SetParent(UIManager.Instance.CurrSceneMainCanvas.transform);
        transform.SetAsLastSibling();
        gameObject.SetActive(true);

    }

    internal void Close()
    {
        confirmBtn.onClick.RemoveAllListeners();
        transform.SetParent(MessageBox.Instance.BannerPanel);
        transform.SetAsFirstSibling();
        gameObject.SetActive(false);
    }
}
