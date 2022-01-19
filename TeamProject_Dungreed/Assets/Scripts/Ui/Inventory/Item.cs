using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item : MonoBehaviour
{

    public GameObject target;
    public int itemcode;

    public enum Item_Detailed_types
    {
        shield,  //방패
        Supplementary_weapon,  //보조 무기
        One_handed_weapon,  //한손무기
        Two_handed_weapon,  //두손 무기

    }
    public enum ItemType
    {
        Equipment,
        Auxiliary_equipment,  //보조 장비
        Consumption_item,  //소비 아이템
        Weapon,  //무기
        Accessory,  //장신구

    }

    public string itemName;
    public ItemType itemType;
    public Item_Detailed_types item_Detailed;
    public Sprite itemImage;
    public GameObject itemPrefab;

    public string weaponType;


  
}
