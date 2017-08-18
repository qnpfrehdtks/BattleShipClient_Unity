using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary; // for BinaryFormatter
using System.IO;
using System;
using System.Runtime.InteropServices;




public enum PACKETSTATE
{
    PK_PLAYER_ENTER = 333,
    PK_ENEMY_ENTER,   // 플레이어가 입장하려고 패킷을 보낸 경우
    PK_PLAYER_WAIT,        // 플레이어 보고 기다려라 라고 패킷을 보낼 경우 // 플레이어가 드디더 상대방과 매칭을 마폈을 경우 보내는 경우 ACK임.
    PK_PLAYER_READY,    // 배치 완료.
    PK_ENEMY_READY,
    PK_BATTLE_START,
    PK_MY_TURN,
    PK_OPPO_TURN,

    PK_ENEMY_ATTACKWAIT,
    PK_ENEMY_ATTACKNOWAIT,

    PK_PLAYER_ATTACK,
    PK_PLAYER_ATTACKWAIT,
    PK_PLAYER_DAMAGESUCC,
    PK_PLAYER_DAMAGEFAIL,
    PK_PLAYER_MISSILEDEFNED,
   


    PK_PLAYER_RADER,
    PK_PLAYER_RADERWAIT,
    PK_PLAYER_RADERRESULT,
    PK_PLAYER_SKILLATTACK,
    PK_PLAYER_SKILLRESULT,

    PK_ENEMY_SKILLATTACK,
    PK_ENEMY_ATTACK,
    PK_ENEMY_RADER,
    PK_ENEMY_ATTACKSUCC,
    PK_ENEMY_ATTACKFAIL,

    PK_TURN_CHECK,
    PK_TURN_CHANGE,
    PK_TURN_NOCHANGE,

    PK_PLAYER_DIE,
    PK_ENEMY_DIE,

    PK_PLAYER_EXIT,
    PK_ENEMY_EXIT


}
public class NetworkManager : Singleton_Manager<NetworkManager>
{
    SocketAsyncEventArgs args = new SocketAsyncEventArgs();
    

    public SKILL m_SelectedSkill;
    public            sVector2                           m_AttackPt { get; set; }
    public            List<sVector2>                     m_AttackPtList { get; set; }
  //  public            List<sVector2>                     m_DamagedRangeAttkList { get; set; }
    // public            List<sVector2> m_AttackPtList { get; set; }

    private           int                               m_FindedShip;
    private           bool                              m_isCanFight;
    private           byte[]                            m_tempBuffer;
    private           Socket                            m_sck;

    private           Serilaizer                        m_se;
    private           IPEndPoint                        m_localEndPoint;
    private           Dictionary<int, Player>           m_Player;
    public bool       m_isConnectServer { get; private set; }


