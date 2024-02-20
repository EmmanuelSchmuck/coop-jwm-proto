using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Toolbox
{
    public static class CoroutineTools
    {
        public static IEnumerator WaitForAll(this MonoBehaviour behaviour, params IEnumerator[] routines)
		{
            int currentlyRunning = 0;

            foreach(var routine in routines)
			{
                behaviour.StartCoroutine(RunAwaitedRoutine(routine));
			}

            while(currentlyRunning != 0)
			{
                yield return null;
			}

            IEnumerator RunAwaitedRoutine(IEnumerator routine)
			{
                currentlyRunning++;
                yield return behaviour.StartCoroutine(routine);
                currentlyRunning--;
			}
		}

        public static IEnumerator Then(this IEnumerator routine, IEnumerator otherRoutine)
        {
            yield return routine;
            yield return otherRoutine;
        }

        public static void TryStopCoroutine(this MonoBehaviour behaviour, Coroutine coroutine)
        {
            if (coroutine == null) return;

            behaviour.StopCoroutine(coroutine);
        }

        public static void RestartCoroutine(this MonoBehaviour behaviour, ref Coroutine coroutine, IEnumerator routine)
        {
            behaviour.TryStopCoroutine(coroutine);

            coroutine = behaviour.StartCoroutine(routine);
        }

        public static IEnumerator WaitUntilConditionThenDoAction(Func<bool> condition, Action action)
        {
            yield return new WaitUntil(condition);

            action.Invoke();
        }

        public static IEnumerator DelayedAction(float delay, Action action, bool useUnscaledTime = false)
        {
            yield return (useUnscaledTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay));

            action.Invoke();
        }

        public static IEnumerator Tween(float from, float to, float duration, Action<float> action, Func<float, float> easing = null, Action onFinish = null, bool useFixedUpdate = false, bool useUnscaledTime = false, bool loop = false, bool reverse = false, float delay = 0, float offset = 0, Func<bool> cancelIf = null)
        {
            float time = offset;
            bool cancelled = false;
            Func<float, float> _easing = easing ?? (x => x);

            // cannot cancel during initial delay...
            if (delay > 0)
            {
                yield return (useUnscaledTime ? new WaitForSecondsRealtime(delay) : new WaitForSeconds(delay));
            }

            while (time < duration)
            {
                if(cancelIf?.Invoke() ?? false) 
				{
                    cancelled = true;
                    break; // exit the while loop
				}

                float alpha = reverse ? 1f - time / duration : time / duration;
                action.Invoke(Mathf.LerpUnclamped(from, to, _easing(alpha)));

                time += useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;

                if (useFixedUpdate)
                {
                    yield return new WaitForFixedUpdate();
                }
                else
                {
                    yield return null;
                }

                if (time >= duration && loop)
                {
                    time = 0f;
                }
            }

            if(cancelled)
			{
                yield break; // exit the routine
			}

            action.Invoke(Mathf.Lerp(from, to, _easing(reverse? 0f : 1f)));

            onFinish?.Invoke();
        }

        public static IEnumerator Tween01(float duration, Action<float> action, Func<float, float> easing = null, Action onFinish = null, bool useFixedUpdate = false, bool useUnscaledTime = false, bool loop = false, bool reverse = false, float delay = 0, float offset = 0, Func<bool> cancelIf = null) => Tween(0, 1, duration, action, easing, onFinish, useFixedUpdate, useUnscaledTime, loop, reverse, delay, offset, cancelIf);

    }
}