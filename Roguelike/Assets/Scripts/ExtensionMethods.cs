using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static  class ExtensionMethods
{
    public static T ToEnum<T>(this string value)
    {
        if(!System.Enum.IsDefined(typeof(T),value))
        {
            return default(T);
        }

        return (T)System.Enum.Parse(typeof(T), value, true);
    }
}
