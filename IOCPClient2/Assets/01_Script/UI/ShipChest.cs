using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipChest : MonoBehaviour {

    private Base_Ship m_Ship;
    
    public bool m_isCanInstall { get; set; }

    private SpriteRenderer m_Renderer;
    private Color m_OriginColor;

    // Use this for initialization
    void Start () {

        m_Ship = GetComponentInParent<Base_Ship>();
      //Debug.Log(m_Ship.m_isInstalled);
        m_isCanInstall = true;
        m_Renderer = GetComponent<SpriteRenderer>();
        m_OriginColor = m_Renderer.color;
    }


    void Update()
    { 
        //transform.Translate(Vector3.forward * Time.deltaTime * 0.1f);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000000.0f))
        {
            if (hit.collider.tag == "ShipChest" || hit.collider.tag == "Wall")
            {
             //   Gizmos.DrawRay(transform.position, transform.forward * hit.distance);
                Debug.Log(hit.collider.tag);
                m_Renderer.color = Color.red;
            }
        }
        else
        {
            m_Renderer.color = Color.green;
        }
    }

    //void OnTriggerEnter(Collider coll)
    //{
    //    if (coll.tag == "Wall")
    //    {
    //        m_isCanInstall = false;
    //        m_Renderer.color = Color.red;
    //    }
    //    else if(coll.tag == "ReadyChest")
    //    {
    //        m_isCanInstall = true;
    //        m_Renderer.color = m_OriginColor;
    //    }

    //}

    //void OnTriggerStay(Collider coll)
    //{
    //    if (coll.tag == "ShipChest")
    //    {
    //        m_isCanInstall = false;
    //        m_Renderer.color = Color.red;
    //    }

    //}
    //void OnTriggerExit(Collider coll)
    //{
    //    if (coll.tag == "ShipChest")
    //    {
    //       m_isCanInstall = true;
    //       m_Renderer.color = m_OriginColor;
    //    }
    //    //if (coll.tag == "ReadyChest")
    //    //{
    //    //    m_isCanInstall = false;
    //    //    m_Renderer.color = Color.red;
    //    //}

    //}


    void OnMouseUp()
    {
      
        //if(!m_Ship.m_isDrag && m_Ship.m_isInstalled)
        //{
        //    UIPanel_Ready.instance.SelectShip(m_Ship);
        //}
    }



}
