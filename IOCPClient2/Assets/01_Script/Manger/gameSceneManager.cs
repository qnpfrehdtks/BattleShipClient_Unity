using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SCENE
{
    SC_MAIN,
    SC_WAIT,
    SC_READY,
    SC_BATTLE

}

public class gameSceneManager : Singleton_Manager<gameSceneManager> {

    private CameraFadeInOut m_fadeInOutCtrl;
    private SceneCtrlController m_scencCon;

    private SCENE m_currentScene;
   
    private float m_FadeOutTime = 0.5f;
    private float m_FadeInTime = 0.5f;
    

    protected override bool Init()
    {
        m_scencCon = new SceneCtrlController();
     //   m_SceneCtrls = new Dictionary<SCENE, SceneCtrl>();

        if (m_fadeInOutCtrl == null)
        {
            m_fadeInOutCtrl = GetComponent<CameraFadeInOut>();
            if (m_fadeInOutCtrl == null)
            {
                m_fadeInOutCtrl = this.gameObject.AddComponent<CameraFadeInOut>();
            }
        }

        m_currentScene = SCENE.SC_MAIN;
        m_scencCon.Init();
        return true;
    }


   public void SceneChange(SCENE state)
    {
        switch(state)
        {
            case SCENE.SC_MAIN:
                LoadScene("Main", () => {
                    m_scencCon.CreateSceneCtrlAndInit(state);
                });
                break;
            case SCENE.SC_WAIT:
                LoadScene("WaitingRoom", () => {
                    m_scencCon.CreateSceneCtrlAndInit(state);
                });
                break;
            case SCENE.SC_READY:
                LoadScene("ReadyRoom", () => { m_scencCon.CreateSceneCtrlAndInit(state); });
                break;
            case SCENE.SC_BATTLE:
                LoadScene("BattleRoom", () => { m_scencCon.CreateSceneCtrlAndInit(state); });
                break;
        }

        m_currentScene = state;
    }
    void LoadScene(string sceneName, System.Action onComplete)
    {
        StartCoroutine(coLoadScene(sceneName, onComplete));
    }

    private IEnumerator coLoadScene(string sceneName, System.Action onComplete)
    {

        //UIManager.Instance.hideAllPanel();

        this.m_fadeInOutCtrl.FadeOut(this.m_FadeOutTime);
      yield return new WaitForSeconds(this.m_FadeOutTime);


        AsyncOperation async = SceneManager.LoadSceneAsync(sceneName);
        yield return async;

        if (onComplete != null)
        {
            onComplete.Invoke();
        }


       // UIManager.Instance.initSceneUI(currentScene);

        this.m_fadeInOutCtrl.FadeIn(this.m_FadeInTime);

    }

  



}
