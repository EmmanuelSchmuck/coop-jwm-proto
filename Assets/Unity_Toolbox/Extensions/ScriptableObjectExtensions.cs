using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public static class ScriptableObjectExtensions
    {
        public static T Clone<T>(this T so) where T : ScriptableObject
        {
            return Object.Instantiate(so);
        }
    }
}
