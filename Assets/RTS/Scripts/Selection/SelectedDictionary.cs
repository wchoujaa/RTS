using Assets.RTS.Scripts.Controllers;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour
{
	public ConcurrentDictionary<int, UnitController> selectedTable = new ConcurrentDictionary<int, UnitController>();

	public void AddSelected(UnitController go)
	{

		//Debug.Log(go.name);
		int id = go.GetInstanceID();

		if (!(selectedTable.ContainsKey(id)))
		{
			selectedTable.TryAdd(id, go);
			 
			 go.GetComponent<UnitController>().SetSelected(true);
		}
	}

	public List<UnitController> getSelection()
	{
		List<UnitController> selection = new List<UnitController>();
		var enumerator = selectedTable.GetEnumerator();
		while (enumerator.MoveNext())
		{
			selection.Add(enumerator.Current.Value); 
		}

		return selection;
	}


	public void Deselect(int id)
	{

		UnitController go = selectedTable[id];
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

	public bool IsEmpty()
	{
		return selectedTable.IsEmpty;
	}

	public int Count => selectedTable.Count;
}
