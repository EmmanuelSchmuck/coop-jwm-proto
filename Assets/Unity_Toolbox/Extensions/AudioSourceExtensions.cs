using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Toolbox
{
	public static class AudioSourceExtensionMethods
	{
		public static IEnumerator FadeVolume(this AudioSource audioSource, float targetVolume, float duration)
		{
			float startVolume = audioSource.volume;
			yield return CoroutineTools.Tween01(duration, t => audioSource.volume = Mathf.Lerp(startVolume, targetVolume, t));
		}
	}
}