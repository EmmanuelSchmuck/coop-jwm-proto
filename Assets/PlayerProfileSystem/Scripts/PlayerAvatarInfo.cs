using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace PlayerProfileSystem
{
    [CreateAssetMenu(fileName = "PlayerAvatarInfo", menuName = "ScriptableObjects/PlayerAvatarInfo", order = 1)]
    public class PlayerAvatarInfo : ScriptableObject
    {
        [ShowAssetPreview]
        public Sprite avatarPortrait;
        public GameObject avatarCharacter;
        public string avatarName;
    }
}