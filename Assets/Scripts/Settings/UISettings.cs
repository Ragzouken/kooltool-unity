using UnityEngine;

public class UISettings : ScriptableObjectSingleton<UISettings>
{
    [System.Serializable]
    public class Navigation
    {
        public AnimationCurve zoomCurve;
    }

    public Navigation navigation;
}
