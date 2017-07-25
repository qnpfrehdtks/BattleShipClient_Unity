using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneCtrlController  {


    private Dictionary<SCENE, SceneCtrl> m_SceneCtrls;


    public void Init()
    {
        m_SceneCtrls = new Dictionary<SCENE, SceneCtrl>();
        
    }

    public void CreateSceneCtrlAndInit(SCENE state)
    {
        // 처음 SceneSctrl이 만들어져 있다면 그냥 Init 함수 실행시키고,
        // 만약 만들어져 있지 않으면 동적할당하고 Dic Container에 넣고 재활용하도록 하자

        if (!m_SceneCtrls.ContainsKey(state))
        {
            SceneCtrl newScencCtrl = null;

            switch (state)
            {
                case SCENE.SC_MAIN:
                        newScencCtrl = new MainCtrl();
                    break;

                case SCENE.SC_WAIT:
                    Debug.Log("Wait Scene Ctrl switch Enter");
                        newScencCtrl = new WaitCtrl();
                    break;

                case SCENE.SC_READY:
                    Debug.Log("Wait Scene Ctrl switch Enter");
                    newScencCtrl = new ReadyCtrl();
                    break;

                case SCENE.SC_BATTLE:
                    Debug.Log("Wait Scene Ctrl switch Enter");
                    newScencCtrl = new BattleCtrl();
                    break;
            }

            if (newScencCtrl != null)
            {
                m_SceneCtrls.Add(state, newScencCtrl);
            }
            else
            {
                Debug.Log(":(  WTF!!!!");
                return;
            }
        }
        m_SceneCtrls[state].SceneInit();
    }



}
