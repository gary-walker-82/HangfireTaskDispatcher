using Hangfire.Extension.TaskDispatcher.Interfaces;
using Ninject.Extensions.Conventions;
using Ninject.Modules;
using System.Reflection;
using Tasks;

namespace TestSite.Infrastructure
{
    public class TaskModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ITaskDispatcher>().To<TaskDispatcher>();

            Kernel.Bind(x => x.From(Assembly.GetAssembly(typeof(T2TaskParameters)))
                .SelectAllClasses()
                .InheritedFrom<ITaskHandler>()
                .BindAllInterfaces());

            Bind<ITaskHandler>().To<MyGenericTypeHandler<Type1>>();
            Bind<ITaskHandler>().To<MyGenericTypeHandler<Type2>>();
        }
    }
}