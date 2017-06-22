using System;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Emitter
{
    public World.Coord coord;

    public World world;
    public EntityBuilding building = null;

    public Tile(World world, World.Coord coord)
    {
        this.world = world;
        this.coord = coord;
    }

    internal void Install(EntityBuilding newBuilding)
    {
        building = newBuilding;
        building.Tile = this;

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

    internal bool CanInstall(EntityBuilding newBuilding)
    {
        if (building == null) return true;
        return false;
    }

    public Tile GetNeighbour(int i, bool canBeNull = true)
    {
        return GetNeighbour(World.Coord.Direction(i), canBeNull);
    }
    public Tile GetNeighbour(World.Coord direction, bool canBeNull = true)
    {
        return world.GetTileAtOrNull(coord + direction);
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
    internal Tile[] GetNeighbours(bool diagonal = true)
    {
        Tile[] tiles = new Tile[8];
        for (int i = 0; i < (diagonal ? 8 : 4); ++i)
        {
            tiles[i] = GetNeighbour(i);
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