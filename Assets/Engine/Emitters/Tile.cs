using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Emitter, IFormattable, IWieghtedNode<Tile>
{
    public World.Coord coord;

    public World world;
    public EntityBuilding building = null;
    private Room room = Room.Default();

    public List<Room.Connection> connectionsFrom = new List<Room.Connection>();
    public List<Room.Connection> connectionsTo = new List<Room.Connection>();

    public Room Room
    {
        get
        {
            return room;
        }

        set
        {
            ChangeTileRoom(value);
            room = value;

            // If tile becomes impassable, reassign room coloring for tiles
            if(room == Room.Boundary())
            {
                SetTileAsBoundary();
            }
        }
    }

    private void ChangeTileRoom(Room newRoom)
    {
        foreach (Room.Connection connection in connectionsFrom)
        {
            room.RemoveRoomConnection(connection.via);
            newRoom.AddRoomConnection(connection);
        }
        foreach (Room.Connection connection in connectionsTo)
        {
            room.RemoveRoomConnection(connection.via);
            newRoom.AddRoomConnection(connection);
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
        Debug.Log(String.Format("Flood filling room {0} with room {1} from tile {2}", startRoom, endRoom, this));
        Queue<Tile> queue = new Queue<Tile>();

        queue.Enqueue(this);
        while(queue.Count != 0)
        {
            Tile next = queue.Dequeue();
            next.room = endRoom;
            next.MarkNorthSouth(queue, startRoom);
            Tile west = next, east = next;
            while (true)
            {
                west = west.GetNeighbour(Direction.West);
                if (west == null || west.room != startRoom) { break; }
                west.room = endRoom;
                west.MarkNorthSouth(queue, startRoom);
            }
            while (true)
            {
                east = east.GetNeighbour(Direction.East);
                if (east == null || east.room != startRoom) { break; }
                east.room = endRoom;
                east.MarkNorthSouth(queue, startRoom);
            }
        }
    }

    private void MarkNorthSouth(Queue<Tile> queue, Room startRoom)
    {
        Tile north = this.GetNeighbour(Direction.North);
        Tile south = this.GetNeighbour(Direction.South);
        if (north != null && north.room == startRoom)
        {
            queue.Enqueue(north);
        }
        if (south != null && south.room == startRoom)
        {
            queue.Enqueue(south);
        }
    }

    void SetTileAsBoundary()
    {

        Tile[] neighbours = GetNeighbours(true, true);

        // Find all potential candidate new rooms
        List<Tile> potentialStartsNonPriority = new List<Tile>();
        SimplePriorityQueue<Tile> potentialStarts = new SimplePriorityQueue<Tile>();
        for (int i = 0; i < 8; i += 2)
        {
            int diagonalNextIndex = (i + 1) % 8;
            int nonDiagonalNextIndex = (i + 2) % 8;
            if ((neighbours[i].IsRoomBoundary() || neighbours[diagonalNextIndex].IsRoomBoundary() ) 
                & !neighbours[nonDiagonalNextIndex].IsRoomBoundary() )
            {
                potentialStartsNonPriority.Add(neighbours[nonDiagonalNextIndex]);
            }
        }

        if (potentialStartsNonPriority.Count == 1) return;

        foreach(Tile potentialStart in potentialStartsNonPriority)
        {
                int windingNumber = LabyrinthPath(potentialStart, this, ref potentialStarts);

                potentialStarts.Enqueue(potentialStart, (potentialStart.room == Room.Default() ? 1f : 8f) + windingNumber);
        }

        // Flood fill potential candidates, keep track of room changes
        List<Room> newRooms = new List<Room>();
        while (potentialStarts.Count > 1)
        {
            Tile start = potentialStarts.Dequeue();

            Room newRoom = new Room();
            newRooms.Add(newRoom);

            start.FloodFill(start.room, newRoom);
        }
        
    }

    public int LabyrinthPath(Tile startTile, Tile startBoundaryTile, ref SimplePriorityQueue<Tile> potentialStarts)
    {
        Tile next = startTile;
        Tile nextBoundaryTile = startBoundaryTile;
        Stack<int> quadrants = new Stack<int>();
        quadrants.Push(Quadrant(next, startBoundaryTile));
        
        int count = 0;
        int maxLabyrinthCount = 1000;
        do
        {
            LabyrinthStep(startTile, startBoundaryTile, ref next, ref nextBoundaryTile, ref quadrants);
            if (potentialStarts.Contains(next))
            {
                potentialStarts.Remove(next);
            }

        }
        while ((next != startTile || startBoundaryTile != nextBoundaryTile) && count++ < maxLabyrinthCount) ;

        // Winding number
        Debug.Log(String.Format("Winding number is {0}", quadrants.Count));
        return quadrants.Count / 4;
    }

    World.Coord[] directionFromQuadrant = new World.Coord[]
    {
        World.Coord.Left,
        World.Coord.Top,
        World.Coord.Right,
        World.Coord.Bottom
    };

    public void LabyrinthStep(Tile startTile, Tile startBoundaryTile, 
        ref Tile next, ref Tile nextBoundaryTile, ref Stack<int> quadrants)
    {
        //Tile candidate = next.GetNeighbour(Tile.Difference(next, nextBoundaryTile).Rotate());
        Tile candidate = next.GetNeighbour(directionFromQuadrant[Tile.Quadrant(next, nextBoundaryTile)]);
        //Debug.Log(String.Format("Labirinth step start {0} and boundary {1}, current {2} with boundary {3}, last quadrant {4}",
            //startTile, startBoundaryTile,
            //next, nextBoundaryTile, quadrants.Peek()));
        if (candidate.IsRoomBoundary())
        {
            nextBoundaryTile = candidate;
        }
        else
        {
            next = candidate;
            int newQuadrant = Quadrant(next, startBoundaryTile);
            if (newQuadrant != quadrants.Peek())
            {
                int previousQuadrant = quadrants.Pop();
                if (quadrants.Count < 1 || newQuadrant != quadrants.Peek())
                {
                    quadrants.Push(previousQuadrant);
                    quadrants.Push(newQuadrant);
                }
            }
        }

    }

    internal void Uninstall()
    {
        bool mustMergeRooms = IsRoomBoundary();
        bool mustRemoveconnections = IsConnecting();
        // RESET ROOMS AND CONNECTIONS

        building = null;

        if(mustMergeRooms)
        {
            MergeRooms();
        }
        if(mustRemoveconnections)
        {
            RemoveRoomConnection();
        }

        Changed();
    }

    private void RemoveRoomConnection()
    {
        room.RemoveRoomConnection(this);
    }

    /// <summary>
    /// Merge all rooms surrounding this tile into one.
    /// </summary>
    private void MergeRooms()
    {
        List<Room> rooms = new List<Room>();
        Dictionary<Room, Tile> tileOfRoom = new Dictionary<Room, Tile>();
        Room finalRoom = null;
        // Create a list of all rooms that should be merged, do not include Default room in list
        // the finalRoom, is the room to which all rooms will be converted
        foreach(Tile neighbour in GetNeighboursList(false))
        {
            if(neighbour != null && neighbour.room != Room.Boundary() && !rooms.Contains(neighbour.room))
            {
                rooms.Add(neighbour.room);
                tileOfRoom[neighbour.room] = neighbour;
                if(neighbour.room == Room.Default())
                {
                    finalRoom = Room.Default();
                }
            }
        }
        // If there is no Default room to merge into, just pick one (TODO: pick bigger?)
        if(finalRoom == null)
        {
            finalRoom = rooms[0];
        }
        // Remember to change the room of this tile
        room = finalRoom;
        // If there is only one room, do not merge
        if (rooms.Count <= 1) return;
        // Flood fill all other rooms
        for (int i = 0; i < rooms.Count; ++i)
        {
            if(rooms[i] == finalRoom)
            {
                continue;
            }
            tileOfRoom[rooms[i]].FloodFill(rooms[i], finalRoom);
        }
    }

    /// <summary>
    /// Return quadrant of tile, relative to pivot. Quadrant is skewed to account for integer coordinates.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="pivot"></param>
    /// <returns></returns>
    private static int Quadrant(Tile tile, Tile pivot)
    {
        //// T = tile
        //// P = pivot
        ////
        ////               |
        ////               |      0
        ////       3       |
        ////               +-+--------------
        ////               |P|
        //// --------------+-+   +-+
        ////                 |   |T|
        ////         2       |   +-+  1
        ////                 |
        ////
        ////

        // NE quadrant
        if (tile.coord.x >= pivot.coord.x && tile.coord.y > pivot.coord.y)
        {
            return 0;
        // SE quadrant
        } else if(tile.coord.x > pivot.coord.x && tile.coord.y <= pivot.coord.y)
        {
            return 1;
        }
        else if (tile.coord.x <= pivot.coord.x && tile.coord.y < pivot.coord.y)
        {
            return 2;
        }
        else if (tile.coord.x < pivot.coord.x && tile.coord.y >= pivot.coord.y)
        {
            return 3;
        }
        Debug.LogError("Invalid quadrant.");
        return -1;
    }

    private static World.Coord Difference(Tile next, Tile nextBoundaryTile)
    {
        return next.coord - nextBoundaryTile.coord;
    }

    public Tile(World world, World.Coord coord)
    {
        this.world = world;
        this.coord = coord;
    }

    public float WalkingCost(Tile previous)
    {
        float distance = Tile.Distance(this, previous);
        return distance * (this.WalkingCost() + previous.WalkingCost()) / 2.0f;
    }

    public float WalkingCost()
    {
        return building == null ? 1 : (building.WalkingSpeed == 0 ? Mathf.Infinity : 1 / building.WalkingSpeed);
    }

    public bool IsPassable()
    {
        if(WalkingCost() == 0f) {
            return false;
        } else {
            return true;
        }
    }

    public bool IsRoomBoundary()
    {
        if (building != null && building.RoomBoundary == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    internal void Install(EntityBuilding newBuilding)
    {
        if (building != null) return;
        building = newBuilding;
        building.Tile = this;

        if(IsRoomBoundary())
        {
            Room = Room.Boundary();
        }
        if(IsConnecting())
        {
            AddRoomConnection();
        }

        //building.Tile.Changed();
    }

    private void AddRoomConnection()
    {
        Debug.Log(String.Format("New room connection on tile {0}", this));
        foreach(Tile tile1 in GetNeighbours(false))
        {
            foreach (Tile tile2 in GetNeighbours(false))
            {
                if (tile1 != tile2 
                    && tile1 != null && tile2 != null 
                    && !tile1.IsRoomBoundary() && !tile2.IsRoomBoundary())
                {
                    Debug.Log(String.Format("New room connection between {0} and {1} via {2} connecting {3} and {4}.", 
                        tile1, tile2, this, tile1.Room, tile2.Room));
                    Room.Connection tile1ToTile2 = new Room.Connection(tile1, tile2, this);
                    Room.Connection tile2ToTile1 = new Room.Connection(tile2, tile1, this);
                    tile1.connectionsFrom.Add(tile1ToTile2);
                    tile1.connectionsTo.Add(tile2ToTile1);
                    tile2.connectionsFrom.Add(tile2ToTile1);
                    tile2.connectionsTo.Add(tile1ToTile2);
                    tile1.Room.AddRoomConnection(tile1ToTile2);
                    tile2.Room.AddRoomConnection(tile2ToTile1);
                }
            }
        }
    }

    public bool IsConnecting()
    {
        return building != null && building.Connecting;
    }

    public void Changed()
    {
        foreach(Tile neighbour in GetNeighboursList())
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

    public IEnumerable<Tile> GetNeighboursList(bool diagonal = true)
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

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return String.Format("Tile at {0}x{1}", coord.x, coord.y);
    }
}