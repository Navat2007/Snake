using System;
using UnityEngine;
using UnityEngine.UI;

public class PopupQuestion : MonoBehaviour
{
    [SerializeField] private TMPro.TMP_Text titleText;
    [SerializeField] private TMPro.TMP_Text btnCancelText;
    [SerializeField] private TMPro.TMP_Text btnOkText;
    [SerializeField] private Button cancelBtn;
    [SerializeField] private Button okBtn;
    [SerializeField] private Color okBtnColor;
    [SerializeField] private Color cancelBtnColor;
    
    public void Init(string title, string okText, string cancelText, Action okCallback, Action cancelCallback)
    {
        titleText.text = title;
        btnCancelText.text = cancelText;
        btnOkText.text = okText;
        
        okBtn.onClick.RemoveAllListeners();
        cancelBtn.onClick.RemoveAllListeners();
        
        okBtn.onClick.AddListener(() =>
        {
            okCallback?.Invoke();
            gameObject.SetActive(false);
        });
        
        cancelBtn.onClick.AddListener(() =>
        {
            cancelCallback?.Invoke();
            gameObject.SetActive(false);
        });
    }
}