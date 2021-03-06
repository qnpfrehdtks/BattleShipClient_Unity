﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SHIP
{
    SUBMARINE,
    PT_BOAT,
    DESTROYER,
    CARRIER,
    WAR_SHIP
}


public enum SELECT_BUTTON_STATE
{
    IDLE,
    INSTALLED,
    SELECTED,    
}



public class ShipSelectButton : MonoBehaviour {


    public Image m_Image;
    public GameObject m_Go;
//    public GameObject m_Info;

    private bool m_isDrag;
    public Base_Ship m_ShipObject { get; set; }
    public SELECT_BUTTON_STATE m_State { get; set; }

    public Text m_ShipName;
    public Text m_StateInfo;

    void Start()
    {
        UIPanel_Ready.instance.addButtonList(this);
        m_ShipObject = m_Go.GetComponent<Base_Ship>();
        m_ShipName.text = m_ShipObject.m_ShipName;

        UnselectButton();
      
    }


    public void InstalledButton()
    {
        m_isDrag = false;
        m_State = SELECT_BUTTON_STATE.INSTALLED;
        m_StateInfo.text = "DISPATCHED";
        m_StateInfo.color = Color.yellow;

     //   m_Info.SetActive(false);
    }


    public void UnselectButton()
    {
        m_ShipObject.setInstallMode(false);

        m_isDrag = false;
        m_State = SELECT_BUTTON_STATE.IDLE;
        m_StateInfo.text = "SELECT";
        m_StateInfo.color = Color.green;
        
     //   m_Info.SetActive(false);
    }

    void SelectButton()
    {
      
        m_ShipObject.setInstallMode(true);

        m_isDrag = true;
        m_State = SELECT_BUTTON_STATE.SELECTED;

        m_StateInfo.text = "SELECTED";
        m_StateInfo.color = Color.blue;
        
     //   m_Info.SetActive(true);
    }


    public void ClickButton()
    {
        if (m_State == SELECT_BUTTON_STATE.IDLE || m_State == SELECT_BUTTON_STATE.INSTALLED)
        {
            SoundManager.Instance.playSoundOnseShot("OK");
            // 버튼에서 배를 불러오자.
            UIPanel_Ready.instance.ShipCall(this,m_Go);

            iTween.ScaleFrom(m_StateInfo.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 1.0f);
            SelectButton();
        }
        else if(m_State == SELECT_BUTTON_STATE.SELECTED)
        {
            SoundManager.Instance.playSoundOnseShot("FAIL");
            //버튼에서 배를 해제하자.
            UIPanel_Ready.instance.ShipCall(this, null);

            iTween.ScaleFrom(m_StateInfo.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 1.0f);
            UnselectButton();
        }
    }

}
