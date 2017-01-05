using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mood.Controllers;
using Mood.Migrations;
using Mood.Models;
using Mood.Services;
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
                new SurveyController(null, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());
            });

            Assert.AreEqual("db", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_NullTime_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new SurveyController(Mock.Of<IApplicationDBContext>(), null, Mock.Of<ISecurity>());
            });

            Assert.AreEqual("time", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_NullSecurity_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new SurveyController(Mock.Of<IApplicationDBContext>(), Mock.Of<IDateTimeService>(), null);
            });

            Assert.AreEqual("security", ex.ParamName);
        }

        #endregion

        #region View

        [TestMethod]
        public void View_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { new Survey() { Id = Guid.NewGuid() } });
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Get(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void View_IdOK_ReturnsViewWithMoods()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Description = "test survey" };
            var moods = new List<Models.Mood>() { new Models.Mood(), new Models.Mood() };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Get(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.AreSame(survey, ((result as ViewResult).Model as SurveyViewModel).Survey);
            Assert.AreEqual(2, ((result as ViewResult).Model as SurveyViewModel).Moods.Count());
        }

        #endregion

        #region Answer

        [TestMethod]
        public void Answer_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();
            var moodId = 10;
            var moods = new List<Models.Mood>() { new Models.Mood() { Id = 10 }, new Models.Mood() { Id = 20 } };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { new Survey() { Id = Guid.NewGuid() } });
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Answer(id.ToString(), moodId).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Answer_MoodIdNotFound_Returns404()
        {
            var id = Guid.NewGuid();
            var moodId = 5;
            var survey = new Survey() { Id = id };
            var moods = new List<Models.Mood>() { new Models.Mood() { Id = 10 }, new Models.Mood() { Id = 20 } };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Answer(id.ToString(), moodId).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Answer_IdsOK_CreatesAnswerAndReturnsModel()
        {
            var id = Guid.NewGuid();
            var moodId = 10;
            var survey = new Survey() { Id = id };
            var moods = new List<Models.Mood>() { new Models.Mood() { Id = moodId }, new Models.Mood() { Id = 20 } };
            var time = DateTime.Now;

            var timeMock = new Mock<IDateTimeService>();
            timeMock.Setup(t => t.Now()).Returns(time);
            
            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            var answersDataMock = new Mock<DbSet<Answer>>();
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);
            dbMock.Setup(db => db.Answers).Returns(answersDataMock.Object);

            var subject = new SurveyController(dbMock.Object, timeMock.Object, Mock.Of<ISecurity>());

            var result = subject.Answer(id.ToString(), moodId).Result;

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsInstanceOfType((result as JsonResult).Data, typeof(Answer));
            var answer = (result as JsonResult).Data as Answer;
            Assert.AreSame(survey, answer.Survey);
            Assert.AreSame(moods[0], answer.Mood);
            Assert.AreEqual(time, answer.Time);

            answersDataMock.Verify(d => d.Add(It.Is<Answer>(a => a.Mood == moods[0] && a.Survey == survey && a.Time == time)));
            dbMock.Verify(db => db.SaveChangesAsync());
        }

        #endregion

        #region Create

        [TestMethod]
        public void Create_RequestOK_ReturnsRedirectToNewSurvey()
        {
            var timeMock = new Mock<IDateTimeService>();

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData();
            var userDataMock = new Mock<DbSet<ApplicationUser>>().SetupData(new[] { new ApplicationUser() { UserName = "tyler" } });
            dbMock.SetupGet(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.SetupGet(db => db.Users).Returns(userDataMock.Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("tyler");

            var subject = new SurveyController(dbMock.Object, timeMock.Object, securityMock.Object);

            var result = subject.Create().Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            surveyDataMock.Verify(d => d.Add(It.Is<Survey>(s => s.Owner.UserName == "tyler" && s.Description == "Default")));
            dbMock.Verify(db => db.SaveChangesAsync());
        }

        [TestMethod]
        public void Create_RequestIncludesDescription_ReturnsRedirectToNewSurvey()
        {
            var timeMock = new Mock<IDateTimeService>();

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData();
            var userDataMock = new Mock<DbSet<ApplicationUser>>().SetupData(new[] { new ApplicationUser() { UserName = "tyler" } });
            dbMock.SetupGet(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.SetupGet(db => db.Users).Returns(userDataMock.Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("tyler");

            var subject = new SurveyController(dbMock.Object, timeMock.Object, securityMock.Object);

            var result = subject.Create("Question?").Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            surveyDataMock.Verify(d => d.Add(It.Is<Survey>(s => s.Owner.UserName == "tyler" && s.Description == "Question?")));
            dbMock.Verify(db => db.SaveChangesAsync());
        }

        #endregion

        #region Delete

        [TestMethod]
        public void Delete_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { new Survey() { Id = Guid.NewGuid() } });
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Delete(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Delete_WrongUser_Returns403()
        {
            var id = Guid.NewGuid();

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new List<Survey> { new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } } });
            dbMock.SetupGet(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.SetupGet(db => db.Answers).Returns(new Mock<DbSet<Answer>>().SetupData().Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Peter");

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), securityMock.Object);

            var result = subject.Delete(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(403, (result as HttpStatusCodeResult).StatusCode);
        }

        [TestMethod]
        public void Delete_IdOK_DeletesAnswersAndDeletesSurveyAndRedirectsToHome()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };
            var moods = new[] { new Models.Mood() { Id = 1 }, new Models.Mood() { Id = 2 } };
            var answer1 = new Answer() { MoodId = 1, SurveyId = id };
            var answer2 = new Answer() { MoodId = 2, SurveyId = id };
            var answers = new List<Answer>() { answer1, answer2  };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new List<Survey> { survey });
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            var answerDataMock = new Mock<DbSet<Answer>>().SetupData(answers);
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);
            dbMock.Setup(db => db.Answers).Returns(answerDataMock.Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Tyler");

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), securityMock.Object);

            var result = subject.Delete(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
            surveyDataMock.Verify(d => d.Remove(survey));
            answerDataMock.Verify(d => d.Remove(answer1));
            answerDataMock.Verify(d => d.Remove(answer2));
            dbMock.Verify(d => d.SaveChangesAsync());
        }

        #endregion

        #region Results

        [TestMethod]
        public void Results_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { new Survey() { Id = Guid.NewGuid() } });
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Results(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Results_WrongUser_Returns403()
        {
            var id = Guid.NewGuid();

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new List<Survey> { new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } } });
            dbMock.SetupGet(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.SetupGet(db => db.Answers).Returns(new Mock<DbSet<Answer>>().SetupData().Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Peter");

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), securityMock.Object);

            var result = subject.Results(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(403, (result as HttpStatusCodeResult).StatusCode);
        }

        [TestMethod]
        public void Results_IdOK_ReturnsViewWithViewModel()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };
            var moods = new[] { new Models.Mood() { Id = 1 }, new Models.Mood() { Id = 2 } };
            var answer1 = new Answer() { MoodId = 1, SurveyId = id };
            var answer2 = new Answer() { MoodId = 2, SurveyId = id };
            var answers = new List<Answer>() { answer1, answer2 };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new List<Survey> { survey });
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            var answerDataMock = new Mock<DbSet<Answer>>().SetupData(answers);
            dbMock.Setup(db => db.Surveys).Returns(surveyDataMock.Object);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);
            dbMock.Setup(db => db.Answers).Returns(answerDataMock.Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Tyler");

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>(), securityMock.Object);

            var result = subject.Results(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var model = (result as ViewResult).Model as ResultsViewModel;
            Assert.IsNotNull(model);
            Assert.AreSame(survey, model.Survey);
            Assert.AreEqual(2, model.Answers.Count());
            Assert.AreSame(answer1, model.Answers.ToList()[0]);
            Assert.AreSame(answer2, model.Answers.ToList()[1]);
        }

        #endregion
    }
}
