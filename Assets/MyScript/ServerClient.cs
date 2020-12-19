using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using System.Net;

//  It should be static originally.......

public class ServerClient : MonoBehaviour
{
    //  the receiver is the rival
    [SerializeField]
    static CSteamID recvCsteamID;
    [SerializeField]
    static CSteamID myCsteamID;
    [SerializeField]
    static CSteamID thisLobbyID;

    private int numberMemberInCurrentLobby;

    [SerializeField]
    public PlayerIsServerOrClient player;

    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;

    // Start is called before the first frame update
    void Start()
    {   
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        numberMemberInCurrentLobby = (int)SteamMatchmaking.GetNumLobbyMembers((CSteamID)(ulong)thisLobbyID);
        if (numberMemberInCurrentLobby == 1)
        {
            //The rival will be dis connected
        }
    }

    void OnLobbyCreated(LobbyCreated_t callback)
    {   //  if player create a lobby, then we see the player as a server
        player = PlayerIsServerOrClient.AsServer;
        ThisLobbyID = (CSteamID) callback.m_ulSteamIDLobby;
    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        if (player == PlayerIsServerOrClient.AsServer)
        {

        }
        else
        {
            player = PlayerIsServerOrClient.AsClient;
        }

    }

    //
    //
    //


    public CSteamID RecvCsteamID
    {
        get { return recvCsteamID; }
        set { recvCsteamID = value; }
    }

    public CSteamID MyCsteamID
    {
        get { return myCsteamID; }
        set { myCsteamID = value; }
    }

    public CSteamID ThisLobbyID
    {
        get { return thisLobbyID; }
        set { thisLobbyID = value; }
    }
}

public enum PlayerIsServerOrClient {
    NonOfAny = 0,
    AsClient = 1,
    AsServer = 2
}

