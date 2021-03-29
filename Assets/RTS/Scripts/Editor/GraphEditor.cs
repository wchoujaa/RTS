using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SelectionGraph))]
public class GraphEditor : Editor
{

	override public void OnInspectorGUI()
	{
		//Called whenever the inspector is drawn for this object.
		DrawDefaultInspector();
		//This draws the default screen.  You don't need this if you want
		//to start from scratch, but I use this when I'm just adding a button or
		//some small addition and don't feel like recreating the whole inspector.
		if (GUILayout.Button("Generate n nodes"))
		{
			//add everthing the button would do.
			SelectionGraph graph = (SelectionGraph)target;
			graph.GenerateTest();
		}
	}
}
 