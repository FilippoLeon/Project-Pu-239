using System;
using System.Xml.Serialization;
using UnityEngine;

public partial class Job : Emitter, IXmlSerializable
{
    public class Parameter
    {
        public object value;
        public Type type;

        public Parameter(Parameter other)
        {
            this.value = other.value;
            this.type = other.type;
        }

        public Parameter(object obj, Type type)
        {
            Debug.Assert(type != null, String.Format("Invalid parameter type for {0}", obj));
            this.value = obj;
            this.type = type;
        }
    }
}
