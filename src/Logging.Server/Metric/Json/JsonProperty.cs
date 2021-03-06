﻿using System.Collections.Generic;

namespace Logging.Server.Metric.Json
{
    public class JsonProperty
    {
        public JsonProperty(string name, IEnumerable<JsonObject> objects)
            : this(name, new CollectionJsonValue(objects))
        { }

        public JsonProperty(string name, IEnumerable<JsonProperty> properties)
            : this(name, new ObjectJsonValue(properties))
        { }

        public JsonProperty(string name, JsonObject @object)
            : this(name, new ObjectJsonValue(@object))
        { }

        public JsonProperty(string name, IEnumerable<string> value)
            : this(name, new StringArrayJsonValue(value))
        { }

        public JsonProperty(string name, string value)
            : this(name, new StringJsonValue(value))
        { }

        public JsonProperty(string name, long value)
            : this(name, new LongJsonValue(value))
        { }

        public JsonProperty(string name, double value)
            : this(name, new DoubleJsonValue(value))
        { }

        public JsonProperty(string name, bool value)
            : this(name, new BoolJsonValue(value))
        { }

        public JsonProperty(string name, JsonValue value)
        {
            this.Name = name;
            this.Value = value;
        }

        public string Name { get; private set; }

        public JsonValue Value { get; private set; }

        public string AsJson(bool indented, int indent)
        {
            indent = indented ? indent : 0;
            return string.Format("{0}\"{1}\":{2}", new string(' ', indent), JsonValue.Escape(this.Name), this.Value.AsJson(indented, indent + 2));
        }
    }
}