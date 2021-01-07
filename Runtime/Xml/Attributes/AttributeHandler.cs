using System;
using System.Collections.Generic;
using System.Linq;
using Facebook.Yoga;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThreeDISevenZeroR.XmlUI
{
    public class AttributeHandler<T> : IAttributeHandler<T>
    {
        private readonly List<PropertyInfo> propertyInfos =
            new List<PropertyInfo>();

        public IAttributeInfo[] Attributes => propertyInfos.Cast<IAttributeInfo>().ToArray();

        public IAttributeCollection<T> ParseAttributes(Dictionary<string, string> attributes)
        {
            var attrs = ParseAttributesInternal(attributes);
            return new AttributeCollection<T>
            {
                SerializableConstants = attrs.Constants.Where(c => c.IsSerializable).Cast<IConstantSetter<T>>().ToArray(),
                NonSerializableConstants = attrs.Constants.Where(c => !c.IsSerializable).Cast<IConstantSetter<T>>().ToArray(),
                Variables = attrs.Variables.Cast<IVariableBinder<T>>().ToArray()
            };
        }

        private ParsedAttributes ParseAttributesInternal(Dictionary<string, string> ownAttributes)
        {
            var attrs = new ParsedAttributes();

            for (var i = 0; i < propertyInfos.Count; i++)
                propertyInfos[i].propertyParser(ownAttributes, attrs);

            return attrs;
        }

        public AttributeHandler<T> AddComponentReferenceAttr<C>(string name, ValueSetterDelegate<T, C> setter, bool isSerializable = true) =>
            AddGenericAttr(AttributeType.String, name, (e, c, v) => setter(e, c, e.FindComponentById<C>(v)));
        
        public AttributeHandler<T> AddStringAttr(string name, ValueSetterDelegate<T, string> setter, bool isSerializable = true) => 
            AddGenericAttr(AttributeType.String, name, setter, isSerializable);

        public AttributeHandler<T> AddBoolAttr(string name, ValueSetterDelegate<T, bool> setter, bool isSerializable = true) =>
            AddGenericAttr(AttributeType.Boolean, name, setter, isSerializable);
        
        public AttributeHandler<T> AddIntAttr(string name, ValueSetterDelegate<T, int> setter, bool isSerializable = true) =>
            AddGenericAttr(AttributeType.Integer, name, setter, isSerializable);
        
        public AttributeHandler<T> AddFloatAttr(string name, ValueSetterDelegate<T, float> setter, bool isSerializable = true) =>
            AddGenericAttr(AttributeType.Float, name, setter, isSerializable);
        
        public AttributeHandler<T> AddColorAttr(string name, ValueSetterDelegate<T, Color> setter, bool isSerializable = true) => 
            AddGenericAttr(AttributeType.HtmlColor, name, setter, isSerializable);

        public AttributeHandler<T> AddVectorAttr(string name, ValueSetterDelegate<T, Vector4> setter, bool isSerializable = true) => 
            AddGenericAttr(AttributeType.VectorValue, name, setter, isSerializable);
        
        public AttributeHandler<T> AddYogaValueAttr(string name, ValueSetterDelegate<T, YogaValue> setter, bool isSerializable = true) =>
            AddGenericAttr(AttributeType.YogaValue, name, setter, isSerializable);

        public AttributeHandler<T> AddResourceAttr<P>(string name, ValueSetterDelegate<T, P> setter, bool isSerializable = true) 
            where P : Object => AddGenericAttr(AttributeType.GetResourceType<P>(), name, setter, isSerializable);
        
        public AttributeHandler<T> AddEnumAttr<P>(string name, ValueSetterDelegate<T, P> setter, bool isSerializable = true) 
            where P : struct, Enum => AddGenericAttr(AttributeType.GetEnumTypeInfo<P>(), name, setter, isSerializable);

        public AttributeHandler<T> AddGenericAttr<P>(TypeInfo<P> typeInfo, string name, 
            ValueSetterDelegate<T, P> setter, bool isSerializable = true)
        {
            var parser = typeInfo.parser;

            propertyInfos.Add(new PropertyInfo
            {
                name = name,
                schema = typeInfo,
                propertyParser = (values, props) =>
                {
                    if(!values.TryGetValue(name, out var attrText))
                        return;

                    if (attrText.StartsWith("$"))
                    {
                        var referenceName = attrText.Substring(1);
                        props.AddVariable(name, referenceName, setter);
                        return;
                    }

                    // Escape $ as \$
                    if (attrText.StartsWith("\\$"))
                        attrText = attrText.Substring(1);

                    if (parser == null)
                    {
                        Debug.LogError($"Value of type {typeof(P)} cannot be created from XML");
                    }
                    else if (parser(attrText, out var result))
                    {
                        props.AddConstant(name, result, setter, isSerializable);
                    }
                    else
                    {
                        Debug.Log($"Cannot parse value \"{attrText}\" of attribute \"{name}\" as {typeof(P)}");
                    }
                }
            });

            return this;
        }
        
        public IAttributeHandler<O> AsBatchForObject<O>(T batchObject, BatchGetter<O, T> getter, BatchSetter<O, T> setter)
        {
            return new BatchAttributeHandler<O>(this, batchObject, getter, setter);
        }

        private class PropertyInfo : IAttributeInfo
        {
            public string name;
            public TypeInfo schema;
            public Action<Dictionary<string, string>, ParsedAttributes> propertyParser;

            public string Name => name;
            public TypeInfo Type => schema;
        }

        private class ParsedAttributes
        {
            public readonly List<ConstantSetter<T>> Constants 
                = new List<ConstantSetter<T>>();
            
            public readonly List<IVariableBinder<T>> Variables
                = new List<IVariableBinder<T>>();
            
            public void AddConstant<P>(string attributeName, P value, 
                ValueSetterDelegate<T, P> apply, bool isInstanceConstant)
            {
                Constants.Add(new ConstantSetter<T>(new[] {attributeName}, (e, t) => apply(e, t, value), isInstanceConstant));
            }

            public void AddVariable<P>(string attributeName, string variableName, ValueSetterDelegate<T, P> apply)
            {
                Variables.Add((new VariableBinder<T, P>(new[] {attributeName}, variableName, apply)));
            }
        }
        
        private class BatchAttributeHandler<O> : IAttributeHandler<O>
        {
            private readonly AttributeHandler<T> parent;
            private readonly BatchGetter<O, T> batchGetter;
            private readonly BatchSetter<O, T> batchSetter;
            private readonly T batchObject;

            public BatchAttributeHandler(AttributeHandler<T> parent, T batchObject, 
                BatchGetter<O, T> batchGetter, BatchSetter<O, T> batchSetter)
            {
                this.parent = parent;
                this.batchObject = batchObject;
                this.batchGetter = batchGetter;
                this.batchSetter = batchSetter;
            }

            public IAttributeInfo[] Attributes => parent.Attributes;
            
            public IAttributeCollection<O> ParseAttributes(Dictionary<string, string> ownAttributes)
            {
                var parsed = parent.ParseAttributesInternal(ownAttributes);
                var result = new AttributeCollection<O>();

                var variables = parsed.Variables;

                IConstantSetter<O>[] CollectConstants(IEnumerable<ConstantSetter<T>> setters, bool isInstance)
                {
                    var list = setters.ToList();
                    
                    if(list.Count == 0)
                        return new IConstantSetter<O>[0];
                    
                    var attributes = list
                        .SelectMany(c => c.AttributeNames)
                        .ToArray();

                    var delegates = list
                        .Select(c => c.Setter)
                        .ToArray();
                    
                    return new IConstantSetter<O>[]
                    {
                        new ConstantSetter<O>(attributes, (e, o) =>
                        {
                            batchGetter(o, batchObject);

                            foreach (var d in delegates)
                                d(e, batchObject);

                            batchSetter(o, batchObject);
                        }, isInstance)
                    };
                }

                result.SerializableConstants = CollectConstants(parsed.Constants.Where(c => !c.IsSerializable), false);
                result.NonSerializableConstants = CollectConstants(parsed.Constants.Where(c => c.IsSerializable), true);

                if (variables.Count > 0)
                {
                    var variableAttributes = variables
                        .SelectMany(c => c.AttributeNames)
                        .ToArray();

                    result.Variables = new IVariableBinder<O>[]
                    {
                        new BatchAttributeBinder<O>(variableAttributes, variables.ToArray(), 
                            batchGetter, batchSetter, batchObject), 
                    };
                }
                else
                {
                    result.Variables = new IVariableBinder<O>[0];
                }

                return result;
            }
        }
        
        private class BatchAttributeBinder<O> : IVariableBinder<O>
        {
            public string[] AttributeNames { get; }

            private readonly IVariableBinder<T>[] variableBinders;
            private readonly BatchGetter<O, T> batchGetter;
            private readonly BatchSetter<O, T> batchSetter;
            private readonly T batchObject;

            public BatchAttributeBinder(string[] attributeNames, IVariableBinder<T>[] variableBinders,
                BatchGetter<O, T> batchGetter, BatchSetter<O, T> batchSetter, T batchObject)
            {
                AttributeNames = attributeNames;
                
                this.variableBinders = variableBinders;
                this.batchGetter = batchGetter;
                this.batchSetter = batchSetter;
                this.batchObject = batchObject;
            }

            public IBoundVariable Bind(XmlLayoutElement element, O instance, IVariableProvider provider)
            {
                var boundVariables = new IBoundVariable[variableBinders.Length];
                
                for (var i = 0; i < boundVariables.Length; i++)
                    boundVariables[i] = variableBinders[i].Bind(element, batchObject, provider);

                return new BoundVariable(this, instance, boundVariables);
            }
                
            private class BoundVariable : IBoundVariable
            {
                private BatchAttributeBinder<O> parent;
                private O instance;
                private IBoundVariable[] boundVariables;
                
                public event Action OnUpdated;

                public BoundVariable(BatchAttributeBinder<O> parent, O instance, IBoundVariable[] boundVariables)
                {
                    this.parent = parent;
                    this.instance = instance;
                    this.boundVariables = boundVariables;

                    foreach (var bound in boundVariables)
                        bound.OnUpdated += ApplyUpdated;
                }
                
                public void Apply()
                {
                    parent.batchGetter(instance, parent.batchObject);

                    for (var i = 0; i < boundVariables.Length; i++)
                        boundVariables[i].Apply();

                    parent.batchSetter(instance, parent.batchObject);
                }
                
                private void ApplyUpdated()
                {
                    Apply();
                    OnUpdated?.Invoke();
                }

                public void Unbind()
                {
                    foreach (var bound in boundVariables)
                        bound.OnUpdated -= ApplyUpdated;
                }
            }
        }
    }
}