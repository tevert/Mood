using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Mood.Tests
{
    public static class Helpers
    {
        public static T MockUrl<T>(this T controller, string url) where T : Controller
        {
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(c => c.HttpContext.Request.Url).Returns(new Uri(url));

            controller.ControllerContext = mock.Object;

            return controller;
        }

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
