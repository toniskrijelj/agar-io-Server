using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServerSend
{
    private static void SendTCPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].tcp.SendData(_packet);
    }

    private static void SendUDPData(int _toClient, Packet _packet)
    {
        _packet.WriteLength();
        Server.clients[_toClient].udp.SendData(_packet);
    }

    private static void SendTCPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].tcp.SendData(_packet);
        }
    }
    private static void SendUDPDataToAll(Packet _packet)
    {
        _packet.WriteLength();
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            Server.clients[i].udp.SendData(_packet);
        }
    }

    private static void SendUDPDataToAll(int _exceptClient, Packet _packet, bool writeLength = true)
    {
		if (writeLength)
		{
			_packet.WriteLength();
		}
        for (int i = 1; i <= Server.MaxPlayers; i++)
        {
            if (i != _exceptClient)
            {
                Server.clients[i].udp.SendData(_packet);
            }
        }
    }
	private static void SendTCPDataToAll(int _exceptClient, Packet _packet)
	{
		_packet.WriteLength();
		for (int i = 1; i <= Server.MaxPlayers; i++)
		{
			if (i != _exceptClient)
			{
				Server.clients[i].tcp.SendData(_packet);
			}
		}
	}

	public static void Welcome(int _toClient, string _msg)
    {
        using (Packet _packet = new Packet((int)ServerPackets.welcome))
        {
            _packet.Write(_msg);
            _packet.Write(_toClient);

            SendTCPData(_toClient, _packet);
        }
    }

    public static void ConnectPlayer(Player player)
    {
        using(Packet _packet = new Packet((int)ServerPackets.connectPlayer))
        {
            _packet.Write((short)player.id);
			_packet.Write(player.color.r);
			_packet.Write(player.color.g);
			_packet.Write(player.color.b);

			SendTCPDataToAll(_packet);
        }
    }
    public static void ConnectPlayer(int _toClient, Player player)
    {
        using (Packet _packet = new Packet((int)ServerPackets.connectPlayer))
        {
            _packet.Write((short)player.id);
			_packet.Write(player.color.r);
			_packet.Write(player.color.g);
			_packet.Write(player.color.b);

			SendTCPData(_toClient, _packet);
        }
    }

    public static void BallPosition(int _playerId, int count, int[] _ballIndexes, Vector2[] _positions)
    {
        using (Packet _packet = new Packet((int)ServerPackets.ballVelocity))
        {
			_packet.Write(Time.time);
			_packet.Write((short)_playerId);
			_packet.Write((short)count);
			for (int i = 0; i < count; i++)
			{
				_packet.Write((short)_ballIndexes[i]);
				_packet.Write(_positions[i]);
			}

			SendTCPDataToAll(_playerId, _packet);
			SendUDPDataToAll(_playerId, _packet, false);
        }
    }

    public static void SpawnFood(Vector2 position)
    {
        using (Packet _packet = new Packet((int)ServerPackets.spawnFood))
        {
			_packet.Write((short)1); // one food
            _packet.Write(position);

			SendUDPDataToAll(_packet);
        }
    }
    public static void SpawnFood(int _toClient)
    {
		int totalCount = Food.foods.Count;
		int sentCount = 0;
		int currentCount;
		while (sentCount < totalCount)
		{
			using (Packet _packet = new Packet((int)ServerPackets.spawnFood))
			{
				currentCount = Mathf.Min(1000, totalCount - sentCount);
				_packet.Write((short)currentCount);
				for (int i = 0; i < currentCount; i++)
				{
					_packet.Write((Vector2)Food.foods[i + sentCount].transform.position);
				}
				sentCount += currentCount;
				SendTCPData(_toClient, _packet);
			}
		}
    }

	public static void PlayerSpawned(int _toClient, int _fromClient, int _ballIndex, Vector2 _position, int mass)
	{
		using (Packet _packet = new Packet((int)ServerPackets.spawnBall))
		{
			_packet.Write((short)_fromClient);
			_packet.Write((short)_ballIndex);
			_packet.Write(_position);
			_packet.Write(mass);

			SendTCPData(_toClient, _packet);
		}
	}
    public static void PlayerSpawned(int _fromClient, int _ballIndex, Vector2 _position, int mass)
    {
        using(Packet _packet = new Packet((int)ServerPackets.spawnBall))
        {
            _packet.Write((short)_fromClient);
            _packet.Write((short)_ballIndex);
            _packet.Write(_position);
			_packet.Write(mass);
			SendTCPDataToAll(_packet);
        }
    }

    public static void BallMass(int _fromClient, int _ballIndex, int _mass, bool TCP = false)
    {
        using(Packet _packet = new Packet((int)ServerPackets.ballMass))
        {
            _packet.Write((short)_fromClient);
            _packet.Write((short)_ballIndex);
			_packet.Write(Time.time);
			_packet.Write(_mass);

			if (TCP)
			{
				SendTCPDataToAll(_packet);
			}
			else
			{
				SendUDPDataToAll(_packet);
			}
        }
    }

    public static void KillBall(int _fromClient, int _ballIndex)
    {
        using(Packet _packet = new Packet((int)ServerPackets.killBall))
        {
            _packet.Write((short)_fromClient);
            _packet.Write((short)_ballIndex);

			SendTCPDataToAll(_packet);
        }
    }

	public static void SendDisconnect(int _fromClient)
	{
		using (Packet _packet = new Packet((int)ServerPackets.disconnect))
		{
			_packet.Write(_fromClient);

			SendTCPDataToAll(_fromClient, _packet);
		}
	}

	public static void SendPlayerName(int _fromClient, string name)
	{
		if (name == "") return;
		using (Packet _packet = new Packet((int)ServerPackets.playerName))
		{
			_packet.Write(_fromClient);
			_packet.Write(name);
			SendUDPDataToAll(_packet);
		}
	}

	public static void SendPlayerName(int _toClient, int _fromClient, string name)
	{
		if (name == "") return;
		using (Packet _packet = new Packet((int)ServerPackets.playerName))
		{
			_packet.Write(_fromClient);
			_packet.Write(name);
			SendUDPData(_toClient, _packet);
		}
	}

	public static void ShootMass(int _fromClient, Vector2 mousePosition)
	{
		using (Packet _packet = new Packet((int)ServerPackets.shootMass))
		{
			_packet.Write((short)_fromClient);
			_packet.Write(mousePosition);
			SendTCPDataToAll(_packet);
		}
	}

	public static void SpawnMass(int _toClient)
	{
		int totalCount = Mass.allMass.Count;
		int currentCount = 0;
		while (currentCount < totalCount)
		{
			using (Packet _packet = new Packet((int)ServerPackets.spawnMass))
			{
				_packet.Write((short)((totalCount - currentCount > 10) ? 10 : totalCount - currentCount));
				for (int i = 0; i < 10 && currentCount < totalCount; i++)
				{
					Mass mass = Mass.allMass[currentCount];
					_packet.Write((short)mass.playerId);
					_packet.Write((short)mass.ballIndex);
					_packet.Write((Vector2)mass.transform.position);
					_packet.Write(mass.rb.velocity);
					Color color = Server.clients[mass.playerId].player.color;
					_packet.Write(color.r);
					_packet.Write(color.g);
					_packet.Write(color.b);
					_packet.Write(mass.spawnTime);
					currentCount++;
				}
				SendUDPData(_toClient, _packet);
			}
		}
	}
}