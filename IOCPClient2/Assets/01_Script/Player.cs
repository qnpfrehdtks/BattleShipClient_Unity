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

public class Player : MonoBehaviour  {

    public bool m_isYourTurn;
    public int m_Id {get; private set; }
    public int m_EnemyId { get; set; }
    public Dictionary<SHIP, Base_Ship> m_InstalledShipMap;

    //  private List<PlayerObject> m_PlayerObjList;


    public Player(int ID)
    {
        m_InstalledShipMap = new Dictionary<SHIP, Base_Ship>();
        m_Id = ID;
    }


    public void AddShip(Base_Ship ship)
    {
        if(m_InstalledShipMap.ContainsKey(ship.m_shipKind))
        {
            Debug.Log(ship.m_shipKind.ToString() + " already have a Ship in a Dic ");
            return;
        }
        else
        {
            Debug.Log(ship.m_shipKind.ToString() + " Add Complete!!! ");
            m_InstalledShipMap.Add(ship.m_shipKind, ship);
        }
    }

    public void DeleteShip(SHIP ship)
    {
        if (m_InstalledShipMap.ContainsKey(ship))
        {
            m_InstalledShipMap.Remove(ship);
            Debug.Log(ship.ToString() + " Delete Complete ");
        }
        else
        {
            Debug.Log(ship.ToString() + " not have in Dictionary ");
        }
    }


    public int ShipCount()
    {
        if (m_InstalledShipMap != null)
        {
            return m_InstalledShipMap.Count;
        }
        else
        {
            Debug.Log(" need a alloc a Memory Map");
            return -1;
        }
    }

    public int ShipRaderCount(sVector2[] pos)
    {
        int count = 0;

        for(SHIP i=0; i < SHIP.WAR_SHIP; i++)
        {
            for (int j = 0; j < pos.Length; j++)
            {
                if (m_InstalledShipMap[i].CheckShipHavePoint(pos[j]))
                {
                    count++;
                    break;
                }
            }
        }

        return count;
    }




}
