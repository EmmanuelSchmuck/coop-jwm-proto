using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public static class VectorExtensions
    {
        public static Vector3 WithX(this Vector3 v, float value)
        {
            return new Vector3(value, v.y, v.z);
        }

        public static Vector3 WithY(this Vector3 v, float value)
        {
            return new Vector3(v.x, value, v.z);
        }
		public static Vector2 WithX(this Vector2 v, float value)
        {
            return new Vector2(value, v.y);
        }

		public static Vector2 WithY(this Vector2 v, float value)
        {
            return new Vector2(v.x, value);
        }

        public static Vector3 WithZ(this Vector3 v, float value)
        {
            return new Vector3(v.x, v.y, value);
        }

        public static Vector3 WithXMultiplied(this Vector3 v, float value)
        {
            return new Vector3(v.x * value, v.y, v.z);
        }

        public static Vector3 WithYMultiplied(this Vector3 v, float value)
        {
            return new Vector3(v.x, v.y * value, v.z);
        }

        public static Vector3 WithZMultiplied(this Vector3 v, float value)
        {
            return new Vector3(v.x, v.y, v.z * value);
        }
    }
}