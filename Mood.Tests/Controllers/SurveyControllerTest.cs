﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
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
                new SurveyController(null, Mock.Of<IDateTimeService>());
            });

            Assert.AreEqual("db", ex.ParamName);
        }

        [TestMethod]
        public void Ctor_NullTime_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new SurveyController(Mock.Of<IApplicationDBContext>(), null);
            });

            Assert.AreEqual("time", ex.ParamName);
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

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>());

            var result = subject.View(id).Result;

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

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>());

            var result = subject.View(id).Result;

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

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>());

            var result = subject.Answer(id, moodId).Result;

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

            var subject = new SurveyController(dbMock.Object, Mock.Of<IDateTimeService>());

            var result = subject.Answer(id, moodId).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Answer_IdsOK_CreatesAnswerAndReturnsOK()
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

            var subject = new SurveyController(dbMock.Object, timeMock.Object);

            var result = subject.Answer(id, moodId).Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(200, (result as HttpStatusCodeResult).StatusCode);

            answersDataMock.Verify(d => d.Add(It.Is<Answer>(a => a.Mood == moods[0] && a.Survey == survey && a.Time == time)));
            dbMock.Verify(db => db.SaveChangesAsync());
        }

        #endregion
    }
}
