﻿using Autofac;
using TagCloud;
using TagCloudApp.Apps;
using Configuration = TagCloud.Configuration.Configuration;


namespace TagCloudApp;

public static class Program
{
    public static void Main(string[] args)
    {
        CommandLineConfigurationProvider.GetConfiguration(args)
            .Validate(c => c != null, "")
            .Then(BuildContainer)
            .Then(c => c.Resolve<IApp>())
            .Then(a => a.Run())
            .OnFail(Console.WriteLine);
    }

    private static IContainer BuildContainer(Configuration configuration)
    {
        var builder = new ContainerBuilder().GetDefaultBuild(configuration);
        builder.RegisterType<ConsoleApp>().As<IApp>();
        return builder.Build();
    }
}