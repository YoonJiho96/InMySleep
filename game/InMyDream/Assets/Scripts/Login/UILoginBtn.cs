using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILoginBtn : MonoBehaviour
{
    public Button btn;
    public UIPopupLogin uiPopupLogin;
    
    public void Init()
    {
        // �α��� �˾����� x Ŭ�� �� �ݱ�
        this.uiPopupLogin.btnClose.onClick.AddListener(() =>
        {
            this.uiPopupLogin.Close();
        });
        
        // �α��� ��ư Ŭ���� �α��� �˾� ����
        this.btn.onClick.AddListener(() =>
        {
            this.uiPopupLogin.Open();
        });
    }
}
