using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Base_Ship : MonoBehaviour {

    public SHIP m_shipKind;
    public Transform m_Positon { get; private set; }
    public Quaternion m_Quat{ get; private set; }
    public Color m_Color { get; private set; }

    public float m_defX;
    public float m_defY;

    public int m_X;
    public int m_Y;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void SetPos(int X, int Y)
    {
        Vector3 pos = new Vector3(0, 0, 0);
   

    }



}
