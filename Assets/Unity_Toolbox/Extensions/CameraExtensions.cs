using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
    public static class CameraExtensions
    {
        public static Vector3 GetCursorPositionOnPlane(this Camera camera, Vector2 cursorScreenPosition, Plane plane)
        {
            Ray ray = camera.ScreenPointToRay(new Vector3(cursorScreenPosition.x, cursorScreenPosition.y, camera.nearClipPlane));

            if (plane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }
            else
            {
                throw new System.Exception("Cannot project cursor position on plane");
            }
        }
    }
}

