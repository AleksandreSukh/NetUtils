using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Net3Migrations.Delegates;

namespace NetUtils
{
    public delegate T TruncWrapperTrunc<T>(object locker, Trunc<T> actionToRun);
    public delegate void FictionWrapperTrunc(object locker, Fiction actionToRun);

    public class TimelyFunctionException : Exception
    {
        public TimelyFunctionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
    //todo: duplicates may be killed here like in SafeLock
    public static class TimelyFunctionHelpers
    {
        static T GetInTime<T>(TruncWrapperTrunc<T> wrapperAction, Trunc<T> innerAction, int timeout)
        {
            object monitorSync = new object();
            bool timedOut;
            IAsyncResult result;
            lock (monitorSync)
            {
                result = wrapperAction.BeginInvoke(monitorSync, innerAction, null, null);
                timedOut = !Monitor.Wait(monitorSync, TimeSpan.FromMilliseconds(timeout));
            }
            if (timedOut)
                throw new TimeoutException("Couldn't get result in time");
            return wrapperAction.EndInvoke(result);
        }
        ///Provide exceptionHandler in case you aren't sure that all exceptions in innerAction are handled because unhandled exceptions in asynchronous calls causes application crash


        public static T GetInTime<T>(this Trunc<T> innerAction, int timeout, ExceptionHandlerDelegate exceptionHandler)
        {
            TruncWrapperTrunc<T> rame = (o, run) =>
            {
                object result = null;
                bool resultReturned = false;
                try
                {
                    result = run();
                    resultReturned = true;
                }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                        exceptionHandler.Invoke(new TimelyFunctionException("Exception occured. See inner exception", ex));
                    else { throw; }
                }
                lock (o) Monitor.Pulse(o);
                if (resultReturned)
                    return (T)result;
                return default(T);
            };
            return GetInTime(rame, innerAction, timeout);
        }
        public static T GetInTime<T>(this Trunc<T> innerAction, int timeout)
        {
            Exception exceptionToThrow = null;
            TruncWrapperTrunc<T> rame = (o, run) =>
            {
                object result = null;
                bool resultReturned = false;
                try
                {
                    result = run();
                    resultReturned = true;
                }
                catch (Exception ex)
                {
                    exceptionToThrow = new TimelyFunctionException("Exception occured. See inner exception", ex);
                }
                lock (o) Monitor.Pulse(o);
                if (resultReturned)
                    return (T)result;
                return default(T);
            };
            var resultToReturn = GetInTime(rame, innerAction, timeout);
            if (exceptionToThrow != null)
                throw exceptionToThrow;
            return resultToReturn;
        }


        static void RunInTime(FictionWrapperTrunc wrapperAction, Fiction innerAction, int timeout)
        {
            object monitorSync = new object();
            bool timedOut;
            IAsyncResult result;
            lock (monitorSync)
            {
                result = wrapperAction.BeginInvoke(monitorSync, innerAction, null, null);
                timedOut = !Monitor.Wait(monitorSync, TimeSpan.FromMilliseconds(timeout));
            }
            if (timedOut)
                throw new TimeoutException("Couldn't get result in time");
            wrapperAction.EndInvoke(result);
        }
        ///Provide exceptionHandler in case you aren't sure that all exceptions in innerAction are handled because unhandled exceptions in asynchronous calls causes application crash
        public static void RunInTime(this Fiction innerAction, int timeout, ExceptionHandlerDelegate exceptionHandler)
        {

            FictionWrapperTrunc rame = (o, run) =>
            {
                try { run(); }
                catch (Exception ex)
                {
                    if (exceptionHandler != null)
                        exceptionHandler.Invoke(new TimelyFunctionException("Exception occured. See inner exception", ex));
                    else { throw; }
                }
                lock (o) Monitor.Pulse(o);
            };
            RunInTime(rame, innerAction, timeout);
        }

        public static void RunInTime(this Fiction innerAction, int timeout)
        {
            Exception exceptionToThrow = null;

            FictionWrapperTrunc rame = (o, run) =>
            {
                try { run(); }
                catch (Exception ex)
                {
                    exceptionToThrow = new TimelyFunctionException("Exception occured. See inner exception", ex); ;
                }
                lock (o) Monitor.Pulse(o);
            };
            RunInTime(rame, innerAction, timeout);
            if (exceptionToThrow != null)
                throw exceptionToThrow;
        }
    }

    public delegate void ExceptionHandlerDelegate(Exception ex);
}
