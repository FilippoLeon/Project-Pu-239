using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : Emitter {

    public struct Coord
    {
        public int x, y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        private static World.Coord[] directions =
        {
            new Coord(0, 1),
            new Coord(-1, 0),
            new Coord(0, -1),
            new Coord(1, 0),
            new Coord(-1, 1),
            new Coord(-1, -1),
            new Coord(1, -1),
            new Coord(1, 1)
        };

        public static World.Coord Direction(int i)
        {
            return directions[i];
        }

        public static Coord operator+(Coord one, Coord other)
        {
            return new Coord(one.x + other.x, one.y + other.y);
        }

        public static Coord Top = directions[0];
        public static Coord Left = directions[1];
        public static Coord Bottom = directions[2];
        public static Coord Right = directions[3];
        public static Coord TopLeft = directions[4];
        public static Coord BottomLeft = directions[5];
        public static Coord BottomRight = directions[6];
        public static Coord TopRight = directions[7];

        public static implicit operator Coord(Vector2 vector)
        {
            return new Coord((int) vector.x, (int) vector.y);
        }

        public static implicit operator Coord(Vector3 vector)
        {
            return new Coord((int) vector.x, (int) vector.y);
        }
    }
    

    Tile[,] tiles;

    public int sizeX, sizeY;

    public World(int sizeX, int sizeY)
    {
        this.sizeX = sizeX;
        this.sizeY = sizeY;
    }

    public void Generate()
    {

        tiles = new Tile[sizeX, sizeY];
        for(int i = 0; i < sizeX; ++i)
        {
            for (int j = 0; j < sizeY; ++j)
            {
                tiles[i, j] = new Tile(this, new Coord(i, j));
            }
        }

        WorldCreated();
    }
    
    public Tile GetTileAt(Coord coord)
    {
        try
        {
            return tiles[coord.x, coord.y];
        } catch(IndexOutOfRangeException)
        {
            Debug.LogError(String.Format("Invalid coordinate '{0}x{1}'", coord.x, coord.y));
            return null;
        }
    }
    public Tile GetTileAtOrNull(Coord coord)
    {
        if (IsValidCoordinate(coord))
            return tiles[coord.x, coord.y];
        else return null;
    }

    public void Tic()
    {

    }

    public void TicLong()
    {

    }

    public void TicLongLong()
    {

    }

    public void PaceEntityAt()
    {
        Emit("OnPlaceEntityAt");
    }

    private bool IsValidCoordinate(Coord coord)
    {
        if (coord.x >= 0 && coord.y >= 0 && coord.x < sizeX && coord.y < sizeY) return true;
        return false;
    }
    
    //// WORLD LISTENERS
    public bool CanInstallAt(EntityBuilding entity, Coord coord)
    {
        if (!IsValidCoordinate(coord)) return false;
        return GetTileAt(coord).CanInstall(entity);
    }


    public void InstallAt(EntityBuilding entity, Coord coord)
    {
        if (!IsValidCoordinate(coord)) return;

        foreach(IListener listener in listeners)
        {
            if(listener is IWorldListener) ((IWorldListener) listener).InstallAt(this, entity, coord);
        }

        GetTileAt(coord).Install(entity);

        Emit("OnInstallAt");
    }

    private void WorldCreated()
    {
        foreach (IWorldListener listener in listeners)
        {
            if (listener is IWorldListener) ((IWorldListener) listener).WorldCreated(this);
        }
    }

}
