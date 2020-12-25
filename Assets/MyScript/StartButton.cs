using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Steamworks;

public class StartButton : MonoBehaviour
{
    [SerializeField]
    private GameObject button;
    [SerializeField]
    private const string HostAddressKey = "HostAddress";
    private const string Client_CsteamID = "Client_CsteamID";
    private ServerClient serverClient;

    [SerializeField]
    //  controll the chatroom object
    private GameObject chatRoom;
    [SerializeField]
    //  access the Manager
    private GameObject gameObj;

    private bool isFindRival;

    public uint numberOfLobby;
    public uint numberMemberInCurrentLobby;

    protected Callback<LobbyCreated_t> Callback_lobbyCreated;
    protected Callback<LobbyMatchList_t> Callback_lobbyList;
    protected Callback<LobbyEnter_t> Callback_lobbyEnter;
    protected Callback<LobbyDataUpdate_t> Callback_lobbyInfo;

    [SerializeField]
    ulong current_lobbyID;
    List<CSteamID> lobbyIDS;

    public CSteamID lobbyOwner;
    public CSteamID myCsteamID;
    public CSteamID rivalCsteamID;

    // Use this for initialization
    void Start()
    {
        current_lobbyID = 0;
        isFindRival = false;
        lobbyIDS = new List<CSteamID>();
        serverClient = gameObject.GetComponent<ServerClient>();
        Callback_lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        Callback_lobbyList = Callback<LobbyMatchList_t>.Create(OnGetLobbiesList);
        Callback_lobbyEnter = Callback<LobbyEnter_t>.Create(OnLobbyEntered);
        Callback_lobbyInfo = Callback<LobbyDataUpdate_t>.Create(OnGetLobbyInfo);
        string userSteamName = SteamFriends.GetPersonaName();
        myCsteamID = Steamworks.SteamUser.GetSteamID();
        serverClient.MyCsteamID = Steamworks.SteamUser.GetSteamID();
        Debug.Log("Hello, " + userSteamName);
    }

    void Update()
    {

        //  always update the member number in this lobby
        numberMemberInCurrentLobby = (uint)SteamMatchmaking.GetNumLobbyMembers((CSteamID)(ulong)current_lobbyID);
        lobbyOwner = SteamMatchmaking.GetLobbyOwner((CSteamID)current_lobbyID);

        // Command - List lobbies
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Trying to get list of available lobbies ...");
            SteamMatchmaking.RequestLobbyList();
        }

        if (!isFindRival && (ulong)rivalCsteamID == 0 && numberMemberInCurrentLobby == 2)
        {
            Debug.Log("Find rival");
            isFindRival = true;
            rivalCsteamID = SteamMatchmaking.GetLobbyMemberByIndex((CSteamID)current_lobbyID, (int)numberMemberInCurrentLobby - 1);
            serverClient.RecvCsteamID = rivalCsteamID;
            SteamNetworking.AcceptP2PSessionWithUser(rivalCsteamID);
        }
        if(numberMemberInCurrentLobby == 2)
        {
            chatRoom.SetActive(true);
            gameObj.SetActive(true);
        }else if(numberMemberInCurrentLobby == 1)
        {
            chatRoom.SetActive(false);
            gameObj.SetActive(false);
        }
    }

    void OnLobbyCreated(LobbyCreated_t result)
    {
        if (result.m_eResult == EResult.k_EResultOK)
        {
            Debug.Log("Lobby created -- SUCCESS!");

            SteamMatchmaking.SetLobbyData(
                (CSteamID)result.m_ulSteamIDLobby,
                HostAddressKey,
                SteamUser.GetSteamID().ToString());
            Debug.Log(SteamUser.GetSteamID().ToString());
        }
        else
        {
            Debug.Log("Lobby created -- failure ...");
            button.SetActive(true);
            return;
        }
    }

    void OnGetLobbiesList(LobbyMatchList_t result)
    {
        lobbyIDS.Clear();
        numberOfLobby = result.m_nLobbiesMatching;
        Debug.Log("Found " + numberOfLobby + " lobbies!");
        for (int i = 0; i < result.m_nLobbiesMatching; i++)
        {
            CSteamID lobbyID = SteamMatchmaking.GetLobbyByIndex(i);
            lobbyIDS.Add(lobbyID);
            SteamMatchmaking.RequestLobbyData(lobbyID);
        }
    }

    void OnGetLobbyInfo(LobbyDataUpdate_t result)
    {

    }

    void OnLobbyEntered(LobbyEnter_t result)
    {
        //  reponse == 1 represent someone enter lobby success
        if (result.m_EChatRoomEnterResponse == 1)
        {
            current_lobbyID = result.m_ulSteamIDLobby;
            Debug.Log("Lobby joined! I join " + current_lobbyID);

            CSteamID hostCsteamID = SteamMatchmaking.GetLobbyOwner((CSteamID)result.m_ulSteamIDLobby);

            if (myCsteamID != hostCsteamID)
            {   //  host's rival CsteamID is written on the Update
                rivalCsteamID = hostCsteamID;
                SteamNetworking.AcceptP2PSessionWithUser(rivalCsteamID);
                isFindRival = true;

                serverClient.ThisLobbyID = (CSteamID)current_lobbyID;
                serverClient.MyCsteamID = myCsteamID;
                serverClient.RecvCsteamID = hostCsteamID;
                
            }
            else if (numberMemberInCurrentLobby == 2)
            {

            }
        }
        else
        {
            Debug.Log("Failed to join lobby.");
        }

    }

    public void AsStartButtonPress()
    {   // Jion the first lobby, if there is no lobby exist, then create one and keep waiting.
        StartCoroutine(TryToCreateLobby());
        //SceneManager.LoadScene("Gaming");
    }

    IEnumerator TryToCreateLobby() {
        button.SetActive(false);

        //  search lobby fitst and set up the number of lobby;
        //  see OnGetLobbiesList
        SteamMatchmaking.RequestLobbyList();

        yield return new WaitForSeconds(1.0f);
        // Command - Join lobby(testing purposes)
        for (int i = 0; i < numberOfLobby; i++)
        {
            Debug.Log("Trying to join " + i + " listed lobby ...");
            SteamMatchmaking.JoinLobby(SteamMatchmaking.GetLobbyByIndex(i));
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(1.0f);

        if (current_lobbyID == 0)
        {
            // Command - Create new lobby
            Debug.Log("Trying to create a new lobby ...");
            SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypePublic, 2);
        }
        yield return null;
    }
}
