using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace kooltool.Editor
{
    public class Editor : MonoBehaviour
    {
        [SerializeField] protected Toolbox Toolbox;

        protected void Start()
        {
            Toolbox.Hide();
        }

        protected void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space)) Toolbox.Show();
            if (Input.GetKeyUp(KeyCode.Space)) Toolbox.Hide();
        }
    }
}
