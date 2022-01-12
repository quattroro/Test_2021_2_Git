using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


public class GameManager : MonoBehaviour
{
    

    




    public void Initsetting()
    {
        UserData userdata = new UserData();
        string data = File.ReadAllText(Application.dataPath + "/UserData/userdata.json");
        userdata = JsonUtility.FromJson<UserData>(data);

        GameObject.Find("Player").GetComponent<CharacterScript>().PlayerSetting(userdata.CharacterSelectNum);

    }



    // Start is called before the first frame update
    void Start()
    {
        Initsetting();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
