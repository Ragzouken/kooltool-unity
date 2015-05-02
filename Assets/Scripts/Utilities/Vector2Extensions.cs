using UnityEngine;
using System.Collections;

public static class Vector2Extensions
{
    public static Vector2 Floor(this Vector2 vector)
    {
        return new Vector2(Mathf.Floor(vector.x), 
                           Mathf.Floor(vector.y));
    }

    public static Vector2 Round(this Vector2 vector)
    {
        return new Vector2(vector.x >= 0 ? Mathf.Floor(vector.x) : Mathf.Ceil(vector.x)-1,
                           vector.y >= 0 ? Mathf.Floor(vector.y) : Mathf.Ceil(vector.y)-1);
    }
}
