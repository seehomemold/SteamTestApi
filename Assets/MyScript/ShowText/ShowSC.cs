using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowSC : MonoBehaviour
{
    public GameObject Manager;

    ServerClient player;
    Text text;
    // Start is called before the first frame update
    void Start()
    {
        player = Manager.GetComponent<ServerClient>();
        text = gameObject.GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((int)player.player == 1)
        {
            text.text = "AsClient";
        }
        else if ((int)player.player == 2)
        {
            text.text = "AsServer";
        }
        else
        {
            text.text = "There is something error";
        }
    }
}
