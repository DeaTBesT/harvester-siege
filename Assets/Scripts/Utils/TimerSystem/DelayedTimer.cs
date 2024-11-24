using System.Collections;
using UnityEngine;
using Utils.TimerSystem.Core;

namespace Utils.TimerSystem
{
    public class DelayedTimer : Timer
    {
        private float _delayedTime;
        
        private Coroutine _timerRoutine;

        public DelayedTimer(MonoBehaviour behaviour, float time, float delayedTime) : base(behaviour, time) => 
            _delayedTime = delayedTime;

        public override void Start()
        {
            if (_timerRoutine != null)
            {
                Stop();
            }

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
            var delayedTime = _delayedTime;
            
            while (delayedTime > 0)
            {
                delayedTime -= Time.deltaTime;
                
                yield return null;
            }
            
            OnTimerStart?.Invoke();

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