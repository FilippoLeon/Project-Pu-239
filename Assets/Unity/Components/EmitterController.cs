using System;
using UnityEngine;

public class EmitterController : MonoBehaviour, IEmitterListener
{

    protected SpriteRenderer spriteRenderer;

    protected Emitter entity;
    public Emitter Emitter
    {
        get
        {
            return entity;
        }

        set
        {
            entity = value;
        }
    }
    
    public void Event(string signal, object[] args)
    {
        throw new NotImplementedException();
    }
}