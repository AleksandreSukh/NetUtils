using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace NetUtils
{
    public static class XmlSerializationExtensions
    {
        public static string SerializeToXmlString<T>(this T obj, Encoding encoding)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(memoryStream, encoding))
                {
                    var type = typeof(T);
                    new XmlSerializer(type
                        //, new XmlRootAttribute(type.Name)
                    ).Serialize(streamWriter, obj);

                    memoryStream.Position = 0;
                    using (var sr = new StreamReader(memoryStream, encoding))
                    {
                        var xmlString = sr.ReadToEnd();
                        return xmlString;
                    }
                }
            }
        }

        public static T DeserializeAsXml<T>(this string xmlSring, Encoding encoding)
        {
            using (var stream = xmlSring.ToStream(encoding))
            {

                using (var streamReader = new StreamReader(stream, encoding))
                {
                    var type = typeof(T);

                    var obj = new XmlSerializer(type
                        //, new XmlRootAttribute(type.Name)
                    ).Deserialize(streamReader);
                    return (T)obj;
                }
            }
        }

        public static void SerializeToXmlFile<T>(this T obj, Encoding encoding, string path)
        {
            using (var fileStream = File.OpenWrite(path))
            {
                using (var streamWriter = new StreamWriter(fileStream, encoding))
                {
                    var type = typeof(T);
                    new XmlSerializer(type
                        //, new XmlRootAttribute(type.Name)
                    ).Serialize(streamWriter, obj);
                }
            }
        }

        public static T DeserializeFromXml<T>(Encoding encoding, string path)
        {
            using (var streamReader = new StreamReader(path, encoding))
            {
                var type = typeof(T);

                var obj = new XmlSerializer(type
                    //, new XmlRootAttribute(type.Name)
                ).Deserialize(streamReader);
                return (T)obj;
            }
        }
    }
}