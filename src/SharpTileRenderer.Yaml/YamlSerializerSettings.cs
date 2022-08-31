using SharpTileRenderer.TileMatching;
using SharpYaml;
using SharpYaml.Events;
using SharpYaml.Serialization;
using SharpYaml.Serialization.Descriptors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace SharpTileRenderer.Yaml
{
    public static class YamlSerializerSettings
    {
        public static readonly IMemberNamingConvention NamingConvention = new CamelCaseNamingConvention();

        public static SerializerSettings CreateDefaultSerializerSettings()
        {
            var serializerSettings = new SerializerSettings
            {
                NamingConvention = NamingConvention,
                EmitAlias = false,
                EmitTags = false,
                EmitDefaultValues = false,
                IgnoreUnmatchedProperties = false,
                SerializeDictionaryItemsAsMembers = true,
                IgnoreNulls = true,
                Attributes =
                {
                    AttributeRemap = RemapDataMemberAttributes,
                    PrepareMembersCallback = FilterUnmarkedProperties
                }
            };
            serializerSettings.RegisterSerializer(typeof(SpriteTag), new SpriteTagFactory());
            return serializerSettings;
        }

        class SpriteTagFactory : IYamlSerializable
        {
            public object? ReadYaml(ref ObjectContext objectContext)
            {
                var scalar = objectContext.Reader.Expect<Scalar>();
                var text = scalar.Value;
                if (SpriteTag.Parse(text).TryGetValue(out var tag))
                {
                    return tag;
                }

                return null;
            }

            public void WriteYaml(ref ObjectContext objectContext)
            {
                var tag = objectContext.Instance is SpriteTag ? (SpriteTag)objectContext.Instance : default;
                var isSchemaImplicitTag = objectContext.SerializerContext.Schema.IsTagImplicit(objectContext.Tag);
                var scalar = new ScalarEventInfo(objectContext.Instance, typeof(SpriteTag))
                {
                    IsPlainImplicit = isSchemaImplicitTag,
                    Style = ScalarStyle.Any,
                    Anchor = objectContext.Anchor,
                    Tag = objectContext.Tag,
                    RenderedValue = tag.ToString()
                };
                objectContext.SerializerContext.Writer.Emit(scalar);
            }
        }
        
        public static void FilterUnmarkedProperties(ObjectDescriptor objectDescriptor, List<IMemberDescriptor> members)
        {
            // check if member has DataMember or Yaml attribute on it.
            for (var i = members.Count - 1; i >= 0; i--)
            {
                var m = members[i];
                if (m is MemberDescriptorBase b)
                {
                    var dm = b.MemberInfo.GetCustomAttributes<DataMemberAttribute>().FirstOrDefault();
                    if (dm == null)
                    {
                        // remove the member if it is not explicitly tagged as serializable. 
                        members.RemoveAt(i);
                        continue;
                    }

                    if (typeof(IDictionary).IsAssignableFrom(b.Type))
                    {
                        
                    }

                    if (typeof(ICollection).IsAssignableFrom(b.Type))
                    {
                        var p = typeof(MemberDescriptorBase).GetProperty(nameof(MemberDescriptorBase.ShouldSerialize));
                        var setter = p?.SetMethod;
                        setter?.Invoke(b, new object?[]{ShouldSerializeList(b, b.ShouldSerialize)});
                    }
                }
            }
            // if List or dictionary, check if empty.

            static Func<object, bool> ShouldSerializeList(MemberDescriptorBase b, Func<object, bool>? old)
            {
                return (o) =>
                {
                    if (old != null && !old.Invoke(o))
                    {
                        return false;
                    }
                    
                    var value = b.Get(o);
                    if (value == null)
                    {
                        return false;
                    }
                    
                    if (value is ICollection c)
                    {
                        return c.Count > 0;
                    }

                    return true;
                };
            }
            
        }

        static readonly YamlIgnoreAttribute ignoreAttribute = new YamlIgnoreAttribute();
        public static Attribute RemapDataMemberAttributes(Attribute a)
        {
            return a switch
            {
                DataMemberAttribute dm => new YamlMemberAttribute(dm.Name)
                {
                    Order = dm.Order
                },
                IgnoreDataMemberAttribute => ignoreAttribute,
                _ => a
            };
        }
    }
}