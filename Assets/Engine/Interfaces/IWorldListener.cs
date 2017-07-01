using UnityEngine;

public interface IWorldListener : IEmitterListener
{
    void Spawn(World world, Entity entity, Vector2 position);
    void Despawn(World world, Entity entity);

    void InstallAt(World world, EntityBuilding entity, World.Coord coord);

    void Uninstall(World world, EntityBuilding entity);

    void WorldCreated(World world);
}