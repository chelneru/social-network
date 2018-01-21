using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebApplication4.Models;
using Moq;
using WebApplication4.DAL;
using WebApplication4.DAL.Interfaces;
using WebApplication4.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Linq.Expressions;
using WebApplication4.ViewModels;

namespace SocialNetwork.Tests
{
    [TestClass]
    public class CountryControllerTest
    {
        private Mock<IUnitOfWork> _countryServiceMock;
        PostController objController;
        List<Post> listPost;

        [TestInitialize]
        public void Initialize()
        {

            _countryServiceMock = new Mock<IUnitOfWork>();
            objController = new PostController();
            listPost = new List<Post>() {
           new Post() { Id = Guid.NewGuid(), Content = "US",UserId = Guid.NewGuid().ToString() },
            new Post() { Id = Guid.NewGuid(), Content = "Russia",UserId = Guid.NewGuid().ToString() },
            new Post() { Id = Guid.NewGuid(), Content = "Romania",UserId = Guid.NewGuid().ToString() },
          };
        }



        //[TestMethod]
        //public void Country_Get_All()
        //{
        //    //Arrange
        //    _countryServiceMock.Setup(x => x.PostRepository.Get(null, null, "")).Returns(listPost);

        //    //Act
        //    var result = ((objController.Index() as ViewResult).Model) as List<Post>;

        //    //Assert
        //    Assert.AreEqual(result.Count, 3);
        //    Assert.AreEqual("US", result[0].Content);
        //    Assert.AreEqual("Russia", result[1].Content);
        //    Assert.AreEqual("Romania", result[2].Content);

        //}

        [TestMethod]
        public void Country_Get_All()
        {
            //Arrange
            _countryServiceMock.Setup(x => x.PostRepository.Get(null, null, string.Empty)).Returns(listPost);

            //Act
            var result = ((objController.Index() as ViewResult).Model) as List<Post>;

            //Assert
            Assert.AreEqual(result.Count, 3);
            Assert.AreEqual("US", result[0].Content);
            Assert.AreEqual("Russia", result[1].Content);
            Assert.AreEqual("Romania", result[2].Content);

        }

        //[TestMethod]
        //public void Invalid_Country_Create()
        //{
        //    // Arrange
        //    Country c = new Country() { Name = "" };
        //    objController.ModelState.AddModelError("Error", "Something went wrong");

        //    //Act
        //    var result = (ViewResult)objController.Create(c);

        //    //Assert
        //    _countryServiceMock.Verify(m => m.Create(c), Times.Never);
        //    Assert.AreEqual("", result.ViewName);
        //}

    }
}
