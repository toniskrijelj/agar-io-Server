using UnityEngine;
using System;
using System.Collections.Generic;

public class FunctionUpdater : MonoBehaviour
{
	private List<Func<bool>> actionList = null;

	private static FunctionUpdater instance = null;

	public static void Create(Func<bool> func)
	{
		if (instance == null)
		{
			instance = new GameObject("FunctionUpdater_Global", typeof(FunctionUpdater)).GetComponent<FunctionUpdater>();
			instance.actionList = new List<Func<bool>>();
		}
		instance.actionList.Add(func);
	}

	public static void Remove(Func<bool> func)
	{
		if (instance.actionList.Contains(func))
		{
			instance.actionList.Remove(func);
		}
	}

	private void FixedUpdate()
	{
		for (int i = 0; i < actionList.Count; i++)
		{
			if (actionList[i]())
			{
				actionList.RemoveAt(i);
				i--;
			}
		}
	}
}