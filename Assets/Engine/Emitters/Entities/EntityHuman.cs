using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHuman : EntityAnimated {

    Job currentJob;

    public override void Tic()
    {
        if(currentJob == null)
        {
            FindJob();
        } else
        {
            if(currentJob.CanProgress())
            {
                currentJob.Progress(this, world);
            } else {

            }
        }

        base.Tic();
    }

    private void FindJob()
    {
        currentJob = world.jobs["build"].Dequeue();
        //throw new NotImplementedException();
    }
}
