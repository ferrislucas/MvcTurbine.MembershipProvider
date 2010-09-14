﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MvcTurbine.Blades;
using MvcTurbine.ComponentModel;

namespace MvcTurbine.MembershipProvider.Blades
{
    public class UnauthenticatedPrincipalCreatorSetupBlade : Blade
    {
        private readonly IServiceLocator serviceLocator;

        public UnauthenticatedPrincipalCreatorSetupBlade(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
        }

        public override void Spin(IRotorContext context)
        {
            var list = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(GetTypesThatImplementTheUnauthenticatedPrincipalCreator);

            if (list.Any())
                serviceLocator.Register<IUnauthenticatedPrincipalCreator>(list.First());
        }

        private static IEnumerable<Type> GetTypesThatImplementTheUnauthenticatedPrincipalCreator(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(x => x.IsAbstract == false)
                .Where(x => x.IsInterface == false)
                .Where(x => x.GetInterfaces().Contains(typeof (IUnauthenticatedPrincipalCreator)))
                .OrderBy(PutTheDefaultLast());
        }

        private static Func<Type, int> PutTheDefaultLast()
        {
            return x => x == typeof (DefaultUnauthenticatedPrincipalCreator) ? 1 : 0;
        }
    }