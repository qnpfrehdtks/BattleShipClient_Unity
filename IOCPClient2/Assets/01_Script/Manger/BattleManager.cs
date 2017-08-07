using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ATTKSHAPE
{
    ONE,
    CROSS,
    SQURE,
    ROW,
    COL,
    X
}



public class BattleManager : Singleton_Manager<BattleManager>
{
   
    private bool m_isCanAttack;
   private bool m_isGameStart;
    public PLAYER m_whoTurn;
    public SKILL m_SelectedSkill { get; set; }

    private BattleChest m_SelectedChest;
    private bool m_isChestSelected;

    protected override bool Init()
    {
        m_isGameStart = false;
        m_isCanAttack = true;
        return true;
    }


    public void GameStart(PLAYER turn)
    {
        m_isGameStart = true;


        m_whoTurn = turn;
        PlayerManager.Instance.SettingStartTurn(turn);
        UIPanel_Battle.instance.UI_TurnUpdate(turn);

        if(m_whoTurn == PLAYER.MINE)
        {
            m_isCanAttack = true;
        }
        else
        {
            m_isCanAttack = false;
            
        }
    }



    public void ClickSkill(Skill_Icon skill)
    {
        if (m_SelectedChest && m_isChestSelected)
        {
            skill.SkillOn();
        }
        else
        {
            UIPanel_Battle.instance.TileSelectCaution();
        }
    }


