﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Web.Generics.ApplicationServices.InversionOfControl;
using System.Diagnostics;
using System.Web;
using NHibernate;
using NHibernate.Context;
using NHibernate.Cfg;
using FluentNHibernate.Cfg;
using FluentNHibernate.Automapping;
using Web.Generics.ApplicationServices.DataAccess;
using Web.Generics.Infrastructure.DataAccess.NHibernate;
using Web.Generics.Infrastructure.Logging;
using Web.Generics.Infrastructure.InversionOfControl.Unity;
using FluentNHibernate.Conventions.Helpers;
using Web.Generics.Infrastructure.DataAccess.FluentNHibernate;

namespace Web.Generics
{
    public static partial class ApplicationManager
    {
		public static ISessionFactory SessionFactory { get; set; }
		public static ApplicationConfiguration ApplicationConfiguration { get; set; }
		public static IInversionOfControlContainer Container { get; set; }

        public static void Initialize(ApplicationConfiguration config)
        {
            ApplicationConfiguration = config;

            SessionFactory = CreateSessionFactory();
            Container = new UnityInversionOfControlContainer();

            var domainAssembly = config.DomainAssembly;

			Container.RegisterType<IRepositoryContext, NHibernateRepositoryContext>();

            var mapper = config.InversionOfControl.MapperInstance;
            if (mapper != null)
            {
                mapper.DefineMappings(Container);
            }

			log4net.Config.XmlConfigurator.Configure();

            // DefineControllerFactory(domainAssembly, container);
        }

        public static void Configure(IInversionOfControlMapper mapper)
        {
            //Configure<NHibernateRepositoryContext, UnityInversionOfControlContainer>();
        }

        public static void Configure<RepositoryContextT, InversionOfControlT>(IInversionOfControlMapper mapper)
        {
            //// define contexto padrão para os repositórios
            //container.RegisterInstance<IRepositoryContext>(repositoryContext);

            //ControllerBuilder.Current.SetControllerFactory(new GenericControllerFactory(container));

            //if (repositoryContext is NHibernateRepositoryContext)
            //{
            //    container.RegisterInstance<ISession>(FluentNHibernateHelper.OpenSession());
            //}

            //// chama custom mapper (IoC)
            //mapper.DefineMappings(container);
        }

        private static ISessionFactory CreateSessionFactory()
        {
            Trace.WriteLine(DateTime.Now + "    Creating a new session factory", "NHTests");

            var nhConfiguration = new Configuration();
            nhConfiguration.Configure();
            //return configuration.BuildSessionFactory();

            var fluentConfiguration = ApplicationConfiguration.Fluent.MappingConfigurationInstance;
            var assembly = ApplicationConfiguration.DomainAssembly;

			var autoMap = AutoMap.Assembly(assembly, fluentConfiguration);

			var overrideAssembly = ApplicationConfiguration.Fluent.OverrideAssembly;
			if (overrideAssembly != null) {
				autoMap.UseOverridesFromAssembly(overrideAssembly);
			}

            autoMap.Conventions.Add(
                DefaultCascade.SaveUpdate(),
                DefaultLazy.Always(),
                new ColumnNullabilityConvention(),
                new ForeignKeyConstraintNameConvention(),
                new StringColumnLengthConvention(),
                new EnumConvention(),
                ForeignKey.EndsWith("_ID"),
                ConventionBuilder.Reference.Always(x => x.Not.Nullable()),
                ConventionBuilder.Reference.Always(x => x.Cascade.None()),
                ConventionBuilder.Reference.Always(x => x.Not.LazyLoad()),
                ConventionBuilder.HasMany.Always(x => x.Inverse()),
                ConventionBuilder.HasManyToMany.Always(x => x.Table(x.TableName.Replace("ListTo", "").Substring(0, x.TableName.Length - 10)))
            );

            var sessionFactory = Fluently.Configure(nhConfiguration)
                .Mappings(m => m.AutoMappings.Add(autoMap))
				.BuildSessionFactory();

            return sessionFactory;
        }

        public static ISession GetCurrentSession()
        {
            return SessionFactory.GetCurrentSession();
        }

    }
}