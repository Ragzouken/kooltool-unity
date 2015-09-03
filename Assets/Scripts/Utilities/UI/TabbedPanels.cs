using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TabbedPanels : MonoBehaviour
{
    [SerializeField] protected ToggleGroup ToggleGroup;
	[SerializeField] protected Transform TabContainer;
	[SerializeField] protected Transform PanelContainer;

	protected Dictionary<string, GameObject> Panels
		= new Dictionary<string, GameObject>();

	public void Start()
	{
		foreach (Transform panel in PanelContainer)
		{
			Panels.Add(panel.name, panel.gameObject);
		}

		foreach (Transform tab in TabContainer)
		{
            if (!Panels.ContainsKey(tab.name)) continue;

			var panel = Panels[tab.name];
			var toggle = tab.GetComponentInChildren<Toggle>();

            if (toggle != null) toggle.onValueChanged.AddListener((bool on) => panel.SetActive(on));
		}
	}

	public void SetTab(string name)
	{
		foreach (var pair in Panels)
		{
			pair.Value.SetActive(pair.Key == name);
		}
	}

    protected void Refresh()
    {

    }
}
