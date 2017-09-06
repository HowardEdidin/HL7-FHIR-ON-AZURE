#region Information

// Solution:  Spark
// Spark.Engine
// File:  ResourceVisitor.cs
// 
// Created: 07/12/2017 : 10:34 AM
// 
// Modified By: Howard Edidin
// Modified:  08/20/2017 : 1:50 PM

#endregion

namespace FhirOnAzure.Engine.Auxiliary
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Hl7.Fhir.Model;

    public delegate void Visitor(Element element, string path);

    public static class ResourceVisitor
    {
        public static void VisitByType(object item, Visitor action, params Type[] filter)
        {
            // This is a filter that returns true if the property in pInfo is a subtype
            // of one of the types given in the filter. Because of this, scan() returns
            // all Elements in item that are of the types in filter, or subclasses.
            Visitor visitor = (elem, path) =>
            {
                foreach (var t in filter)
                {
                    var type = elem.GetType();
                    if (t.IsAssignableFrom(type))
                        action(elem, path);
                }
            };

            scan(item, null, visitor);
        }

        private static bool propertyFilter(MemberInfo mem, object arg)
        {
            // We prefilter on properties, so this cast is always valid
            var prop = (PropertyInfo) mem;

            // Return true if the property is either an Element or an IEnumerable<Element>.
            var isElementProperty = typeof(Element).IsAssignableFrom(prop.PropertyType);
            var collectionInterface = prop.PropertyType.GetInterface("IEnumerable`1");
            var isElementCollection = false;

            if (collectionInterface != null)
            {
                var firstGenericArg = collectionInterface.GetGenericArguments()[0];
                isElementCollection = typeof(Element).IsAssignableFrom(firstGenericArg);
            }

            return isElementProperty || isElementCollection;
        }

        private static string joinPath(string old, string part)
        {
            if (!string.IsNullOrEmpty(old))
                return old + "." + part;
            return part;
        }

        private static void scan(object item, string path, Visitor visitor)
        {
            if (item == null) return;

            if (path == null) path = string.Empty;

            // Scan the object 'item' and find all properties of type Element of IEnumerable<Element>
            var result = item.GetType().FindMembers(MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public,
                propertyFilter, null);

            // Do a depth-first traversal of the properties and their contents
            foreach (PropertyInfo property in result)
                // If this member is an IEnumerable<Element>, go inside and recurse
                if (property.PropertyType.GetInterface("IEnumerable`1") != null)
                {
                    // Since we filter for Properties of Element or IEnumerable<Element>
                    // this cast should always work
                    var list = (IEnumerable<Element>) property.GetValue(item, null);

                    if (list != null)
                    {
                        var index = 0;
                        foreach (var element in list)
                        {
                            var propertyPath = joinPath(path, property.Name + "[" + index + "]");

                            if (element != null)
                            {
                                visitor(element, propertyPath);
                                scan(element, propertyPath, visitor);
                            }
                        }
                    }
                }

                // If this member is an Element, go inside and recurse
                else
                {
                    var propertyPath = joinPath(path, property.Name);

                    var propValue = (Element) property.GetValue(item);

                    // Look into the property to find nested elements
                    if (propValue != null)
                    {
                        visitor(propValue, propertyPath);
                        scan(propValue, propertyPath, visitor);
                    }
                }
        }
    }
}