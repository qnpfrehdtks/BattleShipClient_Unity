using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyChest : Chest {

	// Use this for initialization
	void Awake () {
        init();
    }

    void OnMouseUp()
    {
        UIPanel_Ready.instance.installShip(m_X, m_Y);
       
    }


}
