namespace SIS.MvcFramework.Mapping
{
    using System;
    using System.Collections;
    using System.Linq;
    using System.Reflection;

    public static class ModelMapper
    {
        public static T ProjectTo<T>(object source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source can not be null");
            }

            T dest = (T)Activator.CreateInstance(typeof(T));

            return DoMapping<T>(source, dest);
        }

        private static T DoMapping<T>(object source, T dest)
        {
            var properties = dest
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanWrite)
                .ToArray();

            var srcProperties = source
                .GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .ToArray();


            foreach (var destProperty in properties)
            {
                var srcProperty = srcProperties.
                    Where(p => p.Name == destProperty.Name)
                    .FirstOrDefault();

                if (srcProperty == null)
                {
                    continue;
                }

                var sourceValue = srcProperty
                    .GetMethod
                    .Invoke(source, new object[0]);

                if (sourceValue is null)
                    continue;

                if (ReflectionUtils.IsPrimitive(sourceValue.GetType()))
                {
                    if (destProperty.PropertyType == typeof(string))
                    {
                        destProperty.SetValue(dest, srcProperty.GetValue(source).ToString());
                    }
                    else
                    {
                        destProperty.SetValue(dest, srcProperty.GetValue(source));
                    }

                    continue;
                }

                if (ReflectionUtils.IsGenericCollection(sourceValue.GetType()))
                {
                    if (ReflectionUtils.IsPrimitive(sourceValue.GetType().GetGenericArguments()[0]))
                    {
                        var destinationCollection = sourceValue;
                        destProperty.SetMethod.Invoke(dest, new[] { destinationCollection });
                    }

                    else
                    {
                        var destCollection = destProperty.GetMethod.Invoke(dest, new object[0]);
                        var destType = destCollection.GetType().GetGenericArguments()[0];

                        foreach (var destP in (IEnumerable)sourceValue)
                        {
                            var destInstance = Activator.CreateInstance(destType);
                            ((IList)destCollection).Add(DoMapping(destP, destInstance));
                        }
                    }
                }

                else if (ReflectionUtils.IsNonGenericCollection(sourceValue.GetType()))
                {
                    var destCollection = (IList)Activator.CreateInstance(destProperty.PropertyType,
                        new object[] { ((object[])sourceValue).Length });

                    for (int i = 0; i < ((object[])sourceValue).Length; i++)
                    {
                        destCollection[i] = DoMapping(((object[])sourceValue)[i],
                            destProperty.PropertyType.GetElementType());
                    }

                    destProperty.SetValue(dest, destCollection);
                }

                else
                {
                    // FIXME
                    //var propertyInstance = Activator.CreateInstance(srcProperty.GetValue(source).GetType());

                    var propertyType = destProperty.PropertyType;
                    var name = propertyType.Name;

                    var propertyInstance = Activator.CreateInstance(propertyType);

                    destProperty.SetValue(dest, DoMapping(sourceValue, propertyInstance));
                }
            }

            return dest;
        }
    }
}
