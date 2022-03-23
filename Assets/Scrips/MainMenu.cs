using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkMag;

    [Header("UI")] 
    [SerializeField] private GameObject landingPagePanel = null;

    public void HostLobby(){
        networkMag.StartHost();
        Debug.Log("Starhost");
        landingPagePanel.SetActive(false);
    }


}
