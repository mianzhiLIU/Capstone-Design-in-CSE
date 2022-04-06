using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {

    // 캐릭터를 관리하기 GameObject Array
    public GameObject[] characters;


    // 현재 캐릭터 Index
    private int currentCharacter = 0;

    public int CurrectCharacter
    {
        // 현재 캐릭터 Index 받음
        get
        {
            return currentCharacter;
        }
        // 현재 캐릭터 Index 설치
        set
        {
            currentCharacter = value;   // Unity 전입된 개릭터 Index를 현재 캐릭터에 주기 
            this.UpdateCharacter();     // UpdateCharacter()방법 호출
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void UpdateCharacter()
    {
        // 현재 선택된 캐릭터 활성화
        for (int i=0;i<3;i++)
        {
            characters[i].SetActive(i == this.currentCharacter);
        }
    }
}
