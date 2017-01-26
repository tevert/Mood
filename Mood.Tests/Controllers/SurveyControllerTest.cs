using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mood.Controllers;
using Mood.Migrations;
using Mood.Models;
using Mood.Services;
using Mood.Services.Exceptions;
using Mood.ViewModels;
using Moq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Mood.Tests.Controllers
{
    [TestClass]
    public class SurveyControllerTest
    {
        #region Ctor
        
        [TestMethod]
        public void Ctor_NullDB_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new SurveyController(null, Mock.Of<ISurveyService>(), Mock.Of<ISecurity>());
            });

            Assert.AreEqual("db", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_NullTime_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new SurveyController(Mock.Of<IApplicationDBContext>(), null, Mock.Of<ISecurity>());
            });

            Assert.AreEqual("surveys", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_NullSecurity_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new SurveyController(Mock.Of<IApplicationDBContext>(), Mock.Of<ISurveyService>(), null);
            });

            Assert.AreEqual("security", ex.ParamName);
        }

        #endregion

        #region Get

        [TestMethod]
        public void Get_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync((Survey)null);

            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, Mock.Of<ISecurity>());

            var result = subject.Get(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Get_IdOK_ReturnsViewWithMoods()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Description = "test survey" };
            var moods = new List<Models.Mood>() { new Models.Mood(), new Models.Mood() };

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, Mock.Of<ISecurity>());

            var result = subject.Get(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreSame(survey, ((result as ViewResult).Model as SurveyViewModel).Survey);
            Assert.AreEqual(2, ((result as ViewResult).Model as SurveyViewModel).Moods.Count());
        }

        #endregion

        #region Create

        [TestMethod]
        public void Create_RequestOK_ReturnsRedirectToNewSurvey()
        {
            var surveyMock = new Mock<ISurveyService>();

            var dbMock = new Mock<IApplicationDBContext>();
            var userDataMock = new Mock<DbSet<ApplicationUser>>().SetupData(new[] { new ApplicationUser() { UserName = "tyler" } });
            dbMock.SetupGet(db => db.Users).Returns(userDataMock.Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("tyler");

            var subject = new SurveyController(dbMock.Object, surveyMock.Object, securityMock.Object);

            var result = subject.Create("Question?").Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            surveyMock.Verify(d => d.AddAsync(It.Is<Survey>(s => s.Owner.UserName == "tyler" && s.Description == "Question?")));
        }

        #endregion

        #region Delete

        [TestMethod]
        public void Delete_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync((Survey)null);

            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, Mock.Of<ISecurity>());

            var result = subject.Delete(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Delete_WrongUser_Returns403()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Peter");

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, securityMock.Object);

            var result = subject.Delete(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(403, (result as HttpStatusCodeResult).StatusCode);
        }

        [TestMethod]
        public void Delete_IdOK_DeletesAnswersAndDeletesSurveyAndRedirectsToHome()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Tyler");

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, securityMock.Object);

            var result = subject.Delete(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            surveysMock.Verify(d => d.DeleteAsync(survey));
        }

        #endregion
        
        #region Edit

        [TestMethod]
        public void Edit_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync((Survey)null);

            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, Mock.Of<ISecurity>());

            var result = subject.Edit(id.ToString(), new SurveyEditViewModel()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Edit_WrongUser_Returns403()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };

            var dbMock = new Mock<IApplicationDBContext>();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Peter");

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, securityMock.Object);

            var result = subject.Edit(id.ToString(), new SurveyEditViewModel()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(403, (result as HttpStatusCodeResult).StatusCode);
        }

        [TestMethod]
        public void Edit_ServiceException_ReturnsErrorJson()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };
            var newData = new SurveyEditViewModel() { };
            var error = "Bad data!";

            var dbMock = new Mock<IApplicationDBContext>();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);
            surveysMock.Setup(s => s.EditAsync(survey, newData)).Throws(new SurveyException(error));

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Tyler");

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, securityMock.Object);

            var result = subject.Edit(id.ToString(), newData).Result;

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            dynamic jsonData = (result as JsonResult).Data;
            Assert.AreEqual(error, jsonData.error);
        }

        [TestMethod]
        public void Edit_UserIsCoOwner_ResetsCoOwnersArgAndReturnsSuccessJson()
        {
            var id = Guid.NewGuid();
            var survey = new Survey()
            {
                Id = id,
                Owner = new ApplicationUser() { UserName = "Tyler" },
                SharedUsers = new[] { new ApplicationUser() { UserName = "Shannon" } }
            };
            var newData = new SurveyEditViewModel() { SharedUsers = new[] { "Shannon", "Bill" } };

            var dbMock = new Mock<IApplicationDBContext>();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Shannon");

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, securityMock.Object);

            var result = subject.Edit(id.ToString(), newData).Result;

            surveysMock.Verify(s => s.EditAsync(survey,
                It.Is<SurveyEditViewModel>(vm =>
                    vm == newData &&
                    vm.SharedUsers.Count() == 1 &&
                    vm.SharedUsers.First() == "Shannon")));
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            dynamic jsonData = (result as JsonResult).Data;
            Assert.IsNotNull(jsonData.success);
        }

        [TestMethod]
        public void Edit_ServiceOK_ReturnsSuccessJson()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };
            var newData = new SurveyEditViewModel() { };

            var dbMock = new Mock<IApplicationDBContext>();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Tyler");

            var subject = new SurveyController(dbMock.Object, surveysMock.Object, securityMock.Object);

            var result = subject.Edit(id.ToString(), newData).Result;

            surveysMock.Verify(s => s.EditAsync(survey, newData));
            Assert.IsInstanceOfType(result, typeof(JsonResult));
            dynamic jsonData = (result as JsonResult).Data;
            Assert.IsNotNull(jsonData.success);
        }

        #endregion
    }
}
