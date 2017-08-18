using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EFFECT
{
    SUPER_BOMB,
    WATER,
    BOMB,
    RADER,
    FIRE
       
}

public class EffectController : Singleton_Manager<EffectController> {

    GameObject[] m_ParticleArr;
    Dictionary<EFFECT, List<GameObject>> m_ParticleTable;


    bool m_isInit;

    protected override bool Init() {

        if (m_isInit) return false;

        m_isInit = true;
        m_ParticleTable = new Dictionary<EFFECT, List<GameObject>>();

     
        m_ParticleArr =  Resources.LoadAll<GameObject>("03Effect");

        for(int i=0; i < m_ParticleArr.Length; i++)
        {
            List<GameObject> Temp = new List<GameObject>();

            for(int j =0; j < 10; j++)
            {
                Temp.Add(CreateItem(m_ParticleArr[i]));
            }

            m_ParticleTable.Add(m_ParticleArr[i].GetComponent<Effect>().m_Effect, Temp);
        }

        return true;

	}



    public void EffectOn(EFFECT effect, float Time, Vector3 pos)
    {
         GameObject go = popFromPool(effect);
        go.SetActive(true);


        pos.y = 3;

        go.transform.position = pos;
        go.GetComponent<Effect>().PlayEffect(Time);

        StartCoroutine(pushToPool(Time, go));

    }



    IEnumerator pushToPool(float Time, GameObject go)
    {
        yield return new WaitForSeconds(Time);

        pushToPool(go);
        
    }

    private GameObject popFromPool(EFFECT effect)
    {
      List<GameObject> objList = m_ParticleTable[effect];
        GameObject item;

        if (objList == null) return null;

        if (objList.Count > 1)
        {
            item = objList[0];
            objList.RemoveAt(0);

            return item;
        }

        item = objList[0];
        m_ParticleTable[effect].Add(CreateItem(item));

        return item;

    }


    private bool pushToPool(GameObject item)
    {
        //   Debug.Log(objName);
        List<GameObject> objList = m_ParticleTable[item.GetComponent<Effect>().m_Effect];

        if (objList == null)
        {
            return false;
        }
        item.SetActive(false);
        objList.Add(item);

        return true;

    }


    public GameObject CreateItem(GameObject obj)
    {
        GameObject item = Instantiate(obj) as GameObject;
        item.SetActive(false);
        item.transform.SetParent(transform);
        return item;

    }








}
