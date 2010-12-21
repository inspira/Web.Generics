/*
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
using Web.Generics.ApplicationServices.InversionOfControl;
using StructureMap;

namespace Web.Generics.Infrastructure.InversionOfControl.StructureMap
{
    public class StructureMapInversionOfControlContainer : IInversionOfControlContainer
    {
        global::StructureMap.Container container = new global::StructureMap.Container();
        public void RegisterType<TFrom, TTo>() where TTo : TFrom
        {
            container.Configure(x => x.For<TFrom>().Use<TTo>());
        }

        public void RegisterType(Type interfaceType, Type implementationType)
        {
            container.Configure(x =>
            {
                x.For(interfaceType).Use(implementationType);
            });
        }

        public void RegisterInstance<T>(T obj)
        {
            container.Configure(x =>
            {
                x.For<T>().Use(obj);
            });
        }

        public void RegisterDelayedInstance<T>(Func<T> delayedExpression)
        {
            container.Configure(x =>
            {
                x.For<T>().Use(delayedExpression);
            });
        }

        public T Resolve<T>()
        {
            return container.GetInstance<T>();
        }

        public object Resolve(Type t)
        {
            return container.GetInstance(t);
        }
    }
}
