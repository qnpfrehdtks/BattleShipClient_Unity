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


// 테스트용 임.  여기서 여러가지 실험 과정 거침.

[Serializable]
[StructLayout(LayoutKind.Sequential)]
public class dd
{
    public int ds = 243;
    public char dsd = 'g';
}



public class IOCPTest : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Socket sck;

        OutputStream outputStream = new OutputStream(3000);
       
       Serilaizer se = new Serilaizer(1000,1000);

        sck = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000);

        try
        {
        //    Debug.Log("Unable to connect to remote end point!\r\n");
            sck.Connect(localEndPoint);
        }
        catch
        {
            Debug.Log("Unable to connect to remote end point!\r\n");
        }


            byte[] buffer = new byte[3072];
            Packet d = new Packet();

           
            //d.header.PkProtocol = 9;
            //d.header.PkSize = 5;
            //d.m_Data = "DDDDDDDDDSADSADSADSADSADSDSADSADSADSAAD";
            //d.header.Pkid = 51;
        
        //    d.Wirte(outputStream);
         

          //  sck.Send(se.Serialize(d));


        byte[] tempBuffer = new byte[1024];
     //   InputStream inputStream = new InputStream(3000);

        int RecvSize = sck.Receive(tempBuffer);
     

        if (RecvSize >= 0)
        {
        //  d = (Packet)se.RawDeSerialize(tempBuffer, typeof(Packet));

            //Debug.Log(d.header.Pkid);
            //Debug.Log(d.header.PkProtocol);
            //Debug.Log(d.header.PkSize);
            //  d = (Packet)se.RawDeSerialize(buffer2, typeof(Packet));

            //     Debug.Log(d.header.Pkid);
            //   Debug.Log(d.header.PkSize);


            sck.Close();
        }
    
}

        //   buffer = System.BitConverter.GetBytes(d.i);

        //
        //      buffer = PacketMaker.Serialize(d);

        
        //  Packet ads = PacketMaker.Deserialize(buffer);
        //Packet ads = new Packet(123,'o');
        //ads.i = System.BitConverter.ToInt32(buffer, 0);
        //if (ads != null)
        //{
        //    Debug.Log(ads.i);
        //}

}


