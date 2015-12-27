using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

public static class JsonWrapper
{
    public static JsonSerializerSettings Settings()
    {
        JsonSerializerSettings settings = new JsonSerializerSettings();
        settings.TypeNameHandling = TypeNameHandling.Auto;
        settings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
        settings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
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
