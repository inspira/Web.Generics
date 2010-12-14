﻿/*
Copyright 2010 Inspira Tecnologia.
All Rights Reserved.

Contact: Thiago Alves <thiago.alves@inspira.com.br>

This file is part of Web.Generics

Web.Generics is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Web.Generics is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Web.Generics.  If not, see <http://www.gnu.org/licenses/>.
*/

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
using Web.Generics.Infrastructure.InversionOfControl.StructureMap;

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
            Container = new StructureMapInversionOfControlContainer();

            var domainAssembly = config.DomainAssembly;

            Container.RegisterType<IRepositoryContext, NHibernateRepositoryContext>();
            Container.RegisterType(typeof(IRepository<>), typeof(GenericNHibernateRepository<>));
            Container.RegisterDelayedInstance<ISession>(() => { return ApplicationManager.SessionFactory.GetCurrentSession(); });

            var mapper = config.InversionOfControl.MapperInstance;
            if (mapper != null)
            {
                mapper.DefineMappings(Container);
            }

			log4net.Config.XmlConfigurator.Configure();

            // DefineControllerFactory(domainAssembly, container);
        }

		private static void Configure(IInversionOfControlMapper mapper)
        {
            //Configure<NHibernateRepositoryContext, UnityInversionOfControlContainer>();
        }

        private static void Configure<RepositoryContextT, InversionOfControlT>(IInversionOfControlMapper mapper)
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

        public static ISessionFactory CreateSessionFactory()
        {
            Trace.WriteLine(DateTime.Now + "    Creating a new session factory", "NHTests");

            var nhConfiguration = new Configuration();

            var configPath = ApplicationConfiguration.NHibernate.ConfigurationFilePath;
            if (configPath != null)
            {
                // used to allow visual studio unit tests
                nhConfiguration.Configure(configPath);
            }
            else
            {
                nhConfiguration.Configure();
            }
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
                ConventionBuilder.HasMany.Always(x => x.Inverse())
            );

            var sessionFactory = Fluently.Configure(nhConfiguration)
                .Mappings(m => m.AutoMappings.Add(autoMap))
				.BuildSessionFactory();

            return sessionFactory;
        }

        public static ISession GetCurrentSession()
        {
            if (SessionFactory == null) throw new ApplicationException("Session factory not configured. Did you call ApplicationManager.Initialize()?");
            return SessionFactory.GetCurrentSession();
        }


        public static void Initialize(System.Reflection.Assembly domainAssembly, System.Reflection.Assembly infrastructureAssembly, DefaultAutomappingConfiguration mappingConfiguration, IInversionOfControlMapper inversionOfControlMapper)
        {
            ApplicationManager.Initialize(new ApplicationConfiguration
            {
                DomainAssembly = domainAssembly,
                Fluent = new ApplicationConfiguration.FluentConfiguration { OverrideAssembly = infrastructureAssembly, MappingConfigurationInstance = mappingConfiguration },
                InversionOfControl = new ApplicationConfiguration.InversionOfControlConfiguration { MapperInstance = inversionOfControlMapper }
            });
        }
    }
}
