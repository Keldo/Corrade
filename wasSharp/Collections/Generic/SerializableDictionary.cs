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
    ///     A serializable dictionary class.
    /// </summary>
    /// <typeparam name="TKey">the key</typeparam>
    /// <typeparam name="TValue">the value</typeparam>
    public class SerializableDictionary<TKey, TValue>
        : Dictionary<TKey, TValue>, IXmlSerializable
    {
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            reader.ReadStartElement();
            while (!reader.NodeType.Equals(XmlNodeType.EndElement))
            {
                reader.ReadStartElement(ItemNodeName);

                reader.ReadStartElement(KeyNodeName);
                var key = (TKey) KeySerializer.Deserialize(reader);
                reader.ReadEndElement();

                reader.ReadStartElement(ValueNodeName);
                var value = (TValue) ValueSerializer.Deserialize(reader);
                reader.ReadEndElement();

                Add(key, value);

                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in Keys)
            {
                writer.WriteStartElement(ItemNodeName);

                writer.WriteStartElement(KeyNodeName);
                KeySerializer.Serialize(writer, key);
                writer.WriteEndElement();

                writer.WriteStartElement(ValueNodeName);
                ValueSerializer.Serialize(writer, this[key]);
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        /// <summary>
        ///     Deep-clones the serializable dictionary.
        /// </summary>
        /// <returns>a deep clone of the original dictionary</returns>
        public SerializableDictionary<TKey, TValue> Clone()
        {
            SerializableDictionary<TKey, TValue> clone;
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    new XmlSerializer(
                        typeof(SerializableDictionary<TKey, TValue>), new XmlRootAttribute(DictionaryNodeName))
                        .Serialize(memoryStream, this);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    clone = (SerializableDictionary<TKey, TValue>)
                        new XmlSerializer(
                            typeof(SerializableDictionary<TKey, TValue>), new XmlRootAttribute(DictionaryNodeName))
                            .Deserialize(memoryStream);
                }
            }
                /* cloning failed so return an empty dictionary */
            catch (Exception)
            {
                clone = new SerializableDictionary<TKey, TValue>();
            }
            return clone;
        }

        #region Constants

        public string DictionaryNodeName { get; set; } = "Dictionary";
        public string ItemNodeName { get; set; } = "Item";
        public string KeyNodeName { get; set; } = "Key";
        public string ValueNodeName { get; set; } = "Value";

        #endregion

        #region Constructors

        public SerializableDictionary()
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }

        public SerializableDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey> comparer)
            : base(dictionary, comparer)
        {
        }

        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }

        #endregion

        #region Private Properties

        protected XmlSerializer ValueSerializer
            => valueSerializer ?? (valueSerializer = new XmlSerializer(typeof(TValue)));

        private XmlSerializer KeySerializer => keySerializer ?? (keySerializer = new XmlSerializer(typeof(TKey)));

        #endregion

        #region Private Members

        private XmlSerializer keySerializer;
        private XmlSerializer valueSerializer;

        #endregion
    }
}