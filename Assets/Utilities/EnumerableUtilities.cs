using System;
using System.Collections;
using System.Collections.Generic;

public class EnumerableUtilities {

    public static string PrintValues(IEnumerable myList, int myWidth)
    {
        string str = ",";
        int i = myWidth;
        foreach (Object obj in myList)
        {
            if (i <= 0)
            {
                i = myWidth;
                str += ",";
            }
            i--;
            str += string.Format("{0,8}", obj);
        }
        return str;
    }

}
