using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mood.Controllers;
using Mood.Migrations;
using Moq;
using System.Data.Entity;
using Mood.Models;
using System.Web.Mvc;
using Mood.Services;
using System.Collections.Generic;
using Mood.ViewModels;
using System.Linq;

namespace Mood.Tests.Controllers
{
    [TestClass]
    public class AnswerControllerTest
    {
        #region Ctor

        [TestMethod]
        public void Ctor_DbIsNull_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new AnswerController(null, Mock.Of<ISurveyService>(), Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());
            });

            Assert.AreEqual("db", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_SurveysIsNull_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new AnswerController(Mock.Of<IApplicationDBContext>(), null, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());
            });

            Assert.AreEqual("surveys", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_DateTimeIsNull_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new AnswerController(Mock.Of<IApplicationDBContext>(), Mock.Of<ISurveyService>(), null, Mock.Of<ISecurity>());
            });

            Assert.AreEqual("time", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_SecurityIsNull_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new AnswerController(Mock.Of<IApplicationDBContext>(), Mock.Of<ISurveyService>(), Mock.Of<IDateTimeService>(), null);
            });

            Assert.AreEqual("security", ex.ParamName);
        }

        #endregion

        #region Edit

        [TestMethod]
        public void Edit_IdInvalid_Returns404()
        {
            var mockAnswers = new Mock<DbSet<Answer>>().SetupData();
            var mockDb = new Mock<IApplicationDBContext>();
            mockDb.SetupGet(d => d.Answers).Returns(mockAnswers.Object);

            var subject = new AnswerController(mockDb.Object, Mock.Of<ISurveyService>(), Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Edit(3, null).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Edit_DetailsNull_Returns500()
        {
            var mockAnswers = new Mock<DbSet<Answer>>().SetupData(new[] { new Answer() { Id = 3 } });
            var mockDb = new Mock<IApplicationDBContext>();
            mockDb.SetupGet(d => d.Answers).Returns(mockAnswers.Object);

            var subject = new AnswerController(mockDb.Object, Mock.Of<ISurveyService>(), Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Edit(3, null).Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(500, (result as HttpStatusCodeResult).StatusCode);
        }

        [TestMethod]
        public void Edit_DetailsWhitespace_Returns500()
        {
            var mockAnswers = new Mock<DbSet<Answer>>().SetupData(new[] { new Answer() { Id = 3 } });
            var mockDb = new Mock<IApplicationDBContext>();
            mockDb.SetupGet(d => d.Answers).Returns(mockAnswers.Object);

            var subject = new AnswerController(mockDb.Object, Mock.Of<ISurveyService>(), Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Edit(3, "\n\t ").Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(500, (result as HttpStatusCodeResult).StatusCode);
        }

        [TestMethod]
        public void Edit_DetailsOK_SavesAndReturnsOK()
        {
            var answer = new Answer() { Id = 3 };
            var mockAnswers = new Mock<DbSet<Answer>>().SetupData(new[] { answer });
            var mockDb = new Mock<IApplicationDBContext>();
            mockDb.SetupGet(d => d.Answers).Returns(mockAnswers.Object);

            var subject = new AnswerController(mockDb.Object, Mock.Of<ISurveyService>(), Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Edit(3, "details").Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(200, (result as HttpStatusCodeResult).StatusCode);
            Assert.AreEqual("details", answer.Details);
            mockDb.Verify(d => d.SaveChangesAsync());
        }

        #endregion


        #region Create

        [TestMethod]
        public void Answer_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();
            var moodId = 10;
            var details = "Test details";

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync((Survey)null);

            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new AnswerController(dbMock.Object, Mock.Of<ISurveyService>(), Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Create(id.ToString(), moodId, details).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Answer_MoodIdNotFound_Returns404()
        {
            var id = Guid.NewGuid();
            var moodId = 5;
            var survey = new Survey() { Id = id };
            var moods = new List<Models.Mood>() { new Models.Mood() { Id = 10 }, new Models.Mood() { Id = 20 } };
            var details = "Test details";

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);

            var subject = new AnswerController(dbMock.Object, surveysMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Create(id.ToString(), moodId, details).Result;

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
            var details = "Test details";

            var timeMock = new Mock<IDateTimeService>();
            timeMock.Setup(t => t.Now()).Returns(time);

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            var answersDataMock = new Mock<DbSet<Answer>>();
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);
            dbMock.Setup(db => db.Answers).Returns(answersDataMock.Object);

            var subject = new AnswerController(dbMock.Object, surveysMock.Object, timeMock.Object, Mock.Of<ISecurity>());

            var result = subject.Create(id.ToString(), moodId, details).Result;

            Assert.IsInstanceOfType(result, typeof(JsonResult));
            Assert.IsInstanceOfType((result as JsonResult).Data, typeof(Answer));
            var answer = (result as JsonResult).Data as Answer;
            Assert.AreSame(survey, answer.Survey);
            Assert.AreSame(moods[0], answer.Mood);
            Assert.AreEqual(time, answer.Time);

            answersDataMock.Verify(d => d.Add(It.Is<Answer>(a => a.Mood == moods[0] && a.Survey == survey && a.Time == time && a.Details == details)));
            dbMock.Verify(db => db.SaveChangesAsync());
        }

        #endregion


        #region Results

        [TestMethod]
        public void Results_IdNotFound_Returns404()
        {
            var id = Guid.NewGuid();

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync((Survey)null);

            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new AnswerController(dbMock.Object, surveysMock.Object, Mock.Of<IDateTimeService>(), Mock.Of<ISecurity>());

            var result = subject.Results(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Results_WrongUser_Returns403()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, Owner = new ApplicationUser() { UserName = "Tyler" } };

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();
            dbMock.SetupGet(db => db.Answers).Returns(new Mock<DbSet<Answer>>().SetupData().Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Peter");

            var subject = new AnswerController(dbMock.Object, surveysMock.Object, Mock.Of<IDateTimeService>(), securityMock.Object);

            var result = subject.Results(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(403, (result as HttpStatusCodeResult).StatusCode);
        }

        [TestMethod]
        public void Results_PublicResultsAnonymousUser_ReturnsViewWithViewModel()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id, PublicResults = true, Owner = new ApplicationUser() { UserName = "Tyler" } };
            var moods = new[] { new Models.Mood() { Id = 1 }, new Models.Mood() { Id = 2 } };
            var answer1 = new Answer() { MoodId = 1, SurveyId = id };
            var answer2 = new Answer() { MoodId = 2, SurveyId = id };
            var answers = new List<Answer>() { answer1, answer2 };

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            var answerDataMock = new Mock<DbSet<Answer>>().SetupData(answers);
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);
            dbMock.Setup(db => db.Answers).Returns(answerDataMock.Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Peter");
            securityMock.SetupGet(s => s.IsAuthenticated).Returns(true);

            var subject = new AnswerController(dbMock.Object, surveysMock.Object, Mock.Of<IDateTimeService>(), securityMock.Object);

            var result = subject.Results(id.ToString()).Result;

            Assert.IsInstanceOfType(result, typeof(ViewResult));
            var model = (result as ViewResult).Model as ResultsViewModel;
            Assert.IsNotNull(model);
            Assert.AreSame(survey, model.Survey);
            Assert.AreEqual(2, model.Answers.Count());
            Assert.AreSame(answer1, model.Answers.ToList()[0]);
            Assert.AreSame(answer2, model.Answers.ToList()[1]);
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

            var surveysMock = new Mock<ISurveyService>();
            surveysMock.Setup(s => s.FindAsync(id.ToString())).ReturnsAsync(survey);

            var dbMock = new Mock<IApplicationDBContext>();
            var moodDataMock = new Mock<DbSet<Models.Mood>>().SetupData(moods);
            var answerDataMock = new Mock<DbSet<Answer>>().SetupData(answers);;
            dbMock.Setup(db => db.Moods).Returns(moodDataMock.Object);
            dbMock.Setup(db => db.Answers).Returns(answerDataMock.Object);

            var securityMock = new Mock<ISecurity>();
            securityMock.SetupGet(s => s.UserName).Returns("Tyler");
            securityMock.SetupGet(s => s.IsAuthenticated).Returns(true);

            var subject = new AnswerController(dbMock.Object, surveysMock.Object, Mock.Of<IDateTimeService>(), securityMock.Object);

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
