using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    public class LayoutInflater : MonoBehaviour
    {
        private const char AttrsDelimeter = ',';

        public const string AttrsReferenceAttributeId = "Attrs";
        public const string AttrsCollectionNameAttributeId = "Attrs.Name";
        public const string AttrsCollectionParentAttributeId = "Attrs.Parent";

        public const string AttrsCollectionElementName = "AttributeCollection";
        public const string AttrsCollectionEntryName = "Attrs";
        public const string AttrsChildRootName = "ChildRoot";

        private readonly Dictionary<string, IXmlElementInfo> elementTypes =
            new Dictionary<string, IXmlElementInfo>();
        
        private readonly Dictionary<string, IXmlComponentInfo> componentTypes = 
            new Dictionary<string, IXmlComponentInfo>();

        private readonly Dictionary<string, AttrsCollectionNode> attributeDictionary =
            new Dictionary<string, AttrsCollectionNode>();

        private readonly Dictionary<string, Dictionary<string, string>> flattenedAttributeDictionary =
            new Dictionary<string, Dictionary<string, string>>();
        
        private readonly Dictionary<string, LayoutElement> createdPrefabs =
            new Dictionary<string, LayoutElement>();
        
        private readonly Dictionary<string, Stack<LayoutElement>> pooledObjects = 
            new Dictionary<string, Stack<LayoutElement>>();
        
        private readonly List<PooledLayout> pooledElementsGetComponentList = 
            new List<PooledLayout>();

        [SerializeField] 
        private TextAsset[] attributeCollections;

        [SerializeField] 
        private ElementCollection[] elementCollections;

        private Transform prefabRootTransform;
        private Transform poolRootTransform;
        
        private bool isInitialized;
        
        public IXmlElementInfo[] GetRegisteredElements()
        {
            return elementCollections.SelectMany(e => e.Elements).ToArray();
        }

        public IXmlComponentInfo[] GetRegisteredComponents()
        {
            return elementCollections.SelectMany(e => e.Components).ToArray();
        }

        public void Init()
        {
            if (isInitialized)
                return;

            foreach (var collection in elementCollections)
            {
                foreach (var element in collection.Elements)
                    RegisterElement(element);

                foreach (var component in collection.Components)
                    RegisterComponent(component);
            }

            CreatePrefabRoot();
            AddAttributeCollection(attributeCollections);

            isInitialized = true;
        }

        public void ReturnToPool(Component element)
        {
            element.GetComponentsInChildren(true, pooledElementsGetComponentList);

            if (pooledElementsGetComponentList.Count == 0)
            {
                Debug.Log($"Unable to return {element.name} to pool");
                Destroy(element);
                return;
            }

            for (var i = 0; i < pooledElementsGetComponentList.Count; i++)
            {
                var pooled = pooledElementsGetComponentList[i];

                if (!pooledObjects.TryGetValue(pooled.Layout, out var stack))
                {
                    stack = new Stack<LayoutElement>();
                    pooledObjects[pooled.Layout] = stack;
                }
                
                pooled.Element.DeactivateHierarchy();
                pooled.Element.DetachFromParent();
                pooled.Element.transform.SetParent(poolRootTransform, false);
                stack.Push(pooled.Element);
            }

            // to ensure every object in hierarchy has been moved to pool
            for (var i = 0; i < pooledElementsGetComponentList.Count; i++)
                pooledElementsGetComponentList[i].Element.OnReturnedToPool();
        }
        
        public Layout<T> InflateChild<T>(LayoutElement parent, string layoutXml, IVariableProvider provider = null)
            where T : Component
        {
            var element = InflateChild(parent, layoutXml, provider);
            return new Layout<T>(element, element.GetComponent<T>());
        }

        public LayoutElement InflateChild(LayoutElement parent, string xmlString,
            IVariableProvider provider = null)
        {
            var child = Inflate(parent.ChildParentTransform, xmlString, provider);
            parent.Container.AddChild(child);
            return child;
        }
        
        public Layout<T> Inflate<T>(Transform root, string layoutXml, IVariableProvider provider = null, Dictionary<string, string> outerAttrs = null) 
            where T : Component
        {
            var element = Inflate(root, layoutXml, provider, outerAttrs);
            return new Layout<T>(element, element.GetComponent<T>());
        }

        public LayoutElement Inflate(Transform root, string layoutXml,
            IVariableProvider provider = null, Dictionary<string, string> outerAttrs = null)
        {
            Init();

            LayoutElement instance;

            if (outerAttrs == null)
            {
                if (pooledObjects.TryGetValue(layoutXml, out var list) && list != null && list.Count > 0)
                {
                    // get from pool
                    instance = list.Pop();
                    instance.transform.SetParent(root, false);
                    instance.OnBroughtBackFromPool();
                }
                else
                {
                    if (!createdPrefabs.TryGetValue(layoutXml, out var prefab))
                    {
                        // create new prefab
                        prefab = CreateXmlInstance(prefabRootTransform, layoutXml, outerAttrs);
                        createdPrefabs[layoutXml] = prefab;
                    }

                    // clone from prefab
                    instance = Instantiate(prefab, root, false);
                
                    var pooledLayout = instance.gameObject.AddComponent<PooledLayout>();
                    pooledLayout.SetElement(instance, layoutXml);
                }
            }
            else
            {
                // create new instance
                instance = CreateXmlInstance(root, layoutXml, outerAttrs);
            }
            
            if (instance.TryGetComponent<ComponentVariableBinder>(out var holder))
                holder.SetVariableProvider(provider);
            
            instance.ActivateHierarchy();
            return instance;
        }

        public LayoutElement CreateXmlInstance(Transform root, string xmlString,
            Dictionary<string, string> outerAttrs)
        {
            Init();

            var rootNode = ParseXmlElements(xmlString, outerAttrs);
            var boundAttrs = new BoundAttributeCollection();
            var newPrefab = CreateInstance(null, root, rootNode, boundAttrs);
            
            newPrefab.DeactivateHierarchy();

            if (!boundAttrs.IsEmpty)
                newPrefab.gameObject.AddComponent<ComponentVariableBinder>()
                    .SetBoundAttrs(boundAttrs);

            return newPrefab;
        }

        private void Awake()
        {
            Init();
        }

        private void AddAttributeCollection(TextAsset[] assets)
        {
            foreach (var asset in assets)
                AddAttributeCollection(asset.text);
        }

        private void AddAttributeCollection(string xml)
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            if (doc.DocumentElement == null)
            {
                Debug.LogError($"Unable to parse XML");
                return;
            }

            if (doc.DocumentElement.Name != AttrsCollectionElementName)
            {
                Debug.LogError($"Unknown document element name {doc.DocumentElement.Name}, expected {AttrsCollectionElementName}");
                return;
            }

            foreach (XmlNode childNode in doc.DocumentElement.ChildNodes)
            {
                if (childNode.NodeType != XmlNodeType.Element)
                    continue;

                XmlElement elementNode = (XmlElement) childNode;

                if (elementNode.Name == AttrsCollectionEntryName)
                {
                    var attrName = elementNode.GetAttribute(AttrsCollectionNameAttributeId);

                    if (string.IsNullOrWhiteSpace(attrName))
                    {
                        Debug.LogWarning("Attribute has no name");
                        continue;
                    }

                    var node = new AttrsCollectionNode
                    {
                        ownAttrs = CollectAttributes(elementNode)
                    };

                    if (node.ownAttrs.TryGetValue(AttrsCollectionParentAttributeId, out var parentAttrs))
                    {
                        node.parent = parentAttrs.Split(AttrsDelimeter)
                            .Select(a => a.Trim())
                            .ToArray();
                    }
                    else
                    {
                        node.parent = new string[0];
                    }

                    attributeDictionary[attrName] = node;
                }
            }
        }

        private void RegisterElement(IXmlElementInfo info)
        {
            elementTypes[info.Name] = info;
        }

        private void RegisterComponent(IXmlComponentInfo info)
        {
            componentTypes[info.Name] = info;
        }

        private void CreatePrefabRoot()
        {
            var prefabRoot = new GameObject("Prefabs");
            var poolRoot = new GameObject("Pool");

            prefabRoot.AddComponent<RectTransform>();
            prefabRoot.SetActive(false);
            prefabRoot.transform.SetParent(transform, false);

            poolRoot.AddComponent<RectTransform>();
            poolRoot.SetActive(false);
            poolRoot.transform.SetParent(transform, false);
            
            prefabRootTransform = prefabRoot.transform;
            poolRootTransform = poolRoot.transform;
        }

        private ElementNode ParseXmlElements(string xmlString, Dictionary<string, string> outerAttrs)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            ElementNode ParseNode(XmlElement element)
            {
                var result = new ElementNode();
                var attrs = CollectElementAttributes(element, outerAttrs);

                result.type = element.Name;
                result.ownAttrs = attrs;
                result.childNodes = new List<ElementNode>();

                if (element.Name == AttrsChildRootName)
                    return result;
                
                var components = new List<IXmlComponentFactory>();
                var componentPrefix = element.Name + ".";
                
                result.factory = CreateElementFactory(element.Name, attrs);

                for (var i = 0; i < element.ChildNodes.Count; i++)
                {
                    var xmlChild = element.ChildNodes[i];

                    if (xmlChild.NodeType == XmlNodeType.Element)
                    {
                        // Component
                        if (xmlChild.Name.StartsWith(componentPrefix))
                        {
                            var componentAttrs = CollectElementAttributes((XmlElement) xmlChild, outerAttrs);
                            var factory = CreateComponentFactory(xmlChild.Name.Substring(componentPrefix.Length), 
                                componentAttrs);
                            
                            components.Add(factory);
                        }
                        else // Element
                        {
                            result.childNodes.Add(ParseNode((XmlElement) xmlChild));
                        }
                    }
                }

                result.components = components.ToArray();
                return result;
            }

            return ParseNode(xmlDoc.DocumentElement);
        }

        private Dictionary<string, string> CollectAttributes(XmlElement element,
            Dictionary<string, string> outerAttrs = null)
        {
            var result = new Dictionary<string, string>();

            for (var i = 0; i < element.Attributes.Count; i++)
            {
                var attr = element.Attributes[i];
                var value = outerAttrs != null ? ResolveAttrValue(
                    attr.Value, outerAttrs) : attr.Value;

                if (value != null)
                    result[attr.Name] = value;
            }

            return result;
        }

        private string ResolveAttrValue(string value, Dictionary<string, string> outerAttrs)
        {
            if (value.StartsWith("@"))
            {
                var outerAttrName = value.Substring(1);
                return outerAttrs.TryGetValue(outerAttrName, out var outerValue)
                    ? outerValue
                    : null;
            }

            // Escape @ symbol at start
            if (value.StartsWith("\\@"))
                return value.Substring(1);

            return value;
        }

        private Dictionary<string, string> CollectElementAttributes(XmlElement element,
            Dictionary<string, string> outerAttrs)
        {
            var result = CollectAttributes(element, outerAttrs);

            if (result.TryGetValue(AttrsReferenceAttributeId, out var referencedAttrs))
            {
                foreach (var attr in GetAttributes(referencedAttrs))
                {
                    if (!result.ContainsKey(attr.Key))
                        result[attr.Key] = ResolveAttrValue(attr.Value, outerAttrs);
                }
            }

            return result;
        }

        private Dictionary<string, string> GetAttributes(string attrListString)
        {
            if (flattenedAttributeDictionary.TryGetValue(attrListString, out var existingAttrs))
                return existingAttrs;

            var result = new Dictionary<string, string>();
            var cyclicReferenceSet = new HashSet<object>();

            void AddAttrs(string attrId)
            {
                if (!attributeDictionary.TryGetValue(attrId, out var collection))
                {
                    Debug.LogWarning($"Unknown attribute collection: {attrId}");
                    return;
                }

                if (!cyclicReferenceSet.Add(collection))
                {
                    Debug.LogWarning($"Cyclic reference detected for attribute collection: {attrId}");
                    return;
                }

                foreach (var attr in collection.ownAttrs)
                {
                    if (!result.ContainsKey(attr.Key))
                        result[attr.Key] = attr.Value;
                }

                foreach (var parentAttr in collection.parent)
                    AddAttrs(parentAttr);
            }

            foreach (var attrId in attrListString.Split(AttrsDelimeter))
            {
                AddAttrs(attrId.Trim());
            }

            flattenedAttributeDictionary[attrListString] = result;
            return result;
        }

        private IXmlElementFactory CreateElementFactory(string element,
            Dictionary<string, string> attrs)
        {
            if (!elementTypes.TryGetValue(element, out var type))
                throw new ArgumentException($"Unknown element type: {element}");

            return type.CreateFactory(attrs);
        }

        private IXmlComponentFactory CreateComponentFactory(string component, Dictionary<string, string> attrs)
        {
            if (!componentTypes.TryGetValue(component, out var type))
                throw new ArgumentException($"Unknown element type: {component}");

            return type.CreateFactory(attrs);
        }

        private LayoutElement CreateInstance(LayoutElement elementRoot, Transform root, 
            ElementNode element, BoundAttributeCollection binders)
        {
            var instance = element.factory.CreateElement(root, binders, this, element.ownAttrs);
            var container = instance.Container;

            if (!elementRoot)
                elementRoot = instance;

            for (var i = 0; i < element.childNodes.Count; i++)
            {
                var node = element.childNodes[i];

                switch (node.type)
                {
                    case AttrsChildRootName:
                        elementRoot.Container = instance;
                        break;

                    default:
                        var childInstance = CreateInstance(elementRoot, container.ChildParentTransform, node, binders);
                        container.AddChild(childInstance);
                        break;
                }
            }
            
            element.factory.BindAttrs(instance, binders);

            foreach (var c in element.components)
                c.BindAttrs(instance, binders);
            
            instance.OnCreatedFromXml();
            return instance;
        }

        private class ElementNode
        {
            public string type;
            public Dictionary<string, string> ownAttrs;
            public IXmlElementFactory factory;
            public IXmlComponentFactory[] components;
            public List<ElementNode> childNodes;
        }

        private class AttrsCollectionNode
        {
            public Dictionary<string, string> ownAttrs;
            public string[] parent;
        }
        
        private class PooledLayout : MonoBehaviour
        {
            public LayoutElement Element => element;
            public string Layout => layout;
            
            private LayoutElement element;
            private string layout;

            public void SetElement(LayoutElement e, string layout)
            {
                this.element = e;
                this.layout = layout;
            }
        }
    }
}