using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaderRotate : MonoBehaviour {

    public int m_DetectedNum;


    void Start()
    {
        m_DetectedNum = 0;
    }

   public void SetText( int num)
    {
        m_DetectedNum = num;
    }


	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * Time.deltaTime * 100.0f, Space.World);
	}
}
