using System;
using System.Collections.Generic;

public class Room : Emitter, IFormattable, IWieghtedNode<Room>
{
    int id;
    static int idCounter = 0;

    static Room defaultRoom = new Room();
    static Room impassableRoom = new Room(-1);

    public Dictionary<Tile, List<Connection>> connections = new Dictionary<Tile, List<Connection>>();

    public int Id
    {
        get
        {
            return id;
        }
    }

    private Room(int id)
    {
        this.id = id;
    }

    public Room()
    {
        id = idCounter++;
    }

    public static Room Default()
    {
        return defaultRoom;
    }
    public static Room Boundary()
    {
        return impassableRoom;
    }

    internal void AddRoomConnection(Connection connection)
    {
        if (connections.ContainsKey(connection.via))
        {
            connections[connection.via].Add(connection);
        }
        else
        {
            List<Connection> list = new List<Connection>();
            list.Add(connection);
            connections.Add(connection.via, list);
        }
    }

    public void RemoveRoomConnection(Tile via)
    {
        List<Connection> connectionVia = connections[via];
        foreach (Connection connection in connectionVia)
        {
            connection.from.connectionsTo.Remove(connection);
            connection.to.connectionsFrom.Remove(connection);
        }

        connections.Remove(via);
    }

    public string ToString(string format, IFormatProvider formatProvider)
    {
        return String.Format("Room {0}", id);
    }

    public IEnumerable<Room> GetNeighboursList(bool flag)
    {
        List<Room> adjacent = new List<Room>();
        foreach(List<Connection> tileConnections in connections.Values)
        {
            foreach (Connection connection in tileConnections)
            {
                adjacent.Add(connection.to.Room);
            }
        }
        return adjacent;
    }

    public float WalkingCost(Room from)
    {
        return 1;
    }

    public class Connection
    {
        public Tile from;
        public Tile to;
        public Tile via;

        public Connection(Tile from, Tile to, Tile via)
        {
            this.from = from;
            this.to = to;
            this.via = via;
        }
    }
}