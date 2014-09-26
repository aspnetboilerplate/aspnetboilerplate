using System;
using System.Reflection;
using Abp.Dependency.Conventions;

namespace Abp.Dependency
{
    /// <summary>
    /// Define interface for classes those are used to register dependencies.
    /// </summary>
    public interface IIocRegistrar
    {
        /// <summary>
        /// Adds a dependency registerer for conventional registration.
        /// </summary>
        /// <param name="registerer">dependency registerer</param>
        void AddConventionalRegisterer(IConventionalRegisterer registerer);

        /// <summary>
        /// Registers types of given assembly by all conventional registerers. See <see cref="IocManager.AddConventionalRegisterer"/> method.
        /// </summary>
        /// <param name="assembly">Assembly to register</param>
        void RegisterAssemblyByConvention(Assembly assembly);

        /// <summary>
        /// Registers types of given assembly by all conventional registerers. See <see cref="IocManager.AddConventionalRegisterer"/> method.
        /// </summary>
        /// <param name="assembly">Assembly to register</param>
        /// <param name="config">Additional configuration</param>
        void RegisterAssemblyByConvention(Assembly assembly, ConventionalRegistrationConfig config);

        /// <summary>
        /// Registers a type as self registration.
        /// </summary>
        /// <typeparam name="TService">Type of the class</typeparam>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        void Register<TService>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TService : class;

        /// <summary>
        /// Registers a type as self registration.
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        void Register(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);

        /// <summary>
        /// Registers a type with it's implementation.
        /// </summary>
        /// <typeparam name="TType">Registering type</typeparam>
        /// <typeparam name="TImpl">The type that implements <see cref="TType"/></typeparam>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        void Register<TType, TImpl>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType;

        /// <summary>
        /// Registers a class as self registration.
        /// </summary>
        /// <param name="type">Type of the class</param>
        /// <param name="impl">The type that implements <see cref="type"/></param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        void Register(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton);
    }
}