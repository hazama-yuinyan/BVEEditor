using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BVEEditor.Workbench;
using Caliburn.Micro;
using Ninject;
using Ninject.Activation;
using Ninject.Parameters;

namespace BVEEditor.Startup
{
    internal static class ServiceBootstrapper
    {
        internal static IKernel Create()
        {
            var kernel = new StandardKernel();
            AddCustomBindings(kernel);

            kernel.Bind<IFileService>().ToConstant<IFileService>(null);//.To<FileService>().InSingletonScope();
            kernel.Bind<IEventAggregator>().To<EventAggregator>().InSingletonScope();

            return kernel;
        }

        static void AddCustomBindings(IKernel kernel)
        {
            //Abstract ninject for invokers
            kernel.Bind(typeof(Func<>)).ToMethod(CreateGenericFunc).When(VerifyGenericFactoryFunction);
            kernel.Bind(typeof(Func<,>)).ToMethod(CreateFunc).When(VerifyFactoryFunction);
        }

        static bool VerifyFactoryFunction(IRequest request)
        {
            var generic_args = request.Service.GetGenericArguments();
            if(generic_args.Count() > 2)
                return false;

            return true;
        }

        static object CreateFunc(IContext ctx)
        {
            var function_factory_type = typeof(FunctionFactory<,>).MakeGenericType(ctx.GenericArguments);
            var ctor = function_factory_type.GetConstructors().Single();
            var function_factory = ctor.Invoke(new object[]{ctx.Kernel});
            return function_factory_type.GetMethod("Create").Invoke(function_factory, new object[0]);
        }

        static bool VerifyGenericFactoryFunction(IRequest request)
        {
            var generic_args = request.Service.GetGenericArguments();
            if(generic_args.Count() != 1)
                return false;

            var instance_type = generic_args.Single();
            return request.ParentContext.Kernel.CanResolve(new Request(generic_args[0], null, new IParameter[0], null, false, true)) ||
                TypeIsSelfBindable(instance_type);
        }

        static object CreateGenericFunc(IContext ctx)
        {
            var function_factory_type = typeof(GenericFunctionFactory<>).MakeGenericType(ctx.GenericArguments);
            var ctor = function_factory_type.GetConstructors().Single();
            var function_factory = ctor.Invoke(new object[]{ctx.Kernel});
            return function_factory_type.GetMethod("Create").Invoke(function_factory, new object[0]);
        }

        static bool TypeIsSelfBindable(Type type)
        {
            return !type.IsInterface &&
                !type.IsAbstract &&
                !type.IsValueType &&
                type != typeof(string) &&
                !type.ContainsGenericParameters;
        }

        public class GenericFunctionFactory<T>
        {
            readonly IKernel kernel;

            public GenericFunctionFactory(IKernel kernel)
            {
                this.kernel = kernel;
            }

            public Func<T> Create()
            {
                return (() => kernel.Get<T>());
            }
        }

        public class FunctionFactory<T, TCast> where T : Type where TCast : class
        {
            readonly IKernel kernel;

            public FunctionFactory(IKernel kernel)
            {
                this.kernel = kernel;
            }

            public Func<T, TCast> Create()
            {
                return (t => kernel.Get(t) as TCast);
            }
        }
    }
}
