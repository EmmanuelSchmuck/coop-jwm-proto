using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using NaughtyAttributes;

namespace PlayerProfileSystem.PortraitGeneration
{
    public class AvatarInfoSetup : MonoBehaviour
    {
        [SerializeField] private PlayerAvatarInfo[] avatarInfos;
        [SerializeField] private int avatarIndex;
        [SerializeField] private string portraitFolder;
        [SerializeField] private string portraitName;
        [SerializeField] private string fileExtension;
        [SerializeField] private Transform characterContainer;
        [SerializeField] private Camera portraitCamera;
        [SerializeField] private RenderTexture renderTexture;
        private string assetPath => Path.Combine(Application.dataPath, portraitFolder);
        private string localFolderPath => Path.Combine("Assets", portraitFolder);
        private GameObject currentCharacter;
        [Button("Setup All AvatarInfos")]
        public void SetupAllAvatarInfos()
		{
            foreach (var character in characterContainer.GetComponentsInChildren<Animator>(includeInactive: true))
            {
                character.gameObject.SetActive(false);
            }

            for (int i = 0; i<avatarInfos.Length;i++)
			{
                currentCharacter = characterContainer.GetChild(i).gameObject;
                currentCharacter.SetActive(true);
                avatarIndex = i;
                TakePortraitPicture();
                currentCharacter.SetActive(false);
            }
		}

        [Button("UpdateCharacterFromIndex")]
        public void UpdateCharacterFromIndex()
		{
            int i = 0;
            foreach(var character in characterContainer.GetComponentsInChildren<Animator>(includeInactive: true))
			{
                character.gameObject.SetActive(i == avatarIndex);
                if (i == avatarIndex) currentCharacter = character.gameObject;
                i++;
            }
		}
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
            string path = Path.Combine(dirPath, portraitName + avatarIndex + fileExtension);
            string localPath = Path.Combine(localFolderPath, portraitName + avatarIndex + fileExtension); 
            File.WriteAllBytes(path, bytes);
            Debug.Log($"Picture taken {path}");
            AssetDatabase.Refresh();
            RenderTexture.active = prevRT;
            rt.Release();

            portraitCamera.targetTexture = null;

            Sprite newSprite = AssetDatabase.LoadAssetAtPath(localPath, typeof(Sprite)) as Sprite;
            avatarInfos[avatarIndex].avatarPortrait = newSprite;
            Debug.Log($"Assigned new sprite : {newSprite}, at path {localPath}");

            avatarInfos[avatarIndex].avatarCharacter = PrefabUtility.GetCorrespondingObjectFromSource<GameObject>(currentCharacter);

            AssetDatabase.Refresh();
        }
    }
}


