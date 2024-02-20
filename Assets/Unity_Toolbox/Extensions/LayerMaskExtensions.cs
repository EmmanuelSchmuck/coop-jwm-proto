using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public static class LayerMaskExtensions
    {
        public static bool Contains(this LayerMask layerMask, int layer)
        {
            return layerMask == (layerMask | 1 << layer);
        }
    }
}