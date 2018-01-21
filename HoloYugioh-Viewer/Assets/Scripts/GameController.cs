using UnityEngine;

using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Networking;
using System.Threading;

public struct Constants
{

    //game attributes
    public const string Game_State = "Game State";
    public const string Players = "Players";

    //game states
    public const string Idle = "Idle";
    public const string Reset = "Reset";

    //player names
    public const string Player1 = "Player1";
    public const string Player2 = "Player2";

    //player types
    public const string Life_Points = "Life Points";
    public const string Name = "Name";
    public const string Field = "Field";

    //card types
    public const string Monster = "Monster";
    public const string Spell_Trap = "Spell-Trap";

}

public struct FBData
{
    public bool ready;
    public string gameAttribute;
    public string playerName;
    public string playerAttribute;
    public string cardType;
    public string cardNumber;
    public JSONObject data;
    public string path;
}

public class GameController : MonoBehaviour
{
    //Instance
    public static GameController Instance;

    //Audio Components
    private static AudioSource audioPlayer;

    //Game components;
    public static string currentGame;

    //Firebase info
    FBData currData;

    //Quitting App
    CancellationTokenSource gameTokenSource;

    void Start()
    {
        //initializations
        audioPlayer = gameObject.AddComponent<AudioSource>();
        currentGame = "Game";
        gameTokenSource = new CancellationTokenSource();
               
        //enable firebase
        FirebaseDatabase firebaseDB = new FirebaseDatabase();
        firebaseDB.Changed += Firebase_Changed;
        firebaseDB.AddObserver(gameTokenSource.Token);

		
        Instance = this;
    }



    void Update()
    {
        if (currData.ready)
        {
            var obj = transform.Find(currData.path);
            if(obj != null)
            {
                Debug.Log(currData.path + ": " + currData.data);

                obj.gameObject.SendMessage("Activate", currData);
            }
            currData = default(FBData);
        }
    }

    public void StartGame(string game)
    {
        currentGame = game;
        PlayAudio("Activate Duel Disk");
        StartCoroutine(GetGameInfo());

    }

    IEnumerator GetGameInfo()
    {
        using (UnityWebRequest www = UnityWebRequest.Get("https://holoyugioh.firebaseio.com/" + currentGame + "/.json"))
        {
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                // Show results as text
                var json = www.downloadHandler.text;
                var fbdata = Utilities.JSONUtlities.ReturnPaths(json);
                foreach(var data in fbdata) {
                   // Debug.Log(data.path + ": " + data.data);
                    var obj = transform.Find(data.path);
                    if (obj != null)
                    {
                        obj.gameObject.SendMessage("Activate", data);
                    }
                }
            }
        }
    }

    public void Reset()
    {   
        foreach(Transform child in transform.Find("Players"))
        {
            foreach(Transform card in child.Find("Field/Monster"))
            {
                card.gameObject.SendMessage("Reset");
            }

            foreach (Transform card in child.Find("Field/Spell-Trap"))
            {
                card.gameObject.SendMessage("Reset");
            }

            child.Find("Life Points").SendMessage("Reset");
        }
    }

    private void Firebase_Changed(object sender, GenericArgs e)
    {
        if (!e.data.IsString && !e.data.IsNumber)
        {
            return;
        }
        var path_raw = sender.ToString();
        var path = path_raw.Split('/');
        if(path[0].Equals(currentGame))
        {
            //Field Spell Code
            currData.gameAttribute = path[1];
            int pathLength = path.Length;
            if(currData.gameAttribute.Equals(Constants.Players))
            {
                currData.playerName = path[2];
                currData.playerAttribute = path[3];
                if(currData.playerAttribute.Equals(Constants.Field))
                {
                    pathLength--;
                    currData.cardType = path[4];
                    currData.cardNumber = path[5];
                }
            }
           
            currData.data = e.data;
            var newpath = new string[pathLength - 1];
            for(int i = 1; i < pathLength; i++)
            {
                newpath[i - 1] = path[i];
            }
            currData.path = string.Join("/",newpath);
            currData.ready = true;
        }
    }

    public static void PlayAudio(string effect)
    {
        if (audioPlayer != null) audioPlayer.PlayOneShot((AudioClip)Resources.Load("Sounds" + "/" +  effect));
    }

    void OnApplicationQuit() 
    {
        gameTokenSource.Cancel();
    }
}