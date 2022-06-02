﻿using Entities.LinkModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Entities.Models
{
    public class Entity : DynamicObject, IDictionary<string, object>, IXmlSerializable
    {
        private readonly string root = "EntityWithLinks";
        private readonly IDictionary<string, object> expando = null;

        public Entity()
        {
            expando = new ExpandoObject();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (expando.TryGetValue(binder.Name, out object value))
            {
                result = value;
                return true;
            }
            return base.TryGetMember(binder, out result);
        }
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            expando[binder.Name] = value;

            return true;
        }
        public object this[string key]
        {
            get
            {
                return expando[key];
            }
            set
            {
                expando[key] = value;
            }
        }

        public ICollection<string> Keys
        {
            get { return expando.Keys; }
        }

        public ICollection<object> Values
        {
            get { return expando.Values; }
        }

        public int Count
        {
            get { return expando.Count; }
        }

        public bool IsReadOnly
        {
            get { return expando.IsReadOnly; }
        }

        public void Add(string key, object value)
        {
            expando.Add(key, value);
        }

        public void Add(KeyValuePair<string, object> item)
        {
            expando.Add(item);
        }

        public void Clear()
        {
            expando.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return expando.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return expando.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            expando.CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return expando.GetEnumerator();
        }

        public bool Remove(string key)
        {
            return expando.Remove(key);
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return expando.Remove(item);
        }

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
        {
            return expando.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public XmlSchema GetSchema()
        {
            return new XmlSchema();
        }

        public void ReadXml(XmlReader reader)
        {
            reader.ReadStartElement(root);

            while (!reader.Name.Equals(root))
            {
                string typeContent;
                Type underlyingType;
                var name = reader.Name;

                reader.MoveToAttribute("type");
                typeContent = reader.ReadContentAsString();
                underlyingType = Type.GetType(typeContent);
                reader.MoveToContent();
                expando[name] = reader.ReadElementContentAs(underlyingType, null);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            foreach (var key in expando.Keys)
            {
                var value = expando[key];
                WriteXmlElement(key, value, writer);
            }
        }

        private void WriteXmlElement(string key, object value, XmlWriter writer)
        {
            writer.WriteStartElement(key);

            if (value.GetType() == typeof(List<Link>))
            {
                foreach (var val in value as List<Link>)
                {
                    writer.WriteStartElement(nameof(Link));
                    WriteXmlElement(nameof(val.Href), val.Href, writer);
                    WriteXmlElement(nameof(val.Method), val.Method, writer);
                    WriteXmlElement(nameof(val.Rel), val.Rel, writer);
                    writer.WriteEndElement();
                }
            }
            else
            {
                writer.WriteString(value.ToString());
            }

            writer.WriteEndElement();
        }
    }
}