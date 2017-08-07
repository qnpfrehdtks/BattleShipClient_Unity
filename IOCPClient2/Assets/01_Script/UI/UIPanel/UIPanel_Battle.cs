using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Battle : SingletonUIPanel<UIPanel_Battle>
{

    public List<RaderRotate> m_RaderResultList;
    public List<GameObject> m_RaderTXTList;

    
    public GameObject m_TargetPositionImage;
    public GameObject m_RaderTargetPositionImage;
    public Text m_RaderTxt;


    private int ComboGage;

    public Text m_Combo;
    public Text m_Info;
    public Color m_InfoColor;
    public Vector3 m_OriginCameraPos;
    public Battle_CameraMove m_CameraMove;

  

    // Particle Bomb;
    public GameObject m_BombEffect;
    private ParticleSystem m_BombParticle;


    // Particle Fire;
    private GameObject m_FireEffect;
    private ParticleSystem m_FireParticle;

    // Water Bomb;
    public GameObject m_WaterEffect;
    private ParticleSystem m_WaterParticle;


    // 설치된 칸들의 기준 축이 될 플레이어 측 오브젝ㅌ,
    public GameObject m_ChestAxis;

    // 설치된 칸들의 기준 축이 될 적 측 오브젝ㅌ,
    public GameObject m_EnemyChestAxis;

    public GameObject m_EnemyCameraPos;
    public GameObject m_PlayerCameraPos;

    // 우리가 설치할 칸 오브젝트
    public GameObject m_ChestObject;

    public GameObject m_PlayerCaptain;

    public Text m_Turntext;

    // 칸 리스트 담음.
    public List<BattleChest> m_EnemyChestList;
    public List<BattleChest> m_ChestList;

    // Use this for initialization
    void Start () {

        m_InfoColor = m_Info.color;
        m_Canvas =  GameObject.Find("Canvas");

        m_Info.text = " ";
        m_Combo.text = " ";

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

        m_BombParticle = m_BombEffect.GetComponent<ParticleSystem>();
        m_BombParticle.Stop();
        var main = m_BombParticle.main;
        main.loop = false;


        m_WaterParticle = m_WaterEffect.GetComponent<ParticleSystem>();
        m_WaterParticle.Stop();

        //      GameStart(PACKETSTATE.PK_BATTLE_START);

        //     m_CameraMove.setMoveLerpToPos(m_EnemyCameraPos.transform.position, 1.0f);
        // m_CameraMove.setMoveLerpToPos(m_EnemyCameraPos.transform.position, 1.0f);
        //  Base_Ship ship = m_Ship1.GetComponent<Base_Ship>();
        //   ship =  PlayerManager.Instance.getCurPlayer().m_InstalledShipMap[SHIP.PT_BOAT];

        //    m_TargetPositionImage = Instantiate(m_TargetPositionImage, new Vector3(100, 100, 100), Quaternion.identity);
        ////   Debug.Log(PlayerManager.Instance.getCurPlayer().m_InstalledShipMap.Count);


        ////  ship.SetPositon();

    }


    public void UI_TurnUpdate(PLAYER state)
    {
        m_CameraMove = Camera.main.gameObject.GetComponent<Battle_CameraMove>();

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
        m_Turntext.text = "PLAYER TURN";
        m_Turntext.color = Color.green;
        m_CameraMove.setMoveLerpToPos(m_EnemyCameraPos.transform.position, 5.0f);
        SetActiveAllRaderTXT(true);

    }

    private void UI_oppoTurnUpdate()
    {
        m_Info.text = " ";
        m_Combo.text = " ";
        m_Turntext.text = "ENEMY TURN";
        m_Turntext.color = Color.red;
        m_CameraMove.setMoveLerpToPos(m_PlayerCameraPos.transform.position, 5.0f);
        SetActiveAllRaderTXT(false);
    }


    public void DamagedFromEnemy(sVector2[] pos)
    {
        SoundManager.Instance.playSoundOnseShot("BOMB");

        for (int i = 0; i < pos.Length; i++)
        {
            BattleChest temp = m_ChestList[pos[i].x + pos[i].y * 10];
            StartCoroutine(BombEffect(temp.transform.position));
          
            // 적이 명중을 못한 경우
            if (temp.m_isCanInstalled)
            {
                temp.ChangeState(CHEST_STATE.BLUE);
            }
            // 명중한 경우
            else if (!temp.m_isCanInstalled)
            {
                temp.ChangeState(CHEST_STATE.RED);
                PlayerManager.Instance.CheckShipDamaged(pos[i]);
                FireCreate(temp.transform.position);
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
            StartCoroutine(WaterEffect(temp.transform.position));
            temp.ChangeState(CHEST_STATE.BLUE);
            AttackFailText();

            return false;
        }
        // 명중한 경우
        else if (!temp.m_isCanInstalled)
        {

            StartCoroutine(BombEffect(temp.transform.position));
            temp.ChangeState(CHEST_STATE.RED);
            AttackSuccessText();
            PlayerManager.Instance.CheckShipDamaged(pos);
            FireCreate(temp.transform.position);

         
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

            StartCoroutine(BombEffect(temp.transform.position));
            temp.ChangeState(CHEST_STATE.RED);
            AttackSuccessText();
          //  temp.ShipChestCheck();
        
        }  // 명중 못한 경우
        else if (state == PACKETSTATE.PK_PLAYER_DAMAGEFAIL)
        {

            StartCoroutine(WaterEffect(temp.transform.position));
            temp.ChangeState(CHEST_STATE.BLUE);
            AttackFailText();

        }

        return temp;
    }





    public void DamageToEnemy(sVector2 DamagedPos, SKILL state)
    {
          BattleChest temp = m_EnemyChestList[DamagedPos.x + DamagedPos.y * 10];
          StartCoroutine(BombEffect(temp.transform.position));
          temp.ChangeState(CHEST_STATE.RED);
      
    }


    public void noDamageToEnemy(sVector2 noDamagedPos, SKILL state)
    {
            BattleChest temp = m_EnemyChestList[noDamagedPos.x + noDamagedPos.y * 10];
            StartCoroutine(BombEffect(temp.transform.position));
            temp.ChangeState(CHEST_STATE.BLUE);
    }

    public IEnumerator BombEffect(Vector3 pos)
    {
        m_BombEffect.gameObject.SetActive(true);
        pos.y = 3.0f;
        m_BombParticle.Emit(1);
        m_BombEffect.transform.position = pos;

        yield return new WaitForSeconds(1.0f);

        m_BombEffect.gameObject.SetActive(false);
    }

    public IEnumerator WaterEffect(Vector3 pos)
    {
        m_WaterEffect.gameObject.SetActive(true);
        pos.y = 3.0f;
        m_WaterParticle.Emit(1);
        m_WaterEffect.transform.position = pos;

        yield return new WaitForSeconds(1.0f);

        m_WaterEffect.gameObject.SetActive(false);
    }

    public void FireCreate(Vector3 pos)
    {
        pos.y = 3.0f; pos.z -= 0.9f;
        GameObject go = Instantiate(m_FireEffect);
        go.transform.position = pos;
    }

    public void AttackSuccessText()
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            m_Info.text = "AttackSuccess!!";
            m_Info.color = m_InfoColor;
            ComboUpdate();
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            m_Info.text = "DAMAGED!!";
            m_Info.color = m_InfoColor;
            ComboUpdate();
        }
    }

    private void AttackFailText()
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            m_Info.text = "AttackFAIL!!";
            m_Info.color = m_InfoColor;
            ComboFail();
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            m_Info.text = "Enemy's Attack Fail!!";
            m_Info.color = m_InfoColor;
            ComboFail();
        }
    }


    public void SkillResult()
    {
        StartCoroutine(SkillResultInfo());

    }

    public int RaderResult(int count)
    {
        StartCoroutine(RaderResultInfo(count));

        return count;
    }

    private IEnumerator SkillResultInfo( )
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            SoundManager.Instance.playSoundOnseShot("Rader");
            m_Info.text = "Detected Enemy !!";
            m_Info.color = m_InfoColor;
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            SoundManager.Instance.playSoundOnseShot("FAIL");
            m_Info.text = " Enemy used a Skill!! ";
            m_Info.color = m_InfoColor;
        }

        yield return new WaitForSeconds(4.0f);

        m_Info.text = " ";

    }

    private IEnumerator RaderResultInfo(int count)
    {
        if (BattleManager.Instance.m_whoTurn == PLAYER.MINE)
        {
            DrawTextRaderReult(count);
            SoundManager.Instance.playSoundOnseShot("Rader");
            m_Info.text = "Detected Enemy " + count + "!!";
            m_Info.color = m_InfoColor;
            SetActiveAllRaderTXT(true);
        }
        else if (BattleManager.Instance.m_whoTurn == PLAYER.OPPONENT)
        {
            SoundManager.Instance.playSoundOnseShot("FAIL");
            m_Info.text = " Enemy used a Skill!! ";
            m_Info.color = m_InfoColor;
        }

        yield return new WaitForSeconds(4.0f);

        m_Info.text = " ";
       
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


    public void TileSelectCaution()
    {
        StartCoroutine(SelectTileCaution());
    }



    private IEnumerator SelectTileCaution()
    {
         m_Info.text = "Please Select a Block";
        m_Info.color = Color.red;

        yield return new WaitForSeconds(2.0f);

        m_Info.text = " ";
    }

    private void ComboUpdate()
    {
        ComboGage++;
        m_Combo.text = "COMBO " + ComboGage + "!!";

    }

    private void ComboFail()
    {
        ComboGage = 0;
        m_Combo.text = " ";
    }

    public void resetInfotxt()
    {
        m_Info.text = " ";
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

    public void UnSelectedAttackPt(BattleChest block)
    {
        m_TargetPositionImage.transform.position = new Vector3(100, 100, 100);
    }



}
