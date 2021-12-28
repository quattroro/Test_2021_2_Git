using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance = null;


    public GameObject HPbar;

    public Image GunInfo;
    public Text GunInfo_Text;

    public Image GrenadeInfo;
    public Text Grenade_Text;


    public void UpdataHP(float HPpercent)
    {
        HPbar.transform.localScale = new Vector3(HPpercent, 1, 1);
    }

    public void UpdataGunImage(GunData.GunType type)
    {
        GunInfo.GetComponent<Image>().sprite = Resources.Load<Sprite>($"PNG/{type.ToString()}Image");
    }

    public void UpdataGunBulletInfo(int num)
    {
        GunInfo_Text.GetComponent<Text>().text = $"{num}";
    }

    public void UpdataGrenadeInfo(int num)
    {
        Grenade_Text.GetComponent<Text>().text = $"{num}";
    }


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            if(instance!=this)
            {
                Destroy(this.gameObject);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //HPbar = GameObject.Find("HPBar");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
