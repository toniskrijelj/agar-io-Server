using UnityEngine;
using System;

public static class FunctionPeriodic
{
	public static void Create(Action action, float time)
	{
		float nextCallTime = Time.time + time;
		FunctionUpdater.Create(() =>
		{
			while (Time.time >= nextCallTime)
			{
				action();
				nextCallTime += time;
			}

			return false;
		});
	}
}