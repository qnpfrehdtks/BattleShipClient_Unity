using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Main : SingletonUIPanel<UIPanel_Main>
{

    void Start()
    {
      //  m_audioSource =  gameObject.GetComponent<AudioSource>();
        SoundManager.Instance.PlayBGM("Main");
    }


    public void GotoWaiting()
    {
        SoundManager.Instance.playSoundOnseShot("FAIL");
        gameSceneManager.Instance.SceneChange(SCENE.SC_WAIT);
    }

    public void GotoExit()
    {
        Application.Quit();
    }



}
