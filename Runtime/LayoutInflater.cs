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

        private const string AttrsReferenceId = "Id";
        private const string AttrsReferenceAttributeId = "Attrs";
        private const string AttrsCollectionNameAttributeId = "Attrs.Name";
        private const string AttrsCollectionParentAttributeId = "Attrs.Parent";

        private const string AttrsCollectionElementName = "AttributeCollection";
        private const string AttrsCollectionEntryName = "Attrs";
        private const string AttrsChildRootName = "ChildRoot";

        private readonly Dictionary<string, IXmlElementInfo> elementTypes =
            new Dictionary<string, IXmlElementInfo>();

        private readonly Dictionary<string, XmlElementComponent> createdPrefabs =
            new Dictionary<string, XmlElementComponent>();

        private readonly Dictionary<string, AttrsCollectionNode> attributeDictionary =
            new Dictionary<string, AttrsCollectionNode>();

        private readonly Dictionary<string, Dictionary<string, string>> flattenedAttributeDictionary =
            new Dictionary<string, Dictionary<string, string>>();

        [SerializeField] private TextAsset[] attributeCollections;

        [SerializeField] private ElementCollection[] elementCollections;

        public IXmlElementInfo[] RegisteredElements => elementTypes.Values.ToArray();

        private Transform prefabRootTransform;
        private bool isInitialized;

        public void Init()
        {
            if (isInitialized)
                return;

            AddAttributeCollection(attributeCollections);
            AddElements(elementCollections);
            CreatePrefabRoot();

            isInitialized = true;
        }

        public XmlElementComponent Inflate(Transform root, string xmlString,
            IVariableProvider provider,
            Dictionary<string, string> outerAttrs = null)
        {
            Init();

            XmlElementComponent instance;

            if (outerAttrs == null)
            {
                if (!createdPrefabs.TryGetValue(xmlString, out var prefab))
                {
                    prefab = CreateInstance(prefabRootTransform, xmlString, outerAttrs);
                    createdPrefabs[xmlString] = prefab;
                }

                instance = Instantiate(prefab, root, false);
            }
            else
            {
                instance = CreateInstance(root, xmlString, outerAttrs);
            }

            if (instance.TryGetComponent<ComponentVariableBinder>(out var holder))
                holder.SetVariableProvider(provider);

            return instance;
        }

        public XmlElementComponent CreateInstance(Transform root, string xmlString,
            Dictionary<string, string> outerAttrs)
        {
            Init();

            var rootNode = ParseXmlElements(xmlString, outerAttrs);
            var boundAttrs = new BoundVariableCollection();
            var newPrefab = CreateInstance(null, root, rootNode, boundAttrs);

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

        private void AddElements(ElementCollection[] collections)
        {
            foreach (var c in collections)
            {
                foreach (var e in c.Elements)
                {
                    RegisterElement(e);
                }
            }
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

                    var node = new AttrsCollectionNode();
                    node.ownAttrs = CollectAttributes(elementNode);

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

        private void CreatePrefabRoot()
        {
            var prefabRoot = new GameObject("Prefabs");
            prefabRoot.transform.parent = transform;
            prefabRoot.SetActive(false);
            prefabRootTransform = prefabRoot.transform;
        }

        private ElementNode ParseXmlElements(string xmlString, Dictionary<string, string> outerAttrs)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xmlString);

            ElementNode ParseNode(XmlElement element)
            {
                var result = new ElementNode();
                var attrs = CollectElementAttributes(element, outerAttrs);

                attrs.TryGetValue(AttrsReferenceId, out result.id);

                result.type = element.Name;
                result.factory = CreateFactory(element.Name, attrs);
                result.ownAttrs = attrs;

                for (var i = 0; i < element.ChildNodes.Count; i++)
                {
                    var xmlChild = element.ChildNodes[i];

                    if (xmlChild.NodeType == XmlNodeType.Element)
                    {
                        if (result.childNodes == null)
                            result.childNodes = new List<ElementNode>();

                        result.childNodes.Add(ParseNode((XmlElement) xmlChild));
                    }
                }

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
                var value = outerAttrs != null ? ResolveAttrValue(attr.Value, outerAttrs) : attr.Value;

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

        private IXmlElementFactory CreateFactory(string element,
            Dictionary<string, string> attrs)
        {
            if (!elementTypes.TryGetValue(element, out var type))
                throw new ArgumentException($"Unknown element type: {element}");

            return type.CreateFactory(attrs);
        }

        private XmlElementComponent CreateInstance(XmlElementComponent elementRoot, Transform root, ElementNode element,
            BoundVariableCollection binders)
        {
            var instance = element.factory.Create(root, binders, this, element.ownAttrs);
            instance.Id = element.id;

            if (!elementRoot)
                elementRoot = instance;

            if (element.childNodes != null)
            {
                for (var i = 0; i < element.childNodes.Count; i++)
                {
                    var node = element.childNodes[i];

                    switch (node.type)
                    {
                        case AttrsChildRootName:
                            elementRoot.SetChildRoot(instance);
                            break;

                        default:
                            instance.AddChild(CreateInstance(elementRoot, instance.ChildParent, node, binders));
                            break;
                    }
                }
            }

            return instance;
        }

        private class ElementNode
        {
            public string id;
            public string type;
            public Dictionary<string, string> ownAttrs;
            public IXmlElementFactory factory;
            public List<ElementNode> childNodes;
        }

        private class AttrsCollectionNode
        {
            public Dictionary<string, string> ownAttrs;
            public string[] parent;
        }
    }
}