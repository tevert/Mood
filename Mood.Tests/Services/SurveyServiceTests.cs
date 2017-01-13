using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mood.Services;
using Mood.Migrations;
using Moq;
using Mood.Models;
using System.Data.Entity;
using System.Collections.Generic;
using Mood.ViewModels;
using Mood.Services.Exceptions;

namespace Mood.Tests.Services
{
    [TestClass]
    public class SurveyServiceTests
    {
        #region Ctor

        [TestMethod]
        public void Ctor_NullDB_ThrowsException()
        {
            var ex = Helpers.Catch<ArgumentNullException>(() => {
                new SurveyService(null);
            });

            Assert.AreEqual("db", ex.ParamName);
        }

        #endregion

        #region FindAsync

        [TestMethod]
        public void FindAsync_StringIsGuid_ReturnsSurvey()
        {
            var id = Guid.NewGuid();
            var survey = new Survey() { Id = id };

            var dbMock = new Mock<IApplicationDBContext>();
            var mockSurveyData = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(mockSurveyData.Object);

            var subject = new SurveyService(dbMock.Object);

            var result = subject.FindAsync(id.ToString()).Result;

            Assert.AreEqual(survey, result);
        }

        [TestMethod]
        public void FindAsync_StringIsName_ReturnsSurvey()
        {
            var name = "My survey";
            var survey = new Survey() { Name = name };

            var dbMock = new Mock<IApplicationDBContext>();
            var mockSurveyData = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(mockSurveyData.Object);

            var subject = new SurveyService(dbMock.Object);

            var result = subject.FindAsync(name).Result;

            Assert.AreEqual(survey, result);
        }

        [TestMethod]
        public void FindAsync_StringIsNull_ThrowsException()
        {
            var dbMock = new Mock<IApplicationDBContext>();
            var subject = new SurveyService(dbMock.Object);

            var ex = Helpers.Catch<ArgumentNullException>(() => {
                var result = subject.FindAsync(null).Result;
            });

            Assert.AreEqual("identifier", ex.ParamName);
        }

        [TestMethod]
        public void FindAsync_NoMatch_ReturnsNull()
        {
            var survey = new Survey() { Id = Guid.NewGuid() };

            var dbMock = new Mock<IApplicationDBContext>();
            var mockSurveyData = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(mockSurveyData.Object);

            var subject = new SurveyService(dbMock.Object);

            var result = subject.FindAsync("BadId").Result;

            Assert.IsNull(result);
        }

        #endregion

        #region AddAsync

        [TestMethod]
        public void AddAsync_SurveyIsNull_ThrowsException()
        {
            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyService(dbMock.Object);

            var ex = Helpers.Catch<ArgumentNullException>(() =>
            {
                subject.AddAsync(null).Wait();
            });

            Assert.AreEqual("newSurvey", ex.ParamName);
        }

