using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Emitter
{
    public World.Coord coord;

    public World world;
    public EntityBuilding building = null;
    private Room room = Room.Default();

    public Room Room
    {
        get
        {
            return room;
        }

        set
        {
            room = value;

            // If tile becomes impassable, reassign room coloring for tiles
            if(room == Room.Impassable())
            {
                SetImpassable();
            }
        }
    }

    public enum Direction
    {
        North,
        West,
        South, 
        East
    };

    Tile GetNeighbour(Direction direction)
    {
        switch (direction)
        {
            case Direction.North:
                return world.GetTileAtOrNull(coord + World.Coord.Top);
            case Direction.West:
                return world.GetTileAtOrNull(coord + World.Coord.Left);
            case Direction.South:
                return world.GetTileAtOrNull(coord + World.Coord.Bottom);
            case Direction.East:
                return world.GetTileAtOrNull(coord + World.Coord.Right);
        }
        return null;
    }

    void FloodFill(Room startRoom, Room endRoom)
    {
        Queue<Tile> queue = new Queue<Tile>();

        queue.Enqueue(this);
        while(queue.Count != 0)
        {
            Tile next = queue.Dequeue();
            next.room = endRoom;
            while(true)
            {
                Tile west = next.GetNeighbour(Direction.West);
                if (west == null || west.room != startRoom) { break; }
                west.room = endRoom;
                Tile north = west.GetNeighbour(Direction.North);
                Tile south = west.GetNeighbour(Direction.South);
                if (north == null)
                {
                    break;
                }
                else if (north.room == startRoom)
                {
                    queue.Enqueue(north);
                }
                if (south == null)
                {
                    break;
                }
                else if (south.room == startRoom)
                {
                    queue.Enqueue(south);
                }
            }
            while (true)
            {
                Tile east = next.GetNeighbour(Direction.East);
                if (east == null || east.room != startRoom) { break; }
                east.room = endRoom;
                Tile north = east.GetNeighbour(Direction.North);
                Tile south = east.GetNeighbour(Direction.South);

                if (north == null)
                {
                    break;
                }
                else if (north.room == startRoom)
                {
                    queue.Enqueue(north);
                }
                if (south == null)
                {
                    break;
                }
                else if(south.room == startRoom)
                {
                    queue.Enqueue(south);
                }
            }
        }
    }

    void SetImpassable()
    {

        Tile[] neighbours = GetNeighbours(true, true);

        // Find all potential candidate new rooms
        List<Tile> potentialStarts = new List<Tile>(4);
        for(int i = 0; i < 8; ++i)
        {
            if(!neighbours[i].IsPassable() && neighbours[(i+1)%8].IsPassable())
            {
                potentialStarts.Add(neighbours[(i + 1) % 8]);
            }
        }
        // If there is only a potential room just forget it.
        if (potentialStarts.Count <= 1) return;

        // Flood fill potential candidates, keep track of room changes
        List<Room> newRooms = new List<Room>();
        foreach(Tile start in potentialStarts)
        {
            if(/*start.room != Room.Default() && */ !newRooms.Contains(start.room))
            {
                Room newRoom = new Room();
                newRooms.Add(newRoom);
                start.FloodFill(start.room, newRoom);
            }
        }
    }

    public Tile(World world, World.Coord coord)
    {
        this.world = world;
        this.coord = coord;
    }

    public float WalkingCost()
    {
        return building != null ? 0 : 1;
    }

    public bool IsPassable()
    {
        if(WalkingCost() == 0f) {
            return false;
        } else {
            return true;
        }
    }

    internal void Install(EntityBuilding newBuilding)
    {
        building = newBuilding;
        building.Tile = this;

        if(!newBuilding.Passable)
        {
            Room = Room.Impassable();
        }

        //building.Tile.Changed();
    }

    public void Changed()
    {
        List<Tile> neighbours = GetNeighboursList();
        foreach(Tile neighbour in neighbours)
        {
            neighbour.OnNeighbourChanged(world, this);
        }
    }

    static public float Distance(Tile from, Tile to)
    {
        return Mathf.Sqrt(
            Mathf.Pow(from.coord.x - to.coord.x, 2f) 
          + Mathf.Pow(from.coord.y - to.coord.y, 2f)
        );
    }

    internal bool CanInstall(EntityBuilding newBuilding)
    {
        if (building == null) return true;
        return false;
    }
    
    public Tile GetNeighbour(World.Coord direction, bool canBeNull = true)
    {
        return world.GetTileAtOrNull(coord + direction);
    }

    public Tile GetNeighbour(int i, bool canBeNull = true)
    {
        return GetNeighbour(World.Coord.Direction(i), canBeNull);
    }

    public Tile GetNeighbourOrdered(int i, bool canBeNull = true)
    {
        return GetNeighbour(World.Coord.DirectionOrdered(i), canBeNull);
    }

    internal List<Tile> GetNeighboursList(bool diagonal = true)
    {
        List<Tile> tiles = new List<Tile>();
        for(int i = 0; i < (diagonal ? 8 : 4); ++i)
        {
            Tile neighbour = GetNeighbour(i);
            if (neighbour != null) tiles.Add(neighbour);
        }
        return tiles;
    }
    internal Tile[] GetNeighbours(bool diagonal = true, bool ordered = false)
    {
        Tile[] tiles = new Tile[8];
        if (ordered)
        {
            for (int i = 0; i < (diagonal ? 8 : 4); ++i)
            {
                tiles[i] = GetNeighbourOrdered(i);
            }
        }
        else
        {
            for (int i = 0; i < (diagonal ? 8 : 4); ++i)
            {
                tiles[i] = GetNeighbour(i);
            }
        }
        return tiles;
    }

    internal void OnNeighbourChanged(World world, Tile neighbour)
    {
        //Debug.Log(
        //    String.Format("Neighbour '{0}x{1}' of '{2}x{3}' changed.", 
        //    neighbour.coord.x, neighbour.coord.y, coord.x, coord.y)
        //    );
        if (building != null)
        {
            building.NeighbourChange(world, neighbour);
        }
    }
}