    protected override bool Init()
    {
        m_FindedShip = 0;
        m_AttackPtList = new List<sVector2>();
        m_tempBuffer = new byte[6000];
        m_Player = new Dictionary<int, Player>();
        m_isConnectServer = false;

        m_se = new Serilaizer(6000,6000);
        m_sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001);
       // m_sck.ConnectAsync(args);
        return true;
    }


    public void AllClearInfo()
    {
        m_FindedShip = 0;
        m_isConnectServer = false;
        m_Player.Clear();
        m_AttackPtList.Clear();
        DisConnect();
    }

    // 서버에게 지속적으로 패킷을 보내는 함수임. 코루틴을 써서 매 간격 보낸다.
    IEnumerator whileSendPacket(PACKETSTATE state)
    {
        bool isMatchingComplete = false;
        PacketHeader p;

        while (!isMatchingComplete)
        {
            switch (state)
            {
                // 적 플레이어와 매칭이 됬네요...
                case PACKETSTATE.PK_PLAYER_ENTER:
                    p = baseSendAndRecv(state);

                    if (EnemyExitProcess((PACKETSTATE)p.PkKey))
                    {
                        isMatchingComplete = true;
                    }
                    // 서버로 부터 WAIT란 패킷을 받았을 때
                    else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_WAIT)
                    {
                        Debug.Log("Wait for Enemey Player...");
                        m_se.resetIOStream();

                        //1초 간격으로 보냄, Matching 될때까지
                        yield return new WaitForSeconds(1.0f);
                    }
                    // 서버로 부터 적이 들어왔다는 패킷을 받았을때
                    else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_ENTER)
                    {

                        // 상대 방 적 정보가 무엇인지.
                        int EnemyPlayerID =  BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
                        Debug.Log(EnemyPlayerID + "Enemy Come Here Fight!!!");

                        //매칭 완료
                        isMatchingComplete = true;
                        // 적 플레이어 만든다.
                       PlayerManager.Instance.CreatePlayer(EnemyPlayerID);
                        //레디씬으로
                       gameSceneManager.Instance.SceneChange(SCENE.SC_READY);
                        //  sendToServer();
                        m_se.resetIOStream();
                    }
                    break;

                case PACKETSTATE.PK_PLAYER_READY:
                    p = baseSendAndRecv(state);
                    
                     if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_EXIT)
                    {
                        Debug.Log("wait Ready for Enemey Player...");

                        //버퍼 초기화~
                        m_se.resetIOStream();
                        isMatchingComplete = true;
                        //1초 간격으로 보냄, Matching 될때까지
                        gameSceneManager.Instance.SceneChange(SCENE.SC_BATTLE);

                    }
                    // 서버로 부터 적이 들어왔다는 패킷을 받았을때
                    else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_READY)
                    {
                     //   Debug.Log("Gppg");
                        isMatchingComplete = true;
                        int EnemyPlayerID = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);

                        //// 현재 대치중인 적이랑 서버에서 온 적이랑 아이디가 맞는지 체크,, 아니면 다시 패킷 받자.,
                        if (PlayerManager.Instance.getCurPlayer().m_EnemyId != EnemyPlayerID )
                        {
                            Debug.Log("wait Ready for Enemey Player...");

                            //버퍼 초기화~
                            m_se.resetIOStream();

                            //1초 간격으로 보냄, Matching 될때까지
                            yield return new WaitForSeconds(1.0f);
                        }

                        // 버퍼 초기화~
                        m_se.resetIOStream();

                       Debug.Log(EnemyPlayerID + "Enemy Come Here Fight!!!");

                        //매칭 완료
                        m_isCanFight = true;
                        isMatchingComplete = true;
                        //배틀씬으로
                        gameSceneManager.Instance.SceneChange(SCENE.SC_BATTLE);
                        //  sendToServer();
                    }
                    else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_WAIT)
                    {
                        Debug.Log("wait Ready for Enemey Player...");

                        //버퍼 초기화~
                        m_se.resetIOStream();

                        //1초 간격으로 보냄, Matching 될때까지
                        yield return new WaitForSeconds(1.0f);

                    }


                    break;
                case PACKETSTATE.PK_ENEMY_ATTACKWAIT:

                    p = baseSendAndRecv(state);

                    if (EnemyExitProcess((PACKETSTATE)p.PkKey))
                    {
                    //    m_se.resetIOStream();
                        isMatchingComplete = true;
                    }
                    // 서버로 부터 기다리라는 메세지를 받을때
                    else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_ATTACKWAIT)
                    {
                        Debug.Log("wait Ready for Enemey Player...");

                        //버퍼 초기화~
                        m_se.resetIOStream();

                        //1초 간격으로 보냄, 적의 공격이 올때 까지~
                        yield return new WaitForSeconds(1.0f);
                    }
                    // 적의 공격 패킷이 들어왔다!! 나 공격 받음!!! 단일 공격
                    else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_ATTACK)
                    {
                        Debug.Log("Enemy Attack!!!");
                 
                        // 공격 받은 지점을 패킷으로 부터 꺼내자!!
                        sVector2 pos = AddcoordinateFromPacket();
                        Debug.Log(pos.x + ","  + pos.y + " 공격 받음");
                        // 패킷 버퍼 초기화~
                        m_se.resetIOStream();

                        isMatchingComplete = true;

                       sendPacketState(BattleManager.Instance.EnemyAttackToBlock(pos));

                        if (!PlayerManager.Instance.IsShipAllDie())
                            sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                        else
                        {
                            BattleManager.Instance.BattleResult(PLAYER.OPPONENT);
                            sendPacketState(PACKETSTATE.PK_PLAYER_DIE);
                        }
                    }
                    else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_SKILLATTACK)
                    {
                        int AttackedSize = (p.PkSize - 16) / 8;
                        sVector2[] blocks = new sVector2[AttackedSize];

                        SKILL skilltype = (SKILL)BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);


                        for (int i = 0; i < AttackedSize; i++)
                        {
                            // 레이더 체크 받은 지점을 패킷으로 부터 꺼내자!!
                            blocks[i] = AddcoordinateFromPacket();
                            Debug.Log(blocks[i].x + "," + blocks[i].y + " 공격 받음");
                        }


                        m_se.resetIOStream();

                        m_AttackPtList.Clear();
                        isMatchingComplete = true;

                        RecvedSkillTypeProcess(skilltype, blocks);
                        ////m_FindedShip = PlayerManager.Instance.getCurPlayer().ShipRaderCount(blocks);
                        ////sendPacketState(PACKETSTATE.PK_PLAYER_RADERRESULT);
                        ////sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                    }
                    break;
            }

            // yield return new WaitForSeconds(1.0f);

        }

    }

    public void sendPacketState(PACKETSTATE state)
    {
        switch (state)
        {
            case PACKETSTATE.PK_PLAYER_ENTER:
                StartCoroutine(whileSendPacket(state));
                break;
            case PACKETSTATE.PK_PLAYER_READY:
                StartCoroutine(whileSendPacket(state));
                break;
            case PACKETSTATE.PK_BATTLE_START:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_PLAYER_ATTACK:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_ENEMY_ATTACKSUCC:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_ENEMY_ATTACKFAIL:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_ENEMY_ATTACKWAIT:
                StartCoroutine(whileSendPacket(state));
                break;
            case PACKETSTATE.PK_TURN_CHECK:
                StartCoroutine(CheckTurnFromServer());
                break;
            case PACKETSTATE.PK_PLAYER_RADERRESULT:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_PLAYER_RADER:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_PLAYER_SKILLATTACK:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_PLAYER_SKILLRESULT:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_PLAYER_DIE:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_PLAYER_EXIT:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_ENEMY_EXIT:
                OnceSendandRecv(state);
                break;
            case PACKETSTATE.PK_PLAYER_MISSILEDEFNED:
                OnceSendandRecv(state);
                break;

        }


    }

    public void OnceSendandRecv(PACKETSTATE state)
    {
        PacketHeader p;

        switch (state)
        {
            case PACKETSTATE.PK_BATTLE_START:

                p = baseSendAndRecv(state);
                // 서버로 부터 내턴이란 패킷을 받았을 때
                if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_EXIT)
                {
                    BattleManager.Instance.GameStart(PLAYER.MINE);
                    StartCoroutine(ForcedVictoryForTime(1.0f));
                    m_se.resetIOStream();

                    
                    return;
                }
                else if (p.PkKey == (int)PACKETSTATE.PK_MY_TURN)
                {
                    //버퍼 초기화~
                    m_se.resetIOStream();

                    BattleManager.Instance.GameStart(PLAYER.MINE);
                    //1초 간격으로 보냄, Matching 될때까지
                }
                else
                {

                    BattleManager.Instance.GameStart(PLAYER.OPPONENT);
                    m_se.resetIOStream();
                    sendPacketState(PACKETSTATE.PK_ENEMY_ATTACKWAIT);

                }
                break;

            case PACKETSTATE.PK_PLAYER_ATTACK:

                p = AttackPtSendAndRecv(state);
                if (EnemyExitProcess((PACKETSTATE)p.PkKey))
                {
                    return;
                }
                // 서버로 부터 적의 공격이 명중했다는 소식을 들음!!!
                else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_DAMAGESUCC)
                {
                    m_se.resetIOStream();

                    // 타일좀 정리좀 하고
                    BattleManager.Instance.PlayerAttackToEnemyBlock(m_AttackPt, PACKETSTATE.PK_PLAYER_DAMAGESUCC);

                    // 다음 턴이 누가 될지 서버에서 받자.  (턴의 모든 결정은 서버가 내린다~ 클라는 힘이 없음...)
                    sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                }
                // 서버로부터 명중을 못했다고 들음 ㅠ.
                else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_DAMAGEFAIL)
                {
                    m_se.resetIOStream();
                    BattleManager.Instance.PlayerAttackToEnemyBlock(m_AttackPt, PACKETSTATE.PK_PLAYER_DAMAGEFAIL);

                    // 다음 턴 누가 될지 서버한테 받읍시다.
                    sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                }
                else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_MISSILEDEFNED)
                {
                    m_se.resetIOStream();
                    BattleManager.Instance.EnemyMissileDefend();

                    // 다음 턴 누가 될지 서버한테 받읍시다.
                    sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                }
                else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_DIE)
                {
                    BattleManager.Instance.BattleResult(PLAYER.MINE);
                    m_se.resetIOStream();
                }
                // 서버가 아직 적이 공격 받을 준비를 못해다고 기다리레요.
                // 근데 이거 받을 일 거의 없을듯.
                else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_ATTACKWAIT)
                {
                    m_se.resetIOStream();
                    //버퍼 초기화~

                    //1초 간격으로 보냄, 될때까지
                    //  yield return new WaitForSeconds(1.0f);
                }

                break;
            //레이더로 적의 배가 몇기 있는지 파악.

            case PACKETSTATE.PK_PLAYER_SKILLATTACK:

                p = AttackPtListSendAndRecv(state);

                // 받고 난 뒤 처리
                if (EnemyExitProcess((PACKETSTATE)p.PkKey))
                {
                    return;
                }
               else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_SKILLRESULT)
                {
                    int Blocksize = (p.PkSize - 12) / 8;
                    sVector2[] DamagedPoses = new sVector2[Blocksize];

                    for (int i = 0; i < Blocksize; i++)
                    {
                        DamagedPoses[i] = AddcoordinateFromPacket();
                    }

                    if (m_SelectedSkill != SKILL.DEFEND)
                        BattleManager.Instance.PlayerAttackToEnemyBlocks(DamagedPoses, m_AttackPtList, m_SelectedSkill);
                    else
                        UIPanel_Battle.instance.DefendFailResult();

                    m_AttackPtList.Clear();
                    m_se.resetIOStream();

                    sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                }
                else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_RADERRESULT)
                {
                    int count = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
                    m_AttackPtList.Clear();
                    m_se.resetIOStream();

                    UIPanel_Battle.instance.RaderResult(count);

                    sendPacketState(PACKETSTATE.PK_TURN_CHECK);

                    //1초 간격으로 보냄, 될때까지
                    //  yield return new WaitForSeconds(1.0f);
                }
                else if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_MISSILEDEFNED)
                {
                    m_se.resetIOStream();
                    BattleManager.Instance.EnemyMissileDefend();

                    // 다음 턴 누가 될지 서버한테 받읍시다.
                    sendPacketState(PACKETSTATE.PK_TURN_CHECK);

                    //1초 간격으로 보냄, 될때까지
                    //  yield return new WaitForSeconds(1.0f);
                }
                else if ( p.PkKey == (int)PACKETSTATE.PK_ENEMY_DIE )
                {
                    BattleManager.Instance.BattleResult(PLAYER.MINE);
                    m_se.resetIOStream();
                }

                break;
            // 맞춘거만 보넴
            case PACKETSTATE.PK_PLAYER_SKILLRESULT:

                createPacketFrame(4 * (3 + (m_AttackPtList.Count * 2)), state, PlayerManager.Instance.m_PlayerID);
                Debug.Log("사이즈다 " + 4 * (3 + (m_AttackPtList.Count * 2)));

                // 그리고 스킬의 좌표도
                for (int i = 0; i < m_AttackPtList.Count; i++)
                {
                    Debug.Log("보낼려는 패킷의 양" + m_AttackPtList[i].x + " 와 " + m_AttackPtList[i].y);
                    AddcoordinateToPacket(m_AttackPtList[i]);
                }

                //보낸다 요청을 서버에게
                sendToServer();

                m_se.resetIOStream();

                
                m_AttackPtList.Clear();

                break;

            case PACKETSTATE.PK_ENEMY_ATTACKSUCC:
                justSend(state);
                m_se.resetIOStream();
                //턴을 그대로 이어갈 준비를 한다.
                break;
            case PACKETSTATE.PK_ENEMY_ATTACKFAIL:
                justSend(state);
                m_se.resetIOStream();

                // 턴을 바꿀 준비
                break;
            case PACKETSTATE.PK_PLAYER_MISSILEDEFNED:
                justSend(state);
                m_se.resetIOStream();
                // 턴을 바꿀 준비
                break;
            case PACKETSTATE.PK_PLAYER_DIE:
                justSend(state);
                m_se.resetIOStream();
                break;

            case PACKETSTATE.PK_PLAYER_EXIT:
                justSend(state);
                m_se.resetIOStream();
                break;

            case PACKETSTATE.PK_TURN_CHECK:
                p = baseSendAndRecv(state);

                if (EnemyExitProcess((PACKETSTATE)p.PkKey))
                {
                    return;
                }
                // 서버로 부터 내턴이란 패킷을 받았을 때
                else if (p.PkKey == (int)PACKETSTATE.PK_MY_TURN)
                {
                    //버퍼 초기화~
                    m_se.resetIOStream();
                    BattleManager.Instance.TurnChange(PACKETSTATE.PK_MY_TURN);

                }
                else if (p.PkKey == (int)PACKETSTATE.PK_OPPO_TURN)
                {
                    m_se.resetIOStream();
                    BattleManager.Instance.TurnChange(PACKETSTATE.PK_OPPO_TURN);
                    sendPacketState(PACKETSTATE.PK_ENEMY_ATTACKWAIT);
                    //
                }
                else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_DIE)
                {
                    BattleManager.Instance.BattleResult(PLAYER.MINE);
                    m_se.resetIOStream();
                }
                break;

            case PACKETSTATE.PK_PLAYER_RADERRESULT:

                createPacketFrame(4 * 4, state, PlayerManager.Instance.m_PlayerID);
                m_se.outSerialize(m_FindedShip);
                //보낸다 요청을 서버에게
                sendToServer();

                m_se.resetIOStream();

                //  BattleManager.Instance.EnemyRaderToBlock()
             //   BattleManager.Instance.EnemyRaderToBlock();
             

                // 레이더 결과 보냄.
                break;
        }
    }

    public bool Connect()
    {
        try
        {
            Debug.Log("Connect Succ\r\n");
            m_sck.Connect(m_localEndPoint);
            m_isConnectServer = true;
            return true;
        }
        catch
        {
            Debug.Log("Unable to connect to remote end point!\r\n");
        }
        m_isConnectServer = false;
        return false;
    }

    public bool DisConnect()
    {
        if (m_sck.Connected)
        {
            try
            {
                Debug.Log("DisConnect Succ\r\n");
                m_sck.Disconnect(true);
                m_isConnectServer = false;
                return true;
            }
            catch
            {
                Debug.Log("Unable to connect to remote end point!\r\n");
            }
            m_isConnectServer = false;
        }
        return false;
    }

    // 서버에게 outStrem의 버퍼 내부 값을 보내자.
    public void  sendToServer()
    {
        SendBuffer(m_se.GetOutBuffer());
    }


    private int SendBuffer(byte[] buffer)
    {
        int sendedByte = 0;
        try
        {
            Debug.Log("서버가 받기 까지 대기... Blocking");
            sendedByte = m_sck.Send(buffer);
            Debug.Log(" 보낸 데이더 양 " + sendedByte);
        }
        catch (SocketException e)
        {
            Debug.Log(" ErrorCode " + e.ErrorCode);
            return (e.ErrorCode);
        }


        return sendedByte;
    }


    public void recvedFromServer()
    {
        RecvBuffer(m_se.GetInBuffer());
    }

    private int RecvBuffer(byte[] buffer)
    {
        int RecvedByte = 0;
        try
        {
            byte[] tempBuffer = new byte[6000];
            RecvedByte = m_sck.Receive(tempBuffer);
            m_se.SetInBuffer(tempBuffer);
            Debug.Log(" 받은 데이더 양 " + RecvedByte);
        }
        catch (SocketException e)
        {
            Debug.Log(" ErrorCode " + e.ErrorCode);
            return (e.ErrorCode);
        }

        return RecvedByte;
    }

    public void addPlayer(Player po)
    {
        if (!m_Player.ContainsKey(po.m_Id))
        {
            m_Player.Add(po.m_Id, po);
        }
        else
        {
            Debug.Log("already owns a ID!!!");
        }
    }


    private void createPacketFrame(int Size, PACKETSTATE packetState, int PlayerID)
    {
        m_se.outSerialize(Size);
        m_se.outSerialize((int)packetState);
        m_se.outSerialize(PlayerID);
    }

    private void AddcoordinateToPacket(sVector2 pos)
    {
        m_se.outSerialize(pos.x);
        m_se.outSerialize(pos.y);
    }

    public sVector2 AddcoordinateFromPacket()
    {
        sVector2 pos;
        pos.x  = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
        pos.y =  BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
        return pos;
    }

    public PacketHeader baseSendAndRecv(PACKETSTATE state)
    {
        PacketHeader p;
        //우선 서버에게 보낼 패킷 정보를 담자.
        createPacketFrame(4 * 3, state, PlayerManager.Instance.m_PlayerID);
        //보낸다 요청을 서버에게
        sendToServer();
        //받는다 서버의 응답을
        recvedFromServer();
        //서버에게 받은 패킷을 Deserialize하여 무슨 패킷인지 알아낸다.
        p = classifyPacket();

        return p;
    }


    public PacketHeader AttackPtSendAndRecv(PACKETSTATE state)
    {
        PacketHeader p;
        //우선 서버에게 보낼 패킷 정보를 담자.
        createPacketFrame(4 * 5, state, PlayerManager.Instance.m_PlayerID);

        AddcoordinateToPacket(m_AttackPt);

        //보낸다 요청을 서버에게
        sendToServer();
        //받는다 서버의 응답을
        recvedFromServer();
        //서버에게 받은 패킷을 Deserialize하여 무슨 패킷인지 알아낸다.
        p = classifyPacket();

        return p;
    }


    public PacketHeader AttackPtListSendAndRecv(PACKETSTATE state)
    {
        PacketHeader p;
        //우선 서버에게 보낼 패킷 정보를 담자.
        createPacketFrame(4 *  (4 + (m_AttackPtList.Count *2)), state, PlayerManager.Instance.m_PlayerID);

        m_se.outSerialize((int)m_SelectedSkill);

        for (int i=0; i < m_AttackPtList.Count; i++)
        AddcoordinateToPacket(m_AttackPtList[i]);

        //보낸다 요청을 서버에게
        sendToServer();
        //받는다 서버의 응답을
        recvedFromServer();
        //서버에게 받은 패킷을 Deserialize하여 무슨 패킷인지 알아낸다.
        p = classifyPacket();

        return p;
    }

    public void justSend(PACKETSTATE state)
    {
        //우선 서버에게 보낼 패킷 정보를 담자.
        createPacketFrame(4 * 3, state, PlayerManager.Instance.m_PlayerID);
        //보낸다 요청을 서버에게
        sendToServer();
    }

    IEnumerator CheckTurnFromServer()
    {
        yield return new WaitForSeconds(4.0f);

        OnceSendandRecv(PACKETSTATE.PK_TURN_CHECK);
    }

    public void AddAttackPt(sVector2 pos)
    {

        m_AttackPtList.Add(pos);
    }

    private PacketHeader classifyPacket()
    {
        PacketHeader p;
        p.PkSize = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
        p.PkKey = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
        p.PkPlayerID = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);

        return p;
    }
    // 받은 스킬을 어떻게 처리할 것인가를 나타내는 함수. :D
     void RecvedSkillTypeProcess(SKILL recvSkill, sVector2[] blocks)
    {
        switch (recvSkill)
        {
            case SKILL.RADER:
                 m_FindedShip = PlayerManager.Instance.getCurPlayer().ShipRaderCount(blocks);

                BattleManager.Instance.RaderResult(m_FindedShip);
                sendPacketState(PACKETSTATE.PK_PLAYER_RADERRESULT);
                sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                
                break;
            case SKILL.SUPER_BOMB:
                // 적의 공격 성공하면 true, 막히면 false
                BattleManager.Instance.EnemyAttackToBlocks(blocks, recvSkill);
            
                if (!PlayerManager.Instance.IsShipAllDie())
                    sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                else
                {
                    sendPacketState(PACKETSTATE.PK_PLAYER_DIE);
                    BattleManager.Instance.BattleResult(PLAYER.OPPONENT);
                }
                break;
            case SKILL.DEFEND:
               
                // 적의 공격 성공하면 true, 막히면 false
                BattleManager.Instance.RecvDefendSkill(blocks, recvSkill);
                sendPacketState(PACKETSTATE.PK_TURN_CHECK);
                break;
        }

    }


    bool EnemyExitProcess(PACKETSTATE state)
    {
       
        if (state == PACKETSTATE.PK_ENEMY_EXIT)
        {
            BattleManager.Instance.BattleResult(PLAYER.MINE);
            m_se.resetIOStream();
            return true;
        }

        return false;
    }

     private IEnumerator ForcedVictoryForTime(float second)
    {
        yield return new WaitForSeconds(second);
        BattleManager.Instance.BattleResult(PLAYER.MINE);


    } 



}
