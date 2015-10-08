using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class IconSettings : ScriptableObjectSingleton<IconSettings>,
                            ISerializationCallbackReceiver
{
    public enum Icon
    {
        RemoveObject,
        EditText,
    }

    [HideInInspector][SerializeField] private List<Icon> iconsKeys;
    [HideInInspector][SerializeField] private List<Sprite> iconsValues;

    public class IconListing : Dictionary<Icon, Sprite> { }
        
    public IconListing icons = new IconListing();

    public Sprite this[Icon icon]
    {
        get
        {
            return icons[icon];
        }
    }

    void ISerializationCallbackReceiver.OnAfterDeserialize()
    {
        icons.Clear();

        for (int i = 0; i < iconsKeys.Count; ++i)
        {
            icons.Add(iconsKeys[i], iconsValues[i]);
        }
    }

    void ISerializationCallbackReceiver.OnBeforeSerialize()
    {
        iconsKeys.Clear();
        iconsValues.Clear();

        foreach (var pair in icons)
        {
            iconsKeys.Add(pair.Key);
            iconsValues.Add(pair.Value);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(IconSettings))]
public class IconSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var settings = target as IconSettings;

        foreach (var icon in System.Enum.GetValues(typeof(IconSettings.Icon)).Cast<IconSettings.Icon>())
        {
            if (!settings.icons.ContainsKey(icon)) settings.icons[icon] = null;

            settings.icons[icon] = EditorGUILayout.ObjectField(icon.ToString(),
                                                               settings.icons[icon],
                                                               typeof(Sprite), 
                                                               false) as Sprite;
        }

        EditorUtility.SetDirty(settings);
    }
}
#endif
