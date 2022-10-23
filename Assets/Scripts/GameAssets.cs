using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAssets : MonoBehaviour
{
	private static GameAssets _i;

	public static GameAssets i
	{
		get
		{
			if (_i == null)
			{
				GameObject gameObj = Instantiate(Resources.Load("GameAssets") as GameObject);
				_i = gameObj.GetComponent<GameAssets>();
				_i.name = "Game Assets";
			}
			return _i;
		}
	}

	public Ball ball;
	public Mass mass;
	public BotBall botBall;
}