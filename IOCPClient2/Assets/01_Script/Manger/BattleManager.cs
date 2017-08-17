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



    public bool m_isDefendMode;
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
        m_isDefendMode = false;

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



    public bool IsCanClickSkill()
    {
        if ( m_isCanAttack && m_isChestSelected && m_SelectedChest)
        {
            return true;
        }
        else
        {
            UIPanel_Battle.instance.SelectCaution("Select a Block!!");
            return false;
        }
        
    }


   
    // Skill 아이콘이 이것을 실행시키는 권한을 가짐.
    public bool AttackBlock(int num, ATTKSHAPE shape, SKILL skill)
    {
        if (m_whoTurn == PLAYER.MINE &&
          m_isCanAttack )
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
                case ATTKSHAPE.X:
                    AttackX(m_SelectedChest, num);
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
                case SKILL.DEFEND:
                    m_isDefendMode = true;
                    NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_SKILLATTACK);
                    break;
            }

            return true;
        }

        return false;
     }

    public bool EnemyAttackToBlocks(sVector2[] pos, SKILL skill)
    {
        //방어 모드가 아닐때
        if (!m_isDefendMode)
        {
            UIPanel_Battle.instance.DamagedFromEnemy(pos, skill);
            UIPanel_Battle.instance.SkillResultInfo();
            NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_SKILLRESULT);
            
            m_isDefendMode = false;
            return true;
        }
        else // 방어 모드 일때
        {
            EnemyMissileDefend();
            NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_MISSILEDEFNED);
            m_isDefendMode = false;
            return false;
        }
 
    }

    public void RecvDefendSkill(sVector2[] pos, SKILL skill)
    {
        NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_SKILLRESULT);

        if(m_isDefendMode)
        {
            UIPanel_Battle.instance.DefendFailResult();
        }
        else
        {
            UIPanel_Battle.instance.SkillResultInfo();
        }

        m_isDefendMode = false;

    }

    public PACKETSTATE EnemyAttackToBlock(sVector2 pos)
    {
        if (!m_isDefendMode)
        {
            m_isDefendMode = false;
            if (UIPanel_Battle.instance.DamagedFromEnemy(pos))
            {
                return PACKETSTATE.PK_ENEMY_ATTACKSUCC;

            }
            else
            {
                return PACKETSTATE.PK_ENEMY_ATTACKFAIL;
            }
        }
        else
        {
            m_isDefendMode = false;
            UIPanel_Battle.instance.DefendResult();
            return PACKETSTATE.PK_PLAYER_MISSILEDEFNED;
        }
    }

    public int EnemyRaderToBlock(sVector2[] pos)
    {
        return UIPanel_Battle.instance.RaderedFromEnemy(pos);

    }

    public void RaderResult(int count)
    {
        UIPanel_Battle.instance.RaderResult(count);
        m_isDefendMode = false;

    }

    public void EnemyMissileDefend()
    {
        UIPanel_Battle.instance.DefendResult();
        
    }


    public void PlayerAttackToEnemyBlock(sVector2 pos, PACKETSTATE state)
    {
        UIPanel_Battle.instance.DamageToEnemy(pos, state);
    }

    public void PlayerAttackToEnemyBlocks(sVector2[] DamagedPos, List<sVector2> noDamList, SKILL skill)
    {
        SoundManager.Instance.playSoundOnseShot("BOMB");
    //    StartCoroutine(UIPanel_Battle.instance.SuperBombEffect(m_SelectedChest.transform.position));

        for (int i = 0; i < noDamList.Count; i++)
        {
            UIPanel_Battle.instance.noDamageToEnemy(noDamList[i], skill);
        }
        for (int i = 0; i < DamagedPos.Length; i++)
        {
            UIPanel_Battle.instance.DamageToEnemy(DamagedPos[i], skill);
        }

      //  TileSelectReset(m_SelectedChest);
    }

    public void TurnChange(PACKETSTATE state)
    {

        if (state == PACKETSTATE.PK_MY_TURN)
        {
           // SoundManager.Instance.playSoundOnseShot("TurnChange");
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

        TileSelectReset(m_SelectedChest);
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

        if (block != null)
        {
            block.m_isSelected = false;
            m_isChestSelected = false;
            m_SelectedChest = null;


            UIPanel_Battle.instance.UnSelectedAttackPt();
        }
    }






    // 단일 공격
    //  
    //  ★   
    //
    private void AttackONE()
    {
        if(m_SelectedChest)
        NetworkManager.Instance.m_AttackPt = m_SelectedChest.m_Pt;
        else
        {
            sVector2 temp;
            temp.x = 0; temp.y = 0;
            NetworkManager.Instance.m_AttackPt = temp;
        }
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


    // X로 범위를 집어넣자.
    //  ■    ■
    //    ★   
    //  ■    ■
    private void AttackX(BattleChest center, int num)
    {
        sVector2 vec;

        for (int i = -num; i <= num; i++)
        {
           
            //	printf("%d 번째 행의 차례\n", i);

            for (int j = -Math.Abs(i); j <= Math.Abs(i); j += 2 * Math.Abs(i))
            {
                if ((center.m_Pt.x + j >= 0 && (center.m_Pt.x + j <= 9))
                    && (center.m_Pt.y + i >= 0 && (center.m_Pt.y + i <= 9))
                    )
                {
                    vec.x = center.m_Pt.x + j;
                    vec.y = center.m_Pt.y + i;

                    //printf("%d,%d\n", vec.x, vec.y);
                    //	Debug.Log(vec.x + "," + vec.y);
                    NetworkManager.Instance.AddAttackPt(vec);

                    if(j == 0)
                    {
                        break;
                    }


                }
            }
        }
    }

    // 억지로 승리 시키자!!
    public void ForcedVictory()
    {
        m_isCanAttack = false;

        UIPanel_Battle.instance.BattleResult(PLAYER.MINE);
        PlayerManager.Instance.AllClearPlayerInfo();
        NetworkManager.Instance.AllClearInfo();

        StartCoroutine(GotoMain());
    }


    // 전투 결과 처리할 함수.
    public void BattleResult()
    {
     //   TileSelectReset(m_SelectedChest);
        m_isCanAttack = false;
        UIPanel_Battle.instance.BattleResult(m_whoTurn);
        PlayerManager.Instance.AllClearPlayerInfo();
        NetworkManager.Instance.AllClearInfo();

        StartCoroutine(GotoMain());
    }


    IEnumerator GotoMain()
    {
        yield return new WaitForSeconds(5.0f);

        gameSceneManager.Instance.SceneChange(SCENE.SC_MAIN);
    }


  














}
