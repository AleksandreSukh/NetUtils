using System;

namespace NetUtils
{
    //public delegate string StringFunctionEmpty();
    public delegate string StringFunction(params object[] parameters);
    //Try to make this with Rx method
    public static class StringFunctionHelper
    {
        public static string TryGetString(StringFunction stringFunction, int interval, double intervalGrowthCoefficient, int maxIterations, int iteration = 1)
        {
            Exception firstException = null;

            while (true)
            {
                //Exception lastException = null;
                string result = null;
                if (iteration > maxIterations) return null;

                var lastIteration = iteration == maxIterations;
                if (lastIteration)
                {
                    try
                    {
                        result = stringFunction();
                    }
                    catch
                    {
                        if (firstException != null)
                            throw firstException;
                        throw;
                    }
                }
                else
                {
                    try
                    { result = stringFunction(); }
                    catch (Exception ex)
                    {
                        if (iteration == 1)
                            firstException = ex;
                    }
                }

                if (lastIteration || !String.IsNullOrEmpty(result)) return result;

                ThreadHelper.Sleep(interval);
                interval = (int)(intervalGrowthCoefficient * interval);
                iteration = ++iteration;
            }
        }

    }
}