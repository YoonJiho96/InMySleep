using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIPopupLogin : MonoBehaviour
{
    public Button btnClose;
    public TMP_InputField inputId;
    public TMP_InputField inputPassword;
    public Button btnLogin;
    public Button btnSignup;
    public Button btnForgotPw;
    public Toggle toggleRemember;

    public System.Action<string, string> onClickLogin;

    private void Awake()
    {
        // ���̵� ���� ���
        this.toggleRemember.onValueChanged.AddListener((val) =>
        {
            Debug.LogFormat("isOn: {0}", val);
        });
        // �α��� ��ư ��� �߰�
        this.btnLogin.onClick.AddListener(() =>
        {
            // ID, Password �Է� �޾� ���
            string id = this.inputId.text;
            string password = this.inputPassword.text;
            if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(password))
            {
                Debug.LogFormat("<color=cyan>ID�� Password ��� ä�����մϴ�.</color>");
            }
            else
            {
                Debug.LogFormat("ID: {0}", id);
                Debug.LogFormat("Password: {0}", password);
                this.onClickLogin(id, password);
            }
            // ID, Password ���� Ʃ�÷� ����
        });
    }

    public void Init()
    {

    }

    // �α��� �˾� ����
    public void Open()
    {
        this.gameObject.SetActive(true);
    }
    // �α��� �˾� �ݱ�
    public void Close()
    {
        this.gameObject.SetActive(false);
    }


}