        [TestMethod]
        public void AddAsync_SurveyIsOK_AddsAndSaves()
        {
            var survey = new Survey() { };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>();
            dbMock.SetupGet(d => d.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyService(dbMock.Object);

            subject.AddAsync(survey).Wait();

            surveyDataMock.Verify(d => d.Add(survey));
            dbMock.Verify(d => d.SaveChangesAsync());
        }

        #endregion

        #region DeleteAsync

        [TestMethod]
        public void DeleteAsync_SurveyIsNull_ThrowsException()
        {
            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyService(dbMock.Object);

            var ex = Helpers.Catch<ArgumentNullException>(() =>
            {
                subject.DeleteAsync(null).Wait();
            });

            Assert.AreEqual("survey", ex.ParamName);
        }

        [TestMethod]
        public void DeleteAsync_SurveyHasNoAnswers_DeletesSurvey()
        {
            var survey = new Survey() { Id = Guid.NewGuid() };
            
            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new List<Survey>() { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(surveyDataMock.Object);
            var answerDataMock = new Mock<DbSet<Answer>>().SetupData();
            dbMock.SetupGet(d => d.Answers).Returns(answerDataMock.Object);

            var subject = new SurveyService(dbMock.Object);
            
            subject.DeleteAsync(survey).Wait();

            surveyDataMock.Verify(d => d.Remove(survey));
            dbMock.Verify(d => d.SaveChangesAsync());
        }

        [TestMethod]
        public void DeleteAsync_SurveyHasAnswers_DeletesAnswersAndSurvey()
        {
            var survey = new Survey() { Id = Guid.NewGuid() };
            var answer1 = new Answer() { SurveyId = survey.Id };
            var answer2 = new Answer() { SurveyId = survey.Id };
            var answers = new List<Answer>()
            {
                answer1,
                answer2,
                new Answer() { SurveyId = Guid.NewGuid() }
            };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new List<Survey>() { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(surveyDataMock.Object);
            var answerDataMock = new Mock<DbSet<Answer>>().SetupData(answers);
            dbMock.SetupGet(d => d.Answers).Returns(answerDataMock.Object);

            var subject = new SurveyService(dbMock.Object);

            subject.DeleteAsync(survey).Wait();

            surveyDataMock.Verify(d => d.Remove(survey));
            answerDataMock.Verify(d => d.Remove(answer1));
            answerDataMock.Verify(d => d.Remove(answer2));
            answerDataMock.Verify(d => d.Remove(It.IsAny<Answer>()), Times.Exactly(2));
            dbMock.Verify(d => d.SaveChangesAsync());
        }

        #endregion

        #region EditAsync

        [TestMethod]
        public void EditAsync_SurveyNull_ThrowsException()
        {
            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyService(dbMock.Object);

            var ex = Helpers.Catch<ArgumentNullException>(() =>
            {
                subject.EditAsync(null, new SurveyEditViewModel()).Wait();
            });

            Assert.AreEqual("survey", ex.ParamName);
        }

        [TestMethod]
        public void EditAsync_NewValuesNull_ThrowsException()
        {
            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyService(dbMock.Object);

            var ex = Helpers.Catch<ArgumentNullException>(() =>
            {
                subject.EditAsync(new Survey(), null).Wait();
            });

            Assert.AreEqual("newValues", ex.ParamName);
        }

        [TestMethod]
        public void EditAsync_NewPublicResultsValue_UpdatesPublicResults()
        {
            var survey = new Survey() { PublicResults = false };
            var newValues = new SurveyEditViewModel() { PublicResults = true };
            var dbMock = new Mock<IApplicationDBContext>();

            var subject = new SurveyService(dbMock.Object);

            subject.EditAsync(survey, newValues).Wait();

            Assert.AreEqual(true, survey.PublicResults);
            dbMock.Verify(d => d.SaveChangesAsync());
        }

        [TestMethod]
        public void EditAsync_NewName_UpdatesName()
        {
            var survey = new Survey() { Name = "oldName" };
            var newValues = new SurveyEditViewModel() { Name = "newName" };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyService(dbMock.Object);

            subject.EditAsync(survey, newValues).Wait();

            Assert.AreEqual("newName", survey.Name);
            dbMock.Verify(d => d.SaveChangesAsync());
        }

        [TestMethod]
        public void EditAsync_NewNameAlreadyTaken_ThrowsException()
        {
            var survey = new Survey() { Name = "oldName" };
            var olderSurvey = new Survey() { Name = "newName" };
            var newValues = new SurveyEditViewModel() { Name = "newName" };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { survey, olderSurvey });
            dbMock.SetupGet(d => d.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyService(dbMock.Object);

            var ex = Helpers.Catch<SurveyException>(() => {
                subject.EditAsync(survey, newValues).Wait();
            });

            Assert.AreEqual("That name is already taken", ex.Message);
            Assert.AreEqual("oldName", survey.Name);
            dbMock.Verify(d => d.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void EditAsync_NewNameIsWhitespace_ThrowsException()
        {
            var survey = new Survey() { Name = "oldName" };
            var newValues = new SurveyEditViewModel() { Name = "\n \t" };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyService(dbMock.Object);

            var ex = Helpers.Catch<SurveyException>(() => {
                subject.EditAsync(survey, newValues).Wait();
            });

            Assert.AreEqual("Name must not be whitespace", ex.Message);
            Assert.AreEqual("oldName", survey.Name);
            dbMock.Verify(d => d.SaveChangesAsync(), Times.Never);
        }

        [TestMethod]
        public void EditAsync_NewNameIsNull_NullsTheName()
        {
            var survey = new Survey() { Name = "oldName" };
            var newValues = new SurveyEditViewModel() { Name = null };

            var dbMock = new Mock<IApplicationDBContext>();
            var surveyDataMock = new Mock<DbSet<Survey>>().SetupData(new[] { survey });
            dbMock.SetupGet(d => d.Surveys).Returns(surveyDataMock.Object);

            var subject = new SurveyService(dbMock.Object);

            subject.EditAsync(survey, newValues).Wait();

            Assert.IsNull(survey.Name);
            dbMock.Verify(d => d.SaveChangesAsync());
        }

        #endregion
    }
}
