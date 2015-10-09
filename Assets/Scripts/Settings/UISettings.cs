using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class UISettings : ScriptableObjectSingleton<UISettings>
{
    [System.Serializable]
    public class Navigation
    {
        public AnimationCurve zoomCurve;
    }

    public Navigation navigation;
}
