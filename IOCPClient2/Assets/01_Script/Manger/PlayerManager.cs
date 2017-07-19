using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton_Manager<PlayerManager>
{
    private Dictionary<int,Player> m_PlayerTable;



    public bool m_isEnemyPlayerConnected { get; private set; }
    public bool m_isPlayerConnected { get; private set; }

    public int m_PlayerNum { get; private set; }
    public int m_PlayerID { get; private set; }
    public int m_PlayerEnemyID { get; private set; }

    protected override bool Init()
    {
        m_isPlayerConnected = false;
        m_isEnemyPlayerConnected = false;
        m_PlayerNum = 0;
        m_PlayerID = -1;
        m_PlayerEnemyID = 1;
        m_PlayerTable = new Dictionary<int, Player>();

        return true;

    }

    // 플레이어를 만들어 준다~
    public void CreatePlayer()
    {
        int PlayerID = UnityEngine.Random.Range(0, 1000);

        if (m_PlayerTable.ContainsKey(PlayerID))
        {
            CreatePlayer();
            return;
        }
        Player newPlayer = new Player(PlayerID);
        AddPlayer(newPlayer);
       
    }

    // 플레이어를 만들어 준다~
    public void CreatePlayer(int playerID)
    {
        if (m_PlayerTable.ContainsKey(playerID))
        {
            CreatePlayer();
            return;
        }
        Player newPlayer = new Player(playerID);
        AddPlayer(newPlayer);

    }


    // 플레이어를 내 클라 Map 컨테이너에 보관하자.
    private void AddPlayer(Player player)
    {
        // 클라이언트 플레이어 입장시 서버에게  들어왔다고 통지
        if (m_PlayerNum == 0 && m_PlayerID == -1)
        {
            m_PlayerID = player.m_Id;
            m_isPlayerConnected = true;

            NetworkManager.Instance.sendPacketState(PACKETSTATE.PK_PLAYER_ENTER);
          
            Debug.Log("Player Add and Player Enter Send Succ");
        }
        //적 플레이어 입장시 처리.
        else if (m_PlayerNum > 0 && m_PlayerEnemyID != -1)
        {
            m_PlayerEnemyID = player.m_Id;
            m_isEnemyPlayerConnected = true;

            Debug.Log(" Enemy Player Add and Player Enter Send Succ");
        }
        // 그외 예외
        else
        {
            Debug.Log("WTF??!!");
            return;
        }

        m_PlayerNum++;
        m_PlayerTable.Add(player.m_Id, player);
    }

    public Player getCurPlayer()
    {
        return m_PlayerTable[m_PlayerID];
    }

    public Player getEnemyPlayer()
    {
        return m_PlayerTable[m_PlayerEnemyID];
    }

    public void WritePlayerInfo(OutputStream os)
    {
        os.Serialize(m_PlayerID);
    }

    public void ReadPlayerInfo(InputStream IS)
    {
        IS.Serialize(66);
    }




}
