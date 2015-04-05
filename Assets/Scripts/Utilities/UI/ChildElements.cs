using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class ChildElements<T> where T : Component
{
    protected Transform Parent;
    protected T Prefab; 
    protected IList<T> Elements = new List<T>();

    public ChildElements(Transform parent, T prefab)
    {
        Parent = parent;
        Prefab = prefab;
    }

    public void Clear()
    {
        foreach (T element in Elements)
        {
            element.gameObject.SetActive(false);
        }
    }

    public T Add()
    {
        foreach (T element in Elements)
        {
            if (!element.gameObject.activeSelf)
            {
                element.gameObject.SetActive(true);

                return element;
            }
        }

        var instance = Object.Instantiate<T>(Prefab);
        instance.transform.SetParent(Parent, false);

        Elements.Add(instance);

        return instance;
    }

    public void Remove(T element)
    {
        element.gameObject.SetActive(false);
        element.transform.SetAsLastSibling();
    }
}
