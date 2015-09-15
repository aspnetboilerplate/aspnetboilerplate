//-----------------------------------------------------------------------
// <copyright file="JsonPathUtilities.cs" company="NJsonSchema">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/rsuter/NJsonSchema/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace NJsonSchema
{
    /// <summary>Utilities to work with JSON paths.</summary>
    public static class JsonPathUtilities
    {
        /// <summary>Gets the JSON path of the given object.</summary>
        /// <param name="obj">The object.</param>
        /// <param name="objectToSearch">The object to search.</param>
        /// <returns>The path or <c>null</c> when the object could not be found.</returns>
        /// <exception cref="InvalidOperationException">Could not resolve the path.</exception>
        public static string GetJsonPath(object obj, object objectToSearch)
        {
            var path = GetJsonPath(obj, objectToSearch, "#", new List<object>());
            //if (path == null)
            //    throw new InvalidOperationException("Could not resolve the path.");
            return path;
        }

        /// <summary>Gets the object from the given JSON path.</summary>
        /// <param name="obj">The object.</param>
        /// <param name="path">The JSON path.</param>
        /// <returns>The object or <c>null</c> when the object could not be found.</returns>
        /// <exception cref="InvalidOperationException">Could not resolve the path.</exception>
        /// <exception cref="NotSupportedException">Could not resolve the path.</exception>
        public static JsonSchema4 GetObjectFromJsonPath(object obj, string path)
        {
            if (path == "#")
            {
                if (obj is JsonSchema4)
                    return (JsonSchema4)obj;
                throw new InvalidOperationException("Could not resolve the path.");
            }
            else if (path.StartsWith("#/"))
            {
                var schema = GetObjectFromJsonPath(obj, path.Split('/').Skip(1).ToList(), new List<object>());
                if (schema == null)
                    throw new InvalidOperationException("Could not resolve the path '" + path +  "'.");
                return schema; 
            }
            else if (path.StartsWith("http://") || path.StartsWith("https://"))
                throw new NotSupportedException("Could not resolve the path '" + path + "' because JSON web references are not supported.");
            else
                throw new NotSupportedException("Could not resolve the path '" + path + "' because JSON file references are not supported.");
        }

        private static string GetJsonPath(object obj, object objectToSearch, string basePath, List<object> checkedObjects)
        {
            if (obj == null || obj is string || checkedObjects.Contains(obj))
                return null;

            if (obj == objectToSearch)
                return basePath;

            checkedObjects.Add(obj);

            if (obj is IDictionary)
            {
                foreach (var key in ((IDictionary)obj).Keys)
                {
                    var path = GetJsonPath(((IDictionary)obj)[key], objectToSearch, basePath + "/" + key, checkedObjects);
                    if (path != null)
                        return path;
                }
            }
            else if (obj is IEnumerable)
            {
                var i = 0;
                foreach (var item in (IEnumerable)obj)
                {
                    var path = GetJsonPath(item, objectToSearch, basePath + "/" + i, checkedObjects);
                    if (path != null)
                        return path;
                    i++; 
                }
            }
            else
            {
                foreach (var property in obj.GetType().GetRuntimeProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null))
                {
                    var pathSegment = GetPathSegmentName(property);
                    var value = property.GetValue(obj);
                    if (value != null)
                    {
                        var path = GetJsonPath(value, objectToSearch, basePath + "/" + pathSegment, checkedObjects);
                        if (path != null)
                            return path;
                    }
                }
            }

            return null;
        }

        private static JsonSchema4 GetObjectFromJsonPath(object obj, List<string> segments, List<object> checkedObjects)
        {
            if (obj == null || obj is string || checkedObjects.Contains(obj))
                return null;

            if (segments.Count == 0)
                return (JsonSchema4)obj;

            checkedObjects.Add(obj);

            if (obj is IDictionary)
            {
                if (((IDictionary)obj).Contains(segments.First()))
                    return GetObjectFromJsonPath(((IDictionary)obj)[segments.First()], segments.Skip(1).ToList(), checkedObjects);
            }
            else if (obj is IEnumerable)
            {
                int index;
                if (int.TryParse(segments.First(), out index))
                {
                    var enumerable = ((IEnumerable) obj).Cast<object>().ToArray(); 
                    if (enumerable.Length > index)
                        return GetObjectFromJsonPath(enumerable[index], segments.Skip(1).ToList(), checkedObjects);
                }
            }
            else
            {
                foreach (var property in obj.GetType().GetRuntimeProperties().Where(p => p.GetCustomAttribute<JsonIgnoreAttribute>() == null))
                {
                    var pathSegment = GetPathSegmentName(property);
                    var value = property.GetValue(obj);
                    if (pathSegment == segments.First())
                        return GetObjectFromJsonPath(value, segments.Skip(1).ToList(), checkedObjects);
                }
            }

            return null;
        }

        private static string GetPathSegmentName(PropertyInfo property)
        {
            var attribute = property.GetCustomAttribute<JsonPropertyAttribute>();
            var pathSegment = attribute != null && !string.IsNullOrEmpty(attribute.PropertyName)
                ? attribute.PropertyName
                : property.Name;
            return pathSegment;
        }
    }
}