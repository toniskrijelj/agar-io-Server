using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mass : MonoBehaviour
{
	public static Mass CreateMass(int playerId, int ballIndex, Vector2 spawnPosition, Vector2 direction)
	{
		Mass mass = Instantiate(GameAssets.i.mass, spawnPosition, Quaternion.identity);
		mass.rb.AddForce(direction * 1000, ForceMode2D.Force);
		mass.sender = Server.clients[playerId].player.balls[ballIndex].gameObject;
		mass.playerId = playerId;
		mass.ballIndex = ballIndex;
		mass.spawnTime = Time.time;
		return mass;
	}
	public static List<Mass> allMass = new List<Mass>();

	public Rigidbody2D rb;
	public GameObject sender;
	public int playerId;
	public int ballIndex;
	public float spawnTime;
	public bool eaten = false;

	private void Awake()
	{
		allMass.Add(this);
	}

	private void FixedUpdate()
	{
		if(spawnTime + 10 < Time.time)
		{
			Destroy(gameObject);
		}
	}

	private void OnDestroy()
	{
		allMass.Remove(this);
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (eaten) return;
		if(collision.CompareTag("Wall"))
		{
			Destroy(gameObject);
			return;
		}
		if (collision.gameObject != sender || spawnTime + .5f < Time.time)
		{
			if (collision.CompareTag("Ball"))
			{
				Ball ball = collision.GetComponent<Ball>();
				eaten = true;
				ball.SetMass(ball.mass + 45);
				Destroy(gameObject);
			}
		}
	}

	public void Eat(Ball ball)
	{
		if (eaten) return;
		if( ball.gameObject != sender || spawnTime + 1 < Time.time)
		{
			eaten = true;
			ball.SetMass(ball.mass + 45);
			Destroy(gameObject);
		}
	}
}
