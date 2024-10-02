using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Toolbox;
using System.Linq;

public class SoundManager : PersistentMonoBehaviourSingleton<SoundManager>
{
	private Dictionary<SoundType, AudioSource> audioSources;

	protected override void Awake()
	{
		base.Awake();

		Initialize();
	}

	private void Initialize()
	{
		AudioSource[] childSources = GetComponentsInChildren<AudioSource>();

		audioSources = new Dictionary<SoundType, AudioSource>();

		foreach (SoundType soundType in EnumUtils.GetValues<SoundType>())
		{
			AudioSource audioSource = childSources.Where(x => x.gameObject.name == soundType.ToString()).FirstOrDefault();

			if(audioSource == null)
			{
				throw new System.Exception($"Audiosource not found for SoundType {soundType}");
			}

			audioSources.Add(soundType, audioSource);
		}
	}

	public void PlaySound(SoundType soundType)
	{
		AudioSource audioSource = audioSources[soundType];

		audioSource.Play();
	}
}
