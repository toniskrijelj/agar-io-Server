using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public Player playerPrefab;
	public Bot botPrefab;
    public Food foodPrefab;

    private void Awake()
    {
		if (instance == null)
		{
			instance = this;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
		}
		else if (instance != this)
		{
			Destroy(this);
		}
    }

    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        Server.Start(105, 1234);

		for(int i = 0; i < 500; i++)
		{
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
			Instantiate(foodPrefab, new Vector3(Random.Range(-149f, 149f), Random.Range(-149, 149)), Quaternion.identity);
		}
	}

	private void OnApplicationQuit()
    {
        Server.Stop();
    }

}
