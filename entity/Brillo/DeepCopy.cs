using System;
using System.Reflection;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;

namespace entity.Brillo
{
    [Serializable]
    public static class DeepCopy
    {
        public static T CloneEntity<T>(this T entity)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                var bf = new BinaryFormatter();
                bf.Context = new StreamingContext(StreamingContextStates.Clone);
                bf.Serialize(stream, entity);
                stream.Position = 0;
                return (T)bf.Deserialize(stream);
            }
        }

        public static T ConeProperties<T>(T Entity)
        {
            var Type = Entity.GetType();
            var Clone = Activator.CreateInstance(Type);

            foreach (var Property in Type.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.SetProperty))
            {
                if (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(EntityReference<>)) continue;
                if (Property.PropertyType.IsGenericType && Property.PropertyType.GetGenericTypeDefinition() == typeof(EntityCollection<>)) continue;
                if (Property.PropertyType.IsSubclassOf(typeof(EntityObject))) continue;

                if (Property.CanWrite)
                {
                    Property.SetValue(Clone, Property.GetValue(Entity, null), null);
                }
            }

            return (T)Clone;
        }
    }
}
