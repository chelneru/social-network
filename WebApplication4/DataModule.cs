using Autofac;
using Autofac.Core;
using WebApplication4.Models;

namespace WebApplication4
{
    public class DataModule : Module
    {
        private string _connStr;
        public DataModule(string connStr)
        {
            _connStr = connStr;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new ApplicationDbContext(_connStr)).InstancePerRequest();
        }
    }
}