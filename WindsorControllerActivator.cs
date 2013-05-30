using System;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Castle.MicroKernel;

namespace Castle.Windsor.Mvc
{
    public class WindsorControllerActivator : IHttpControllerActivator
    {
        readonly IKernel kernel;

        public WindsorControllerActivator(IKernel kernel)
        {
            this.kernel = kernel;
        }

        public IHttpController Create(HttpRequestMessage request, HttpControllerDescriptor controllerDescriptor,
                                      Type controllerType)
        {
            var controller = (IHttpController)kernel.Resolve(controllerType);
            request.RegisterForDispose(new Release(() => kernel.ReleaseComponent(controller)));
            return controller;
        }

        private class Release : IDisposable
        {
            readonly Action release;

            public Release(Action release)
            {
                this.release = release;
            }

            public void Dispose()
            {
                release();
            }
        }
    }
}