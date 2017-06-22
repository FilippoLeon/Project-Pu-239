public interface IWorldListener : IEmitterListener
{
    void InstallAt(World world, EntityBuilding entity, World.Coord coord);
    void WorldCreated(World world);
}