using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPanel_Wait : SingletonUIPanel<UIPanel_Wait>
{
    public GameObject m_isServerClosedWin;
   public GameObject Logo;
    RectTransform tr;
    float speed = 90.0f;

	// Use this for initialization
	void Start () {
       
        tr = Logo.GetComponent<RectTransform>();

        //if (NetworkManager.Instance.m_isConnectServer)
        //    m_isServerClosedWin.SetActive(true);
    }
	
	// Update is called once per frame
	void Update () {


        tr.Rotate(Vector3.up * speed * Time.deltaTime);
	}
}
