using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class UIPanel_Base : MonoBehaviour
{

    private StringBuilder _strBuilder = new StringBuilder("");

    #region 인터페이스

    protected virtual void init() { }


    public void showPanel()
    {
        this.gameObject.SetActive(true);
    }

    public void hidePanel()
    {
        this.gameObject.SetActive(false);
    }

    #endregion

}

public class SingletonUIPanel<T> : UIPanel_Base where T : SingletonUIPanel<T>
{

    public GameObject m_Canvas;
    public static bool m_isDrag { get; set; }
    public static bool m_isClickDown { get; set; }
    public static bool m_isClickUp { get; set; }

    private static T _instance = null;

    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.Log(" _instance == null ");
            }
            return _instance;
        }
    }
    void Awake()
    {
        _instance = this as T;
        _instance.init();
        //   this.gameObject.name = this.GetType().ToString();
        //  this.gameObject.SetActive(false);
    }
    //public static bool Create()
    //{
    //    if (_instance != null)
    //    {
    //        Debug.Log("Panel Already exists ");
    //        return false;
    //    }

    //    if (_instance == null)
    //    {
    //        Debug.Log("Panel Create is Fail");
    //        return false;
    //    }

    //    _instance.init();

    //    return true;
    //}

}