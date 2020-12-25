using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;

public class Gaming : MonoBehaviour
{
    [SerializeField]
    int[] numberI_Draw = new int[5];
    [SerializeField]
    int round;
    [SerializeField]
    GameObject[] buttonCard;
    [SerializeField]
    Text[] buttonText;
    [SerializeField]
    Text WLMsg;
    [SerializeField]
    GameObject Restart;


    bool isRoundOver;
    //  -1 represent you haven't throw the card
    int thisRoundThrow;

    private string saveMyCardString;
    private string saveRivalCardString;
    private ServerClient serverClient;

    string[] roundKey = new string[5] { "round1", "round2", "round3", "round4", "round5" };

    protected Callback<LobbyDataUpdate_t> Callback_lobbyDataUpdate;

    // Start is called before the first frame update
    void Start()
    {
        serverClient = GameObject.Find("Manager").GetComponent<ServerClient>();
        Callback_lobbyDataUpdate = Callback<LobbyDataUpdate_t>.Create(OnLobbyDataUpdate);
        Restart.SetActive(false);
    }
    private void OnEnable()
    {
        Init();
    }

    void OnLobbyDataUpdate(LobbyDataUpdate_t result)
    {
        if((CSteamID)result.m_ulSteamIDMember == serverClient.ThisLobbyID)
        {
        }
        else if((CSteamID)result.m_ulSteamIDMember == serverClient.MyCsteamID)
        {

        }
        else
        {
            string recvData = SteamMatchmaking.GetLobbyMemberData(
               serverClient.ThisLobbyID,
               (CSteamID)serverClient.RecvCsteamID,
               roundKey[round]
               );
            saveRivalCardString += recvData;
            if (recvData != "Null" && recvData != "" && recvData != null)
            {
                StartCoroutine(WaitToCompare(result, recvData));
            }
            else
            {
            }
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void ThrowCard(int cardIndex)
    {
        if (isRoundOver && serverClient.RecvCsteamID != new CSteamID(0))
        {
            Debug.Log("we throw number " + numberI_Draw[cardIndex] + " to compare.");
            saveMyCardString += numberI_Draw[cardIndex].ToString();
            SteamMatchmaking.SetLobbyMemberData(
                serverClient.ThisLobbyID,
                roundKey[round],
                numberI_Draw[cardIndex].ToString()
                );
            thisRoundThrow = numberI_Draw[cardIndex];
            buttonCard[cardIndex].SetActive(false);
            isRoundOver = false;
        }

    }

    //  initial on game start, will be reused when restarted;
    public void Init()
    {
        saveMyCardString = "";
        saveRivalCardString = "";
        round = 0;
        thisRoundThrow = -1;
        for (int i = 0; i < 5; i++)
        {
            numberI_Draw[i] = Random.Range(1, 10);
            buttonText[i].text = numberI_Draw[i].ToString();
            buttonCard[i].SetActive(true);
        }
        isRoundOver = true;
    }

    public void OnRestartButtonClicked()
    {
        //  set a Msg on to the lobby
        StartCoroutine(ContinueToCheckRestart());
        Restart.SetActive(false);
        SteamMatchmaking.SetLobbyMemberData(
            serverClient.ThisLobbyID,
            "isRestart",
            "Y".ToString());
        for(int i = 0; i < 5; i++)
        {   // reset the round data for a new round
            SteamMatchmaking.SetLobbyMemberData(
                serverClient.ThisLobbyID,
                roundKey[i],
                ""
                );
        }
    }


    IEnumerator WaitToCompare(LobbyDataUpdate_t result, string recvData)
    {
        while (thisRoundThrow == -1)
        {   //  -1 represent you haven't throw the card
            WLMsg.text = "Your rival is ready.";
            yield return new WaitForSeconds(0.2f);
        }
        Debug.Log("We recv user " + result.m_ulSteamIDMember + "'s Msg");
        Debug.Log("We recv number " + recvData);
        if (thisRoundThrow.ToString().CompareTo(recvData) == 1)
        {
            WLMsg.text = "I win this round!";
        }
        else if (thisRoundThrow.ToString().CompareTo(recvData) == 0)
        {
            WLMsg.text = "We draw this round.";
        }
        else if (thisRoundThrow.ToString().CompareTo(recvData) == -1)
        {
            WLMsg.text = "I lose this round.";
        }
        else
        {
            Debug.Log("Something error when comparing number.");
        }
        Debug.Log("Rival use " + recvData +".");
        Debug.Log("round " + round + " is over.");

        //  set next round number
        round++;
        isRoundOver = true;
        thisRoundThrow = -1;
        if(round == 5)
        {
            Restart.SetActive(true);
            SaveData();
            round = 0;
        }
        yield return null;
    }

    IEnumerator ContinueToCheckRestart()
    {   //  check until your rival press the restart button
        string recvData = SteamMatchmaking.GetLobbyMemberData(
            serverClient.ThisLobbyID,
            (CSteamID)serverClient.RecvCsteamID,
            "isRestart"
            );
        while(recvData != "Y" || recvData =="" || recvData == null)
        {
            recvData = SteamMatchmaking.GetLobbyMemberData(
               serverClient.ThisLobbyID,
               (CSteamID)serverClient.RecvCsteamID,
               "isRestart"
               );
            yield return new WaitForSeconds(0.3f);
        }
        Init();
        SteamMatchmaking.SetLobbyMemberData(
            serverClient.ThisLobbyID,
            "isRestart",
            "".ToString());
        yield return null;
    }

    private void  SaveData()
    {
        PlayerPrefs.SetString("SavedMyCard", saveMyCardString);
        PlayerPrefs.SetString("SavedRivalCard", saveRivalCardString);
        PlayerPrefs.Save();
        Debug.Log("Game data saved!");
    }
}
