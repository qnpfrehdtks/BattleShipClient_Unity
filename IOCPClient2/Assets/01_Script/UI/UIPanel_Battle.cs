using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel_Battle : SingletonUIPanel<UIPanel_Battle>
{

    


    public Vector3 m_OriginCameraPos;
    public Battle_CameraMove m_CameraMove;
    // 설치된 칸들의 기준 축이 될 플레이어 측 오브젝ㅌ,
    public GameObject m_ChestAxis;

    // 설치된 칸들의 기준 축이 될 적 측 오브젝ㅌ,
    public GameObject m_EnemyChestAxis;



    // 우리가 설치할 칸 오브젝트
    public GameObject m_ChestObject;

    public GameObject m_PlayerCaptain;

    // 칸 리스트 담음.
    public List<BattleChest> m_EnemyChestList;
    public List<BattleChest> m_ChestList;

    // Use this for initialization
    void Start () {

        m_CameraMove = Camera.main.gameObject.GetComponent<Battle_CameraMove>();

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
                battle.m_X = j;
                battle.m_Y = i;

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
                battle.m_X = j;
                battle.m_Y = i;

                battle.init();


                m_EnemyChestList.Add(battle);
            }
        }

        m_CameraMove.setMoveLerpToPos(m_EnemyChestAxis.transform.position, 1.0f);
        //  Base_Ship ship = m_Ship1.GetComponent<Base_Ship>();
        //   ship =  PlayerManager.Instance.getCurPlayer().m_InstalledShipMap[SHIP.PT_BOAT];


        ////   Debug.Log(PlayerManager.Instance.getCurPlayer().m_InstalledShipMap.Count);


        ////  ship.SetPositon();

    }

    void Update()
    {
        


    }


	

}
