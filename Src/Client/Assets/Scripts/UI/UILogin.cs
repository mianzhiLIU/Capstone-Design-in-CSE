using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Services;
using SkillBridge.Message;

public class UILogin : MonoBehaviour {

    // 오픈 컴포넌트
    public InputField Username;
    public InputField Password;
    public Button ButtonLogin;
    public Button ButtonRegister;

    // Use this for initializations
    void Start () {
        UserService.Instance.OnLogin = this.OnLogin;  // 로그인 감청 사건
    }


    // Update is called once per frame
    void Update () {
		
	}

    // 로그인 버튼 방법
    public void OnClickLogin()
    {
        if (string.IsNullOrEmpty(this.Username.text))
        {
            MessageBox.Show("아이디를 입력하세요！");
            return;
        }
        if (string.IsNullOrEmpty(this.Password.text))
        {
            MessageBox.Show("비밀 번호를 입력하세요！");
            return;
        }

        // Enter Game
        UserService.Instance.SendLogin(this.Username.text, this.Password.text);
    }

    // 로그인 방법
    void OnLogin(Result result, string message)
    {
        // 로그인 성공
        if (result == Result.Success)
        {
            // 로그인 성공, 캐릭터 선택 화명 접속
            SceneManager.Instance.LoadScene("CharacterSelect");
        }
        else  // 로그인 실패, 메시지 알림
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }
}
