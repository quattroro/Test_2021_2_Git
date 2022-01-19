using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//플레이어와 상호작용가능한 오브젝트들에 붙여서 사용
//상호작용 범위를 지정할 boxcollider 필요
public class PlayerInteraction : MonoBehaviour
{
    public LayerMask PlayerLayer;

    [System.Serializable]
    public class CurrentValues
    {
        public bool PlayerEnter;
        public float CheckSecond;
    }


    //public CircleCollider2D range;
    public BoxCollider2D range;
    public float lastTime;
    public CurrentValues current = new CurrentValues();
    public delegate void Action();
    public Action action;

    //주변에 플레이어가 있을때만 f를 눌르면 설정된 action을 실행한다.
    public void AddAction(Action action)
    {
        this.action += action;
    }

    public void DeleteAction(Action action)
    {
        this.action -= action;
    }

    public void CheckPlayer()
    {
        
        if (Time.time >= lastTime + current.CheckSecond)
        {
            lastTime = Time.time;
            RaycastHit2D hit = Physics2D.BoxCast(this.transform.position, range.size, 0, new Vector2(1, 1), 0, PlayerLayer);
            //RaycastHit2D hit = Physics2D.CircleCast(this.transform.position, range.radius, new Vector2(1, 1), 0, PlayerLayer);
            if(hit)
            {
                current.PlayerEnter = true;
                //Debug.Log("플레이어 감지!");
            }
            else
            {
                current.PlayerEnter = false;
               // Debug.Log("플레이어 감지 못함!");
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        CheckPlayer();
        if(current.PlayerEnter)
        {
            if(Input.GetKeyDown(KeyCode.F))
            {
                if (action != null)
                {
                    action();
                }
            }
        }
    }
}
