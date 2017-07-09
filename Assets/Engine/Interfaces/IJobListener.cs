using UnityEngine;

public interface IJobListener : IEmitterListener
{
    void Schedule(World world);

    void Progress(World world);
}