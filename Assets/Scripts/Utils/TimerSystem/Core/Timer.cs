using System;
using UnityEngine;

namespace Utils.TimerSystem.Core
{
    public abstract class Timer
    {
        public Action OnTimerStart  { get; set; }
        public Action<float> OnChangedTime { get; set; }
        public Action OnTimerForceStop { get; set; } //Принудительное завершение
        public Action OnTimerStop { get; set; } //Завершение по истечении времени
        
        protected MonoBehaviour _behaviour;
        protected float _timeCountdown;
 
        public Timer(MonoBehaviour behaviour, float time)
        {
            _behaviour = behaviour;
            _timeCountdown = time;
        }

        public abstract void Start();

        public abstract void Stop();
    }
}