    // Skill 아이콘이 이것을 실행시키는 권한을 가짐.
    public void AttackBlock(int num, ATTKSHAPE shape, SKILL skill)
    {
        if (m_whoTurn == PLAYER.MINE &&
          m_isCanAttack &&
          m_isChestSelected)
        {
            m_SelectedSkill = skill;
            m_isCanAttack = false;

            switch(shape)
            {
                case ATTKSHAPE.ONE:
                    AttackONE();
                    break;
                case ATTKSHAPE.COL:
                    AttackCOL(m_SelectedChest, num);
                    break;
                case ATTKSHAPE.ROW:
                    AttackROW(m_SelectedChest, num);
                    break;
                case ATTKSHAPE.SQURE:
                   AttackSqure(m_SelectedChest, num);
                    break;
                case ATTKSHAPE.CROSS:
                    AttackCross(m_SelectedChest, num);
                    break;
            }

            switch (skill)
            {
                case SKILL.BASE:
                    NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_ATTACK);
                    break;
                case SKILL.SUPER_BOMB:
                    NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_SKILLATTACK);
                    break;
                case SKILL.RADER:
                    UIPanel_Battle.instance.SelectRaderPt(m_SelectedChest);
                    NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_SKILLATTACK);
                    break;
                case SKILL.REPAIR:
                    NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_SKILLATTACK);
                    break;
            }
        }  
     }

    public void EnemyAttackToBlocks(sVector2[] pos)
    {
        UIPanel_Battle.instance.DamagedFromEnemy(pos);
 
    }


    public bool EnemyAttackToBlock(sVector2 pos)
    {
         if(UIPanel_Battle.instance.DamagedFromEnemy(pos))
        {
            return true;
        }
         else
        {
            return false;
        }
    }

    public int EnemyRaderToBlock(sVector2[] pos)
    {
        return UIPanel_Battle.instance.RaderedFromEnemy(pos);

    }

    public void PlayerAttackToEnemyBlock(sVector2 pos, PACKETSTATE state)
    {
        TileSelectReset(UIPanel_Battle.instance.DamageToEnemy(pos, state));
    }

    public void PlayerAttackToEnemyBlocks(sVector2[] DamagedPos, List<sVector2> noDamList, SKILL skill)
    {
        for (int i = 0; i < DamagedPos.Length; i++)
        {
            UIPanel_Battle.instance.DamageToEnemy(DamagedPos[i], skill);
        }

        for (int i = 0; i < noDamList.Count; i++)
        {
            UIPanel_Battle.instance.noDamageToEnemy(noDamList[i], skill);
        }

        TileSelectReset(m_SelectedChest);
    }

    public void TurnChange(PACKETSTATE state)
    {
        if(state == PACKETSTATE.PK_MY_TURN)
        {
            m_isCanAttack = true;
            m_whoTurn = PLAYER.MINE;
            UIPanel_Battle.instance.UI_TurnUpdate(PLAYER.MINE);
            PlayerManager.Instance.SettingStartTurn(PLAYER.MINE);
        }
        else
        {
            m_isCanAttack = false;
            m_whoTurn = PLAYER.OPPONENT;
            UIPanel_Battle.instance.UI_TurnUpdate(PLAYER.OPPONENT);
            PlayerManager.Instance.SettingStartTurn(PLAYER.OPPONENT);
        }
    }



    public void SelectAttackPt(BattleChest block)
    {
        block.m_isSelected = true;
        m_isChestSelected = true;
        m_SelectedChest = block;

    }

    public void UnSelectedAttackPt(BattleChest block)
    {
        block.m_isSelected = false;
        m_isChestSelected = false;
        m_SelectedChest = null;

    }

    private void TileSelectReset(BattleChest block)
    {
        block.m_isSelected = false;
        m_isChestSelected = false;
        m_SelectedChest = null;
    }






    // 단일 공격
    //  
    //  ★   
    //
    private void AttackONE()
    {
        NetworkManager.Instance.m_AttackPt = m_SelectedChest.m_Pt;
    }

    // 가로로
    //  
    //  ■ ★ ■   
    //
    private void AttackROW(BattleChest center, int num)
    {
        sVector2 vec;
        vec.y = center.m_Pt.y;


        for (int i = -num ; i <= num; i++)
        {
            if ((center.m_Pt.x + i >= 0 && center.m_Pt.y + i >= 0) &&
                 (center.m_Pt.x + i <= 9 && center.m_Pt.y + i <= 9))
            {
                vec.x = center.m_Pt.x + i;
                NetworkManager.Instance.AddAttackPt(vec);
            }
        }
    }



    // 세로로
    //  ■ 
    //  ★   
    //  ■
    private void AttackCOL(BattleChest center, int num)
    {
        sVector2 vec;
        vec.x = center.m_Pt.x;


        for (int i = -num; i <= num; i++)
        {
            if ((center.m_Pt.x + i >= 0 && center.m_Pt.y + i >= 0) &&
                 (center.m_Pt.x + i <= 9 && center.m_Pt.y + i <= 9))
            {
                vec.y = center.m_Pt.y + i;
                NetworkManager.Instance.AddAttackPt(vec);
            }
        }
    }

    // 십자가으로 범위를 집어넣자.
    //    ■ 
    //  ■ ★ ■   
    //    ■
    private void AttackCross(BattleChest center, int num)
    {
        sVector2 vec;

        vec.y = center.m_Pt.y;

        for (int j = -num; j <= num; j++)
        {
            if (j == 0) continue;

            if ((center.m_Pt.x + j >= 0  && (center.m_Pt.x + j <= 9) ))
            {

                vec.x = center.m_Pt.x + j;
                NetworkManager.Instance.AddAttackPt(vec);
            }
        }


        vec.x = center.m_Pt.x;

        for (int i = -num; i <= num; i++)
        {
            if ((center.m_Pt.y + i >= 0 && (center.m_Pt.y + i <= 9)))
            {
                vec.y = center.m_Pt.y + i;
                NetworkManager.Instance.AddAttackPt(vec);
            }
        }
    }


    // 정사각형으로 범위를 집어넣자.
    //  ■ ■ ■
    //  ■ ★■   
    //  ■ ■ ■
    private void AttackSqure(BattleChest center, int num)
    {
        sVector2 vec;

        for (int i = -num / 2; i <= num / 2; i++)
        {
            for (int j = -num / 2; j <= num / 2; j++)
            {
                if ((center.m_Pt.x + j >= 0 && center.m_Pt.y + i >= 0) &&
                     (center.m_Pt.x + j <= 9 && center.m_Pt.y + i <= 9))
                {
                    vec.x = center.m_Pt.x + j; vec.y = center.m_Pt.y + i;
                    NetworkManager.Instance.AddAttackPt(vec);
                }
            }
        }
    }







}
