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


enum PACKET_PROTOCOL
{
    PK_TEST_REQUEST = 0,
    PK_LOGIN_REQUEST
}

[Serializable]
[StructLayout(LayoutKind.Sequential)]
struct PacketHeader
{
    public int PkSize;
    public PACKETSTATE PkKey;
    public int PkPlayerID;

}

    class Serilaizer
    {

        private OutputStream m_outputStream;
        private InputStream m_inputStream;


    public Serilaizer(int OutSize, int InputSize)
    {
        m_outputStream = new OutputStream(InputSize);
        m_inputStream = new InputStream(OutSize);
    }


    public void outSerialize<T>(T data)
    {
        m_outputStream.Serialize(data);
    }

    public byte[] inSerialize(Type t)
    {
        int Typesize = Marshal.SizeOf(t);
        byte[] bytes = new byte[Typesize];
        m_inputStream.Serialize(bytes, Typesize);

        return bytes;
      
    }

    public void resetIOStream()
    {
        m_outputStream.resetStream();
        m_inputStream.resetStream();
    }

    public byte[] GetInBuffer()
    {
        return m_inputStream.GetBuffer();
    }

    public byte[] GetOutBuffer()
    {
        return m_outputStream.GetBuffer();
    }

    public void SetOutBuffer(byte[] buffer)
    {
        m_outputStream.SetBuffer(buffer);
    }
    public void SetInBuffer(byte[] buffer)
    {
        m_inputStream.SetBuffer(buffer);
    }




}







[Serializable]
[StructLayout(LayoutKind.Sequential)]
class Packet
{
    
    public PacketHeader header;
    public string m_Data;
    //  [MarshalAs(UnmanagedType.I4)]
    public void Wirte(OutputStream stream)
    {
        //stream.Wirte((UInt32)header.Pkid);

        //stream.Wirte((UInt32)header.PkProtocol);

        //stream.Wirte((UInt32)header.PkSize);

        //byte[] Data = Encoding.Default.GetBytes(m_Data);
        //stream.Wirte(Data);
    }


    public void Read(InputStream stream)
    {
       
       ////byte[] m1 =  stream.Read(Marshal.SizeOf(header.Pkid));
       ////header.Pkid =  BitConverter.ToInt32(m1, 0);
       //// Debug.Log(" 1st " + header.Pkid);

       //// byte[] m2 = stream.Read(Marshal.SizeOf(header.PkProtocol));
       //// header.PkProtocol = BitConverter.ToInt32(m2, 0);
       //// Debug.Log(" 2nd " + header.PkProtocol);

       //// byte[] m3 = stream.Read(Marshal.SizeOf(header.PkSize));
       //// header.PkSize = BitConverter.ToInt32(m3, 0);
       //// Debug.Log(" 3rd " + header.PkSize);


    }




}
public abstract class  MemoryIOStream
{
    protected MemoryStream m_ms;
    protected int m_Capacity;
    protected int m_Head;
    protected bool m_isInput;

    public MemoryStream GetStream()
    {
        return m_ms;
    }

    public byte[] GetBuffer()
    {
        return m_ms.GetBuffer();
    }

    public void SetBuffer(byte[] buff)
    {
        m_ms.Write(buff, 0, buff.Length);
    }

    public bool isInput()        { return m_isInput; }



    public virtual void Serialize<T>(T data)
    {
        if (!m_isInput)
        {
            byte[] tempBuffer = new byte[Marshal.SizeOf(data)];

            if (typeof(T) == typeof(int))
            {
                tempBuffer = BitConverter.GetBytes(Convert.ToInt32(data));
            }
            else if (typeof(T) == typeof(float))
            {
                tempBuffer = BitConverter.GetBytes(Convert.ToSingle(data));
            }
            else if (typeof(T) == typeof(string))
            {
                tempBuffer = Encoding.UTF8.GetBytes(data.ToString());

            }
            else if (typeof(T) == typeof(bool))
            {
                tempBuffer = BitConverter.GetBytes(Convert.ToBoolean(data));
            }


            Serialize(tempBuffer, tempBuffer.Length);
        }
    }


    public abstract void Serialize(byte[] data, int size);


    public void resetStream()
    {
      // m_ms.Capacity = 0;
        m_ms.Position = 0;
      m_ms.SetLength(0);
        m_Head = 0;
        m_Capacity = 0;
    }


}




public class OutputStream : MemoryIOStream
{

    public OutputStream(int capacity)
    {
        m_ms = new MemoryStream();
        m_Capacity = capacity;
        m_ms.Capacity = capacity;
        m_ms.Position = 0;
        m_isInput = false;
    }


    public override void Serialize(byte[] data, int size)
    {
        if(m_Capacity > m_Head + size)
        {
            m_ms.Capacity = m_Head + size;
        }

        m_ms.Position = m_Head;
        m_ms.Write(data, 0, size);
        m_Head += size; 
    }

}

public class InputStream : MemoryIOStream
{

    public InputStream(int capacity)
    {
        m_ms = new MemoryStream();
        m_ms.Position = 0;
        m_Capacity = capacity;
        m_Head = 0;
        m_isInput = true;
    }

    public override void Serialize(byte[] data, int size)
    {
      //  byte[] buffer = new byte[size];
        int resultSize = m_Head + size;
         m_ms.Position = m_Head;
        m_ms.Read(data, 0, size);

     //   Debug.Log("즉석 결과 " + BitConverter.ToInt32(data, 0));
       
        if (m_Head + size < m_Capacity )
        {
            m_Capacity = resultSize;
        }

        m_Head = resultSize;

    //    Debug.Log(" 받은 것 " + m_Head);
    }
}


