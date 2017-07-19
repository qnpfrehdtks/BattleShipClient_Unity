using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton_Manager<T> : MonoBehaviour where T : Singleton_Manager<T> {

    private static T instance  = null;
    private static bool isInitiated = false;


    protected abstract bool Init(); 

    public static T Instance
    {
        get
         {
           if(isInitiated)
            {
                return instance;
            }
            return null;
        }
    }




    public static bool createManager()
    {
        if(isInitiated)
        {
            Debug.Log("already exist Manager");
            return false;
        }

        instance = new GameObject("[Manager]" + typeof(T).ToString(), typeof(T)).GetComponent<T>();

        isInitiated = true;
        instance.Init();
        DontDestroyOnLoad(instance.gameObject);

        return true;

    }
    
       


}
