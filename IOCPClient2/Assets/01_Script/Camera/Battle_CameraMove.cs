using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle_CameraMove : MonoBehaviour
{

    public Vector3 m_OriginPos;

    private float m_Upsize;
    private Vector3 m_distance;


    void Start()
    {
        m_OriginPos = transform.position;
    }
    

    public void setMoveToPos(Vector3 pos, float Upsize)
    {
     
        transform.position = pos + (m_distance * Upsize);
    }

    public void setMoveLerpToPos(Vector3 pos, float timeScele)
    {
        StartCoroutine(coLerpMove(transform.position, pos, timeScele));
    }

    public void setMoveLerpToPosPingpong(Vector3 pos, float timeScele, float upSize = 1.0f)
    {
   
        m_Upsize = upSize;
        StartCoroutine(coLerpMovePingpong(transform.position, pos, timeScele));
    }

    IEnumerator coLerpMove(Vector3 origin, Vector3 pos, float timeScale)
    {
        float Rate = 0;
        yield return null;

        while (Rate < 1.0f)
        {
            transform.position = Vector3.Lerp(origin, pos + m_distance, Rate += timeScale * Time.unscaledDeltaTime);
            yield return null;

        }
        transform.position = pos;

    }


    IEnumerator coLerpMovePingpong(Vector3 origin, Vector3 pos, float timeScale)
    {
        float Rate = 0;

        yield return null;

        while (Rate < 1.0f)
        {
            transform.position = Vector3.Lerp(origin, pos + (m_distance * m_Upsize), Rate += timeScale * Time.unscaledDeltaTime);
            yield return null;

        }
        transform.position = pos + (m_distance * m_Upsize);

        StartCoroutine(wationgforOriginPosPingpong(2.0f));

    }

    IEnumerator wationgforOriginPosPingpong(float timeScale)
    {
        yield return new WaitForSeconds(timeScale);

        StartCoroutine(ToOriginalLerp(transform.position, transform.position, 2.0f));
    }

    IEnumerator ToOriginalLerp(Vector3 origin, Vector3 pos, float timeScale)
    {
        float Rate = 0;
        m_Upsize = 1.0f;
        yield return null;

        while (Rate < 1.0f)
        {
            transform.position = Vector3.Lerp(origin, pos + (m_distance * m_Upsize), Rate += timeScale * Time.unscaledDeltaTime);
            yield return null;

        }
        transform.position = pos + (m_distance * m_Upsize);
     


    }


}


