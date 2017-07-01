

using Priority_Queue;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding
    {

    /// <summary>
    /// A generic A* algorithm implemented using Priority Queues
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="startTile"></param>
    /// <param name="endTile"></param>
    /// <param name="IsValidNeighbour"></param>
    /// <param name="Heuristic"></param>
    /// <returns></returns>
    public static Dictionary<T, Cost<T>> FindPath<T>(T startTile, T endTile,
        Func<T, bool> IsInvalidNeighbour, Func<T, T, float> Heuristic, bool flag)
        where T : class, IWieghtedNode<T>
    {

        SimplePriorityQueue<T, float> frontier = new SimplePriorityQueue<T, float>();
        frontier.Enqueue(startTile, 0);
        Dictionary<T, Cost<T>> comeFrom = new Dictionary<T, Cost<T>>();
        comeFrom[startTile] = new Cost<T>(null, 0);

        while (frontier.Count != 0)
        {
            T current = frontier.Dequeue();
            if (current == endTile)
            {
                break;
            }
            Debug.Log(current);
            foreach (T next in current.GetNeighboursList(flag))
            {
                // Ignore impassable tiles
                if (IsInvalidNeighbour(next)) continue;

                float newCost = comeFrom[current].cost + next.WalkingCost(current); // cost in graph
                if (!comeFrom.ContainsKey(next) || newCost < comeFrom[next].cost)
                {
                    // Heuristic to find cost to arrive to end
                    float priority = newCost + Heuristic(next, endTile);
                    frontier.Enqueue(next, priority);
                    comeFrom[next] = new Cost<T>(current, newCost);
                }
            }
        }

        return comeFrom;
    }

    public static LinkedList<T> ReconstructPath<T>(Dictionary<T, Cost<T>> comeFrom,
        T startTile, T endTile) where T : class
    {

        LinkedList<T> list = new LinkedList<T>();
        T current = endTile;
        list.AddFirst(endTile);
        while (current != startTile && comeFrom.ContainsKey(current))
        {
            Cost<T> CF = comeFrom[current];
            T previous = CF.node;
            if (previous == null) return null;
            list.AddFirst(previous);
            current = previous;
        }
        //list.AddFirst(startTile);

        if (list.Count == 1) return null;

        return list;
    }
}
