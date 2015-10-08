﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using System;
using System.IO;
using System.Reflection;

namespace CreateScriptableObject
{
    public class Window : EditorWindow
    {
        private string folder;
        private string query = "";
        private Vector2 scroll;

        [MenuItem("Assets/Create/ScriptableObject")]
        public static void Create(MenuCommand command)
        {
            string path = AssetDatabase.GetAssetPath(Selection.activeObject.GetInstanceID());
            string folder = Directory.Exists(path) ? path : Path.GetDirectoryName(path);

            var window = GetWindow<Window>(utility: true,
                                           title: "Create Scriptable Object");
            window.Open(folder);
        }

        public void Open(string folder)
        {
            this.folder = folder;

            Show();
        }

        private void OnGUI()
        {
            query = EditorGUILayout.TextField("Search", query);

            EditorGUILayout.BeginScrollView(scroll);

            var types = Assembly.GetExecutingAssembly().GetTypes();
            var valid = types.Where(type => type.IsSubclassOf(typeof(ScriptableObject)));

            foreach (Type type in valid)
            {
                if ((query == "" || type.Name.Contains(query))
                 && GUILayout.Button(type.Name))
                {
                    Close();

                    var obj = ScriptableObject.CreateInstance(type);

                    string path = AssetDatabase.GenerateUniqueAssetPath(folder + "/" + type.Name + ".asset");

                    AssetDatabase.CreateAsset(obj, path);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = obj;
                }
            }

            EditorGUILayout.EndScrollView();
        }
    }
}
#endif
