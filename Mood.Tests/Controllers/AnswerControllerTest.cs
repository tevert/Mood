using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mood.Controllers;
using Mood.Migrations;
using Moq;
using System.Data.Entity;
using Mood.Models;
using System.Web.Mvc;

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
                new AnswerController(null);
            });

            Assert.AreEqual("db", ex.ParamName);
        }

        #endregion

        #region Edit

        [TestMethod]
        public void Edit_IdInvalid_Returns404()
        {
            var mockAnswers = new Mock<DbSet<Answer>>().SetupData();
            var mockDb = new Mock<IApplicationDBContext>();
            mockDb.SetupGet(d => d.Answers).Returns(mockAnswers.Object);

            var subject = new AnswerController(mockDb.Object);

            var result = subject.Edit(3, null).Result;

            Assert.IsInstanceOfType(result, typeof(HttpNotFoundResult));
        }

        [TestMethod]
        public void Edit_DetailsNull_Returns500()
        {
            var mockAnswers = new Mock<DbSet<Answer>>().SetupData(new[] { new Answer() { Id = 3 } });
            var mockDb = new Mock<IApplicationDBContext>();
            mockDb.SetupGet(d => d.Answers).Returns(mockAnswers.Object);

            var subject = new AnswerController(mockDb.Object);

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

            var subject = new AnswerController(mockDb.Object);

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

            var subject = new AnswerController(mockDb.Object);

            var result = subject.Edit(3, "details").Result;

            Assert.IsInstanceOfType(result, typeof(HttpStatusCodeResult));
            Assert.AreEqual(200, (result as HttpStatusCodeResult).StatusCode);
            Assert.AreEqual("details", answer.Details);
            mockDb.Verify(d => d.SaveChangesAsync());
        }

        #endregion
    }
}
