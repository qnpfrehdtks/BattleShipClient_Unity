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
    PK_ENEMY_ENTER,   // 플레이어가 적을 만날 경우
    PK_PLAYEROBJ_FIRE,   // 플레이어가 목표물을 향해 사격한 경우
    PK_PLAYEROBJ_DAMAGED,   // 데미지 받았다고 패킷 보낼때
    PK_PLAYER_EXIT,  // 플레이어 나갔다고 패킷 보낼떼
    PK_ENEMY_EXIT   // 적 나갔다고 패킷 보낼때


}
public class NetworkManager : Singleton_Manager<NetworkManager>
{

    private           byte[]                            m_tempBuffer;
    private           Socket                            m_sck;
    private           OutputStream                      m_outputStream;
    private           InputStream                       m_inputStream;
    private           Serilaizer                        m_se;
    private           IPEndPoint                        m_localEndPoint;
    private           Dictionary<int, Player>           m_Player;
    private           bool                              m_isOnServer;


    protected override bool Init()
    {
        m_tempBuffer = new byte[90];
        m_Player = new Dictionary<int, Player>();
        m_isOnServer = true;
        m_outputStream = new OutputStream(3000);
        m_inputStream = new InputStream(3000);

        m_se = new Serilaizer();
        m_sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5001);

        return true;
    }

    IEnumerator whileSendPacket(PACKETSTATE state)
    {
        bool isMatchingComplete = false;

        while (!isMatchingComplete)
        {
            switch (state)
            {
                case PACKETSTATE.PK_PLAYER_ENTER:

                    m_outputStream.Serialize(4 * 3);
                    m_outputStream.Serialize((int)PACKETSTATE.PK_PLAYER_ENTER);
                    m_outputStream.Serialize(PlayerManager.Instance.m_PlayerID);

                    sendToServer();

                    recvedFromServer();

                    // 패킷의 사이즈
                    byte[] bytes = new byte[sizeof(int)];
                    m_inputStream.Serialize(bytes, sizeof(int));
                    int PacketSize = BitConverter.ToInt32(bytes, 0);
                    Debug.Log(PacketSize);

                    // 패킷의 Key
                    bytes = new byte[sizeof(int)];
                    m_inputStream.Serialize(bytes, sizeof(int));
                    int PacketKey = BitConverter.ToInt32(bytes, 0);
                    Debug.Log(PacketKey);

              
                    // 서버로 부터 WAIT란 패킷을 받았을 때
                    if ((PACKETSTATE)PacketKey == PACKETSTATE.PK_PLAYER_WAIT)
                    {
                        Debug.Log("Wait for Enemey Player...");
                        m_outputStream.resetStream();
                        m_inputStream.resetStream();

                        yield return new WaitForSeconds(1.0f);
                    }
                    // 서버로 부터 적이 들어왔다는 패킷을 받았을때
                    else if ((PACKETSTATE)PacketKey == PACKETSTATE.PK_ENEMY_ENTER)
                    {
                        // 상대 방 적 정보가 무엇인지.
                        bytes = new byte[sizeof(int)];
                        m_inputStream.Serialize(bytes, sizeof(int));
                        int EnemyPlayerID = BitConverter.ToInt32(bytes, 0);
                        Debug.Log(EnemyPlayerID + "Enemy Come Here Fight!!!");

                        isMatchingComplete = true;
                       PlayerManager.Instance.CreatePlayer(EnemyPlayerID);
                      gameSceneManager.Instance.SceneChange(SCENE.SC_READY);


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
            return true;
        }
        catch
        {
            Debug.Log("Unable to connect to remote end point!\r\n");
        }

        return false;
    }

    public void sendPacketState(PACKETSTATE state)
    {
        switch (state)
        {
            case PACKETSTATE.PK_PLAYER_ENTER:
                StartCoroutine(whileSendPacket(state));
                break;
            case PACKETSTATE.PK_PLAYER_WAIT:




                break;
        }


    }

   public void ResetOutStream()
    {
        m_outputStream.resetStream();
    }

    public void ResetInStream()
    {
        m_outputStream.resetStream();
    }


   

    // 서버에게 outStrem의 버퍼 내부 값을 보내자.
    public void  sendToServer()
    {
        SendBuffer(m_outputStream.GetBuffer());
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
        RecvBuffer(m_inputStream.GetBuffer());
    }

    private int RecvBuffer(byte[] buffer)
    {
        int RecvedByte = 0;
        try
        {
            byte[] tempBuffer = new byte[3000];
            RecvedByte = m_sck.Receive(tempBuffer);
            m_inputStream.SetBuffer(tempBuffer);
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

    public void SendToServerTest(int ID)
    {
        if (m_Player.ContainsKey(ID))
        {
            m_Player[ID].Wirte(m_outputStream);
            sendToServer();
        }
        else
        {
            Debug.Log("I can't Find a Player Key");
        }
    }


    public void RecvedFromServerTest(int ID)
    {
        if (m_Player.ContainsKey(ID))
        {
            recvedFromServer();
            m_Player[ID].Read(m_inputStream);
        }
        else
        {
            Debug.Log("I can't Find a Player Key");
        }
    }





}
