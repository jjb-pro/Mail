using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Mail.Helpers;

public static class XMLHelper
{
    public static T? Deserialize<T>(string path)
    {
        XmlSerializer serializer = new(typeof(T));

        using FileStream stream = new(path, FileMode.Open);
        return (T?)serializer.Deserialize(stream);
    }

    public static T? DeserializeFromString<T>(string text)
    {
        XmlSerializer serializer = new(typeof(T));

        using StringReader stream = new(text);
        return (T?)serializer.Deserialize(stream);
    }

    public static void Serialize<T>(T @object, string path)
    {
        var serializer = new XmlSerializer(typeof(T));
        using var fileStream = new FileStream(path, FileMode.Create);

        using var writer = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true });
        serializer.Serialize(writer, @object);
    }
}