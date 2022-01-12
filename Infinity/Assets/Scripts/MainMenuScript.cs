using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuScript : MonoBehaviour
{
    public enum CHARACTER { C1, C2,CMAX };

    public Sprite[] Selected;
    public Sprite[] NonSeleted;

    public Button[] Select;
    public int NowSelect;


    public void GameStart()
    {
        UserData userdata = new UserData();
        userdata.CharacterSelectNum = NowSelect;

        string data = JsonUtility.ToJson(userdata);
        File.WriteAllText(Application.dataPath + "/UserData/userdata.json", data);

        Debug.Log("ToJson : " + data);
        SceneManager.LoadScene(1);
    }

    public void CharacterSelect(int type)
    {
        NowSelect = type;
        Select[type].GetComponent<Image>().sprite = Selected[type];
        Select[(type + 1) % (int)CHARACTER.CMAX].GetComponent<Image>().sprite = NonSeleted[(type + 1) % (int)CHARACTER.CMAX];
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
