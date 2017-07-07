using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobComponent : EmitterController, IJobListener {

    public static JobComponent ScheduleJobControllerInWorld(
        WorldComponent worldComponent, Job job)
    {
        GameObject entityObject = new GameObject();
        JobComponent entityComponent = entityObject.AddComponent<JobComponent>();
        entityObject.transform.SetParent(worldComponent.transform);
        //entityComponent.SetPosition(coord);

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
        throw new NotImplementedException();
    }
}
