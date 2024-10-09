using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PlayerProfileSystem
{
    public class AvatarOptionGridElement : MonoBehaviour
    {
        [SerializeField] private Image portraitImage;
        [SerializeField] private Image emptyImage;
        public event System.Action<AvatarOptionGridElement> Clicked;
        public PlayerAvatarInfo AvatarInfo { get; private set; }
        public void Initialize(PlayerAvatarInfo avatarInfo)
		{
            this.AvatarInfo = avatarInfo;

            if(AvatarInfo != null)
			{
                portraitImage.sprite = avatarInfo.avatarPortrait;
            }
            else
			{
                emptyImage.enabled = true;
            }
        }

        public void OnClick()
		{
            Clicked?.Invoke(this);
		}
    }
}