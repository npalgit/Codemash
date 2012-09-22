﻿using System.Reflection;
using Codemash.Api.Data.Modules;
using Codemash.DeltaApi.Modules;
using Ninject;
using Ninject.Modules;

namespace Codemash.DeltaApi.Core
{
    public class WebContainer : StandardKernel
    {
        public WebContainer(Assembly theAssembly)
        {
            // load the other modules
            var controllerModule = new AssemblyIHttpControllerNinjectModule();
            var repositoryModule = new RepositoryNinjectModule();

            // load em
            Load(new INinjectModule[] { controllerModule, repositoryModule });
        }
    }
}