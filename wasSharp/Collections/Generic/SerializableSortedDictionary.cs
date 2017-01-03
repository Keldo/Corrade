///////////////////////////////////////////////////////////////////////////
//  Copyright (C) Wizardry and Steamworks 2013 - License: GNU GPLv3      //
//  Please see: http://www.gnu.org/licenses/gpl.html for legal details,  //
//  rights of fair usage, the disclaimer and warranty conditions.        //
///////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace wasSharp.Collections.Generic
{
    /// <summary>
    ///     A serializable sorted dictionary class.
    /// </summary>
    /// <typeparam name="TKey">the key</typeparam>
    /// <typeparam name="TValue">the value</typeparam>
    [XmlRoot("SortedDictionary")]
    public class SerializableSortedDictionary<TKey, TValue>
        : SortedDictionary<TKey, TValue>, IXmlSerializable
    {
        #region IXmlSerializable Members

        public SerializableSortedDictionary(IEnumerable<KeyValuePair<TKey, TValue>> kvp)
        {
            foreach (var i in kvp)
            {
                Add(i.Key, i.Value);
            }
        }

        public SerializableSortedDictionary()
        {
        }

        /// <summary>
        ///     Deep-clones the serializable dictionary.
        /// </summary>
        /// <returns>a deep clone of the original dictionary</returns>
        public SerializableSortedDictionary<TKey, TValue> Clone()
        {
            SerializableSortedDictionary<TKey, TValue> clone;
            try
            {
                using (var writer = new MemoryStream())
                {
                    var serializer =
                        new XmlSerializer(
                            typeof(SerializableDictionary<TKey, TValue>));
                    serializer.Serialize(writer, this);
                    writer.Seek(0, SeekOrigin.Begin);
                    clone = (SerializableSortedDictionary<TKey, TValue>)
                        new XmlSerializer(
                            typeof(SerializableSortedDictionary<TKey, TValue>))
                            .Deserialize(writer);
                }
            }
                /* cloning failed so return an empty dictionary */
            catch (Exception)
            {
                clone = new SerializableSortedDictionary<TKey, TValue>();
            }
            return clone;
        }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            var wasEmpty = reader.IsEmptyElement;
            reader.Read();

            if (wasEmpty)
                return;

            while (!reader.NodeType.Equals(XmlNodeType.EndElement))
            {
                reader.ReadStartElement("Item");

                reader.ReadStartElement("Key");
                var key = (TKey) keySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement("Value");
                var value = (TValue) valueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            var keySerializer = new XmlSerializer(typeof(TKey));
            var valueSerializer = new XmlSerializer(typeof(TValue));

            foreach (var key in Keys)
            {
                writer.WriteStartElement("Item");

                writer.WriteStartElement("Key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement("Value");
                var value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        #endregion
    }
}