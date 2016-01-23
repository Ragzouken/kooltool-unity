using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public static class JsonWrapper
{
    public static JsonSerializerSettings Settings()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Auto;
        settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
        settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
        settings.Converters.Add(new Vector2Converter());
        settings.ObjectCreationHandling = ObjectCreationHandling.Replace;
        settings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

        return settings;
    }

    public static T Deserialise<T>(string value)
    {
        T returnObject = JsonConvert.DeserializeObject<T>(value, Settings());

        return returnObject;
    }

    public static string Serialise<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, Formatting.Indented, Settings());
    }

    public static T Copy<T>(T obj)
    {
        return Deserialise<T>(Serialise(obj));
    }
}

public class Vector2Converter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(Vector2);
    }

    public override object ReadJson(JsonReader reader, 
                                    Type objectType, 
                                    object existingValue, 
                                    JsonSerializer serializer)
    {
        Vector2 vector = new Vector2();

        vector.x = (float) reader.ReadAsDecimal().GetValueOrDefault();
        vector.y = (float) reader.ReadAsDecimal().GetValueOrDefault();
        reader.Read();

        return vector;
    }

    public override void WriteJson(JsonWriter writer, 
                                   object value, 
                                   JsonSerializer serializer)
    {
        Vector2 vector = (Vector2) value;

        writer.WriteStartArray();
        writer.WriteValue(vector.x);
        writer.WriteValue(vector.y);
        writer.WriteEndArray();
    }
}
