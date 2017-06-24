using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;
using System;

public class EntityAnimated : Entity {
    Vector2 target;

    bool moving = false;
    public LinkedList<Tile> path;
    Vector2 previousWaypoint;
    Vector2 nextWaypoint;
    float theta = 0f;

    public string name;

    float Heuristic(Tile from, Tile to)
    {
        return Tile.Distance(from, to);
    }

    Dictionary<Tile, TileCost> FindPath(Tile startTile, Tile endTile)
    {

        SimplePriorityQueue<Tile, float> frontier = new SimplePriorityQueue<Tile, float>();
        frontier.Enqueue(startTile, 0);
        Dictionary<Tile, TileCost> comeFrom = new Dictionary<Tile, TileCost>();
        comeFrom[startTile] = new TileCost(null, 0);

        while (frontier.Count != 0)
        {
            Tile current = frontier.Dequeue();
            if(current == endTile)
            {
                break;
            }
            foreach(Tile next in current.GetNeighboursList())
            {
                // Ignore impassable tiles
                if (!next.IsPassable()) continue;

                float newCost = comeFrom[current].cost + ( current.WalkingCost() + next.WalkingCost() ) * 0.5f; // cost in graph
                if(!comeFrom.ContainsKey(next) || newCost < comeFrom[next].cost)
                {
                    // Heuristic to find cost to arrive to end
                    float priority = newCost + Heuristic(next, endTile);
                    frontier.Enqueue(next, priority);
                    comeFrom[next] = new TileCost(current, newCost);
                }
            }
        }

        return comeFrom;
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
        path.RemoveFirst();
        theta = 0f;
    }

    public void Tic()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (!moving) return;
        theta += 0.1f;
        Position = Vector2.Lerp(previousWaypoint, nextWaypoint, theta);
        if(theta >= 1f)
        {
            if(path.Count == 0)
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

        Dictionary<Tile, TileCost> comeFrom = FindPath(startTile, endTile);

        LinkedList<Tile> list = new LinkedList<Tile>();
        Tile current = endTile;
        list.AddFirst(endTile);
        while (current != startTile && comeFrom.ContainsKey(current))
        {
            Tile previous = comeFrom[current].tile;
            list.AddFirst(previous);
            current = previous;
        }
        //list.AddFirst(startTile);

        if (list.Count == 1) return null;

        /// DEBUG DRAW PATH
        /// 
        Tile previousTile = null;
        foreach (Tile tile in list)
        {
            if (previousTile != null) {
                Debug.DrawLine(previousTile.coord.ToVector2(), tile.coord.ToVector2(), Color.blue, 20);
            }
            previousTile = tile;
        }

        return list;
    }
}

internal class TileCost
{
    public Tile tile;
    public float cost;

    public TileCost(Tile tile, float cost)
    {
        this.tile = tile;
        this.cost = cost;
    }
}