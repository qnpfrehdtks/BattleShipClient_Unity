using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Battle : SingletonUIPanel<UIPanel_Battle>
{
 //   private EffectControll m_EffectController;


    public BattleInfo m_ComboInfo;
    public BattleInfo m_BattleInfo;
    public BattleInfo m_TurnInfo;
    public BattleInfo m_BattleResultInfo;

    public List<RaderRotate> m_RaderResultList;
    public List<GameObject> m_RaderTXTList;

    
    public GameObject m_TargetPositionImage;
    public GameObject m_RaderTargetPositionImage;
    public Text m_RaderTxt;
    public Text m_BattleTXT;

    private int ComboGage;

    public Color m_InfoColor;
    public Vector3 m_OriginCameraPos;
    public Battle_CameraMove m_CameraMove;

    //// Particle Fire;
    private GameObject m_FireEffect;
    private ParticleSystem m_FireParticle;


    // 설치된 칸들의 기준 축이 될 플레이어 측 오브젝ㅌ,
    public GameObject m_ChestAxis;

    // 설치된 칸들의 기준 축이 될 적 측 오브젝ㅌ,
    public GameObject m_EnemyChestAxis;

    public GameObject m_EnemyCameraPos;
    public GameObject m_PlayerCameraPos;

    // 우리가 설치할 칸 오브젝트
    public GameObject m_ChestObject;

    public GameObject m_PlayerCaptain;


    // 칸 리스트 담음.
    public List<BattleChest> m_EnemyChestList;
    public List<BattleChest> m_ChestList;

    // Use this for initialization
    void Start () {

        m_InfoColor = new Color(1, 0.8f, 0);
        m_Canvas =  GameObject.Find("Canvas");

        ComboGage = 0;

        m_FireEffect = Resources.Load<GameObject>("01Model/Fire");

        m_RaderTXTList = new List<GameObject>();
        m_RaderResultList = new List<RaderRotate>();
        m_ChestList = new List<BattleChest>();
        m_EnemyChestList = new List<BattleChest>();


        //    m_InstalledShipMap = new Dictionary<SHIP, Base_Ship>();

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject chest = Instantiate(m_ChestObject);
                chest.transform.parent = m_ChestAxis.transform;
                chest.transform.localPosition = new Vector3(-0.9f * j, 0.1f, 0.9f * i);

                BattleChest battle = chest.AddComponent<BattleChest>();
                battle.m_Pt.x = j;
                battle.m_Pt.y = i;

                battle.init();

                m_ChestList.Add(battle);
            }
        }


        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject chest = Instantiate(m_ChestObject);
                chest.transform.parent = m_EnemyChestAxis.transform;
                chest.transform.localPosition = new Vector3(-0.9f * j, 0.1f, 0.9f * i);

                BattleChest battle = chest.AddComponent<BattleChest>();
                battle.m_Pt.x = j;
                battle.m_Pt.y = i;

                battle.init();


                m_EnemyChestList.Add(battle);
            }
        }

        PlayerManager.Instance.CheckShipPos();

    }


    public void UI_TurnUpdate(PLAYER state)
    {
        m_CameraMove = Camera.main.gameObject.GetComponent<Battle_CameraMove>();
       SoundManager.Instance.playSoundOnseShot("UseSkill");
        

        if (state == PLAYER.MINE)
        {
        
            UI_ourTurnUpdate();
        }
        else
        {
         
            UI_oppoTurnUpdate();
        }
    }


    private void UI_ourTurnUpdate( )
    {
        m_TurnInfo.SetText("PLAYER TURN", UI_ANIMATION.DOWNNER,new Color(0.35f,0,1.0f));
        m_CameraMove.setMoveLerpToPos(m_EnemyCameraPos.transform.position, 5.0f);
        SetActiveAllRaderTXT(true);
    }

    private void UI_oppoTurnUpdate()
    {
        m_TurnInfo.SetText("ENEMY TURN", UI_ANIMATION.DOWNNER, Color.red);
        m_CameraMove.setMoveLerpToPos(m_PlayerCameraPos.transform.position, 5.0f);
        SetActiveAllRaderTXT(false);
    }


    public void DamagedFromEnemy(sVector2[] pos, SKILL skill)
    {
       if(skill == SKILL.SUPER_BOMB)
        {
            SoundManager.Instance.playSoundOnseShot("BOMB");
        }

        for (int i = 0; i < pos.Length; i++)
        {
            BattleChest temp = m_ChestList[pos[i].x + pos[i].y * 10];
            Debug.Log(pos[i].x + "," + pos[i].y);

            // 적이 명중을 못한 경우
            if (temp.m_isCanInstalled)
            {
                TileUpdate(temp, EFFECT.WATER, CHEST_STATE.BLUE);
            }
            // 명중한 경우
            else if (!temp.m_isCanInstalled)
            {
                TileUpdate(temp, EFFECT.SUPER_BOMB, CHEST_STATE.RED);
                DamagedResult(temp);
                NetworkManager.Instance.AddAttackPt(pos[i]);
            }

        }
    }

    public bool DamagedFromEnemy(sVector2 pos)
    {
        BattleChest temp = m_ChestList[pos.x + pos.y * 10];
        SoundManager.Instance.playSoundOnseShot("BOMB");

        // 적이 명중을 못한 경우
        if (temp.m_isCanInstalled)
        {
            TileUpdate(temp, EFFECT.WATER, CHEST_STATE.BLUE);
            AttackFailText();

            return false;
        }
        // 명중한 경우
        else if (!temp.m_isCanInstalled)
        {
            TileUpdate(temp, EFFECT.BOMB, CHEST_STATE.RED);
            AttackSuccessText();
            DamagedResult(temp);

            return true;
        }

        return false;
    }


    public int RaderedFromEnemy(sVector2[] pos)
    {
       return RaderResult(PlayerManager.Instance.getCurPlayer().ShipRaderCount(pos));
    }

    public BattleChest DamageToEnemy(sVector2 pos, PACKETSTATE state)
    {
        BattleChest temp = m_EnemyChestList[pos.x + pos.y * 10];
        SoundManager.Instance.playSoundOnseShot("BOMB");

        // 명중한 경우
        if (state == PACKETSTATE.PK_PLAYER_DAMAGESUCC)
        {
            TileUpdate(temp, EFFECT.BOMB, CHEST_STATE.RED);
           // DamagedResult(temp);
            AttackSuccessText();
        }  // 명중 못한 경우
        else if (state == PACKETSTATE.PK_PLAYER_DAMAGEFAIL)
        {
            TileUpdate(temp, EFFECT.WATER, CHEST_STATE.BLUE);
            AttackFailText();
        }

        return temp;
    }


    public void TileUpdate(sVector2 pos , EFFECT effect, CHEST_STATE state,PLAYER whoTile)
    {
        BattleChest temp;
        if (whoTile == PLAYER.MINE)
            temp = m_ChestList[pos.x + pos.y * 10];
        else
            temp = m_EnemyChestList[pos.x + pos.y * 10];

        TileUpdate(temp, effect, state);
    }

    public void TileUpdate(BattleChest DamagedPos, EFFECT effect, CHEST_STATE state)
    {
        EffectController.Instance.EffectOn(effect, 1.0f, DamagedPos.transform.position);
        DamagedPos.ChangeState(state);
       
    }

    public void DamagedResult(BattleChest DamagedPos)
    {
       PlayerManager.Instance.CheckShipDamaged(DamagedPos.m_Pt);
       FireCreate(DamagedPos.transform.position);
        
    }



    public void FireCreate(Vector3 pos)
    {
        pos.y = 3.0f; pos.z -= 0.55f;
        GameObject go = Instantiate(m_FireEffect);
        go.transform.position = pos;
    }

    public void AttackSuccessText()
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            m_BattleInfo.SetText("AttackSuccess!!", UI_ANIMATION.DOWNNER, m_InfoColor);
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            m_BattleInfo.SetText("DAMAGED!!", UI_ANIMATION.SHAKE, m_InfoColor);
        }

        ComboUpdate();
    }

    private void AttackFailText()
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            m_BattleInfo.SetText("AttackFAIL!!", UI_ANIMATION.SHAKE, m_InfoColor);
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            SoundManager.Instance.playSoundOnseShot("UseSkill");
            m_BattleInfo.SetText("Enemy's Attack Fail!!", UI_ANIMATION.SHAKE, m_InfoColor);
        }

        ComboFail();
    }

    public void SkillResultInfo( )
    {

        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            m_BattleInfo.SetText("Skill Attack!!", UI_ANIMATION.UPPER, m_InfoColor);
           
        }
        if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            SoundManager.Instance.playSoundOnseShot("UseSkill");
            m_BattleInfo.SetText("Enemy used a Skill!!", UI_ANIMATION.UPPER, m_InfoColor);
          
        }

        ComboFail();
        m_BattleInfo.reSetText(4.0f);

    }

    public int RaderResult(int count)
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            DrawTextRaderReult(count);
            SoundManager.Instance.playSoundOnseShot("Rader");

            m_BattleInfo.SetText("Detected Enemy " + count + "!!", UI_ANIMATION.UPPER, m_InfoColor);

            SetActiveAllRaderTXT(true);
           
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {

            if (!BattleManager.Instance.m_isDefendMode)
            {
                SoundManager.Instance.playSoundOnseShot("UseSkill");
                m_BattleInfo.SetText("Enemy used a Skill!!", UI_ANIMATION.UPPER, m_InfoColor);
            }
            else
            {
                SoundManager.Instance.playSoundOnseShot("DefendFail");
                m_BattleInfo.SetText("Defend Fail", UI_ANIMATION.SHAKE, m_InfoColor);
            }
        }

        ComboFail();
        m_BattleInfo.reSetText(4.0f);

        return count;
       
    }

    public void DefendResult()
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            SoundManager.Instance.playSoundOnseShot("AttackBlock");

            m_BattleInfo.SetText(" Enemy block Attack!!", UI_ANIMATION.SHAKE, m_InfoColor);

            SetActiveAllRaderTXT(true);
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            SoundManager.Instance.playSoundOnseShot("DefendSuccess");
            m_BattleInfo.SetText(" Defend Success!! ", UI_ANIMATION.DOWNNER, m_InfoColor);
        }

        ComboFail();
        m_BattleInfo.reSetText(4.0f);
    }


    public void DefendFailResult()
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            SoundManager.Instance.playSoundOnseShot("AttackBlock");
            m_BattleInfo.SetText(" Defend Attack Once!! ", UI_ANIMATION.UPPER, m_InfoColor);
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            SoundManager.Instance.playSoundOnseShot("DefendFail");
            m_BattleInfo.SetText(" Defend Fail!! ", UI_ANIMATION.SHAKE, m_InfoColor);
        }

        ComboFail();
        m_BattleInfo.reSetText(4.0f);

    }

    private void DrawTextRaderReult(int count)
    {
        GameObject go = Instantiate(m_RaderTxt.gameObject, m_Canvas.transform);
        go.GetComponent<Text>().text = count.ToString();
        go.GetComponent<Text>().rectTransform.position = Camera.main.WorldToScreenPoint(m_RaderResultList[m_RaderResultList.Count - 1].transform.position);
        m_RaderResultList[m_RaderResultList.Count - 1].SetText(count);
        m_RaderTXTList.Add(go);
    }


    private void SetActiveAllRaderTXT(bool TF)
    {
        for(int i=0; i < m_RaderTXTList.Count; i++)
        {
            m_RaderTXTList[i].SetActive(TF);
            
        }
    }

    public void SelectCaution(string Str)
    {
        SoundManager.Instance.playSoundOnseShot("DefendFail");
        m_BattleInfo.SetText(Str, UI_ANIMATION.SHAKE, Color.red);
        m_BattleInfo.reSetText(4.0f);
    }


    private void ComboUpdate()
    {
        ComboGage++;
        m_ComboInfo.SetText("COMBO " + ComboGage + "!!", UI_ANIMATION.DOWNNER, Color.red);
    }

    private void ComboFail()
    {
        ComboGage = 0;
        m_ComboInfo.reSetText(0.1f);
    }

    public void SelectRaderPt(BattleChest block)
    {
        GameObject raderGO = Instantiate<GameObject>(m_RaderTargetPositionImage);
        raderGO.transform.position = block.transform.position;
        m_RaderResultList.Add(raderGO.GetComponent<RaderRotate>());
    }

    public void SelectAttackPt(BattleChest block)
    {
        m_TargetPositionImage.transform.position = block.transform.position;
    }

    public void UnSelectedAttackPt()
    { 
        m_TargetPositionImage.transform.position = new Vector3(100, 100, 100);
    }

    public void BattleResult(PLAYER victoryPlayer)
    {
        if(victoryPlayer == PLAYER.MINE)
        {
            m_BattleResultInfo.SetText("VICTORY!!", UI_ANIMATION.UP_DOWN, new Color(0.5f,1,0));
            SoundManager.Instance.playSoundOnseShot("Victory");
        }
        else if (victoryPlayer == PLAYER.OPPONENT)
        {
            m_BattleResultInfo.SetText("DEFEAT!!", UI_ANIMATION.UP_DOWN, Color.red);
            SoundManager.Instance.playSoundOnseShot("Defeat");
        }
    }
}
