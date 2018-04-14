using System;
using System.Linq;
using NUnit.Framework;
using WebApplication4.Models;
using System.Collections.Generic;
using System.Data.Entity;
using Moq;
using NSubstitute;
using WebApplication4.DAL.Interfaces;
using WebApplication4.Services;

namespace Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Test1()
        {
            /* Arrange */
            
            IQueryable<UserProfile> userProfiles =
                new List<UserProfile>
                {
                    new UserProfile { Id = new Guid("de96cbf6-d3b8-4d02-aae1-825261d2a4d9"), Name = "Alin",About="Something about me" },
                    new UserProfile { Id = new Guid("3440b5dc-5e67-4dfe-99a5-c20899625152"), Name = "Alin2",About="Smething bout me" },
                    new UserProfile { Id = new Guid("49afa87f-6242-4d0b-9298-db0315ba5440"), Name = "Alin4",About="Somthing about m" },
                    new UserProfile { Id = new Guid("4dcb3129-985d-41a5-a1ad-5da877317f16"), Name = "Alin5",About="Somethingabout me" },
                    new UserProfile { Id = new Guid("a405e5b5-82df-4feb-a96c-1d54ade5c1a4"), Name = "Alin6",About="Somthing abt me" },
                    new UserProfile { Id = new Guid("55c2b643-b030-47ee-91a7-7cc20aac2f4e"), Name = "Alin7",About="Somethin about me" }
                }.AsQueryable();
            
            var mockSet = new Mock<DbSet<UserProfile>>();
            mockSet.As<IQueryable<UserProfile>>().Setup(m => m.Provider).Returns(userProfiles.Provider);
            mockSet.As<IQueryable<UserProfile>>().Setup(m => m.Expression).Returns(userProfiles.Expression);
            mockSet.As<IQueryable<UserProfile>>().Setup(m => m.ElementType).Returns(userProfiles.ElementType);
            mockSet.As<IQueryable<UserProfile>>().Setup(m => m.GetEnumerator()).Returns(userProfiles.GetEnumerator());

            var mockContext = new Mock<ApplicationDbContext>();
            mockContext.Setup(c => c.UserProfile).Returns(mockSet.Object);

            var service = new UserProfileService(mockContext.Object);
            var blogs = service.GetUserProfileTest(new Guid("55c2b643-b030-47ee-91a7-7cc20aac2f4e"));
            UserProfile text = new UserProfile();
           Assert.AreEqual(blogs.Id , new Guid("55c2b643-b030-47ee-91a7-7cc20aac2f4e"));
        }
    }
}