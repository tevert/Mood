using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mood.Controllers;
using Mood.Migrations;
using Moq;
using System.Web.Mvc;
using System.Web;
using Mood.Services;
using System.Collections.Generic;
using Mood.Models;
using System.Data.Entity;
using System.Linq;
using Mood.ViewModels;

namespace Mood.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        #region Ctor

        [TestMethod]
        public void Ctor_NullDB_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new HomeController(null, Mock.Of<ISecurity>());
            });

            Assert.AreEqual("db", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_NullSecurity_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new HomeController(Mock.Of<IApplicationDBContext>(), null);
            });

            Assert.AreEqual("security", ex.ParamName);
        }

        #endregion

        #region Index

        [TestMethod]
        public void Index_UserNotLoggedIn_ShowsEmptyView()
        {
            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.IsAuthenticated).Returns(false);
            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new HomeController(dbMock.Object, securityMock.Object);

            var result = subject.Index().Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual("IndexNotAuthenticated", (result as ViewResult).ViewName);
        }

        [TestMethod]
        public void Index_UserLoggedIn_ShowsSurveys()
        {
            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.IsAuthenticated).Returns(true);
            securityMock.SetupGet(s => s.UserName).Returns("tyler");

            var usersMock = new Mock<DbSet<ApplicationUser>>().SetupData(new[]
            {
                new ApplicationUser() { UserName = "tyler", OwnedSurveys = new []{
                    new Survey(), new Survey()
                } }
            });

            var dbMock = new Mock<IApplicationDBContext>();
            dbMock.SetupGet(d => d.Users).Returns(usersMock.Object);

            var subject = new HomeController(dbMock.Object, securityMock.Object);

            var result = subject.Index().Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(2, ((result as ViewResult).Model as HomeViewModel).MySurveys.Count());
        }

        [TestMethod]
        public void Index_UserLoggedIn_ShowsSurveysAndSharedSurveys()
        {
            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.IsAuthenticated).Returns(true);
            securityMock.SetupGet(s => s.UserName).Returns("tyler");

            var usersMock = new Mock<DbSet<ApplicationUser>>().SetupData(new[]
            {
                new ApplicationUser() { UserName = "tyler", OwnedSurveys = new []{
                    new Survey(), new Survey()
                }, SharedSurveys = new  []{
                    new Survey(), new Survey(), new Survey()
                } }
            });

            var dbMock = new Mock<IApplicationDBContext>();
            dbMock.SetupGet(d => d.Users).Returns(usersMock.Object);

            var subject = new HomeController(dbMock.Object, securityMock.Object);

            var result = subject.Index().Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreEqual(2, ((result as ViewResult).Model as HomeViewModel).MySurveys.Count());
            Assert.AreEqual(3, ((result as ViewResult).Model as HomeViewModel).SharedSurveys.Count());
        }

        #endregion
    }
}
