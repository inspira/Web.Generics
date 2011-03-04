/*
Copyright 2010-2011 Inspira Tecnologia.
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
using Web.Generics.Web.Mvc.Infrastructure;
using System.Web.Mvc;
using System.Reflection;
using Web.Generics.ApplicationServices.InversionOfControl;
using NHibernate;
using System.Web;

namespace Web.Generics
{
    public static partial class MvcApplicationManager
    {
        private const String NHIBERNATE_SESSION = "NHibernate.Session";
        public static void DefineControllerFactory()
        {
			var domainAssembly = ApplicationManager.ApplicationConfiguration.DomainAssembly;
			var container = ApplicationManager.Container;
            var controllerFactory = new GenericControllerFactory(domainAssembly, container);
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

            /*ApplicationManager.Container.RegisterDelayedInstance<ISession>(() => {
                if (!HttpContext.Current.Items.Contains(NHIBERNATE_SESSION))
                {
                    var session = ApplicationManager.SessionFactory.OpenSession();
                    HttpContext.Current.Items.Add(NHIBERNATE_SESSION, session);
                }
                return (ISession)HttpContext.Current.Items[NHIBERNATE_SESSION];
            });*/
        }
    }
}
