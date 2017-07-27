using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipChest : Chest {

    private bool m_isDamaged;
    private Base_Ship m_Ship;
    
    // Use this for initialization
    void Start () {
      
        m_Ship = GetComponentInParent<Base_Ship>();
        m_isCanInstalled = true;
        m_isDamaged = false;

        init();
        ChangeState(CHEST_STATE.ORIGIN);
    }

    void Update()
    {
        if(m_CurScene == SCENE.SC_READY)
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


    // ReadyChest 와 비교하여 자신의 XY 위치를 알아옴 ㅋ
    public void CheckMyXY()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, -1 * transform.up, out hit, 100.0f))
        {
            if (hit.collider.tag == "ReadyChest")
            {
                ReadyChest ready =  hit.collider.GetComponent<ReadyChest>();
                m_X = ready.m_X;
                m_Y = ready.m_Y;
            }
        }
        else
        {
            Debug.Log("충돌? 안함ㅋ");
        }
      
    }



}
