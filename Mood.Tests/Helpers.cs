using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

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

            Assert.Fail($"Expected exception of type {typeof(T)}, but did not encounter.");
            return null;
        }
    }
}
