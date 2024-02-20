using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Toolbox
{
    public static class ImageExtensions
    {
        public static Material CloneMaterial(this Image image)
        {
            Material instance = Object.Instantiate(image.material);
            image.material = instance;
            return instance;
        }
    }
}