using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CharacterSelectMenuScript : MonoBehaviour
{
    public Image[] characterimage;
    public Vector2[] pos;
    public int NowSelect = -1;//0~7
    public int MaxIndex = -1;//사용하는 이미지 개수 캐릭터들 + 랜덤

    public void Next()
    {
        NowSelect = (NowSelect - 1 < 0) ? MaxIndex - 1 : NowSelect - 1;


        Image temp = characterimage[MaxIndex - 1];
        for (int i = MaxIndex - 2; i > -1; i--)
        {
            characterimage[i + 1] = characterimage[i];
            //Vector2 temppos = characterimage[next].transform.position;
            //characterimage[i].transform.position = temppos;
        }
        characterimage[0] = temp;

        for (int i = 0; i < MaxIndex; i++)
        {
            characterimage[i].transform.position = pos[i];
        }

        //int next = (i + 1) % MaxIndex;
        //characterimage[i].transform.position = pos[next];
    }

    public void Back()
    {
        NowSelect = (NowSelect + 1) % MaxIndex;
        Image temp = characterimage[0];
        for (int i = 1; i < MaxIndex; i++)
        {
            characterimage[i - 1] = characterimage[i];
            //Vector2 temppos = characterimage[next].transform.position;
            //characterimage[i].transform.position = temppos;
        }
        characterimage[MaxIndex - 1] = temp;

        for (int i = 0; i < MaxIndex; i++)
        {
            characterimage[i].transform.position = pos[i];
        }

        //NowSelect = (NowSelect - 1);
        //if (NowSelect < 0)
        //{
        //    NowSelect = MaxIndex - 1;
        //}
        //for (int i = 0; i < MaxIndex; i++)
        //{
        //    int next = (i - 1 < 0) ? MaxIndex - 1 : i - 1;
        //    characterimage[i].transform.position = pos[next];
        //    //Vector2 temppos = characterimage[next].transform.position;
        //    //characterimage[i].transform.position = temppos;
        //}
    }


    public void InitSetting()
    {
        characterimage = new Image[(int)CharacterStatus.Characters.CMax + 1];
        pos = new Vector2[(int)CharacterStatus.Characters.CMax + 1];
        Image[] temp = GetComponentsInChildren<Image>();
        foreach(var a in temp)
        {
            if(a.name=="Random")
            {
                characterimage[MaxIndex] = a;
                pos[MaxIndex] = a.transform.position;
            }
            for (CharacterStatus.Characters i = CharacterStatus.Characters.Isaac; i < CharacterStatus.Characters.CMax; i++)
            {
                if(a.name==i.ToString())
                {
                    characterimage[(int)i] = a;
                    pos[(int)i] = a.transform.position;
                }
            }
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        InitSetting();
        MaxIndex = (int)CharacterStatus.Characters.CMax + 1;
        NowSelect= (int)CharacterStatus.Characters.CMax;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.RightArrow))
        {
            Next();
        }
        if(Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Back();
        }


    }
}
