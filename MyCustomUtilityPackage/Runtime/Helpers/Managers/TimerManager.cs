using System;
using System.Collections.Generic;
using DH.Core.Helpers.Utility;

namespace DH.Core.Helpers.Managers
{
    public static class TimerManager
    {
        public static List<Timer> Timers = new List<Timer>();
        public static Timer RegisterTimer(float time, Action onComplete, Action onUpdate, bool autoStart = true)
        {
            Timer newTimer = Timer.RegisterTimer(time, onComplete, onUpdate, autoStart);
            onComplete += () => Timers.Remove(newTimer);
            Timers.Add(newTimer);
            return newTimer;
        }

        public static void PauseAllTimers()
        {
            foreach (var timer in Timers)
            {
                timer.PauseTimer();
            }
        }

        public static void ResumeAllTimers()
        {
            foreach (var timer in Timers)
            {
                timer.ResumeTimer();
            }
        }
    }
}

