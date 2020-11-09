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
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var inflater = (LayoutInflater) target;

            EditorGUILayout.Space(32);
            if (GUILayout.Button("Generate .XSD schema\n(For IDE autocompletion)"))
            {
                var schemaString = GenerateXmlSchema(
                    inflater.GetRegisteredElements(), 
                    inflater.GetRegisteredComponents());
                
                var selectedPath = EditorUtility.OpenFilePanel("Save .xsd file", null, "xsd");

                if (selectedPath != null)
                    File.WriteAllText(selectedPath, schemaString);
            }
        }

        private string GenerateXmlSchema(IXmlElementInfo[] elements, IXmlComponentInfo[] components)
        {
            var allAttrs = elements
                .SelectMany(e => e.Attributes)
                .Concat(components.SelectMany(e => e.Attributes))
                .ToList();
            
            var types = GetXmlTypes(allAttrs);
            var attributes = GetAttributeHandlers(allAttrs);
            
            var root = new XElement(SchemaNamespace + "schema", 
                new XAttribute(XNamespace.Xmlns + XS, SchemaNamespace));

            // Attribute types
            foreach (var type in types)
                root.Add(GetSchemaTypeElement(type));

            // Attributes
            foreach (var attr in attributes)
                root.Add(GetSchemaAttributeElement(attr));

            // Default group
            root.Add(CreateElementGroup(LayoutElementsGroup, elements.Select(i => i.Name)
                .Append(LayoutInflater.AttrsChildRootName)
                .ToList()));

            // Base components
            foreach (var component in components)
            {
                root.Add(CreateSchemaComponentElement(component));
            }

            // Elements
            foreach (var element in elements)
            {
                // Element.Component
                foreach (var component in components)
                    root.Add(CreateSchemaComponentSubstitutionElement(element, component));

                root.Add(CreateSchemaElementElement(element, components, LayoutElementsGroup));
            }

            root.Add(CreateSchemaAttrsCollectionElement());
            root.Add(CreateSchemaAttrsElement(attributes));
            
            // ChildRoot element
            root.Add(new XElement(SchemaNamespace + "element", 
                new XAttribute("name", LayoutInflater.AttrsChildRootName)));
            
            return ConvertToString(root);
        }

        private List<TypeInfo> GetXmlTypes(IEnumerable<IAttributeInfo> attrs)
        {
            return attrs
                .Select(a => a.Type)
                .GroupBy(i => i.typeName)
                .Select(g => g.First())
                .ToList();
        }

        private List<IAttributeInfo> GetAttributeHandlers(IEnumerable<IAttributeInfo> attrs)
        {
            return attrs
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

        private XElement CreateSchemaElementElement(IXmlElementInfo element, 
            IXmlComponentInfo[] components, string group)
        {
            var xmlElement = CreateSchemaElement(element.Name, out var complexType);
            var chooseGroup = new XElement(SchemaNamespace + "choice", 
                new XAttribute("minOccurs", "0"), 
                new XAttribute("maxOccurs", "unbounded"));
            complexType.Add(chooseGroup);
            
            foreach (var component in components)
            {
                var name = GetComponentName(element, component);
                chooseGroup.Add(new XElement(SchemaNamespace + "element", 
                    new XAttribute("ref", name)));
            }
            
            if (element.SupportsChildren)
            {
                chooseGroup.Add(new XElement(SchemaNamespace + "group",
                    new XAttribute("ref", group),
                    new XAttribute("minOccurs", "0"),
                    new XAttribute("maxOccurs", "unbounded")));
            }
            
            AddAttributeReferences(complexType, element.Attributes);
            
            return xmlElement;
        }

        private XElement CreateSchemaComponentElement(IXmlComponentInfo element)
        {
            var xmlElement = CreateSchemaElement(element.Name, out var complexType);
            AddAttributeReferences(complexType, element.Attributes);

            return xmlElement;
        }

        private string GetComponentName(IXmlElementInfo element, IXmlComponentInfo component)
        {
            return element.Name + "." + component.Name;
        }

        private XElement CreateSchemaComponentSubstitutionElement(IXmlElementInfo element, IXmlComponentInfo component)
        {
            return new XElement(SchemaNamespace + "element", 
                new XAttribute("name", GetComponentName(element, component)),
                new XAttribute("substitutionGroup", component.Name));
        }

        private void AddAttributeReferences(XElement complexType, IAttributeInfo[] attributes)
        {
            complexType.Add(new XElement(SchemaNamespace + "attribute", 
                new XAttribute("name", "Attrs"),
                new XAttribute("type", XS + ":" + "string")));

            foreach (var attr in attributes)
            {
                complexType.Add(new XElement(SchemaNamespace + "attribute", 
                    new XAttribute("ref", attr.Name)));
            }
        }
        
        private XElement CreateSchemaAttrsCollectionElement()
        {
            var xmlElement = CreateSchemaElement(LayoutInflater.AttrsCollectionElementName, out var complexType);
            
            complexType.Add(new XElement(SchemaNamespace + "choice", 
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
                complexType.Add(GetSchemaAttributeRefElement(attr));
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
                new XAttribute("type", attr.Type.typeName));
        }

        private XElement GetSchemaAttributeRefElement(IAttributeInfo attr)
        {
            return new XElement(SchemaNamespace + "attribute", 
                new XAttribute("ref", attr.Name));
        }

        private XElement GetAutocompleteRestrictionElement(string baseType, string[] entries)
        {
            var typeElement = new XElement(SchemaNamespace + "simpleType");
            var restrictionElement = new XElement(SchemaNamespace + "restriction", 
                new XAttribute("base", XS + ":" + baseType));
            typeElement.Add(restrictionElement);

            if (entries != null)
            {
                foreach (var value in entries)
                {
                    restrictionElement.Add(new XElement(SchemaNamespace + "enumeration", 
                        new XAttribute("value", value)));
                }
            }

            return typeElement;
        }

        private XElement GetRegexRestrictionElement(string baseType, string regex)
        {
            return new XElement(SchemaNamespace + "simpleType", 
                new XElement(SchemaNamespace + "restriction", new XAttribute("base", XS + ":" + baseType),
                    new XElement(SchemaNamespace + "pattern", new XAttribute("value", regex))));
        }

        private XElement GetSchemaTypeElement(TypeInfo schema)
        {
            var typeElement = new XElement(SchemaNamespace + "simpleType", 
                new XAttribute("name", schema.typeName));
            
            var unionElement = new XElement(SchemaNamespace + "union");
            typeElement.Add(unionElement);
            
            unionElement.Add(GetAutocompleteRestrictionElement(schema.baseType, schema.autocompleteValues));
            unionElement.Add(GetRegexRestrictionElement("string", "@|\\$.+")); // Variables and placeholders
            
            if (schema.validationRegex != null)
                unionElement.Add(GetRegexRestrictionElement("string", schema.validationRegex));

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