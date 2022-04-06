using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 
using Services; 
using SkillBridge.Message;


public class UIRegister : MonoBehaviour
{
    // 오픈 컴포넌트
    public InputField UserName;
    public InputField Password;
    public InputField PasswordConfirm;
    public Button ButtonRegister;

    public GameObject UILogin;

    // Start is called before the first frame update
    void Start()
    {
        // Register 사건 감청
        UserService.Instance.OnRegister = this.OnRegister;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // Register Button Method
    public void OnClickRegister()
    {
        if (string.IsNullOrEmpty(this.UserName.text))
        {
            MessageBox.Show("아이디를 입력하세요!");
            return;
        }

        if (string.IsNullOrEmpty(this.Password.text))
        {
            MessageBox.Show("비밀 번호를 입력하세요!");
            return;
        }

        if (string.IsNullOrEmpty(this.PasswordConfirm.text))
        {
            MessageBox.Show("비밀 확인을 위해 다시 입력하세요!");
            return;
        }

        if (this.Password.text != this.PasswordConfirm.text)
        {
            MessageBox.Show("두 번 입력된 비밀 번호가 다르다!");
            return;
        }

        // UserService 로직층을 동해 Rigister 정보를 Server에 전송
        UserService.Instance.SendRegister(this.UserName.text, this.Password.text); 
    }

    // Register Method
    void OnRegister(Result result, string message)
    {
        if (result == Result.Success)
        {
            // 성공 메시지 알리하고, 취소 번튼 방법을 호출
            MessageBox.Show("등록이 완료되었으니 로그인하세요.", "알림", MessageBoxType.Information).OnYes = this.CloseRegister;
        }
        else // 실패
            MessageBox.Show(message, "에러", MessageBoxType.Error);
    }

    // Cancel Button Method
    void CloseRegister()
    {
        // close current UIRegister
        this.gameObject.SetActive(false);
        // open UILogin
        UILogin.SetActive(true);
    }
}
