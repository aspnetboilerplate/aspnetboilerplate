using System;

namespace Abp.Dependency
{
    /// <summary>
    /// Extension methods for <see cref="IIocRegistrar"/> interface.
    /// </summary>
    public static class IocRegistrarExtensions
    {
        #region RegisterIfNot

        /// <summary>
        /// Registers a type as self registration if it's not registered before.
        /// </summary>
        /// <typeparam name="T">Type of the class</typeparam>
        /// <param name="iocRegistrar">Registrar</param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        /// <returns>True, if registered for given implementation.</returns>
        public static bool RegisterIfNot<T>(this IIocRegistrar iocRegistrar, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where T : class
        {
            if (iocRegistrar.IsRegistered<T>())
            {
                return false;
            }

            iocRegistrar.Register<T>(lifeStyle);
            return true;
        }

        /// <summary>
        /// Registers a type as self registration if it's not registered before.
        /// </summary>
        /// <param name="iocRegistrar">Registrar</param>
        /// <param name="type">Type of the class</param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        /// <returns>True, if registered for given implementation.</returns>
        public static bool RegisterIfNot(this IIocRegistrar iocRegistrar, Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type))
            {
                return false;
            }

            iocRegistrar.Register(type, lifeStyle);
            return true;
        }

        /// <summary>
        /// Registers a type with it's implementation if it's not registered before.
        /// </summary>
        /// <typeparam name="TType">Registering type</typeparam>
        /// <typeparam name="TImpl">The type that implements <see cref="TType"/></typeparam>
        /// <param name="iocRegistrar">Registrar</param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        /// <returns>True, if registered for given implementation.</returns>
        public static bool RegisterIfNot<TType, TImpl>(this IIocRegistrar iocRegistrar, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            if (iocRegistrar.IsRegistered<TType>())
            {
                return false;
            }

            iocRegistrar.Register<TType, TImpl>(lifeStyle);
            return true;
        }


        /// <summary>
        /// Registers a type with it's implementation if it's not registered before.
        /// </summary>
        /// <param name="iocRegistrar">Registrar</param>
        /// <param name="type">Type of the class</param>
        /// <param name="impl">The type that implements <paramref name="type"/></param>
        /// <param name="lifeStyle">Lifestyle of the objects of this type</param>
        /// <returns>True, if registered for given implementation.</returns>
        public static bool RegisterIfNot(this IIocRegistrar iocRegistrar, Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            if (iocRegistrar.IsRegistered(type))
            {
                return false;
            }

            iocRegistrar.Register(type, impl, lifeStyle);
            return true;
        }

        #endregion
    }
}