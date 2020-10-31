﻿using System;
using System.IO;
using Melville.MVVM.FileSystem;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Planner.Models.Blobs;
using Planner.Repository.SqLite;

namespace Planner.Web.CompositionRoot
{
    public static class DatabaseFactory
    {
        public static void ConfigureDatabase(IServiceCollection services, IWebHostEnvironment environment)
        {
            var dir = new FileSystemDirectory(Path.Join(environment.ContentRootPath, "App_Data"));
            RegisterDataDirectory(services, dir);
            RegisterDatabase(services, dir);
        }

        private static void RegisterDatabase(IServiceCollection services, IDirectory dir)
        {
            services.AddSingleton(DirectoryDatabaseFactory.DatabaseCreator(dir));
        }

        private static void RegisterDataDirectory(IServiceCollection services, IDirectory dataDir)
        {
            services.AddSingleton<IBlobContentStore>(new BlobContentStore(dataDir));
        }
    }
}