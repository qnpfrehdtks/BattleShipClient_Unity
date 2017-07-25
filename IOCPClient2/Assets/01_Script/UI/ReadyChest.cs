using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyChest : MonoBehaviour {

    private SpriteRenderer m_Renderer;
    private Color m_OriginColor;

	// Use this for initialization
	void Awake () {
        m_Renderer = GetComponent<SpriteRenderer>();
        m_OriginColor = m_Renderer.color;
    }

    void Update()
    {


    }

    void OnMouseUp()
    {
        UIPanel_Ready.instance.installShip();
    }


}
