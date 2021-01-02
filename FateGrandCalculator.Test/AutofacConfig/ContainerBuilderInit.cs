﻿using Autofac;

using FateGrandCalculator.Test.Fixture;

using System;
using System.IO;

namespace FateGrandCalculator.Test.AutofacConfig
{
    public static class ContainerBuilderInit
    {
        /// <summary>
        /// Create the container Autofac needs for Dependency Injection
        /// </summary>
        /// <param name="region">NA or JP</param>
        /// <returns></returns>
        public static IContainer Create(string region)
        {
            string filename = $"ChaldeaSaveData{region}.json";
            string chaldeaFileLocation = Path.Combine(Environment.CurrentDirectory, "Json", filename);

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterModule(new AutofacModule
            {
                BaseApiUrl = new NamedParameter("baseApiUrl", WireMockFixture.ServerUrl),
                ChaldeaFileLocation = new NamedParameter("chaldeaFileLocation", chaldeaFileLocation),
                Region = new NamedParameter("region", region)
            });

            return builder.Build();
        }
    }
}
