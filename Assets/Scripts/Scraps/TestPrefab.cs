using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class TestPrefab : MonoBehaviour 
{
    [SerializeField] private GameObject prefab;

    private void Start()
    {
        var instance = Instantiate<GameObject>(prefab);

        instance.transform.SetParent(transform, false);
    }
}
