using System;
using System.Collections;
using UnityEngine;

namespace Utils
{
    public class Timer
    {
        public Action OnTimerStart  { get; set; }
        public Action<float> OnChangedTime { get; set; }
        public Action OnTimerForceStop { get; set; } //Принудительное завершение
        public Action OnTimerStop { get; set; } //Завершение по истечении времени
        
        private MonoBehaviour _behaviour;
        private float _timeCountdown;

        private Coroutine _timerRoutine;
        
        public Timer(MonoBehaviour behaviour) => 
            _behaviour = behaviour;

        public Timer(MonoBehaviour behaviour, float time)
        {
            _behaviour = behaviour;
            _timeCountdown = time;
        }

        public void SetTime(float time) => 
            _timeCountdown = time;
        
        public void Start()
        {
            Debug.Log("Start timer");
            if (_timerRoutine != null)
            {
                Stop();
            }

            OnTimerStart?.Invoke();
            _timerRoutine = _behaviour.StartCoroutine(TimerRoutine());
        }

        public void Stop()
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