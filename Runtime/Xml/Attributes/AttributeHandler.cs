using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ThreeDISevenZeroR.XmlUI
{
    public class AttributeHandler<T> : IAttributeHandler<T>
    {
        public delegate bool StringParser<P>(string text, out P value);

        private readonly List<PropertyInfo> propertyInfos =
            new List<PropertyInfo>();

        private readonly List<string> propertyNames =
            new List<string>();

        private bool isConstantsSerializable = true;

        public IAttributeInfo[] Attributes => propertyInfos.Cast<IAttributeInfo>().ToArray();

        public bool HasRequiredAttributes(Dictionary<string, string> attributes)
        {
            for (var i = 0; i < propertyNames.Count; i++)
            {
                if (attributes.ContainsKey(propertyNames[i]))
                    return true;
            }

            return false;
        }

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

        public AttributeHandler<T> SetConstantsSerializable(bool isSerializable)
        {
            isConstantsSerializable = isSerializable;
            return this;
        }

        public AttributeHandler<T> AddResourceProperty<P>(string name, ValueSetterDelegate<T, P> setter) 
            where P : Object => AddGenericProperty(name, "Unity Resource Path", (string text, out P value) => {
                value = Resources.Load<P>(text);

                if (!value)
                    value = Resources.GetBuiltinResource<P>(text);
                
                return value;
            }, setter);
        
        public AttributeHandler<T> AddEnumProperty<P>(string name, ValueSetterDelegate<T, P> setter) 
            where P : struct, Enum => AddGenericProperty(name, string.Join("\n", Enum.GetNames(typeof(P))), Enum.TryParse, setter);

        public AttributeHandler<T> AddGenericProperty<P>(string name, string format, StringParser<P> parser, ValueSetterDelegate<T, P> setter)
        {
            propertyNames.Add(name);
            propertyInfos.Add(new PropertyInfo
            {
                name = name,
                format = format,
                type = typeof(P),
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
                        props.AddConstant(name, result, setter, 
                            isConstantsSerializable);
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
            public string format;
            public Type type;
            public Action<Dictionary<string, string>, ParsedAttributes> propertyParser;

            public string Name => name;
            public Type Type => type;
            public Type TargetType => typeof(T);
            public string FormatHint => format;
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
                Constants.Add(new ConstantSetter<T>(new[] {attributeName}, t => apply(t, value), isInstanceConstant));
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

            public bool HasRequiredAttributes(Dictionary<string, string> attributes)
            {
                return parent.HasRequiredAttributes(attributes);
            }

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
                        .Select(c => c.SetterDelegate)
                        .ToArray();
                    
                    return new IConstantSetter<O>[]
                    {
                        new ConstantSetter<O>(attributes, o =>
                        {
                            batchGetter(o, batchObject);

                            foreach (var d in delegates)
                                d(batchObject);

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

            public IBoundVariable Bind(O instance, IVariableProvider provider)
            {
                var boundVariables = new IBoundVariable[variableBinders.Length];
                
                for (var i = 0; i < boundVariables.Length; i++)
                    boundVariables[i] = variableBinders[i].Bind(batchObject, provider);

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