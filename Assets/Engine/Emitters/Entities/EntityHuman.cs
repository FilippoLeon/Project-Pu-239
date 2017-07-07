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
        }

        base.Tic();
    }

    private void FindJob()
    {
        //currentJob = world.jobs["building"].Dequeue();
        //throw new NotImplementedException();
    }
}
