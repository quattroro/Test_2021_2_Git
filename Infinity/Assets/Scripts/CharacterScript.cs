using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterScript : MonoBehaviour
{
    enum PlayerType { C1, C2, CMAX };

    public Animator[] PlayerAnimator;
    public Sprite[] PlayerSprite;

    public GameObject[] Characters;


    public void PlayerSetting(int type)
    {
        GameObject obj = GameObject.Instantiate(Characters[type]);
        obj.transform.parent = GameObject.Find("Player").transform;
        obj.transform.localPosition = new Vector3(0f, 0f, 0);
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
