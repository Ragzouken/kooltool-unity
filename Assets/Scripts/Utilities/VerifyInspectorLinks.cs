using System;
using System.Linq;
using System.Reflection;

#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
#endif

[AttributeUsage(AttributeTargets.Field)]
public class Optional : Attribute { }

[AttributeUsage(AttributeTargets.Field)]
public class SceneOnly : Attribute { }

#if UNITY_EDITOR
[InitializeOnLoad]
public class VerifyInspectorLinks
{
    private static bool stopping;

    private static bool Inspector(FieldInfo info)
    {
        return (info.IsPublic 
            && !info.IsNotSerialized
            && info.FieldType.IsSubclassOf(typeof(UnityEngine.Object))
            && !Attribute.IsDefined(info, typeof(HideInInspector)))
            || Attribute.IsDefined(info, typeof(SerializeField));
    }

    private static bool Optional(FieldInfo info)
    {
        return Attribute.IsDefined(info, typeof(Optional));
    }

    private static bool Required(FieldInfo info)
    {
        return !Optional(info);
    }

    static VerifyInspectorLinks()
    {
        EditorApplication.playmodeStateChanged = delegate
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                Verify();
            }
        };
    }

    public static bool Verify()
    {
        bool problem = false;

        var types = Assembly.GetExecutingAssembly()
                                    .GetTypes()
                                    .Where(t => t.IsSubclassOf(typeof(UnityEngine.Object)));

        var flags = BindingFlags.Public
                  | BindingFlags.NonPublic
                  | BindingFlags.Instance
                  | BindingFlags.DeclaredOnly;

        foreach (Type type in types)
        {
            var fields = type.GetFields(flags)
                             .Where(Inspector)
                             .Where(Required);

            foreach (var obj in Resources.FindObjectsOfTypeAll(type))
            {
                foreach (var field in fields)
                {
                    bool asset = AssetDatabase.Contains(obj);
                    bool valid = asset && Attribute.IsDefined(field, typeof(SceneOnly));

                    if (!valid && field.GetValue(obj) == null)
                    {
                        problem = true;

                        if (!asset)
                        {
                            Debug.LogErrorFormat(obj, "'{0}' unlinked on {1} called '{2}'!", field.Name, type.Name, obj.name);
                        }
                        else
                        {
                            Debug.LogErrorFormat("'{0}' unlinked on {1} at '{2}'", field.Name, type.Name, AssetDatabase.GetAssetPath(obj));
                        }
                    }
                }
            }
        }

        return problem;
    }
}
#endif