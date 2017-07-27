using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHEST_STATE
{
   ORIGIN, GREEN, RED, YELLOW

}


public class BattleChest : Chest {

 
    // Use this for initialization
    void Awake()
    {
        init();
    }

   
    void OnMouseUp()
    {
        Debug.Log(m_X + "와 " + m_Y + " 를 클릭");


    }



    void OnMouseEnter()
    {
        if (m_ChestState == CHEST_STATE.ORIGIN)
            ChangeState(CHEST_STATE.YELLOW);
    }
    void OnMouseExit()
    {
        if(m_ChestState == CHEST_STATE.YELLOW)
        ChangeState(CHEST_STATE.ORIGIN);
    }



}
