using UnityEngine;
using System;


public static class FunctionTimer
{
	public static void Create(Action action, float time)
	{
		float callTime = Time.time + time;
		FunctionUpdater.Create(() =>
		{
			if (Time.time >= callTime)
			{
				action();
				return true;
			}

			return false;
		});
	}
}