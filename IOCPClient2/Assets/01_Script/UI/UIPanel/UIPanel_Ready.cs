﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Ready : SingletonUIPanel<UIPanel_Ready>
{

    public bool m_isWait;
   public bool m_isSelected { get; set; }

    public GameObject m_WaitPanel;
    public GameObject m_ToBattleButton;

    // 설치된 칸들의 기준 축이 될 오브젝ㅌ,
    public GameObject m_ChestAxis;
    // 우리가 설치할 칸 오브젝트
    public GameObject m_ChestObject;

    // 오른쪽 설치 시 도움을 줄 인터페이스 UI
    public InstallIconButton m_InstallButtons;

    // 칸 리스트 담음.
    public List<GameObject> m_ChestList;

    // 왼쪽 배 건설 버튼 모은 리스트
    public List<ShipSelectButton> m_ButtonList;

   //// 설치된 배의 리스트, 이 리스트를 전투 씬에서 넘겨받아서 전투해야한다~
   // public Dictionary<SHIP,Base_Ship> m_InstalledShipMap;

    // 현재 선택한 배를 담은 오브젝트 변수
    private GameObject m_TempShip;
    // 현재 선택한 버튼의 오브젝트를 담을 변수.
    private ShipSelectButton m_TempSelectButton;

	// Use this for initialization
	void Start () {
        Screen.SetResolution(1280, 720, false);
        m_isSelected = false;
        m_isWait = false;

        //  m_ShipList = new List<Base_Ship>();
        m_ChestList = new List<GameObject>();
    //    m_InstalledShipMap = new Dictionary<SHIP, Base_Ship>();

        for (int i=0; i < 10; i++)
        {
            for(int j=0; j < 10; j++)
            {
                GameObject chest = Instantiate(m_ChestObject);
                chest.transform.parent = m_ChestAxis.transform;
                chest.transform.localPosition = new Vector3(-0.9f * j, 0.1f, 0.9f * i);

                ReadyChest ready = chest.AddComponent<ReadyChest>();
                ready.m_Pt.x = j;
                ready.m_Pt.y = i;

                m_ChestList.Add(chest);
            }
        }
	}


    public void addButtonList(ShipSelectButton boat)
    {
        if (boat != null)
        {
            m_ButtonList.Add(boat);
        }
        else
        {
            Debug.Log("Fail to insert");
        }
    }

    // 배를 버튼에서 불러올 때 사용하는 함수 // 둘다 널값 넣으면 아무것도 설치하지 않은 상태가 됨.
    public void ShipCall(ShipSelectButton button, GameObject ship)
    {
        if (!m_isWait)
        {

            // 만약 선택된 버튼이 있다면 안 선택된 버튼으로 바꾸고
            for (int i = 0; i < m_ButtonList.Count; i++)
            {
                if (m_ButtonList[i].m_State != SELECT_BUTTON_STATE.INSTALLED)
                    m_ButtonList[i].UnselectButton();
            }

            // 배를 설치할 경우
            if (ship != null)
            {
                Base_Ship shipinfo = ship.GetComponent<Base_Ship>();

                if (!shipinfo)
                {
                    Debug.Log("So Strange... No Ship Info..?");
                    return;
                }


                PlayerManager.Instance.DeleteShipFromPlayer(shipinfo, true);
                //// 만약 설치된 배 목록 이었다면 설치된 배 목록에서 우선 지워야 함.
                //if(m_InstalledShipMap.ContainsKey(shipinfo.m_shipKind))
                //{
                //    m_InstalledShipMap.Remove(shipinfo.m_shipKind);
                //}

                m_isSelected = true;
                m_TempShip = ship;
                m_TempSelectButton = button;

                // 설치 버튼 설정
                m_InstallButtons.SetShip(m_TempShip, shipinfo);
            }
            else // 배를 설치하지 않을 경우 
            {
                m_isSelected = false;
                m_TempShip = null;
                m_TempSelectButton = null;

                //설치 버튼 설정
                m_InstallButtons.SetShip(null, null);
            }
        }
    }




    // 칸에 설치된에서 실행하는 함수임.
    // 배를 설치할 때 사용하는 함수.
    public void installShip(int X, int Y)
    {
        if(m_isSelected)
        {
            Base_Ship ship =  m_TempShip.GetComponent<Base_Ship>();
            if(ship.checkCanInstall())
            {
                // 셀렉트 끝.
                m_isSelected = false;
                ship.InstallSetting(X, Y);      

                // 버튼을 설치 됫다고 알리자.
                m_TempSelectButton.InstalledButton();

                // 만약 딕셔너리에 추가되지 않았다면 배를 설치햇다고 추가시키자~
                PlayerManager.Instance.AddShipToPlayer(ship, true);

                m_TempShip = null;
                m_TempSelectButton = null;

                // 설치 했으니 오른쩍 UI 안보이게
                m_InstallButtons.SetShip(m_TempShip, null);
            }
            else
            {

            }

        }
    }

    public void ClickGameStartButton()
    {
        Debug.Log(PlayerManager.Instance.CheckDispatchedShipCount(true));
        ShipCall(null, null);

        if (PlayerManager.Instance.CheckDispatchedShipCount(true) >= 5)
        {
            SoundManager.Instance.playSoundOnseShot("OK");
            m_isWait = true;
            m_WaitPanel.SetActive(true);
            NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_READY);
           
        }
        else
        {
            SoundManager.Instance.playSoundOnseShot("FAIL");
            m_ToBattleButton.SetActive(true);
        }
    }


    public void ClickBackToReady()
    {
        m_ToBattleButton.SetActive(false);
    }
}
