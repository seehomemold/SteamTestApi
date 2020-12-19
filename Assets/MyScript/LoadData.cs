using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadData : MonoBehaviour
{
    string myCardRecord;
    string rivalCardRecord;

    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("SavedMyCard"))
        {
            myCardRecord = PlayerPrefs.GetString("SavedMyCard");
            rivalCardRecord = PlayerPrefs.GetString("SavedRivalCard");
            Debug.Log("Last My Card Seqeunce is " + myCardRecord);
            Debug.Log("Last My Rival Card Seqeunce is " + rivalCardRecord);
            Debug.Log("Game data loaded!");
        }
        else
            Debug.Log("There is no save data!");
    }
}
