using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using kooltool.Data;

namespace kooltool.Editor
{
    public class NoteboxView : MonoBehaviour
    {
        [SerializeField] private Text text;

        public void SetNotebox(Notebox notebox)
        {
            text.text = notebox.text;
        }
    }
}
