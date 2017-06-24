using UnityEngine;

public interface IWorldListener : IEmitterListener
{
    void Spawn(World world, Entity entity, Vector2 position);

    void InstallAt(World world, EntityBuilding entity, World.Coord coord);

    void WorldCreated(World world);
}