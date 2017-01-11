using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mood.Services;
using Mood.Migrations;
using Moq;
using Mood.Models;
using System.Data.Entity;

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

    }
}
