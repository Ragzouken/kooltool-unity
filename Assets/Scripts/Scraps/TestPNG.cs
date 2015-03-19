using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Diagnostics;

public class TestPNG : MonoBehaviour
{
    [SerializeField] protected Tilemap Tilemap;

    [Conditional("UNITY_STANDALONE")]
    public void Update()
    {
        string path = Application.persistentDataPath;

        #if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.F5))
        {
            File.WriteAllBytes(path + "/tileset.png", Tilemap.Tileset.Texture.EncodeToPNG());
        }
        else if (Input.GetKeyDown(KeyCode.F8))
        {
            Tilemap.Tileset.Texture.LoadImage(File.ReadAllBytes(path + "/tileset.png"));
        }
        #endif
    }
}
