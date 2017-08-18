using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	public void Awake () {


        Screen.SetResolution(1280, 720, false);
        this.gameObject.name = "[System]GameManager";
        DontDestroyOnLoad(this.gameObject);

        Init();

	}

    public void Init()
    {
        LoginManager.createManager();
        gameSceneManager.createManager();
        NetworkManager.createManager();
        PlayerManager.createManager();
        BattleManager.createManager();
        SoundManager.createManager();
        EffectController.createManager();

    }




}
