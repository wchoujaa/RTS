using Assets.RTS.Scripts.Controllers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour
{
	public ConcurrentDictionary<int, GameObject> selectedTable = new ConcurrentDictionary<int, GameObject>();

	public void AddSelected(GameObject go)
	{
		//Debug.Log(go.name);
		int id = go.GetInstanceID();

		if (!(selectedTable.ContainsKey(id)))
		{
			selectedTable.TryAdd(id, go);
			if(go.GetComponent<UnitController>() != null)
				go.GetComponent<UnitController>().SetSelected(true);
		}
	}

	public List<GameObject> getSelection()
	{
		List<GameObject> selection = new List<GameObject>();
		var enumerator = selectedTable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			selection.Add(enumerator.Current.Value);

		}

		return selection;
	}


	public void Deselect(int id)
	{

		GameObject go = selectedTable[id];
		if (go.GetComponent<UnitController>() != null)
			go.GetComponent<UnitController>().SetSelected(false);
 
		selectedTable.TryRemove(id, out go);
	}

	public void DeselectAll()
	{


		var enumerator = selectedTable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			Deselect(enumerator.Current.Key);

		}

		selectedTable.Clear();
	}
}
