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

public class Player  {

    public int m_Id {get; private set; }
    private List<PlayerObject> m_PlayerObjList;



    public Player(int ID)
    {
        m_Id = ID;
    }


    public void Wirte(OutputStream stream)
    {
        stream.Serialize(BitConverter.GetBytes(sizeof(float) * 4), sizeof(int));

        //stream.Serialize(BitConverter.GetBytes(m_tr.position.x), sizeof(float));
        //stream.Serialize(BitConverter.GetBytes(m_tr.position.y), sizeof(float));
        //stream.Serialize(BitConverter.GetBytes(m_tr.position.z), sizeof(float));

        //stream.Serialize(BitConverter.GetBytes(m_Qut.x), sizeof(float));
        //stream.Serialize(BitConverter.GetBytes(m_Qut.y), sizeof(float));
        //stream.Serialize(BitConverter.GetBytes(m_Qut.z), sizeof(float));
        //stream.Serialize(BitConverter.GetBytes(m_Qut.w), sizeof(float));

        //stream.Wirte((UInt32)header.PkProtocol);

        //stream.Wirte((UInt32)header.PkSize);

        //byte[] Data = Encoding.Default.GetBytes(m_Data);
        //stream.Wirte(Data);
    }


    public void Read(InputStream stream)
    {

        ////byte[] m1 =  stream.Read(Marshal.SizeOf(header.Pkid));
        ////header.Pkid =  BitConverter.ToInt32(m1, 0);

        byte[] m1 = new byte[sizeof(float)];
        stream.Serialize(m1, sizeof(float));
        Debug.Log(BitConverter.ToSingle(m1, 0));

        m1 = new byte[sizeof(float)];
        stream.Serialize(m1, sizeof(float));
        Debug.Log(BitConverter.ToSingle(m1, 0));


        m1 = new byte[sizeof(float)];
        stream.Serialize(m1, sizeof(float));
        Debug.Log(BitConverter.ToSingle(m1, 0));




        //stream.Wirte((UInt32)header.PkProtocol);

        //stream.Wirte((UInt32)header.PkSize);

        //byte[] Data = Encoding.Default.GetBytes(m_Data);
        //stream.Wirte(Data);
    }
}
