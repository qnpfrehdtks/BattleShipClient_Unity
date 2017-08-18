using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour {

    public EFFECT m_Effect;

    private ParticleSystem m_PS;


    public void init()
    {

        m_PS = GetComponent<ParticleSystem>();

        var main = m_PS.main;
        main.loop = false;

       
    }
    
    public void PlayEffect(float time)
    {
        if (!m_PS) init();

        m_PS.Emit(1);
        
    }

}
