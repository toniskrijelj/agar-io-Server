using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;

public class ServerHandle
{
    public static void WelcomeReceived(int _fromClient, Packet _packet)
    {
        int _clientIdCheck = _packet.ReadInt();

        Debug.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {_fromClient}.");
		if (_fromClient != _clientIdCheck)
		{
			Debug.Log($"Player ID: {_fromClient}) has assumed the wrong client ID ({_clientIdCheck})!");
		}
		ConnectPlayer(_fromClient);
    }

    private static void ConnectPlayer(int _fromClient, bool isBot = false)
    {
		Player _player;
		if (!isBot)
		{
			_player = Object.Instantiate(NetworkManager.instance.playerPrefab);
		}
		else
		{
			_player = Object.Instantiate(NetworkManager.instance.botPrefab);
		}
        _player.id = _fromClient;
		_player.isBot = isBot;
		_player.color = RandomColor.GetRandomColor(.1f);
        Server.clients[_fromClient].player = _player;
        ServerSend.ConnectPlayer(_player);
		if (isBot) return;
		bool hasPlayers = false;
		int maxPlayer = Server.MaxPlayers;
		Client _client;
        for(int i = 1; i <= maxPlayer; i++)
        {
			_client = Server.clients[i];
            if(_client.id != _fromClient)
            {
                if (_client.player != null)
                {
					hasPlayers = true;
                    ServerSend.ConnectPlayer(_fromClient, _client.player);
					for(int j = 0; j < 16; j++)
					{
						Ball current = _client.player.balls[j];
						if (current != null)
						{
							ServerSend.PlayerSpawned(_fromClient, _client.id, j, current.transform.position, current.mass);
						}
					}
					ServerSend.SendPlayerName(_fromClient, _client.id, _client.player.playerName);
				}
            }
        }

		ServerSend.SpawnFood(_fromClient);
		//ServerSend.SpawnMass(_fromClient);

		if (!hasPlayers)
		{
			for (int i = 2; i < 32; i++)
			{
				ConnectPlayer(i, true);
			}
		}
	}

    public static void ReceiveSpaceInput(int _fromClient, Packet _packet)
    {
        Player _player = Server.clients[_fromClient].player;
		_player.spacePressed = true;
    }

    public static void SpawnPlayer(int _fromClient, Packet _packet)
    {
        Vector2 spawnPosition = _packet.ReadVector2();

		ServerSend.PlayerSpawned(_fromClient, 0, spawnPosition, 40);
		string name = _packet.ReadString();
		Server.clients[_fromClient].player.playerName = name;
		ServerSend.SendPlayerName(_fromClient, name);
        Server.clients[_fromClient].player.SetBall(spawnPosition);
    }

	static int[] indexes = new int[16];
	static Vector2[] positions = new Vector2[16];

	public static void ReceivePosition(int _fromClient, Packet _packet)
	{
		int count = _packet.ReadInt();
		Client client = Server.clients[_fromClient];
		for (int i = 0; i < count; i++)
		{
			indexes[i] = _packet.ReadInt();
			positions[i] = _packet.ReadVector2();
			client.player.SetPosition(indexes[i], positions[i]);
		}
		if(count > 0)
		{
			ServerSend.BallPosition(_fromClient, count, indexes, positions);
		}
	}

	public static void ReceiveWInput(int _fromPlayer, Packet _packet)
	{
		Vector2 mousePosition = _packet.ReadVector2();
		ServerSend.ShootMass(_fromPlayer, mousePosition);
		Player player = Server.clients[_fromPlayer].player;
		for(int i = 0; i < 16; i++)
		{
			if(player.balls[i] != null)
			{
				player.balls[i].ShootMass(mousePosition);
			}
		}
	}

	public static void PlayerDisconnect(int _fromClient, Packet _packet)
	{
		if (Server.clients[_fromClient] != null)
		{
			Server.clients[_fromClient].Disconnect();
		}
	}
}