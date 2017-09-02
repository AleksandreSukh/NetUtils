using System.Threading;

namespace NetUtils
{
    public static class ThreadHelper
    {
        public static void Sleep(int milliSeconds)
        {
            if (milliSeconds == 0) return;

            var locker = new object();
            lock (locker)
            {
                // ReSharper disable once UnusedVariable
                using (var t = new Timer(
                    state =>
                    { lock (locker) Monitor.Pulse(locker); },
                    null, milliSeconds, -1))

                    Monitor.Wait(locker);
            }
        }
    }
}