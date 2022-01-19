using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustCreate : MonoBehaviour
{
    public GameObject Player;
    public GameObject Dust;
    
    public float DustDelay = 0.4f; //먼지날리는거 딜레이타임 에니메이션 0.4초라서 
    public float TestDelay = 3f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        CheckPlayerPosition();
    }

    void CheckPlayerPosition() //플레이어좌표를 받아서 움직이는 중이라면 거기다가 먼지생성.
    {
        if (!Player.GetComponent<Player>().isground)
        {
            return;
        }
        Vector2 Scale = Dust.transform.localScale;
        Vector2 Pos = Dust.transform.position;
        if (Player.GetComponent<Player>().walk_right )
        {
            Pos.x = Player.transform.position.x - 0.1f;
            Pos.y = Player.transform.position.y - 0.1f;
            Scale.x = 4;
            Dust.transform.localScale = Scale;
            Dust.transform.position = Pos;
            TestDelay += Time.deltaTime;
            if (TestDelay >= DustDelay)
            {
                GameObject copyobj = Instantiate(Dust);             
                Destroy(copyobj, DustDelay);
                TestDelay = 0f;
            }
        }
        if(Player.GetComponent<Player>().walk_left)
        {
            Pos.x = Player.transform.position.x + 0.1f;
            Pos.y = Player.transform.position.y - 0.1f;
            Scale.x = -4;
            Dust.transform.localScale = Scale;
            Dust.transform.position = Pos;
            TestDelay += Time.deltaTime;
            if (TestDelay >= DustDelay)
            {
                GameObject copyobj = Instantiate(Dust);             
                Destroy(copyobj, DustDelay);
                TestDelay = 0f;
            }
        }
        
    }
}
