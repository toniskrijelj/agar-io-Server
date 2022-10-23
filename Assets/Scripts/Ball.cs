using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State
{
	None,
	Flying,
	Cooldown
}

public class Ball : MonoBehaviour
{
	Player player;
	int index;

	public bool isBot;

	public int lastPacketId = -100000;
	public float massLoss;
	public float massLossRemaining = 0;
	public int mass = 1000;
	public double speed;
	public Rigidbody2D rb;

	float spawnTime = 0;
	float cooldownTime;

	public void Initialize(Player _player, int _index, int _mass, bool _isBot = false)
	{
		player = _player;
		index = _index;
		isBot = _isBot;
		if (isBot)
		{
			rb = transform.parent.GetComponent<Rigidbody2D>();
		}
		else
		{
			rb = GetComponent<Rigidbody2D>();
		}
		spawnTime = Time.time;
		massTimer = Time.time + .5f;
		SetMass(_mass);
	}

	public static float GetAngleFromVectorFloat(Vector3 dir)
	{
		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		if (n < 0) n += 360;

		return n;
	}

	public static Vector3 GetVectorFromAngle(int angle)
	{
		// angle = 0 -> 360
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	public float lastTimeRunChase;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Food"))
		{
			SetMass(mass + 1);
			Vector3 newPosition = new Vector3(UnityEngine.Random.Range(-149f, 149f), UnityEngine.Random.Range(-149, 149));
			collision.transform.position = newPosition;
			ServerSend.SpawnFood(newPosition);
			return;
		}
	}

	float massTimer;
	int previousSentMass = 0;

	private void FixedUpdate()
	{
		if (!isBot)
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				SetMass(mass + 100);
			}
			if (Input.GetKeyDown(KeyCode.Q))
			{
				SetMass(mass - 100);
			}
		}
		massLossRemaining += massLoss * Time.fixedDeltaTime;
		int currentMassLoss = Mathf.FloorToInt(massLossRemaining);
		massLossRemaining -= currentMassLoss;
		mass = Mathf.Clamp(mass - currentMassLoss, 40, 1000000);
		if(massTimer < Time.time)
		{
			massTimer += .5f;
			if (previousSentMass != mass)
			{
				previousSentMass = mass;
				ServerSend.BallMass(player.id, index, mass);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D collision)
	{
		Ball ball = collision.GetComponent<Ball>();
		if (ball == null) return;
		if(isBot)
		{
			if(mass > ball.mass * 1.3f)
			{
				SetVelocity(((Vector2)(collision.transform.position - transform.position)).normalized * Convert.ToSingle(speed));
				lastTimeRunChase = Time.time;
			}
			else if(ball.mass > mass * 1.3f)
			{
				SetVelocity(((Vector2)(transform.position - collision.transform.position)).normalized * Convert.ToSingle(speed));
				lastTimeRunChase = Time.time;
			}
		}
		if (Vector2.Distance(
			transform.position, collision.transform.position)
			+ ball.GetRadius() * 0.0375 <= GetRadius() * .25f)
		{
			CheckBall(ball);
		}

	}
	// 2,5 + 2.25
	private void CheckBall(Ball ball)
	{
		if ((ball.player != player && mass > ball.mass * 1.3f) 
			|| (ball.player == player 
				&& !killed 
				&& !ball.killed 
				&& cooldownTime + spawnTime < Time.time 
				&& ball.spawnTime +  ball.cooldownTime < Time.time 
				&& mass >= ball.mass))
		{
			SetMass(mass + ball.mass);
			ball.Kill();
		}
	}

	public Ball Split()
	{
		if (mass / 2 < 40) return null;
		SetMass(mass / 2);
		Ball ball = Instantiate(GameAssets.i.ball, transform.position, Quaternion.identity, transform.parent);
		return ball;
	}

	public void SetMass(int _mass)
	{
		int prevMass = mass;
		mass = Mathf.Clamp(_mass, 40, 1000000);

		cooldownTime = 0.003030303f * _mass + 29.69697f;

		float scale = Convert.ToSingle(33656.95 + (0.1347952 - 33656.95) / (1 + Math.Pow(mass / 2059981000.0, 0.5041123)));
		speed = -1143.788 + (1154.788 - -1143.788) / (1 + Math.Pow(mass / 779.436, 0.001584008));
		massLoss = 0.001111111f * mass + 0.8888889f;
		if (isBot)
		{
			transform.parent.localScale = new Vector3(scale, scale, 1);
			SetVelocity(rb.velocity.normalized * Convert.ToSingle(speed));
		}
		else
		{
			transform.localScale = new Vector3(scale, scale, 1);
		}

		if(Mathf.Abs(prevMass - mass) > 24)
		{
			ServerSend.BallMass(player.id, index, mass, true);
		}
	}

	private bool killed = false;

	public void Kill()
	{
		killed = true;
		if (isBot)
		{
			Destroy(transform.parent.gameObject);
		}
		else
		{
			Destroy(gameObject);
		}
		ServerSend.KillBall(player.id, index);
	}

	public void SetVelocity(Vector2 velocity)
	{
		rb.velocity = velocity;
	}

	public float GetRadius()
	{
		if(isBot)
		{
			return transform.parent.localScale.x;
		}
		return transform.localScale.x;
	}

	public void ShootMass(Vector2 mousePosition)
	{
		if (mass >= 90)
		{
			SetMass(mass - 50);
			Vector2 toMouseDireciton = (mousePosition - (Vector2)transform.position).normalized;
			Mass.CreateMass(player.id, index, (Vector2)transform.position + (toMouseDireciton * (transform.localScale.x * .25f - 1)), toMouseDireciton);

		}
	}
}


// 10
// 20
// radijus iz 5 -> 4.3