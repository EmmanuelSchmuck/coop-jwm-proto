using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace PlayerProfileSystem
{
	public class PlayerProfileSetup : MonoBehaviour
	{
		[SerializeField] private PlayerAvatarInfo[] avatarInfos;
		[SerializeField] private Transform optionGrid;
		[SerializeField] private Transform characterContainer;
		[SerializeField] private RuntimeAnimatorController animController;
		[SerializeField] private float characterRotationSpeed;
		[SerializeField] private float characterBaseRotation = 180f;
		[SerializeField] private Image portraitImage;
		[SerializeField] private TMPro.TMP_InputField playerNameInputField;
		[SerializeField] private Button validateButton;
		public string PlayerName { get; private set; }
		public PlayerAvatarInfo SelectedAvatarInfo { get; private set; }
		public PlayerInfo PlayerInfo { get; private set; }
		public event System.Action ValidateButtonClicked;

		void Start()
		{
			int i = 0;

			int avatarCount = avatarInfos.Length;

			foreach (var gridElement in optionGrid.GetComponentsInChildren<AvatarOptionGridElement>())
			{
				gridElement.Clicked += OnOptionElementClicked;
				gridElement.Initialize(i < avatarCount ? avatarInfos[i] : null);
				i++;
			}

			playerNameInputField.onValueChanged.AddListener(OnPlayerNameInputChanged);

			SelectAvatarInfo(avatarInfos.First());

			validateButton.interactable = false;

			validateButton.onClick.AddListener(OnValidateButtonClick);
		}

		private void OnValidateButtonClick()
		{
			PlayerInfo = new PlayerInfo() {playerAvatar = SelectedAvatarInfo, playerName = PlayerName };
			ValidateButtonClicked?.Invoke();
		}

		private void OnPlayerNameInputChanged(string value)
		{
			PlayerName = value;

			validateButton.interactable = value != "" && value.Length > 2;
		}

		private void Update()
		{
			characterContainer.Rotate(Vector3.up, characterRotationSpeed * Time.deltaTime, Space.World);
		}

		private void OnOptionElementClicked(AvatarOptionGridElement option)
		{
			SelectAvatarInfo(option.AvatarInfo);
		}

		private void SelectAvatarInfo(PlayerAvatarInfo avatarInfo)
		{
			if (avatarInfo == SelectedAvatarInfo) return;

			if (characterContainer.GetComponentInChildren<Animator>().gameObject is GameObject toDestroy)
			{
				Destroy(toDestroy);
			}

			GameObject newCharacter = Instantiate(avatarInfo.avatarCharacter, characterContainer);
			newCharacter.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
			newCharacter.GetComponentInChildren<Animator>().runtimeAnimatorController = animController;

			portraitImage.sprite = avatarInfo.avatarPortrait;

			SelectedAvatarInfo = avatarInfo;

			characterContainer.rotation = Quaternion.Euler(0, characterBaseRotation, 0);
		}
	}
}