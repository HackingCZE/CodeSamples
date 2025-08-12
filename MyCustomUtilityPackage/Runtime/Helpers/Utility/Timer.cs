using System;
using System.Collections;
using UnityEngine;

namespace DH.Core.Helpers.Utility
{
    public class Timer
    {
        public Timer(float time, Action onComplete, Action onUpdate = null)
        {
            TimeRemaining = time;
            OnComplete = onComplete;
            OnUpdate = onUpdate;
        }

        public float TimeRemaining { get; private set; }
        public bool IsRunning { get; private set; }
        public Action OnComplete { get; private set; }
        public Action OnPause { get; set; }
        public Action OnStart { get; set; }
        public Action OnResume { get; set; }
        public Action OnUpdate { get; set; }
        private Coroutine _currentCoroutine;
        public static Timer RegisterTimer(float time, Action onComplete, Action onUpdate, bool autoStart = true)
        {
            Timer timer = new Timer(time, onComplete, onUpdate);
            if (autoStart) timer.StartTimer();

            return timer;
        }

        public void StartTimer()
        {
            IsRunning = true;
            OnStart?.Invoke();
            _currentCoroutine = CoroutineHelper.Instance.StartCoroutine(TimerCoroutine());
        }

        public void PauseTimer()
        {
            IsRunning = false;
            OnPause?.Invoke();
            CoroutineHelper.Instance.StopCoroutine(_currentCoroutine);
        }

        public void ResumeTimer()
        {
            IsRunning = true;
            OnResume?.Invoke();
            StartTimer();
        }

        public void StopTimer()
        {
            IsRunning = false;
            OnComplete?.Invoke();
            if (_currentCoroutine != null) CoroutineHelper.Instance.StopCoroutine(_currentCoroutine);
        }

        private IEnumerator TimerCoroutine()
        {
            while (TimeRemaining > 0)
            {
                TimeRemaining -= Time.deltaTime;
                yield return null;
                OnUpdate?.Invoke();
            }
            TimeRemaining = 0;
            OnUpdate?.Invoke();

            StopTimer();
        }
    }
}

