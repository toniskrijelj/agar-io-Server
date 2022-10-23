using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bot : Player
{
	static int botNumber = 1;

	private void SpawnBot()
	{
		using (Packet packet = new Packet())
		{
			packet.Write(new Vector2(Random.Range(-147f, 147), Random.Range(-147f, 147)));
			packet.Write("Bot " + (botNumber++));

			packet.SetBytes();
			ServerHandle.SpawnPlayer(id, packet);
		}
	}

	int[] ballIndexes = new int[1];
	Vector2[] ballPositions = new Vector2[1];
	bool changedDirection = false;

	protected override void FixedUpdate()
	{
		if(balls[0] != null)
		{
			ballIndexes[0] = 0;
			ballPositions[0] = balls[0].transform.position;
			ServerSend.BallPosition(id, 1, ballIndexes, ballPositions);
			if(balls[0].lastTimeRunChase + 5 < Time.time)
			{
				if(!changedDirection)
				{
					changedDirection = true;
					balls[0].SetVelocity(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * System.Convert.ToSingle(balls[0].speed));
				}
			}
			else
			{
				changedDirection = false;
			}
		}
		else
		{
			SpawnBot();
			balls[0].SetVelocity(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized * System.Convert.ToSingle(balls[0].speed));
		}
	}
}
