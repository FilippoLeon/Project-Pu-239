using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobComponent : EmitterController, IJobListener {

    public Job job;

    public static JobComponent ScheduleJobControllerInWorld(
        WorldComponent worldComponent, Job job)
    {
        GameObject entityObject = new GameObject();
        JobComponent entityComponent = entityObject.AddComponent<JobComponent>();
        entityObject.transform.SetParent(worldComponent.transform);

        entityObject.name = job.id + "_indicator";
        entityComponent.job = job;

        PolygonCollider2D collider = entityObject.AddComponent<PolygonCollider2D>();
        collider.points = new Vector2[] {
            new Vector2(-0.5f, -0.5f),
            new Vector2(-0.5f, 0.5f),
            new Vector2(0.5f, 0.5f),
            new Vector2(0.5f, -0.5f)
        };

        entityComponent.spriteRenderer = entityObject.AddComponent<SpriteRenderer>();
        entityComponent.spriteRenderer.sprite = SpriteLoader.GetSprite("test");

        return entityComponent;
    }

    public void Schedule(World world)
    {
        transform.position = ((World.Coord) job.GetTarget().value).ToVector2();
    }
}
