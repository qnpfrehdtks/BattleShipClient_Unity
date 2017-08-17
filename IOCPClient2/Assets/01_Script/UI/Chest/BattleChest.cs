using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CHEST_STATE
{
   ORIGIN, GREEN, RED, YELLOW, BLUE, TRANS

}


public class BattleChest : Chest {

    public bool m_isSelected { get; set; }
 
   
    // Use this for initialization
    void Awake()
    {
      //  m_isSelected = false;
        init();
    }

    public override void init()
    {
        m_isSelected = false;
        m_isCanInstalled = true;
        m_CurScene = gameSceneManager.Instance.m_currentScene;
        m_Renderer = GetComponent<SpriteRenderer>();
        m_OriginColor = m_Renderer.color;

    }


    void OnMouseUp()
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            SoundManager.Instance.playSoundOnseShot("ClickBlock");


            if (!m_isSelected)
            {
                BattleManager.Instance.SelectAttackPt(this);
                UIPanel_Battle.instance.SelectAttackPt(this);
            }
            else if (m_isSelected)
            {
                BattleManager.Instance.UnSelectedAttackPt(this);
                UIPanel_Battle.instance.UnSelectedAttackPt();
            }
        }
    //   BattleManager.Instance.baseAttackBlock(this);
         //   ChangeState(CHEST_STATE.RED);

    }

    void OnMouseEnter()
    {
        if (m_ChestState == CHEST_STATE.ORIGIN
            && m_ChestState != CHEST_STATE.RED
            && m_ChestState != CHEST_STATE.BLUE)
            ChangeState(CHEST_STATE.YELLOW);
    }
    void OnMouseExit()
    {
        if (m_ChestState == CHEST_STATE.YELLOW && m_ChestState != CHEST_STATE.RED
            && m_ChestState != CHEST_STATE.BLUE)
            ChangeState(CHEST_STATE.ORIGIN);
    }



}
