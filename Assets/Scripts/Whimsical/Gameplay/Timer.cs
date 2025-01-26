namespace Whimsical.Gameplay
{
    using System;
    using UnityEngine;

    public class Timer : MonoBehaviour
    {
        private float targetTime;

        private Action _finishedAction;
        public event Action OnTimeElapsed
        {
            add => _finishedAction += value;
            remove => _finishedAction -= value;
        }

        public float TargetTime
        {
            get => targetTime;
            set
            {
                if (value <= 0)
                    throw new InvalidOperationException($"The time can't be non positive. current value: {value}");

                targetTime = value;
            }
        }

        public float ElapsedTime { get; private set; }
        public bool IsFinished { get; private set; }
        public bool IsStarted { get; private set; }
        public bool IsRunning => IsStarted && !IsFinished;

        public void Update()
        {
            if (IsFinished || !IsStarted) return;

            ElapsedTime += Time.deltaTime;
            
            if (ElapsedTime < targetTime) return;
            IsFinished = true;
            _finishedAction?.Invoke();
        }

        public void StartTimer()
        {
            IsStarted = true;
        }

        public void StopTimer()
        {
            IsStarted = false;
            IsFinished = false;
            ElapsedTime = 0.0f;
        }

        public void RestartTimer()
        {
            this.StopTimer();
            this.StartTimer();
        }
    }
}
