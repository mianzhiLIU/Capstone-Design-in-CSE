using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Models;
using Services;
using SkillBridge.Message;
public class UICharacterSelect : MonoBehaviour {

    // 오픈 컴포넌트
    public GameObject panelCreate;
    public GameObject panelSelect;

    public GameObject btnCreateCancel;

    public InputField charName;
    CharacterClass charClass;

    public Transform uiCharList;
    public GameObject uiCharInfo;

    public List<GameObject> uiChars = new List<GameObject>();

    public Image[] titles;  // 캐릭터 제목 （ create panel ） 
    public Text descs;      // 개릭터 설명 （ create panel ）

    private int selectCharacterIdx = -1;

    public UICharacterView characterView;  // Character View 객체 

    // Use this for initialization
    void Start()
    {
        DataManager.Instance.Load();
        InitCharacterSelect(true);   
        UserService.Instance.OnCharacterCreate = this.OnCharacterCreate; // OnCharacterCreate 이벤트 감청
    }


    public void InitCharacterCreate()
    {
        panelCreate.SetActive(true);
        panelSelect.SetActive(false);
        OnSelectClass(1);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    // 캐릭터 버튼 이벤트
    public void OnClickCreate()
    {
        // 이름을 입력할 지 확인
        if (string.IsNullOrEmpty(this.charName.text))
        {
            MessageBox.Show("게임 이름을 입력하세요!");
            return;
        }

        // UserService를 통해  생성된 캐릭터 정보를 Server에 전송합니다.
        UserService.Instance.SendCharacterCreate(this.charName.text, this.charClass);
    }

    /// <summary>
    /// 직업 선택
    /// </summary>
    /// <param name="charClass"></param>
    public void OnSelectClass(int charClass)
    {
        this.charClass = (CharacterClass)charClass;  // 들어오는 캐릭터 직업 index 받음

        characterView.CurrectCharacter = charClass - 1;   // character view의 캐릭터 모델 업데이트

        for (int i = 0; i < 3; i++)
        {
            titles[i].gameObject.SetActive(i == charClass - 1);  // 캐릭터 제목 업데이트
        }

        descs.text = DataManager.Instance.Characters[charClass].Description; // 개릭터 Description 업데이트
    }

    // 캐릭터 만들 버튼 방법
    void OnCharacterCreate(Result result, string message)
    {
        //  생성 성공, 캐릭터 선택 초기화
        if (result == Result.Success)
        {
            InitCharacterSelect(true);
        }
        else // 생성 실패, 오류 정보 제시
            MessageBox.Show(message, "错误", MessageBoxType.Error);
    }

    // 캐릭터 선택 화면 초기화
    public void InitCharacterSelect(bool init)
    {
        // 성공
        panelCreate.SetActive(false);  // 캐릭터 만든 화면 닫기
        panelSelect.SetActive(true);   // 캐릭터 선택 화면 열기

        // 이 화면 초기회이면
        // 원래 있던 캐릭터 정보를 지우고
        // 로그이된 사용자의 캐릭터 정보를 불어오기
        if (init)
        {
            // 원래 있던 캐릭터 정보 지우기
            foreach (var old in uiChars)
            {
                Destroy(old);
            }
            uiChars.Clear();

            // 그로인 성고,  존재한 캐릭터 정보를 캐릭터 선택 박스에 물어오기
            for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
            {
                GameObject go = Instantiate(uiCharInfo, this.uiCharList);
                UICharInfo chrInfo = go.GetComponent<UICharInfo>();
                chrInfo.info = User.Instance.Info.Player.Characters[i];

                Button button = go.GetComponent<Button>();
                int idx = i;
                button.onClick.AddListener(() =>
                {
                    OnSelectCharacter(idx);
                });

                uiChars.Add(go);
                go.SetActive(true);
            }
        }
    }

    // 개릭터 선택 버튼 방법
    public void OnSelectCharacter(int idx)
    {
        // 선택된 캐릭터 Index 설치
        this.selectCharacterIdx = idx;
        var cha = User.Instance.Info.Player.Characters[idx];
        Debug.LogFormat("Select Char:[{0}]{1}[{2}]", cha.Id, cha.Name, cha.Class);
        User.Instance.CurrentCharacter = cha;
        characterView.CurrectCharacter = idx;


        // 캐릭터 선택 항목을 클릭하면 HighLight 효과로 나올 수 있고,
        // 다른 항목을 클릭하면 전 HighLight 효과가 취소되고,
        // 새로 클릭한 항목을 HightLigh 효과가 나오기
        for (int i = 0; i < User.Instance.Info.Player.Characters.Count; i++)
        {
            UICharInfo ci = this.uiChars[i].GetComponent<UICharInfo>();
            ci.Selected = idx == i;
        }
    }

    // 게임 접소
    public void OnClickPlay()
    {
        // 접속 성공
        if (selectCharacterIdx >= 0)
        {
            MessageBox.Show("게임 시작", "게임 시작", MessageBoxType.Confirm);
        }
    }
}
