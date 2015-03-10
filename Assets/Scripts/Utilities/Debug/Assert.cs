
public static class Assert
{ 
    [System.Diagnostics.Conditional("UNITY_EDITOR")] 
    public static void True(bool condition, string message) 
    {
        if (!condition) throw new System.Exception(message); 
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")] 
    public static void False(bool condition, string message) 
    {
        if (condition) throw new System.Exception(message); 
    }

    [System.Diagnostics.Conditional("UNITY_EDITOR")] 
    public static void NotNull(object obj, string message) 
    {
        if (obj == null) throw new System.Exception(message); 
    }
}
