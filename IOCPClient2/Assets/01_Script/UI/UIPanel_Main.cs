using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPanel_Main : SingletonUIPanel<UIPanel_Main>
{

    public void GotoWaiting()
    {
        gameSceneManager.Instance.SceneChange(SCENE.SC_WAIT);
    }

    public void GotoExit()
    {
        Application.Quit();
    }



}
