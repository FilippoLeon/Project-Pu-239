internal interface IEntityBuildingListener : IEmitterListener
{
    void InstallAt(World world, World.Coord coord);
    void NeighbourChanged(World world, Tile neighbour);
}