using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject PlayerPrefab = null;
    private static List<Transform> spawnPoints = new List<Transform>();
    private int nextIndex = 0;

    public static void AddSpawnPonint(Transform transform){
        spawnPoints.Add(transform);
        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();

    }
    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);
    public override void OnStartServer()
    {
        //base.OnStartServer();
        Debug.Log($"this is {nextIndex}");
        NetworkManagerLobby.OnServerReadied += SpawnPlayer;
    }

    [ServerCallback]
    private void  OnDestroy() => NetworkManagerLobby.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn){
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);
        if(spawnPoint == null){
            Debug.LogError($"Missing spwn point for player {nextIndex}");
            return;
        }

        GameObject playerInstance = Instantiate(PlayerPrefab, spawnPoints[nextIndex].position,spawnPoints[nextIndex].rotation);
        NetworkServer.Spawn(playerInstance,conn);
        Debug.Log("success one");
        nextIndex++;
    }












}
