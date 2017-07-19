﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	// Use this for initialization
	public void Awake () {

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

    }




}
