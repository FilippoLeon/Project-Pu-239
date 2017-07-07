using System;
using UnityEngine;

public class EmitterController : MonoBehaviour, IEmitterListener
{

    protected SpriteRenderer spriteRenderer;

    protected Entity entity;
    public Emitter Emitter
    {
        get
        {
            return entity;
        }

        set
        {
            entity = (Entity)value;
        }
    }
    
    public void Event(string signal, object[] args)
    {
        throw new NotImplementedException();
    }
}