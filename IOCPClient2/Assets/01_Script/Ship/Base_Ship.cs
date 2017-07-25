using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Ship : MonoBehaviour {


    public bool m_isInstalled
    {
        get; set;
    }

    private List<ShipChest> m_ShipChestList;

    public GameObject m_Go;
    public SHIP m_shipKind;
    public Transform m_Trans { get; private set; }
    public Quaternion m_Quat{ get; private set; }
    public Color m_Color { get; private set; }

    //public float m_defX;
    //public float m_defY;

    //public int m_X;
    //public int m_Y;

    //public int m_SizeX;
    //public int m_SizeY;

    public bool m_isDrag
    {
        get; private set;
    } // 지금 현재 설치 할려고 끌려 다니는중 인가???
    public int m_ChestSize
    {
        get; private set;
    }

    public bool m_isCanInstall {
        get; set; }

    public string m_ShipName;
    // Use this for initialization
    void Start () {

        m_isInstalled = false;
        m_isCanInstall = true;
         m_ShipChestList = new List<ShipChest>();
        ShipChest[] shipChests = GetComponentsInChildren<ShipChest>();   

        for (int i=0; i < shipChests.Length; i++)
        {
            m_ShipChestList.Add(shipChests[i]);
        }

        m_ChestSize = m_ShipChestList.Count;

        m_Trans = GetComponent<Transform>();

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

            m_Trans.position = tempVec;
        }
    }
    

    void SetPos(int X, int Y)
    {
        ////m_X = X;
        ////m_Y = Y;

        Vector3 pos = new Vector3(X * -1.2f, 0, Y * -1.2f);
        m_Trans.position = pos;
        
    }
    void SetRotate(Vector3 Y)
    {
        m_Trans.Rotate(Y);
    }


    public bool checkCanInstall()
    {
        Debug.Log(m_ChestSize);
        int count = 0;

        // 확인한다 이 배가 설치 가능한 위치에 있는지
        for (int i=0; i < m_ChestSize; i++)
        {
            if (m_ShipChestList[i].m_isCanInstall)
                count++;
        }

       if(count >= m_ChestSize)
        {
            m_isDrag = false;
            return true;
        }

        return false;
    }


    public void InstallSetting()
    {
        m_isDrag = false;

        Vector3 tempVec = transform.position;
        tempVec.y = 0.5f;
        transform.position = tempVec;
        
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


    public void setInstallMode(bool TF )
    {
        if(TF)
        {
            m_Go.SetActive(true);
            m_isDrag = true;
            
        }
        else
        {
            m_isDrag = false;
            m_Go.SetActive(false);
        }

        
    }

}
