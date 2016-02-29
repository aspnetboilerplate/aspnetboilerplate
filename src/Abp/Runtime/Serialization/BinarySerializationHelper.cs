using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Adorable.Runtime.Serialization
{
    /// <summary>
    /// This class is used to simplify serialization/deserialization operations.
    /// Uses .NET binary serialization.
    /// </summary>
    public static class BinarySerializationHelper
    {
        /// <summary>
        /// Serializes an object and returns as a byte array.
        /// </summary>
        /// <param name="obj">object to be serialized</param>
        /// <returns>bytes of object</returns>
        public static byte[] Serialize(object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                new BinaryFormatter().Serialize(memoryStream, obj);
                return memoryStream.ToArray();
            }
        }

        /// <summary>
        /// Serializes an object into a stream.
        /// </summary>
        /// <param name="obj">object to be serialized</param>
        /// <param name="stream">Stream to serialize in</param>
        /// <returns>bytes of object</returns>
        public static void Serialize(object obj, Stream stream)
        {
            new BinaryFormatter().Serialize(stream, obj);
        }

        /// <summary>
        /// Deserializes an object from given byte array.
        /// </summary>
        /// <param name="bytes">The byte array that contains object</param>
        /// <returns>deserialized object</returns>
        public static object Deserialize(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                var binaryFormatter = new BinaryFormatter
                {
                    AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple
                };

                return binaryFormatter.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// Deserializes an object from given stream.
        /// </summary>
        /// <param name="stream">The stream that contains object</param>
        /// <returns>deserialized object</returns> 
        public static object Deserialize(Stream stream)
        {
            var binaryFormatter = new BinaryFormatter
            {
                AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            };

            return binaryFormatter.Deserialize(stream);
        }

        /// <summary>
        /// Deserializes an object from given byte array.
        /// Difference from <see cref="Deserialize(byte[])"/> is that; this method can also deserialize
        /// types that are defined in dynamically loaded assemblies (like PlugIns).
        /// </summary>
        /// <param name="bytes">The byte array that contains object</param>
        /// <returns>deserialized object</returns>        
        public static object DeserializeExtended(byte[] bytes)
        {
            using (var memoryStream = new MemoryStream(bytes))
            {
                var binaryFormatter = new BinaryFormatter
                {
                    AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                    Binder = new DeserializationAppDomainBinder()
                };

                return binaryFormatter.Deserialize(memoryStream);
            }
        }

        /// <summary>
        /// Deserializes an object from given stream.
        /// Difference from <see cref="Deserialize(Stream)"/> is that; this method can also deserialize
        /// types that are defined in dynamically loaded assemblies (like PlugIns).
        /// </summary>
        /// <param name="stream">The stream that contains object</param>
        /// <returns>deserialized object</returns> 
        public static object DeserializeExtended(Stream stream)
        {
            var binaryFormatter = new BinaryFormatter
            {
                AssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
                Binder = new DeserializationAppDomainBinder()
            };

            return binaryFormatter.Deserialize(stream);
        }

        /// <summary>
        /// This class is used in deserializing to allow deserializing objects that are defined
        /// in assemlies that are load in runtime (like PlugIns).
        /// </summary>
        private sealed class DeserializationAppDomainBinder : SerializationBinder
        {
            public override Type BindToType(string assemblyName, string typeName)
            {
                var toAssemblyName = assemblyName.Split(',')[0];
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    if (assembly.FullName.Split(',')[0] == toAssemblyName)
                    {
                        return assembly.GetType(typeName);
                    }
                }

                return null;
            }
        }
    }
}
