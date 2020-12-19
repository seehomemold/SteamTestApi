using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Steamworks;
using System;

public class ChatRoom : MonoBehaviour
{
    public int maxMessage = 6;

    //  textObj is the text we show on the game panel
    [SerializeField]
    GameObject chatPanel, textObj;
    [SerializeField]
    InputField ChatBox;
    [SerializeField]
    List<Message> messageList = new List<Message>();

    protected Callback<LobbyChatMsg_t> Callback_Chat;

    //  suggest don't use this method to access MonoBehavier, but I am lazy
    ServerClient serverClient = new ServerClient();

    // Start is called before the first frame update
    void Start()
    {
        chatPanel = GameObject.Find("Content");
        ChatBox = GameObject.Find("InputField").GetComponent<InputField>();
        Callback_Chat = Callback<LobbyChatMsg_t>.Create(OnChatMsgRecv);
    }

    // Update is called once per frame
    void Update()
    {
        //  the input rule is write on here
        if (ChatBox.text != "")
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                byte[] bytes = new byte[ChatBox.text.Length * sizeof(char)];
                System.Buffer.BlockCopy(ChatBox.text.ToCharArray(), 0, bytes, 0, ChatBox.text.Length * sizeof(char));
                bool isSendSuccess = SteamMatchmaking.SendLobbyChatMsg(serverClient.ThisLobbyID,
                    bytes,
                    bytes.Length + 1
                    );
                ChatBox.text = "";
                if (isSendSuccess) Debug.Log("Send msg success");
            }
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    string hello = "Hello!";

        //    // allocate new bytes array and copy string characters as bytes
        //    //byte[] bytes = new byte[hello.Length * sizeof(char)];
        //    //System.Buffer.BlockCopy(hello.ToCharArray(), 0, bytes, 0, bytes.Length);

        //    byte[] sendMsgByte = new byte[hello.Length * sizeof(char)];
        //    System.Buffer.BlockCopy(hello.ToCharArray(), 0, sendMsgByte, 0, hello.Length * sizeof(char));

        //    bool isSendSuccess = SteamMatchmaking.SendLobbyChatMsg(serverClient.ThisLobbyID,
        //        sendMsgByte,
        //        sendMsgByte.Length + 1
        //        );
        //    if (isSendSuccess) Debug.Log("Send msg success");

        //    //SteamNetworking.SendP2PPacket(receiver, bytes, (uint)bytes.Length, EP2PSend.k_EP2PSendReliable);

        //}
    }


    //  whenever someone call SteamMatchmaking.SendLobbyChatMsg()
    //  will trigger this callback, and try to catch the Msg
    void OnChatMsgRecv(LobbyChatMsg_t result)
    {
        Debug.Log("Msg Recv from " + result.m_ulSteamIDUser);
        EChatEntryType peChatEntryType;
        CSteamID IDLobby = new CSteamID(result.m_ulSteamIDLobby);
        CSteamID whoSent;
        int msgSize = 50 * sizeof(char);
        byte[] msgByte =  new byte[msgSize];
        int number = SteamMatchmaking.GetLobbyChatEntry(IDLobby, (int) result.m_iChatID, out whoSent, msgByte, msgByte.Length + 1, out peChatEntryType);


        // convert to string
        char[] chars = new char[msgSize / sizeof(char)];
        Buffer.BlockCopy(msgByte, 0, chars, 0, number);
        string message = new string(chars, 0, chars.Length);

        Debug.Log("Received a message: " + message);

        SendMessageToChat(message);

    }

    //  Send the Msg we recieved from method OnChatMsgRecv()
    public void SendMessageToChat(string content) {
        if (messageList.Count > maxMessage)
        {
            Destroy(messageList[0].textObj.gameObject);
            messageList.Remove(messageList[0]);
        }
        GameObject newTextObj = Instantiate(textObj, chatPanel.transform);
        Message message = new Message(content,newTextObj.GetComponent<Text>());
        messageList.Add(message);
    }

}

public class Message
{
    public Message(string content,Text textObj)
    {
        this.text = content;
        this.textObj = textObj;
        this.textObj.text = content;
    }
    public string text;
    public Text textObj;
}
