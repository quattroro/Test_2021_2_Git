using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    public GunData.GunType ItemType;

    public float MaxFloat;
    public float MinFloat;

    public float FloatSpeed;

    public bool flag;

    public MeshRenderer meshrenderer;
    public void ItemSet(GunData.GunType itemtype)
    {
        this.ItemType = itemtype;
        this.meshrenderer.material = Resources.Load<Material>($"Material/{itemtype.ToString()}");
    }



    private void OnTriggerEnter(Collider other)
    {
        GunData.GunType temptype;
        if (other.tag=="Player")
        {
            Debug.Log($"아이템 플레이어 충돌{ItemType.ToString()}");
            temptype = other.GetComponent<Test3dMove>().playerGun.gundata.type;
            other.GetComponent<Test3dMove>().playerGun.LoadGunData(temptype);
            Destroy(this.gameObject);
        }
    }

    

    IEnumerator ItemMove()
    {
        Vector3 temp;
        while(true)
        {
            if(flag)
            {

                ///temp = new Vector3(transform.position.x, MaxFloat, transform.position.z);
                //this.transform.position = Vector3.Lerp(this.transform.position, temp, 0.3f);

                temp = new Vector3(transform.position.x, transform.position.y + FloatSpeed, transform.position.z);
                this.transform.position = temp;
                //Debug.Log($"아이템 올라감{transform.position.y}");
                if (this.transform.position.y>=MaxFloat)
                {
                    flag = false;
                }
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                
                //temp = new Vector3(transform.position.x, MinFloat, transform.position.z);
                //this.transform.position = Vector3.Lerp(this.transform.position, temp, 0.3f);
                temp = new Vector3(transform.position.x, transform.position.y - FloatSpeed, transform.position.z);
                this.transform.position = temp;
                //Debug.Log($"아이템 내려감 {transform.position.y}");
                if (this.transform.position.y <= MinFloat)
                {
                    flag = true;
                }
                yield return new WaitForSeconds(0.1f);
            }

        }


    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("ItemMove");
    }

    // Update is called once per frame
    void Update()
    {
       
    }
}
