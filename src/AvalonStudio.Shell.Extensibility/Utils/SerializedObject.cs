using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System.Collections;
using System.IO;
using System.Reflection;

namespace AvalonStudio.Utils
{
    public class IgnoreEmptyEnumerableResolver : DefaultContractResolver
    {
        public static readonly IgnoreEmptyEnumerableResolver Instance = new IgnoreEmptyEnumerableResolver();

        public IgnoreEmptyEnumerableResolver()
        {
            NamingStrategy = new CamelCaseNamingStrategy(false, false);
        }

        protected override JsonProperty CreateProperty(MemberInfo member,
            MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType != typeof(string) &&
                typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
            {
                property.ShouldSerialize = instance =>
                {
                    IEnumerable enumerable = null;
                    // this value could be in a public field or public property
                    switch (member.MemberType)
                    {
                        case MemberTypes.Property:
                            enumerable = instance
                                .GetType()
                                .GetProperty(member.Name)
                                ?.GetValue(instance, null) as IEnumerable;
                            break;
                        case MemberTypes.Field:
                            enumerable = instance
                                .GetType()
                                .GetField(member.Name)
                                .GetValue(instance) as IEnumerable;
                            break;
                    }

                    return enumerable == null ||
                           enumerable.GetEnumerator().MoveNext();
                    // if the list is null, we defer the decision to NullValueHandling
                };
            }

            if (property.PropertyType == typeof(string))
            {
                // Do not include emptry strings
                property.ShouldSerialize = instance =>
                {
                    return !string.IsNullOrWhiteSpace(instance.GetType().GetProperty(member.Name).GetValue(instance, null) as string);
                };
            }

            return property;
        }
    }

    public class SerializedObject
    {
        private static JsonSerializerSettings DefaultSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
            DefaultValueHandling = DefaultValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            ContractResolver = IgnoreEmptyEnumerableResolver.Instance,
            Converters = new JsonConverter[] { new StringEnumConverter(), new VersionConverter() },
        };

        private static JsonSerializer DefaultSerializer = JsonSerializer.Create(DefaultSettings);

        public static void Serialize(string filename, object item)
        {
            using (var writer = File.CreateText(filename))
            {
                writer.Write(JsonConvert.SerializeObject(item, Formatting.Indented, DefaultSettings));
            }
        }

        public static T FromString<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static T Deserialize<T>(string filename)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filename));
        }

        public static void PopulateObject(TextReader reader, object target)
        {
            DefaultSerializer.Populate(reader, target);
        }
    }
}