
using Hangfire.Extension.TaskDispatcher.Interfaces;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using System.Reflection;
using TestSite.Tasks;

namespace TestSite.Infrastructure
{
    public class TaskModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITaskDispatcher>().To<TaskDispatcher>();


            Kernel.Bind(x => x.From(Assembly.GetAssembly(typeof(BaseTaskParameters)))
                .SelectAllClasses()
                .InheritedFrom<ITaskHandler>()
                .BindAllInterfaces());
        }
    }
}