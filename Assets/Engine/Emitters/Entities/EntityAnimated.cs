using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;

public class EntityAnimated : Entity
{
    Vector2 target;

    bool moving = false;
    public LinkedList<Tile> path;
    Vector2 previousWaypoint;
    Vector2 nextWaypoint;
    float distance = 0f;
    float theta = 0f;
    float speed = 1f;

    public string name;

    float Heuristic(Tile from, Tile to)
    {
        return Tile.Distance(from, to);
    }
    

    public void StartMovingTo(Vector2 endPosition)
    {
        path = CreatePath(Position, endPosition);
        if (path == null)
        {
            Debug.LogWarning("No Path.");
            return;
        }
        moving = true;
        target = endPosition;
        NextWaypoint();
        theta = 0f;
    }

    public void NextWaypoint()
    {
        previousWaypoint = Position;
        nextWaypoint = path.First.Value.coord.ToVector2();
        distance = (previousWaypoint - nextWaypoint).magnitude;
        path.RemoveFirst();
        theta = 0f;
    }

    virtual public void Tic()
    {


        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!moving) return;
        theta += 0.1f / distance * speed;
        Position = Vector2.Lerp(previousWaypoint, nextWaypoint, theta);
        if (theta >= 1f)
        {
            if (path.Count == 0)
            {
                moving = false;
                path = null;
                return;
            }
            NextWaypoint();
        }
    }

    public LinkedList<Tile> CreatePath(Vector2 startPosition, Vector2 endPosition)
    {
        Tile startTile = world.GetTileAtCoord(startPosition);
        Tile endTile = world.GetTileAtCoord(endPosition);

        //// Quick check with Room based pathfinding
        if(startTile.Room != endTile.Room)
        {
            Func<Room, Room, float> RoomHeuristic = (Room A, Room B) => { return 1; };

            Dictionary<Room, Cost<Room>> comeFromRoom = Pathfinding.FindPath(startTile.Room, endTile.Room,
                (Room r) => false, RoomHeuristic, true /* diagonal */);

            LinkedList<Room> roomList = Pathfinding.ReconstructPath(comeFromRoom, startTile.Room, endTile.Room);
            if(roomList == null)
            {
                Debug.LogWarning("No path according to room adjacency.");
                return null;
            }
        }

        Dictionary<Tile, Cost<Tile>> comeFrom = Pathfinding.FindPath(startTile, endTile,
            (Tile next) => !next.IsPassable(), Heuristic, true /* diagonal */);

        LinkedList<Tile> list = Pathfinding.ReconstructPath(comeFrom, startTile, endTile);

        //Vector2 firstCoord = list.First.Value.coord.ToVector2();
        //Vector2 secondCoord = list.First.Next.Value.coord.ToVector2();

        // Remove first tile if is faster to go to the second tile
            list.RemoveFirst();

        if(list == null)
        {
            Debug.LogError("No path.");
            return null;
        }

        /// DEBUG DRAW PATH
        /// 
        Tile previousTile = null;
        foreach (Tile tile in list)
        {
            if (previousTile != null)
            {
                Debug.DrawLine(previousTile.coord.ToVector2(), tile.coord.ToVector2(), Color.blue, 20);
            }
            previousTile = tile;
        }

        return list;
    }
    
}
