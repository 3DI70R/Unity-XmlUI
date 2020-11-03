using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace ThreeDISevenZeroR.XmlUI
{
    [CustomEditor(typeof(LayoutInflater))]
    public class LayoutInflaterEditor : Editor
    {
        private readonly HashSet<string> expandedGroups = new HashSet<string>();
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var inflater = (LayoutInflater) target;

            EditorGUILayout.Space(32);
            EditorGUILayout.LabelField("Element attributes", EditorStyles.centeredGreyMiniLabel);
            EditorGUILayout.BeginVertical();

            foreach (var element in inflater.RegisteredElements)
            {
                var wasExpanded = expandedGroups.Contains(element.Name);
                var isExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(wasExpanded, element.Name);

                if (isExpanded != wasExpanded)
                {
                    if (isExpanded)
                    {
                        expandedGroups.Add(element.Name);
                    }
                    else
                    {
                        expandedGroups.Remove(element.Name);
                    }
                }

                if (isExpanded)
                    DrawAttributes(element);
                
                EditorGUILayout.EndFoldoutHeaderGroup();
            }

            if (GUILayout.Button("Generate .XSD schema (For IDE autocompletion)"))
            {
                var schemaString = GenerateXmlSchema(inflater.RegisteredElements);
                var selectedPath = EditorUtility.OpenFilePanel("Save .xsd file", null, "xsd");

                if (selectedPath != null)
                    File.WriteAllText(selectedPath, schemaString);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawAttributes(IXmlElementInfo element)
        {
            foreach (var groups in element.Attributes.GroupBy(e => e.TargetType))
            {
                EditorGUILayout.LabelField(groups.Key.Name, EditorStyles.miniLabel);

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                foreach (var attr in groups)
                {
                    /*EditorGUILayout.LabelField($"{attr.Name}:", attr.FormatHint, 
                        EditorStyles.wordWrappedMiniLabel);*/
                }
                
                EditorGUILayout.EndVertical();
            }
        }

        private string GenerateXmlSchema(IXmlElementInfo[] infos)
        {
            var types = GetXmlTypes(infos);
            var attributes = GetAttributeHandlers(infos);
            
            var root = new XElement(SchemaNamespace + "schema", 
                new XAttribute(XNamespace.Xmlns + XS, SchemaNamespace));

            foreach (var type in types)
            {
                root.Add(GetSchemaTypeElement(type));
            }

            foreach (var attr in attributes)
            {
                root.Add(GetSchemaAttributeElement(attr));
            }
            
            root.Add(new XElement(SchemaNamespace + "element", 
                new XAttribute("name", LayoutInflater.AttrsChildRootName)));
            
            root.Add(CreateElementGroup(LayoutElementsGroup, infos.Select(i => i.Name)
                .Append(LayoutInflater.AttrsChildRootName)
                .ToList()));

            foreach (var xmlElement in infos)
            {
                root.Add(CreateSchemaElementElement(xmlElement, LayoutElementsGroup));
            }

            root.Add(CreateSchemaAttrsCollectionElement());
            root.Add(CreateSchemaAttrsElement(attributes));
            
            return ConvertToString(root);
        }

        private List<XmlTypeSchema> GetXmlTypes(IXmlElementInfo[] elements)
        {
            return elements.SelectMany(s => s.Attributes)
                .Select(a => a.SchemaInfo)
                .GroupBy(i => i.typeName)
                .Select(g => g.First())
                .ToList();
        }

        private List<IAttributeInfo> GetAttributeHandlers(IXmlElementInfo[] elements)
        {
            return elements.SelectMany(e => e.Attributes)
                .GroupBy(a => a.Name)
                .Select(g => g.First())
                .ToList();
        }

        private XElement CreateElementGroup(string groupName, List<string> elementNames)
        {
            var element = new XElement(SchemaNamespace + "group", 
                new XAttribute("name", groupName));
            
            var sequence = new XElement(SchemaNamespace + "choice");
            element.Add(sequence);

            foreach (var elementName in elementNames)
            {
                sequence.Add(new XElement(SchemaNamespace + "element", 
                    new XAttribute("ref", elementName)));
            }

            return element;
        }

        private XElement CreateSchemaElement(string name, out XElement complexType)
        {
            var xmlElement = new XElement(SchemaNamespace + "element", 
                new XAttribute("name", name));
            
            complexType = new XElement(SchemaNamespace + "complexType");
            xmlElement.Add(complexType);

            return xmlElement;
        }

        private XElement CreateSchemaElementElement(IXmlElementInfo element, string group)
        {
            var xmlElement = CreateSchemaElement(element.Name, out var complexType);

            if (element.SupportsChildren)
            {
                complexType.Add(new XElement(SchemaNamespace + "sequence", 
                    new XElement(SchemaNamespace + "group",
                        new XAttribute("ref", group),
                        new XAttribute("minOccurs", "0"),
                        new XAttribute("maxOccurs", "unbounded"))));
            }
            
            complexType.Add(new XElement(SchemaNamespace + "attribute", 
                new XAttribute("name", "Attrs"),
                new XAttribute("type", XS + ":" + "string")));

            foreach (var attr in element.Attributes)
            {
                complexType.Add(new XElement(SchemaNamespace + "attribute", 
                    new XAttribute("ref", attr.Name)));
            }

            return xmlElement;
        }
        
        private XElement CreateSchemaAttrsCollectionElement()
        {
            var xmlElement = CreateSchemaElement(LayoutInflater.AttrsCollectionElementName, out var complexType);
            
            complexType.Add(new XElement(SchemaNamespace + "sequence", 
                new XElement(SchemaNamespace + "element",
                    new XAttribute("ref", LayoutInflater.AttrsCollectionEntryName),
                    new XAttribute("minOccurs", "0"),
                    new XAttribute("maxOccurs", "unbounded"))));

            return xmlElement;
        }

        private XElement CreateSchemaAttrsElement(List<IAttributeInfo> allAttrs)
        {
            var xmlElement = CreateSchemaElement(LayoutInflater.AttrsCollectionEntryName, out var complexType);

            foreach (var attr in allAttrs)
            {
                complexType.Add(new XElement(SchemaNamespace + "attribute", 
                    new XAttribute("ref", attr.Name)));
            }
            
            complexType.Add(new XElement(SchemaNamespace + "attribute", 
                    new XAttribute("name", LayoutInflater.AttrsCollectionParentAttributeId), 
                    new XAttribute("type", XS + ":" + "string")), 
                new XElement(SchemaNamespace + "attribute", 
                    new XAttribute("name", LayoutInflater.AttrsCollectionNameAttributeId),
                    new XAttribute("type", XS + ":" + "string"),
                    new XAttribute("use", "required")));

            return xmlElement;
        }

        private XElement GetSchemaAttributeElement(IAttributeInfo attr)
        {
            return new XElement(SchemaNamespace + "attribute", 
                new XAttribute("name", attr.Name),
                new XAttribute("type", attr.SchemaInfo.typeName));
        }

        private XElement GetSchemaTypeElement(XmlTypeSchema schema)
        {
            var typeElement = new XElement(SchemaNamespace + "simpleType", 
                new XAttribute("name", schema.typeName));
            
            var restrictionElement = new XElement(SchemaNamespace + "restriction", 
                new XAttribute("base", XS + ":" + schema.baseType));
            
            typeElement.Add(restrictionElement);
            
            if (schema.validationRegex != null)
            {
                restrictionElement.Add(new XElement(SchemaNamespace + "pattern", 
                    new XAttribute("value", schema.validationRegex)));
            }
            
            if (schema.autocompleteValues != null)
            {
                foreach (var value in schema.autocompleteValues)
                {
                    restrictionElement.Add(new XElement(SchemaNamespace + "enumeration", 
                        new XAttribute("value", value)));
                }
            }

            return typeElement;
        }

        private string ConvertToString(XElement doc)
        {
            var memoryWriter = new MemoryStream();
            var xmlWriter = new XmlTextWriter(memoryWriter, Encoding.UTF8);
            xmlWriter.Formatting = Formatting.Indented;
            doc.WriteTo(xmlWriter);
            xmlWriter.Flush();
            return Encoding.UTF8.GetString(memoryWriter.ToArray());
        }

        private static readonly string LayoutElementsGroup = "LayoutElements";
        private static readonly XNamespace SchemaNamespace = "http://www.w3.org/2001/XMLSchema";
        private static readonly string XS = "xs";
    }
}