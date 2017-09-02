using System;
using System.Collections.Generic;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Net3Migrations.Delegates;

namespace NetUtils
{
    public interface IMutexHelpers
    {
        void RunActionAcrossMutex(string mutexid, Fiction action, MutexSecurity mutexSecurity = null, int waitingTimeout = Timeout.Infinite, int executionTimeout = Timeout.Infinite);
        void RunActionAcrossGlobalMutex(string mutexid, Fiction action, int waitingTimeout = Timeout.Infinite, int executionTimeout = Timeout.Infinite);
        void TakeMutexAndThenRelease(Mutex startingMutex);
        bool CrossMutex(string mutexid, MutexSecurity mutexSecurity = null, int timeout = Timeout.Infinite);
        bool CrossGlobalMutex(string mutexid, int timeout = Timeout.Infinite);
        //void WaitForMutexHolderToAppear(string mutexId, MutexSecurity mutexSecurity);
        //bool MutexAvailable(Mutex mutex, int waitMiliseconds);
    }
    public class MutexHelpers : IMutexHelpers
    {
        /// <summary>
        /// Throws: TimelyFunctionException in case action could not fit in excecutionTimeout
        /// </summary>
        /// <param name="mutexid"></param>
        /// <param name="action"></param>
        /// <param name="mutexSecurity"></param>
        /// <param name="waitingTimeout"></param>
        /// <param name="executionTimeout"></param>
        public void RunActionAcrossMutex(string mutexid, Fiction action, MutexSecurity mutexSecurity = null, int waitingTimeout = Timeout.Infinite, int executionTimeout = Timeout.Infinite)
        {
            bool createdNew;
            using (var mutex = mutexSecurity == null ? new Mutex(false, mutexid, out createdNew) : new Mutex(false, mutexid, out createdNew, mutexSecurity))
            {
                var hasHandle = false;
                try
                {
                    try
                    {
                        // note, you may want to time out here instead of waiting forever
                        //mutex.WaitOne(Timeout.Infinite, false);
                        hasHandle = mutex.WaitOne(waitingTimeout, false);
                        if (hasHandle == false)
                            throw new TimeoutException("Timeout waiting for exclusive access");
                    }
                    catch (AbandonedMutexException)
                    {
                        // Log the fact the mutex was abandoned in another process, it will still get aquired
                        hasHandle = true;
                    }
                    if (executionTimeout == Timeout.Infinite)
                        action();
                    else action.RunInTime(executionTimeout);
                    // Perform your work here.
                }
                finally
                {
                    // added if statemnet
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }
            }
        }
        public static MutexSecurity GlobalAccess;

        static MutexHelpers()
        {
            var allowEveryoneRule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            GlobalAccess = new MutexSecurity();
            GlobalAccess.AddAccessRule(allowEveryoneRule);
        }

        public void RunActionAcrossGlobalMutex(string mutexid, Fiction action, int waitingTimeout = Timeout.Infinite,
            int executionTimeout = Timeout.Infinite)
        {
            mutexid = PrependGlobalPrefixIfMissing(mutexid);
            RunActionAcrossMutex(mutexid, action, GlobalAccess, waitingTimeout, executionTimeout);
        }

        string PrependGlobalPrefixIfMissing(string mutexid)
        {
            if (!mutexid.StartsWith(MutexHelperConstants.GlobalMutexPrefix, StringComparison.InvariantCultureIgnoreCase))
                return MutexHelperConstants.GlobalMutexPrefix + mutexid;
            return mutexid;
        }

        public void TakeMutexAndThenRelease(Mutex mutex)
        {
            try { mutex.WaitOne(Timeout.Infinite, false); }
            catch (AbandonedMutexException)
            {
                //GlobalVariables.Instance.TextLogger.WriteLog2TextFile("Starting Mutex abandoned");
            }
            try { mutex.ReleaseMutex(); }
            catch //(Exception ex)
            {
                //GlobalVariables.Instance.TextLogger.WriteLog2TextFileex.w2t(); 
            }
        }

        public bool CrossMutex(string mutexid, MutexSecurity mutexSecurity = null, int timeout = Timeout.Infinite)
        {
            if (string.IsNullOrEmpty(mutexid))
                throw new ArgumentNullException($"Argument \"{nameof(mutexid)}\" mustn't be null or empty");

            bool createdNew;
            using (
                var mutex = mutexSecurity == null
                    ? new Mutex(false, mutexid, out createdNew)
                    : new Mutex(false, mutexid, out createdNew, mutexSecurity))
            {
                var hasHandle = false;
                try
                {
                    try
                    {
                        hasHandle = mutex.WaitOne(timeout, false);
                        return hasHandle;
                    }
                    catch (AbandonedMutexException)
                    {
                        return true;
                    }
                }
                finally
                {
                    // added if statemnet
                    if (hasHandle)
                        mutex.ReleaseMutex();
                }

            }
        }

        public bool CrossGlobalMutex(string mutexid, int timeout = Timeout.Infinite)
        {
            mutexid = PrependGlobalPrefixIfMissing(mutexid);

            //if (!mutexid.StartsWith(MutexHelperConstants.GlobalMutexPrefix))
            //    throw new ArgumentException($"Argument \"{mutexid}\" should be valid global mutex id. It should start with \"{MutexHelperConstants.GlobalMutexPrefix}\" ");
            return CrossMutex(mutexid, GlobalAccess, timeout);
        }

    }

    public static class MutexHelperConstants
    {
        public const string GlobalMutexPrefix = "Global\\";
    }

}
