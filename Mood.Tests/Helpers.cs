using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Mood.Tests
{
    public static class Helpers
    {
        public static T Catch<T>(Action action) where T : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (T ex)
            {
                return ex;
            }
            catch (AggregateException ex)
            {
                var tEx = ex.InnerExceptions.OfType<T>().FirstOrDefault();
                if (tEx == null)
                {
                    throw;
                }
                return tEx;
            }

            Assert.Fail($"Expected exception of type {typeof(T)}, but did not encounter.");
            return null;
        }
    }
}
