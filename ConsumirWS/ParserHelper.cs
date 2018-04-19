using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace ConsumirWS
{
    public static class ParserHelper
    {

        public static Stream ToStream(this string texto)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(texto);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static T ParseXML<T>(this string texto) where T : class
        {
            var reader = XmlReader.Create(texto.Trim().ToClean().ToStream(), new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Document });
            return new XmlSerializer(typeof(T)).Deserialize(reader) as T;
        }

        public static string ToClean(this string texto)
        {
            texto = texto.Replace("<string xmlns=\"http://tempuri.org/\">", "");
            texto = texto.Replace("</string>", "");
            return texto;
        }
    }
}
