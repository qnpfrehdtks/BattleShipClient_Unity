using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PLAYER
{
    MINE,
    OPPONENT
}




public class PlayerManager : Singleton_Manager<PlayerManager>
{

    private PLAYER m_CurTurn;

    private Dictionary<int,Player> m_PlayerTable;

    private GameObject m_PlayerCaptain;
    private GameObject m_EnemyCaptain;

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
        m_PlayerEnemyID = -1;
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
        else if (m_PlayerNum > 0 && m_PlayerEnemyID == -1)
        {
            m_PlayerEnemyID = player.m_Id;
            getCurPlayer().m_EnemyId = m_PlayerEnemyID;
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

    public void AddShipToPlayer(Base_Ship ship, bool isCurPlayer)
    {
        int playerID;

        if (isCurPlayer)
        {
            playerID = m_PlayerID;
        }
        else playerID = m_PlayerEnemyID;

        if (playerID != -1)
        {
            m_PlayerTable[playerID].AddShip(ship);
         }
        else
        {
            Debug.Log("Um Not have a PlayerID....");
        }
    }

    public void DeleteShipFromPlayer(Base_Ship ship, bool isCurPlayer)
    {

        int playerID;

        if (isCurPlayer)
        {
            playerID = m_PlayerID;
        }
        else playerID = m_PlayerEnemyID;

        if (playerID != -1)
        {
            m_PlayerTable[playerID].DeleteShip(ship.m_shipKind);
        }
        else
        {
            Debug.Log("Not have a PlayerID....");
        }
    }


    public int CheckDispatchedShipCount(bool isCurPlayer)
    {
        if (isCurPlayer)
            return m_PlayerTable[m_PlayerID].ShipCount();
        else return m_PlayerTable[m_PlayerEnemyID].ShipCount();
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

    public void CreatePlayerCaptain()
    {
        m_PlayerCaptain = new GameObject("[Player]", typeof(Player));
        Player player = m_PlayerCaptain.GetComponent<Player>();

        m_PlayerCaptain.transform.position = new Vector3(0, 0, 0);
        player = m_PlayerTable[m_PlayerID];
    }





    public PLAYER PlayerNextTurn()
    {
        if (m_CurTurn == PLAYER.MINE )
        {
            getCurPlayer().m_isYourTurn = false;
            getEnemyPlayer().m_isYourTurn = true;

            // 서버에 보낸다. 이 사실을 턴을 넘긴 사실을....
            ///NetWorkManager
            // 서버에 

            // Camera Move 필요함.
            m_CurTurn = PLAYER.OPPONENT;
        }
        else
        {
            getCurPlayer().m_isYourTurn = true;
            getEnemyPlayer().m_isYourTurn = false;

            // 서버에 턴이 바뀐 사실을 알린다.

            //Camera Move 필요.

            m_CurTurn = PLAYER.MINE;
        }

        return m_CurTurn;
    }

    public void SettingStartTurn(PLAYER FirstPlayer)
    {
        if(FirstPlayer == PLAYER.MINE)
        {
            getCurPlayer().m_isYourTurn = true;
            getEnemyPlayer().m_isYourTurn = false;
        }
        else if (FirstPlayer == PLAYER.OPPONENT)
        {
            getCurPlayer().m_isYourTurn = false;
            getEnemyPlayer().m_isYourTurn = true;
        }

        m_CurTurn = FirstPlayer;
    }


    public void CheckShipPos()
    {
       for(SHIP i=0; i < SHIP.WAR_SHIP; i++)
        {
            getCurPlayer().m_InstalledShipMap[i].BattleCheckBlock();
        }

    }

    public void CheckShipDamaged(sVector2 pt)
    {
        for (SHIP i = 0; i < SHIP.WAR_SHIP; i++)
        {
            if (getCurPlayer().m_InstalledShipMap[i].DamagedBattleChest(pt))
            {
                break;
            }

        }

    }

}
