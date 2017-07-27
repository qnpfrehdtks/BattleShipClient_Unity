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
    PK_PLAYER_ENTER = 333,  // 플레이어가 입장하려고 패킷을 보낸 경우
    PK_PLAYER_WAIT,        // 플레이어 보고 기다려라 라고 패킷을 보낼 경우
    PK_PLAYER_WAITCOMPLETE_ACK,  // 플레이어가 드디더 상대방과 매칭을 마폈을 경우 보내는 경우 ACK임.
    PK_PLAYER_ENEMYENTER_WAIT,
    PK_PLAYER_READY,    // 배치 완료.
    PK_ENEMY_READY,
    PK_ENEMY_ENTER,   // 플레이어가 적을 만날 경우
    PK_PLAYEROBJ_FIRE,   // 플레이어가 목표물을 향해 사격한 경우
    PK_PLAYEROBJ_DAMAGED,   // 데미지 받았다고 패킷 보낼때
    PK_PLAYER_EXIT,  // 플레이어 나갔다고 패킷 보낼떼
    PK_ENEMY_EXIT   // 적 나갔다고 패킷 보낼때


}
public class NetworkManager : Singleton_Manager<NetworkManager>
{
    private           bool                              m_isCanFight;
    private           byte[]                            m_tempBuffer;
    private           Socket                            m_sck;

    private           Serilaizer                        m_se;
    private           IPEndPoint                        m_localEndPoint;
    private           Dictionary<int, Player>           m_Player;
    public bool       m_isConnectServer { get; private set; }


    protected override bool Init()
    {
        m_tempBuffer = new byte[90];
        m_Player = new Dictionary<int, Player>();
        m_isConnectServer = false;

        m_se = new Serilaizer(3000,3000);
        m_sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001);

        return true;
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
                    //우선 서버에게 보낼 패킷 정보를 담자.
                    createPacketFrame(4 * 3, PACKETSTATE.PK_PLAYER_ENTER, PlayerManager.Instance.m_PlayerID);
                    //보낸다 요청을 서버에게
                    sendToServer();
                    //받는다 서버의 응답을
                    recvedFromServer();

                    //서버에게 받은 패킷을 Deserialize하여 무슨 패킷인지 알아낸다.
                   p = classifyPacket();

                    // 서버로 부터 WAIT란 패킷을 받았을 때
                    if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_WAIT)
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

                    }
                    break;

                case PACKETSTATE.PK_PLAYER_READY:
                    //우선 서버에게 보낼 패킷 정보를 담자.
                    createPacketFrame(4 * 3, PACKETSTATE.PK_PLAYER_ENTER, PlayerManager.Instance.m_PlayerID);
                    //보낸다 요청을 서버에게
                    sendToServer();
                    //받는다 서버의 응답을
                    recvedFromServer();
                    //서버에게 받은 패킷을 Deserialize하여 무슨 패킷인지 알아낸다.
                    p = classifyPacket();


                    // 서버로 부터 WAIT란 패킷을 받았을 때
                    if (p.PkKey == (int)PACKETSTATE.PK_PLAYER_WAIT)
                    {
                        Debug.Log("wait Ready for Enemey Player...");

                        //버퍼 초기화~
                        m_se.resetIOStream();

                        //1초 간격으로 보냄, Matching 될때까지
                        yield return new WaitForSeconds(1.0f);
                    }
                    // 서버로 부터 적이 들어왔다는 패킷을 받았을때
                    else if (p.PkKey == (int)PACKETSTATE.PK_ENEMY_READY)
                    {
                        
                        int EnemyPlayerID = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);

                        // 현재 대치중인 적이랑 서버에서 온 적이랑 아이디가 맞는지 체크,, 아니면 다시 패킷 받자.,
                        if (PlayerManager.Instance.getCurPlayer().m_EnemyId != EnemyPlayerID )
                        {
                            Debug.Log("wait Ready for Enemey Player...");

                            //버퍼 초기화~
                            m_se.resetIOStream();

                            //1초 간격으로 보냄, Matching 될때까지
                            yield return new WaitForSeconds(1.0f);
                        }

                        Debug.Log(EnemyPlayerID + "Enemy Come Here Fight!!!");

                        //매칭 완료
                        m_isCanFight = true;
                  
                        //배틀씬으로
                        gameSceneManager.Instance.SceneChange(SCENE.SC_BATTLE);
                        //  sendToServer();

                    }


                    break;

            }

           // yield return new WaitForSeconds(1.0f);

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
        }


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
            byte[] tempBuffer = new byte[3000];
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
        else {
            Debug.Log("already owns a ID!!!");
        }
    }


    private void createPacketFrame(int Size, PACKETSTATE packetState, int PlayerID)
    {
        m_se.outSerialize(Size);
        m_se.outSerialize((int)packetState);
        m_se.outSerialize(PlayerID);
    }

    private PacketHeader classifyPacket()
    {
        PacketHeader p;
        p.PkSize = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
        p.PkKey = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);
        p.PkPlayerID = BitConverter.ToInt32(m_se.inSerialize(typeof(int)), 0);

        return p;
    }




    //public void SendToServerTest(int ID)
    //{
    //    if (m_Player.ContainsKey(ID))
    //    {
    //        m_Player[ID].Wirte(m_outputStream);
    //        sendToServer();
    //    }
    //    else
    //    {
    //        Debug.Log("I can't Find a Player Key");
    //    }
    //}


    //public void RecvedFromServerTest(int ID)
    //{
    //    if (m_Player.ContainsKey(ID))
    //    {
    //        recvedFromServer();
    //        m_Player[ID].Read(m_inputStream);
    //    }
    //    else
    //    {
    //        Debug.Log("I can't Find a Player Key");
    //    }
    //}





}
