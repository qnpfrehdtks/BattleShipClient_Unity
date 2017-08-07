using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipChest : Chest {

    private bool m_isDamaged;
    private Base_Ship m_Ship;
    private bool m_isCheckOnce;
    
    // Use this for initialization
    void Start () {

        m_isCheckOnce = false;

        m_Ship = GetComponentInParent<Base_Ship>();
        m_isCanInstalled = true;
        m_isDamaged = false;

        init();
        ChangeState(CHEST_STATE.ORIGIN);
    }

    void Update()
    {
        if(gameSceneManager.Instance.m_currentScene == SCENE.SC_READY)
           CheckRay();


    }



    public void DamageBlock()
    {
         m_isDamaged = true;
         ChangeState(CHEST_STATE.RED);
         m_Ship.Damaged();
            // 서버에게 날려야함.
    }

    public void RepairBlock()
    {
        if (m_ChestState == CHEST_STATE.RED)
        {
            m_isDamaged = false;
            ChangeState(CHEST_STATE.GREEN);
            m_Ship.Repair();
            //서버에게 날리자.
        }
    }

    void OnMouseEnter()
    {
       DamageBlock();
    }


    //// ReadyChest 와 비교하여 자신의 XY 위치를 알아옴 ㅋ
    //public void CheckMyXY()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.up, out hit, 100000.0f))
    //    {
    //        if (hit.collider.tag == "ReadyChest")
    //        {
    //            ReadyChest ready =  hit.collider.GetComponent<ReadyChest>();
    //            m_Pt.x = ready.m_Pt.x;
    //            m_Pt.y = ready.m_Pt.y;
    //            Debug.Log("확인 완료" + m_Pt.x);
    //            Debug.Log("확인 완료" + m_Pt.y);
    //            ChangeState(CHEST_STATE.YELLOW);
    //            // ready.ChangeState(CHEST_STATE.RED);
    //        }
    //    }
    //    else
    //    {
    //        // ChangeState(CHEST_STATE.ORIGIN);
    //        ChangeState(CHEST_STATE.BLUE);
    //    }
      
    //}

    public void BattleCheckBlock()
    {
        Vector3 pos = transform.position;
        pos.z += 0.5f;

        RaycastHit hit;

        if (Physics.Raycast(pos, -Vector3.forward, out hit, 100000.0f))
        {
            if (hit.collider.tag == "ReadyChest")
            {
                BattleChest battleBlock = hit.collider.GetComponent<BattleChest>();
                battleBlock.m_isCanInstalled = false;
                battleBlock.ChangeState(CHEST_STATE.RED);
                m_Pt.x = battleBlock.m_Pt.x;
                m_Pt.y = battleBlock.m_Pt.y;
            }
        }
        else
        {
            Debug.Log("충돌? 안함ㅋ");
           
        }

    }




}
