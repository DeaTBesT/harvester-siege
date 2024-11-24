using System.Collections;
using UnityEngine;
using Utils.TimerSystem.Core;

namespace Utils.TimerSystem
{
    public class SimpleTimer : Timer
    {
        private Coroutine _timerRoutine;

        public SimpleTimer(MonoBehaviour behaviour, float time) : base(behaviour, time)
        {
        }

        public override void Start()
        {
            if (_timerRoutine != null)
            {
                Stop();
            }

            OnTimerStart?.Invoke();
            _timerRoutine = _behaviour.StartCoroutine(TimerRoutine());
        }

        public override void Stop()
        {
            if (_timerRoutine == null)
            {
#if UNITY_EDITOR
                Debug.LogWarning("You try stop none active timer");
#endif
                return;
            }

            OnTimerForceStop?.Invoke();
            _behaviour.StopCoroutine(_timerRoutine);
        }

        private IEnumerator TimerRoutine()
        {
            var currentTime = _timeCountdown;

            while (currentTime > 0)
            {
                currentTime -= Time.deltaTime;
                OnChangedTime?.Invoke(currentTime);

                yield return null;
            }

            OnTimerStop?.Invoke();
            _timerRoutine = null;
        }
    }
}