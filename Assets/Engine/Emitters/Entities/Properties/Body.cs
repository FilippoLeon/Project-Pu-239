using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class Body
{
    class Part
    {
        Dictionary<string, Condition> conditions = new Dictionary<string, Condition>();
    }

    Dictionary<string, Part> bodyParts = new Dictionary<string, Part>();
}
