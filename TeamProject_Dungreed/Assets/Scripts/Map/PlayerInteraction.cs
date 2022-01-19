using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//�÷��̾�� ��ȣ�ۿ밡���� ������Ʈ�鿡 �ٿ��� ���
//��ȣ�ۿ� ������ ������ boxcollider �ʿ�
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

    //�ֺ��� �÷��̾ �������� f�� ������ ������ action�� �����Ѵ�.
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
                //Debug.Log("�÷��̾� ����!");
            }
            else
            {
                current.PlayerEnter = false;
               // Debug.Log("�÷��̾� ���� ����!");
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
