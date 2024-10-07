using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using NaughtyAttributes;

namespace PlayerAvatar.PortraitGeneration
{
    public class PortraitGenerator : MonoBehaviour
    {
        [SerializeField] private Camera portraitCamera;
        [SerializeField] private RenderTexture renderTexture;
        [SerializeField] private string portraitFolder;
        [SerializeField] private string portraitName;
        [SerializeField] private string fileExtension;
        private string assetPath => Path.Combine(Application.dataPath, portraitFolder);
        [Button("Take Picture")]
        public void TakePortraitPicture()
		{

            RenderTexture prevRT = RenderTexture.active;
            RenderTexture rt = renderTexture; // new RenderTexture(new RenderTextureDescriptor(256,256,UnityEngine.Experimental.Rendering.GraphicsFormat.R16G16B16A16_UNorm,0));
            //rt.antiAliasing = 2;
            
            Texture2D tex = new(256, 256, TextureFormat.ARGB32, false); //, rt.graphicsFormat, UnityEngine.Experimental.Rendering.TextureCreationFlags.Crunch);
            //tex.graphicsFormat = rt.graphicsFormat;

            portraitCamera.targetTexture = rt;
            portraitCamera.Render();

            RenderTexture.active = rt;

            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);

            byte[] bytes = tex.EncodeToPNG();
            var dirPath = assetPath;
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            File.WriteAllBytes(Path.Combine(dirPath, portraitName + fileExtension), bytes);
            Debug.Log($"Picture taken {dirPath + portraitName + fileExtension}");
            AssetDatabase.Refresh();
            RenderTexture.active = prevRT;
            rt.Release();

            portraitCamera.targetTexture = null;
        }
    }
}


