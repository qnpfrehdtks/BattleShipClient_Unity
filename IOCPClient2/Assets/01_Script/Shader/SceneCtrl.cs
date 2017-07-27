using System;
using System.Collections;
using UnityEngine;


public abstract class SceneCtrl  {

    public abstract bool SceneInit();
    public abstract bool SceneRelease();
}


public class MainCtrl : SceneCtrl
{

      public override bool SceneInit()
    {
        UIPanel_Main.m_isClickDown = false;
        UIPanel_Main.m_isDrag = false;
        UIPanel_Main.m_isClickUp = false;

        return true;
    }

    public override bool SceneRelease()
    {
        throw new NotImplementedException();
    }




}

public class WaitCtrl : SceneCtrl
{

    public override bool SceneInit()
    { 
        UIPanel_Wait.m_isClickDown = false;
        UIPanel_Wait.m_isDrag = false;
        UIPanel_Wait.m_isClickUp = false;

        if (NetworkManager.Instance.Connect())
        { PlayerManager.Instance.CreatePlayer(); }
     //   PlayerManager.Instance.CreatePlayer();

        return true;
    }

    public override bool SceneRelease()
    {
        throw new NotImplementedException();
    }

}


public class ReadyCtrl : SceneCtrl
{

    public override bool SceneInit()
    {

        PlayerManager.Instance.CreatePlayer();
        UIPanel_Ready.m_isClickDown = false;
        UIPanel_Ready.m_isDrag = false;
        UIPanel_Ready.m_isClickUp = false;

        return true;
    }

    public override bool SceneRelease()
    {
        throw new NotImplementedException();
    }

}

public class BattleCtrl : SceneCtrl
{

    public override bool SceneInit()
    {
        UIPanel_Ready.m_isClickDown = false;
        UIPanel_Ready.m_isDrag = false;
        UIPanel_Ready.m_isClickUp = false;

        PlayerManager.Instance.CreatePlayerCaptain();


        return true;
    }

    public override bool SceneRelease()
    {
        throw new NotImplementedException();
    }

}