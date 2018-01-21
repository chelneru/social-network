using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using WebApplication4.DAL;

namespace MVCModelTest
{
    [TestFixture]
    public class CheckUserTest
    {
        [Test]
        public void CheckUserExist()
        {
            UnitOfWork unitOfWork = new UnitOfWork();
            var users = unitOfWork.UserRepository.Get();
            //Assert.That()
        }

    }
}
