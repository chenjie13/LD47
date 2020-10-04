using RSG;
using UnityEngine;

namespace CGDK.Core
{
    public static class TimerUtil
    {
        public static IPromiseTimer RealTimer;
        public static IPromiseTimer ScaledTimer;

        static TimerUtil()
        {
            RealTimer = new PromiseTimer();
            ScaledTimer = new PromiseTimer();

            MonoAgent.Inst.OnUpdate += UpdateTimer;
        }

        private static void UpdateTimer()
        {
            RealTimer.Update(Time.unscaledDeltaTime);
            ScaledTimer.Update(Time.deltaTime);
        }
    }
}