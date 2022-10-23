using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int id;
	public Color color;
    public Vector3 mouseWorldPos;
    public bool spacePressed;
	public string playerName = "";
	public bool isBot = false;

    public Ball[] balls = new Ball[16];
    List<Ball> ballsToSplit = new List<Ball>();
    List<int> freeIndexes = new List<int>();

    public void SetBall(Vector2 position) /// DODAJ OVDE ZA BOTA
    {
		if (isBot)
		{
			balls[0] = Instantiate(GameAssets.i.botBall, position, Quaternion.identity, transform).ball;
		}
		else
		{
			balls[0] = Instantiate(GameAssets.i.ball, position, Quaternion.identity, transform);
		}
        balls[0].Initialize(this, 0, 40, isBot);
    }

    protected virtual void FixedUpdate()
    {
        if (spacePressed)
        {
            ballsToSplit.Clear();
            freeIndexes.Clear();
        }

        for (int i = 0; i < 16; i++)
        {
            if (balls[i] != null)
            {
                if (spacePressed)
                {
                    ballsToSplit.Add(balls[i]);
                }
            }
            else
            {
                if (spacePressed)
                {
                    freeIndexes.Add(i);
                }
            }
        
        }

        if (spacePressed)
        {
			spacePressed = false;
            int ballsToSplitCount = ballsToSplit.Count;
            int freeIndexesCount = freeIndexes.Count;

            if (ballsToSplitCount <= freeIndexesCount)
            {
                for (int i = 0, j = 0; i < ballsToSplitCount; i++, j++)
                {
                    Ball ball = ballsToSplit[i].Split();
                    if (ball != null)
                    {
                        balls[freeIndexes[j]] = ball;
						ServerSend.PlayerSpawned(id, freeIndexes[j], ballsToSplit[i].transform.position, ballsToSplit[i].mass);
						ball.Initialize(this, freeIndexes[j], ballsToSplit[i].mass);
                    }
                    else
                    {
                        j--;
                    }
                }
            }
            else
            {
                for (int i = 0, j = 0; j < freeIndexesCount && i < ballsToSplitCount; i++, j++)
                {
                    Ball ball = ballsToSplit[i].Split();
                    if (ball != null)
                    {
						balls[freeIndexes[j]] = ball;
						ServerSend.PlayerSpawned(id, freeIndexes[j], ballsToSplit[i].transform.position, ballsToSplit[i].mass);
						ball.Initialize(this, freeIndexes[j], ballsToSplit[i].mass);
					}
                    else
                    {
                        j--;
                    }
                }
            }
        }
    }

	public void SetPosition(int index, Vector2 position)
	{
		if (balls[index] == null) return;
		balls[index].rb.position = position;
	}
}