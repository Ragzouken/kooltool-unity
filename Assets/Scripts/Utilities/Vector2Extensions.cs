using UnityEngine;
using System.Collections;

public static class Vector2Extensions
{
    public static Vector2 Floor(this Vector2 vector)
    {
        return new Vector2(Mathf.Floor(vector.x), 
                           Mathf.Floor(vector.y));
    }
}
