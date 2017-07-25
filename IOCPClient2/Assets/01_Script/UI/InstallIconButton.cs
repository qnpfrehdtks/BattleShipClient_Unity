using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstallIconButton : MonoBehaviour {

    private Vector3 m_DefPos;

    public bool m_isSelected { get; set; }

    private GameObject m_ShipObj;
    private Base_Ship m_Ship;

    RectTransform m_MyTr;
    GameObject m_MyGo;

    Vector3 m_rotateAngle;

	// Use this for initialization
	void Start () {
        m_DefPos = new Vector3(0,0, -1);
        // m_MyGo = GetComponent<GameObject>();
        m_MyTr = GetComponent<RectTransform>();
        m_rotateAngle = new Vector3(0, 90, 0);

        m_MyTr.gameObject.SetActive(false);

    }


    void Update()
    {
        //if (m_isSelected)
        //{ 
        //    SetPos(Camera.main.WorldToScreenPoint(m_Ship.transform.position
        //        + ((m_Ship.m_ChestSize) / 2 ) * 0.9f * Vector3.right +  m_DefPos));
        //}
    }


    public void SetPos(Vector3 pos)
    {
        m_MyTr.transform.position = pos;
    }

    public void Rotate()
    {
        if (m_Ship != null)
        {
            m_Ship.transform.Rotate(m_rotateAngle);
        }
    }

    public void OK()
    {
        UIPanel_Ready.instance.installShip();
    }

    public void Cancle()
    {
        UIPanel_Ready.instance.ShipCall(null, null);
    }

    public void SetShip(GameObject shipObj, Base_Ship shipInfo)
    {
        if (shipObj != null)
        {
            m_isSelected = true;
            gameObject.SetActive(true);
        }
        else
        {
            m_isSelected = false;
            gameObject.SetActive(false);
        }

        m_Ship = shipInfo;
        m_ShipObj = shipObj;
    }
}
