using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class ScriptableObjectSingleton<T> : ScriptableObject
    where T : ScriptableObject
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                var instances = Resources.LoadAll<T>("Singletons");

                Assert.IsTrue(instances.Length > 0, string.Format("Could not find any instances of {0} singleton!", typeof(T).Name));
                Assert.IsTrue(instances.Length < 2, string.Format("Found multiple instances of {0} singleton!", typeof(T).Name));

                instance = instances[0];
            }

            return instance;
        }
    }
}
