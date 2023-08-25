// =============================================================================
// Author: Vladyslav Zaiets | https://sarmkadan.com
// CTO & Software Architect
// =============================================================================

using System.Xml;
using System.Xml.Serialization;
using System.Text;

namespace BlazorComponentLibrary.Formatters;

/// <summary>
/// XML formatter for data serialization and deserialization.
/// Uses System.Xml for standards-based XML handling.
/// Supports schema validation and indentation.
/// </summary>
public class XmlFormatter
{
    private readonly XmlWriterSettings _writerSettings;
    private readonly XmlReaderSettings _readerSettings;

    public XmlFormatter(bool indent = true)
    {
        _writerSettings = new XmlWriterSettings
        {
            Indent = indent,
            IndentChars = "  ",
            Encoding = Encoding.UTF8,
            ConformanceLevel = ConformanceLevel.Document
        };

        _readerSettings = new XmlReaderSettings
        {
            ConformanceLevel = ConformanceLevel.Document,
            IgnoreWhitespace = true,
            IgnoreComments = true
        };
    }

    /// <summary>
    /// Serializes object to XML string.
    /// Uses XmlSerializer for automatic type mapping.
    /// </summary>
    public string ToXml<T>(T? obj) where T : class
    {
        try
        {
            var serializer = new XmlSerializer(typeof(T));
            var sb = new StringBuilder();

            using (var writer = XmlWriter.Create(sb, _writerSettings))
            {
                serializer.Serialize(writer, obj);
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new XmlException("Failed to serialize object to XML", ex);
        }
    }

    /// <summary>
    /// Deserializes XML string to object.
    /// Validates XML structure and type compatibility.
    /// </summary>
    public T? FromXml<T>(string xml) where T : class
    {
        if (string.IsNullOrWhiteSpace(xml))
            return null;

        try
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var reader = XmlReader.Create(new StringReader(xml), _readerSettings))
            {
                return serializer.Deserialize(reader) as T;
            }
        }
        catch (Exception ex)
        {
            throw new XmlException("Failed to deserialize XML to object", ex);
        }
    }

    /// <summary>
    /// Serializes object to XML bytes.
    /// Useful for storage or transmission.
    /// </summary>
    public byte[] ToXmlBytes<T>(T? obj) where T : class
    {
        var xml = ToXml(obj);
        return Encoding.UTF8.GetBytes(xml);
    }

    /// <summary>
    /// Deserializes XML bytes to object.
    /// </summary>
    public T? FromXmlBytes<T>(byte[] data) where T : class
    {
        if (data == null || data.Length == 0)
            return null;

        var xml = Encoding.UTF8.GetString(data);
        return FromXml<T>(xml);
    }

    /// <summary>
    /// Validates XML string format.
    /// Checks structure without full deserialization.
    /// </summary>
    public bool IsValidXml(string? xml)
    {
        if (string.IsNullOrWhiteSpace(xml))
            return false;

        try
        {
            using (var reader = XmlReader.Create(new StringReader(xml), _readerSettings))
            {
                while (reader.Read()) { }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Extracts value from XML by XPath.
    /// Useful for selective data extraction.
    /// </summary>
    public string? GetXmlValue(string xml, string xpath)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);
            var node = doc.SelectSingleNode(xpath);
            return node?.InnerText;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Converts XML to dictionary representation.
    /// Flattens hierarchical data into key-value pairs.
    /// </summary>
    public Dictionary<string, string> XmlToDictionary(string xml, string elementPrefix = "")
    {
        var result = new Dictionary<string, string>();

        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(xml);

            ParseXmlNode(doc.DocumentElement, elementPrefix, result);
        }
        catch (Exception ex)
        {
            throw new XmlException("Failed to convert XML to dictionary", ex);
        }

        return result;
    }

    /// <summary>
    /// Recursively parses XML nodes into dictionary.
    /// Builds flattened key-value representation.
    /// </summary>
    private void ParseXmlNode(XmlNode? node, string prefix, Dictionary<string, string> result)
    {
        if (node == null)
            return;

        var nodeName = string.IsNullOrEmpty(prefix) ? node.Name : $"{prefix}.{node.Name}";

        if (node.ChildNodes.Count == 0 && !string.IsNullOrEmpty(node.InnerText))
        {
            result[nodeName] = node.InnerText;
        }
        else
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.NodeType == XmlNodeType.Element)
                {
                    ParseXmlNode(child, nodeName, result);
                }
            }
        }

        // Add attributes
        if (node.Attributes != null)
        {
            foreach (XmlAttribute attr in node.Attributes)
            {
                result[$"{nodeName}@{attr.Name}"] = attr.Value;
            }
        }
    }

    /// <summary>
    /// Merges two XML documents.
    /// Combines elements at root level.
    /// </summary>
    public string MergeXml(string xml1, string xml2)
    {
        try
        {
            var doc1 = new XmlDocument();
            var doc2 = new XmlDocument();
            doc1.LoadXml(xml1);
            doc2.LoadXml(xml2);

            var importedNode = doc1.ImportNode(doc2.DocumentElement, true);
            doc1.DocumentElement?.AppendChild(importedNode);

            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, _writerSettings))
            {
                doc1.WriteTo(writer);
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new XmlException("Failed to merge XML documents", ex);
        }
    }
}
