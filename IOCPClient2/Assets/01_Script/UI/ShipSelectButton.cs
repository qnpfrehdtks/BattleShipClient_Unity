using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SHIP
{
    SUBMARINE,
    TRANSPORT_SHIP,
    SCOUT_SHIP,
    CARRIER,
    BATTLE_SHIP
}


public enum SELECT_BUTTON_STATE
{
    IDLE,
    INSTALLED,
    SELECTED,    
}



public class ShipSelectButton : MonoBehaviour {

    public GameObject m_Go;

    private bool m_isDrag;
    public Base_Ship m_ShipObject { get; set; }
    public SELECT_BUTTON_STATE m_State { get; set; }


    void Start()
    {
        m_isDrag = false;
        m_State = SELECT_BUTTON_STATE.IDLE;
        m_ShipObject = m_Go.GetComponent<Base_Ship>();

    }

    void OnMouseDrag()
    {
        
        m_State = SELECT_BUTTON_STATE.SELECTED;
    }

    void OnMouseDown()
    {
        Debug.Log("IDLE");
        m_State = SELECT_BUTTON_STATE.IDLE;
    }
}
