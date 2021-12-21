﻿using System;
using System.Drawing;
using Autofac;
using TagCloud;
using TagCloud.CloudLayouter;
using TagCloud.PointGenerator;
using TagCloud.Templates;
using TagCloud.Templates.Colors;
using TagCloud.TextHandlers;
using TagCloud.TextHandlers.Converters;
using TagCloud.TextHandlers.Filters;
using TagCloud.TextHandlers.Parser;
using TagCloudApp.Apps;
using TagCloudApp.Configurations;
using IContainer = Autofac.IContainer;


namespace TagCloudApp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var configuration = CommandLineConfigurationProvider.GetConfiguration(args);
            configuration
                .Then(CompositionRootInitialize)
                .Then(c => c.Resolve<IApp>())
                .Then(a=>a.Run(configuration.Value))
                .OnFail(Console.WriteLine);
        }


        private static IContainer CompositionRootInitialize(Configuration configuration)
        {
            var builder = new ContainerBuilder();
            RegisterTextHandlers(builder);
            RegisterCloudLayouter(builder, configuration);
            RegisterTemplateHandlers(builder, configuration);
            builder.RegisterType<ConsoleApp>().As<IApp>();
            builder.RegisterType<Visualizer>().As<IVisualizer>();
            return builder.Build();
        }

        private static void RegisterTextHandlers(ContainerBuilder builder)
        {
            builder.RegisterType<WordsReader>().As<IReader>();
            builder.RegisterType<WordsFromTextParser>().As<ITextParser>();
            builder.RegisterType<BoringWordsFilter>().As<IFilter>();
            builder.RegisterType<TextFilter>().As<ITextFilter>();
            builder.RegisterType<ToLowerConverter>().As<IConverter>();
            builder.RegisterType<Stemer>().As<IConverter>();
            builder.RegisterType<TextFilter>().As<ITextFilter>();
            builder.RegisterType<ConvertersPool>().As<IConvertersPool>();
            builder.RegisterType<FontSizeByCountCalculator>().AsSelf();
        }

        private static void RegisterTemplateHandlers(ContainerBuilder builder, Configuration configuration)
        {
            builder.Register(_ => configuration.FontFamily).As<FontFamily>();
            builder.RegisterType<Template>().As<ITemplate>();
            builder.Register((c, _) =>
                    new TemplateCreator(configuration.FontFamily, configuration.BackgroundColor,
                        configuration.ImageSize,
                        c.Resolve<IFontSizeCalculator>(), c.Resolve<IColorGenerator>(), c.Resolve<ICloudLayouter>()))
                .As<ITemplateCreator>();
            builder.RegisterType<WordParameter>().AsSelf();
            builder.Register(_ => configuration.ColorGenerator).As<IColorGenerator>();
            builder.Register(_
                    => new FontSizeByCountCalculator(Configuration.MinFontSize, Configuration.MaxFontSize))
                .As<IFontSizeCalculator>();
        }

        private static void RegisterCloudLayouter(ContainerBuilder builder, Configuration configuration)
        {
            builder.RegisterType<Cache>().As<ICache>();
            builder.Register(_ => configuration.PointGenerator).As<IPointGenerator>();
            builder.RegisterType<CloudLayouter>().AsSelf().As<ICloudLayouter>();
        }
    }
}