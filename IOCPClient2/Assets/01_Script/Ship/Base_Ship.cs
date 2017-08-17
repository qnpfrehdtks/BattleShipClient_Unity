using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Ship : MonoBehaviour {

    private List<ShipChest> m_ShipChestList;

    public SHIP m_shipKind;

    private Vector3 m_Postion;
    public Quaternion m_Quat{ get; private set; }

    public int m_X { get; set; }
    public int m_Y { get; set; }
    public int m_ChestSize { get; private set; }
    public int m_Life { get; private set; }

    public bool m_isDrag { get; private set; } // 지금 현재 설치 할려고 끌려 다니는중 인가???
    public bool m_isCanInstall { get; set; }
    public bool m_isInstalled { get; set; }
    public bool m_isDead { get; private set; }

    public string m_ShipName;
    // Use this for initialization
    void Awake () {
        gameObject.SetActive(true);
        DontDestroyOnLoad(gameObject);
        m_Postion = transform.position;
        m_isDead = false;
        m_isInstalled = false;
        m_isCanInstall = true;
         m_ShipChestList = new List<ShipChest>();
        ShipChest[] shipChests = GetComponentsInChildren<ShipChest>();   

        for (int i=0; i < shipChests.Length; i++)
        {
           m_ShipChestList.Add(shipChests[i]);
        }
       
        m_ChestSize = m_ShipChestList.Count;
        m_Life = m_ChestSize;
    }
	

    public void Reset()
    {
        m_Postion = transform.position;
        m_isDead = false;
        m_isInstalled = false;
        m_isCanInstall = true;
        m_ShipChestList.Clear();
        m_ChestSize = 0;
        m_Life = 0;
        gameObject.SetActive(false);

    }


	// Update is called once per frame
	void Update () {
        if (m_isDrag)
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Input.mousePosition.z));
            Vector3 tempVec = pos;
            tempVec.y = 1.0f;

            int tempXCan = (int)(pos.x / 0.9f);
            int tempYCan = (int)(pos.z / 0.9f);

            float tempX = tempXCan * 0.9f;
            float tempY = tempYCan * 0.9f;

            tempVec.x = tempX;
            tempVec.z = tempY;

            transform.position = tempVec;
        }
    }
    


    void SetRotate(Vector3 Y)
    {
     //   m_Quat = m_Trans.rotation;
        transform.Rotate(Y);
    }


    public bool checkCanInstall()
    {
        Debug.Log(m_ChestSize);
        int count = 0;

        // 확인한다 이 배가 설치 가능한 위치에 있는지
        for (int i=0; i < m_ChestSize; i++)
        {
            if (m_ShipChestList[i].m_ChestState != CHEST_STATE.RED)
                count++;
        }

       if(count >= m_ChestSize)
        {
            m_isDrag = false;
            return true;
        }

        return false;
    }


    public void InstallSetting(int X = 0, int Y = 0)
    {
        m_isDrag = false;

        Vector3 tempVec = transform.position;
        tempVec.y = 0.5f;
        transform.position = tempVec;

        m_Postion = tempVec;
        m_Quat = transform.rotation;

        m_X = X; m_Y = Y;

        Debug.Log("들어갔자나");
   //     CheckMyBlockXY();

        m_isInstalled = true;
    }

    public void UnInstallSetting()
    {
        m_isDrag = true;

        Vector3 tempVec = transform.position;
        tempVec.y = 1.0f;
        transform.position = tempVec;
        m_isInstalled = false;
    }



    private void CheckMyBlockXY()
    {
        for(int i=0; i < m_ShipChestList.Count; i++)
        {
         //   m_ShipChestList[i].CheckMyXY();
        }
    }

    public void setInstallMode(bool TF )
    {
        if(TF)
        {
            gameObject.SetActive(true);
            m_isDrag = true;
            
        }
        else
        {
            m_isDrag = false;
            gameObject.SetActive(false);
        }
    }

 
    public void SetPositon()
    {

        transform.position = m_Postion;
        transform.rotation = m_Quat;
        //  transform.position = m_Trans.position;
        // transform.rotation = m_Quat;
    }

    public void Damaged()
    {
        m_Life -= 1;
        if(m_Life <= 0)
        {
            m_Life = 0;
            m_isDead = true;
        }
    }

    public bool Repair()
    {
        if (!m_isDead)
        {
            m_Life += 1;
            if (m_Life > m_ChestSize)
            {
                m_Life = m_ChestSize;
            }

            return true;
        }

        return false;

    }



    public void BattleCheckBlock()
    {
        for(int i=0; i < m_ShipChestList.Count; i++)
        {
            m_ShipChestList[i].BattleCheckBlock();
        }
    }

    public bool DamagedBattleChest(sVector2 pt)
    {
        for (int i = 0; i < m_ShipChestList.Count; i++)
        {
            if (pt.x == m_ShipChestList[i].m_Pt.x
                && pt.y == m_ShipChestList[i].m_Pt.y)
            {
                m_ShipChestList[i].ChangeState(CHEST_STATE.RED);
                Damaged();

                return true;
            }
        }

        return false;
    }


    public bool CheckShipHavePoint(sVector2 pt)
    {
        for (int i = 0; i < m_ShipChestList.Count; i++)
        {
            if (pt.x == m_ShipChestList[i].m_Pt.x
                && pt.y == m_ShipChestList[i].m_Pt.y)
            {
                return true;
            }
        }

        return false;
    }


    public void AllClear()
    {
        for(int i=0; i < m_ShipChestList.Count; i++)
        {

        }
    }


}
