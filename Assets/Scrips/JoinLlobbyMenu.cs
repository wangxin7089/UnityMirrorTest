using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class JoinLlobbyMenu : MonoBehaviour
{
    [SerializeField] private NetworkManagerLobby networkMag = null;

    [Header("UI")]
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private TMP_InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private void  OnEnable() {
        NetworkManagerLobby.OnClientConnected += HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected += HandleClientDisconnected;
        Debug.Log("OnEnable");
    }
    
    private void  OnDisable() {
        NetworkManagerLobby.OnClientConnected -= HandleClientConnected;
        NetworkManagerLobby.OnClientDisconnected -= HandleClientDisconnected;
        Debug.Log("OnDisable");
    }

    public void JoinLobby(){
        string ipAddress = ipAddressInputField.text;

        networkMag.networkAddress = ipAddress;
        networkMag.StartClient();
        Debug.Log("JoinLobby");
        joinButton.interactable = false;
    }
    private void  HandleClientConnected(){
        joinButton.interactable = true;

        gameObject.SetActive(false);
        landingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected(){
        joinButton.interactable = true;
    }
}
