using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public int m_positionV;
    public int m_positionH;
    public bool m_mountain;
    public bool m_water;
    public bool m_trap;
    public bool m_player =false;
    public bool m_endPlayer1;
    public bool m_endPlayer2;

    public bool m_active = false;
    public List<Tile> m_AdjacentTiles = new List<Tile>();  //Group of different Tile free to move 

    public GameObject m_selectorIndicator;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Accessors

    public int getPositionH() 
    {
        return this.m_positionH;
    }

    public int getPositionV() 
    {
        return m_positionV;
    }

    public int[] getPosition()
    {
        int[] position = new int[2] {this.getPositionH(),this.getPositionV()};
        return position;
    }
    public bool hasMountain() 
    {
        return this.m_mountain;
    }

    public bool hasWater() 
    {
        return this.m_water;
    }

    public bool hasTrap() 
    {
        return this.m_trap;
    }

    public bool hasPlayer() 
    {
        return this.m_player;
    }

    public bool isWalkable()
    {
        return !this.hasMountain() && !this.hasWater();
    }
    public List<Tile> GetAdjacentTiles()
    {
        return this.m_AdjacentTiles;
    }

    public bool isActive()
    {
        return this.m_active;
    }
    #endregion

    #region Mutators

    public void setPositionH(int value)
    {
        this.m_positionH = value;
    }
    public void setPositionV(int value)
    {
        this.m_positionV = value;
    }
    public void setMountain()
    {
        this.m_mountain = !this.m_mountain;
    }
    public void setPlayer(bool value)
    {
        this.m_player = value;
    }
    public void setWater()
    {
        this.m_water = !this.m_water;
    }
    public void setTrap()
    {
        this.m_trap = !this.m_trap;
    }

    public void setActive(bool value)
    {
        this.m_active = value;
    }
    #endregion

    #region Utils

    public void SetAdjacentTiles(List<Tile> boardtiles) // optimisable
    {
        foreach (Tile tile in boardtiles)
        {
            if (tile.getPositionH() == this.getPositionH() -1 && tile.getPositionV() == this.getPositionV() || tile.getPositionH() == this.getPositionH() +1 && tile.getPositionV() == this.getPositionV() || tile.getPositionV() == this.getPositionV() -1 && tile.getPositionH() == this.getPositionH()||tile.getPositionV() == this.getPositionV() +1 && tile.getPositionH() == this.getPositionH())
            {
                this.m_AdjacentTiles.Add(tile);
            }
        }
        setSelectorIndicator(false);
    }
    public List<Tile> getMovements()
    {
        List<Tile> walkableTiles = new List<Tile>();
        List<Tile> tiles = this.GetAdjacentTiles();
        foreach (Tile tile in tiles)
        {
            if (tile.isWalkable())
            {
                walkableTiles.Add(tile);
                tile.setSelectorIndicator(true);
            }
            
        }
        return walkableTiles;
    }

    public void setSelectorIndicator(bool active)
    {
        this.m_selectorIndicator.SetActive(active);
        this.setActive(active);
    }

    #endregion

    #region OnClick

    void OnMouseUp() 
    {
        if(this.isActive())
        {
            this.setPlayer(true);
            Controller.ctrl.MovePlayrRec(this);
        }
    }

    public static object Deserialize(byte[] data)
    {
        Debug.Log("deserialize");

        byte[] hb = new byte[4];
        Array.Copy(data, 0, hb, 0, hb.Length);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(hb);
        int h = BitConverter.ToInt32(hb,0);

        byte[] vb = new byte[4];
        Array.Copy(data, 4,vb, 0, vb.Length);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(vb);
        int v = BitConverter.ToInt32(vb, 0);

        Debug.Log(h + " " + v);
        Tile tile = Controller.ctrl.GetTile(h,v);
        return tile;
    }

    public static byte[] Serialize(object obj)
    {
        Debug.Log("serialize");
        var tile = (Tile)obj;
        Debug.Log(tile.getPositionH()+ " "+ tile.getPositionV());

        byte[] h = BitConverter.GetBytes(tile.getPositionH());
        if (BitConverter.IsLittleEndian)
            Array.Reverse(h);

        byte[] v = BitConverter.GetBytes(tile.getPositionV());
        if (BitConverter.IsLittleEndian)
            Array.Reverse(v);

        Byte[] data = new byte[2*4];

        return JoinBytes(h,v);
    }

    private static byte[] JoinBytes(params byte[][] arrays)
    {
        byte[] rv = new byte[arrays.Sum(a => a.Length)];
        int offset = 0;
        foreach (byte[] array in arrays)
        {
            System.Buffer.BlockCopy(array, 0, rv, offset, array.Length);
            offset += array.Length;
        }
        return rv;
    }
    #endregion
}
