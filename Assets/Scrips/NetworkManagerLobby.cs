using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerLobby : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    [SerializeField] private string menuScene = string.Empty;

    [Header("Maps")]
    [SerializeField] private int numberOfRounds = 1;
    //[SerializeField] private MapSet mapSet = null;

    [Header("Room")]
    //[SerializeField] private int numberOfRounds1 = 1;
    [SerializeField] private NetworkRoomPlayerLobby roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject roundSystem = null;

    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;


    public List<NetworkRoomPlayerLobby> RoomPlayers{get;}  = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> GamePlayers{get;}  = new List<NetworkGamePlayerLobby>();

    public override void OnStartServer()=>spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

    public override void OnStartClient()
    {
        var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

        foreach(var prefab in spawnablePrefabs){
            NetworkClient.RegisterPrefab(prefab);
            Debug.Log("one prefab");
        }
    }

    public override void OnClientConnect()//NetworkClient.connection
    {
        base.OnClientConnect();
        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();
        OnClientConnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if(numPlayers >= maxConnections){
            conn.Disconnect();
            Debug.Log("maxNumber");
            return;
            
        }

        if(SceneManager.GetActiveScene().name != menuScene){
            conn.Disconnect();
            Debug.Log("not menuscene");
            Debug.Log(menuScene);
            Debug.Log(SceneManager.GetActiveScene().name);

            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        //base.OnServerAddPlayer(conn);
        Debug.Log("AddPlayer");
        if(SceneManager.GetActiveScene().name == menuScene){
            bool isLeader = RoomPlayers.Count == 0;
            NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(roomPlayerPrefab);
            roomPlayerInstance.IsLeader = isLeader;
            NetworkServer.AddPlayerForConnection(conn,roomPlayerInstance.gameObject);
            Debug.Log("AddPlayer success");
        }
    }
    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(conn.identity != null){
            var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
            RoomPlayers.Remove(player);
            NotfyPlayersOfReadState();
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        RoomPlayers.Clear();
        base.OnStopServer();
    }

    public void NotfyPlayersOfReadState(){
        foreach(var player in RoomPlayers){
            player.HandleReadyToStart(IsReadToStart());
        }
    }

    private bool IsReadToStart(){
        if(numPlayers < minPlayers) {return false;}

        foreach(var player in RoomPlayers){
            if(!player.IsReady) {return false;}
        }
        return true;
    }

    public void StartGame(){
        if(SceneManager.GetActiveScene().name == menuScene){
            if(!IsReadToStart()) {return;}
            ServerChangeScene("scene_Map_01");
        }
    }

    public override  void ServerChangeScene(string newSceneName)
    {
        Debug.Log("serverChangeScene");
        Debug.Log(SceneManager.GetActiveScene().name+menuScene+newSceneName);
        if(SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("scene_Map")){
            for(int i = RoomPlayers.Count - 1;i >= 0;i--){
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                NetworkServer.Destroy(conn.identity.gameObject);
                NetworkServer.ReplacePlayerForConnection(conn,gameplayerInstance.gameObject);

            }
        }
        base.ServerChangeScene(newSceneName);
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if(sceneName.StartsWith("scene_Map")){
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);
        }
    }

    public override void OnServerReady(NetworkConnectionToClient conn)
    {
        base.OnServerReady(conn);
        OnServerReadied?.Invoke(conn);
    }

}
