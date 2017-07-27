using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Chest : MonoBehaviour {


    public PLAYER m_PlayerOwn
    {
        get; set;
    }


    protected CHEST_STATE m_ChestState;
    protected SpriteRenderer m_Renderer;
    protected Color m_OriginColor;
    protected SCENE m_CurScene;

    public bool m_isCanInstalled { get; set;}

    public int m_X;
    public int m_Y;

   public void init()
    {
        m_CurScene = gameSceneManager.Instance.m_currentScene;
        m_Renderer = GetComponent<SpriteRenderer>();
        m_OriginColor = m_Renderer.color;
    }



    public virtual void CheckRay()
    {
         //   Debug.Log("DDSAD");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10000.0f))
            {
                if (hit.collider.tag == "ShipChest" || hit.collider.tag == "Wall")
                {
                   ChangeState(CHEST_STATE.RED);
                }
            }
            else
            {
                  ChangeState(CHEST_STATE.GREEN);
            }
    }


    public void ChangeState(CHEST_STATE state)
    {
        switch (state)
        {
            case CHEST_STATE.ORIGIN:
                m_Renderer.color = m_OriginColor;
                break;
            case CHEST_STATE.GREEN:
                m_Renderer.color = Color.green;
                break;
            case CHEST_STATE.RED:
                m_Renderer.color = Color.red;
                break;
            case CHEST_STATE.YELLOW:
                m_Renderer.color = Color.yellow;
                break;
        }

        m_ChestState = state;

    }



}
