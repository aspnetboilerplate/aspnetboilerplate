using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Collections.Extensions;
using Castle.Core.Logging;

namespace Abp.Reflection
{
    public class TypeFinder : ITypeFinder
    {
        public ILogger Logger { get; set; }

        public IAssemblyFinder AssemblyFinder { get; set; }

        public List<Type> _types;

        public TypeFinder()
        {
            AssemblyFinder = DefaultAssemblyFinder.Instance;
            Logger = NullLogger.Instance;
        }

        public Type[] Find(Func<Type, bool> predicate)
        {
            EnsureAllTypesLoaded();
            return _types.Where(predicate).ToArray();
        }

        public Type[] FindAll()
        {
            EnsureAllTypesLoaded();
            return _types.ToArray();
        }

        private void EnsureAllTypesLoaded()
        {
            if (_types != null)
            {
                return;
            }

            _types = new List<Type>();

            foreach (var assembly in AssemblyFinder.GetAllAssemblies().Distinct())
            {
                try
                {
                    Type[] types;

                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException ex)
                    {
                        types = ex.Types;
                    }

                    if (types.IsNullOrEmpty())
                    {
                        continue;
                    }

                    _types.AddRange(types.Where(type => type != null));
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.ToString(), ex);
                }
            }
        }
    }
}