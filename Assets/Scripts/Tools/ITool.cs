using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public interface ITool
{
	void BeginStroke(Vector2 start);
	void ContinueStroke(Vector2 start, Vector2 end);
	void EndStroke(Vector2 end);
}
