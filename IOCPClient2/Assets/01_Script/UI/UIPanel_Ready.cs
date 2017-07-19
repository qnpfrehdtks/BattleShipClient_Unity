using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Ready : SingletonUIPanel<UIPanel_Ready>
{

    public GameObject m_ChestAxis;
    public GameObject m_ChestObject;
    public List<GameObject> m_ChestList;
    public List<ShipSelectButton> m_ButtonList;

    private GameObject m_TempShip;

	// Use this for initialization
	void Start () {
        m_ChestList = new List<GameObject>();

        for(int i=0; i < 10; i++)
        {
            for(int j=0; j < 10; j++)
            {
                GameObject chest = Instantiate(m_ChestObject);
                chest.transform.parent = m_ChestAxis.transform;
                chest.transform.localPosition = new Vector3(-0.8f * j, 0, 0.8f * i);
                m_ChestList.Add(chest);
            }
        }
	}




	
	// Update is called once per frame
	void Update () {
	}
}
