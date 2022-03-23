using Mirror;
using System;
using UnityEngine;
using TMPro;
using System.Collections;



public class ChatBehaviour : NetworkBehaviour 
{
    [SerializeField] private GameObject chatUI = null;
    [SerializeField] private TMP_Text chatText = null;
    [SerializeField] private TMP_InputField inputfield= null;

    private static event Action<string> OnMessage;
    public override void OnStartAuthority()
    {
        chatUI.SetActive(true);
        OnMessage += HandleNewMessage;
        //Debug.Log("ui active");
    }

    [ClientCallback]
    private void OnDestroy()
    {
        //Debug.Log("destroy");
        if(!hasAuthority){return;}
        OnMessage -= HandleNewMessage;
        
    }
    private void HandleNewMessage(string message) 
    {
        chatText.text += message;
        //Debug.Log("HandleNewMessage");
    }

    [Client]
    public void Send(string message){
        //Debug.Log("send");
        if(!Input.GetKeyDown(KeyCode.Return)) {return;}
        //Debug.Log("send1");
        if(string.IsNullOrWhiteSpace(message)) {return;}
        //Debug.Log("send2");
        CmdSendMessage(message);
        inputfield.text = string.Empty;

    }
    [Client]
    public void SendButton(){
        if(string.IsNullOrWhiteSpace(inputfield.text)) {return;}
        CmdSendMessage(inputfield.text);
        inputfield.text = string.Empty;
    }
    

    [Command]
    private void CmdSendMessage(string message){
        //Debug.Log("cmdsendmessage");
        RpcHandleMessage($"[{connectionToClient.connectionId}]:{message}");
    }

    [ClientRpc]

    private void RpcHandleMessage(string message){
        //Debug.Log("rpchandlemessage");
        OnMessage?.Invoke($"\n{message}");
    }

    
}
