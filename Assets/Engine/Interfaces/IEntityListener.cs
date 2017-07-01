using UnityEngine;

public interface IEntityListener : IEmitterListener
{
    void Spawn(World world, Vector2 coord);

    void PositionChanged(World world, Vector2 position);
    void Despawn(World world);
}