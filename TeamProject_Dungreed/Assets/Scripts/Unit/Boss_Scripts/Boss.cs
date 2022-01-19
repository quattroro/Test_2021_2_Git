using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Unit
{
    private Animator anim;

    // 보스 손
    [SerializeField]
    private GameObject[] boss_Hand = null; // 0. left hand , 1.right hand
    [SerializeField]
    private GameObject boss_Sword_Prefab = null;
    [SerializeField]
    private GameObject die_Effect_Prefab = null;
    [SerializeField]
    private GameObject die_Head = null;
    [SerializeField]
    private GameObject die_Mouth = null;
    [SerializeField]
    private Transform[] sword_Pos;
    [SerializeField]
    private Transform[] die_Direction;
    [SerializeField]
    private Transform bullet_Pos;
    [SerializeField]
    private Material damaged_Material;

    private GameObject[] boss_Sword = new GameObject[6];
    [SerializeField]
    private int boss_Pattern = 0; // 보스 패턴
                                  // 1. 레이저 한번, 2. 레이저 3번, 3. 칼 발사, 4. 나선형 구체 발사

    void Start()
    {
        Init(); // 생성 시 보스 Status 초기화

        anim = GetComponent<Animator>();

        //StartCoroutine(ShootBullet_Coroutine());
    }

    void Init()
    {
        Hp = 100;
        Damage = 100;

        for (int i = 0; i < 6; i++)
        {
            boss_Sword[i] = Instantiate(boss_Sword_Prefab);
            boss_Sword[i].transform.position = sword_Pos[i].position;
            boss_Sword[i].SetActive(false);
        }
    }

    public IEnumerator Pattern_Coroutine() // 랜덤 패턴 코루틴
    {
        yield return new WaitForSeconds(3f);

        if (Hp > 0) // 살아있으면 호출
        {
            boss_Pattern = Random.Range(0,1);
            Attack();
        }
    }

    int laserDir = 0;
    int laserCount = 0;
    IEnumerator Triple_Laser()
    {
        while (true)
        {
            if (laserCount == 3)
            {
                StartCoroutine(Pattern_Coroutine());
                laserCount = 0;
                break;
            }

            laserCount++;
            int dir = laserDir++ % 2;
            Shoot_Laser(dir);

            yield return new WaitForSeconds(1f);
        }
    }

    protected override void Attack()
    {
        switch (boss_Pattern)
        {
            case 1:
                int randDir = Random.Range(0, 2);
                Shoot_Laser(randDir);
                StartCoroutine(Pattern_Coroutine());
                break;
            case 2:
                StartCoroutine(Triple_Laser());
                break;
            case 3:
                StartCoroutine(CreateSword_Coroutine(0));
                break;
            case 4:
                StartCoroutine(ShootBullet_Coroutine());
                break;
        }
    }

    protected override void Damaged()
    {
        StartCoroutine(Damaged_Coroutine());
    }

    IEnumerator Damaged_Coroutine()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();

        Material originalMat = renderer.material;

        renderer.material = damaged_Material;

        yield return new WaitForSeconds(0.1f);

        renderer.material = originalMat;
    }

    protected override void Move()
    {
        throw new System.NotImplementedException();
    }

    void Shoot_Laser(int dir)
    {
        GameObject hand = boss_Hand[dir];

        hand.GetComponent<Animator>().SetBool("isAttack", true);
        StartCoroutine(hand.GetComponent<Boss_Hand>().Anim_Coroutine());
    }

    IEnumerator CreateSword_Coroutine(int index)
    {
        while (index < 6)
        {
            yield return new WaitForSeconds(0.2f);

            Create_Sword(index++);
        }

        StartCoroutine(ShootSword_Coroutine(0));
    }

    IEnumerator ShootSword_Coroutine(int index)
    {
        while (index < 6)
        {
            yield return new WaitForSeconds(0.2f);

            Shoot_Sword(index++);
        }

        StartCoroutine(Pattern_Coroutine());

        yield return new WaitForSeconds(1.5f);

        StartCoroutine(ReturnSword_Coroutine(0));
    }

    IEnumerator ReturnSword_Coroutine(int index)
    {
        while (index < 6)
        {
            yield return new WaitForSeconds(0.2f);

            Return_Sword(index++);
        }
    }

    void Create_Sword(int index)
    {
        Boss_Sword sword = boss_Sword[index].GetComponent<Boss_Sword>();

        sword.Sword_Init();
        boss_Sword[index].SetActive(true);
        boss_Sword[index].transform.position = sword_Pos[index].position;
    }

    void Shoot_Sword(int index)
    {
        Boss_Sword sword = boss_Sword[index].GetComponent<Boss_Sword>();
        sword.targetVec = sword.transform.position - sword.target.transform.position;
        sword.shootStart = true;
    }

    void Return_Sword(int index)
    {
        boss_Sword[index].GetComponent<Boss_Sword>().shootStart = false;
        boss_Sword[index].SetActive(false);
    }

    IEnumerator ShootBullet_Coroutine()
    {
        Vector3 newVec = new Vector3(0, 0.2f, 0);
        anim.SetBool("isAttack", true);
        transform.position += newVec;
        yield return new WaitForSeconds(0.5f);
        while (bulletCount < 50)
        {
            yield return new WaitForSeconds(0.1f);
            Shoot_Bullet();
        }

        StartCoroutine(Pattern_Coroutine());
        transform.position -= newVec;

        anim.SetBool("isAttack", false);
        bulletCount = 0;
    }

    int bulletCount = 0;
    void Shoot_Bullet()
    {
        GameObject obj1 = GameManager.Resource.Instantiate("BossBullet");
        GameObject obj2 = GameManager.Resource.Instantiate("BossBullet");
        GameObject obj3 = GameManager.Resource.Instantiate("BossBullet");
        GameObject obj4 = GameManager.Resource.Instantiate("BossBullet");

        #region transform
        obj1.transform.position = bullet_Pos.position;
        obj1.transform.rotation = this.transform.rotation;
        obj2.transform.position = bullet_Pos.position;
        obj2.transform.rotation = this.transform.rotation;
        obj3.transform.position = bullet_Pos.position;
        obj3.transform.rotation = this.transform.rotation;
        obj4.transform.position = bullet_Pos.position;
        obj4.transform.rotation = this.transform.rotation;
        #endregion

        Rigidbody2D rigid1 = obj1.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid2 = obj2.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid3 = obj3.GetComponent<Rigidbody2D>();
        Rigidbody2D rigid4 = obj4.GetComponent<Rigidbody2D>();

        Vector2 dirVec1 = new Vector2(-Mathf.Cos(Mathf.PI * bulletCount / 50), -Mathf.Sin(Mathf.PI * bulletCount / 50));
        Vector2 dirVec2 = new Vector2(Mathf.Sin(Mathf.PI * bulletCount / 50), -Mathf.Cos(Mathf.PI * bulletCount / 50));
        Vector2 dirVec3 = new Vector2(Mathf.Cos(Mathf.PI * bulletCount / 50), Mathf.Sin(Mathf.PI * bulletCount / 50));
        Vector2 dirVec4 = new Vector2(-Mathf.Sin(Mathf.PI * bulletCount / 50), Mathf.Cos(Mathf.PI * bulletCount / 50));

        rigid1.AddForce(dirVec1 * 5, ForceMode2D.Impulse);
        rigid2.AddForce(dirVec2 * 5, ForceMode2D.Impulse);
        rigid3.AddForce(dirVec3 * 5, ForceMode2D.Impulse);
        rigid4.AddForce(dirVec4 * 5, ForceMode2D.Impulse);

        bulletCount++;

        List<GameObject> bulletList = new List<GameObject>();

        bulletList.Add(obj1);
        bulletList.Add(obj2);
        bulletList.Add(obj3);
        bulletList.Add(obj4);

        StartCoroutine(Bullet_Off_Coroutine(bulletList));
    }

    IEnumerator Bullet_Off_Coroutine(List<GameObject> list)
    {
        yield return new WaitForSeconds(3f);

        foreach(GameObject obj in list)
        {
            GameManager.Resource.Destroy(obj);
        }
    }

    IEnumerator Die_Effect_Off_Coroutine(GameObject obj)
    {
        yield return new WaitForSeconds(1f);

        GameManager.Resource.Destroy(obj);
    }

    IEnumerator Die_Coroutine()
    {
        Time.timeScale = 0.5f;

        yield return new WaitForSeconds(2f);

        Time.timeScale = 1f;

        int Count = 0;
        int lastCount = 1;

        while(true)
        {
            for (int i = 0; i < 10; i++)
            {
                float randPosX = Random.Range(-5f, 5f);
                float randPosY = Random.Range(-4f, 4f);

                GameObject obj = GameManager.Resource.Instantiate("DieEffect");

                obj.transform.position = new Vector3(randPosX, randPosY, 0);

                StartCoroutine(Die_Effect_Off_Coroutine(obj));
            }
            yield return new WaitForSeconds(0.3f);

            if (++Count > 15)
                break;
        }


        StartCoroutine(Die_Head_Move_Coroutine());
        while (true)
        {
            foreach (Transform dir in die_Direction)
            {
                GameObject obj = GameManager.Resource.Instantiate("DieEffect");

                obj.transform.position = transform.position;

                Vector3 dirVec = dir.position - transform.position;

                obj.transform.position += dirVec.normalized * lastCount;

                StartCoroutine(Die_Effect_Off_Coroutine(obj));
            }
            yield return new WaitForSeconds(0.1f);
            if (++lastCount > 6)
                break;
        }
    }

    void Die()
    {
        StartCoroutine(Die_Coroutine());
    }

    IEnumerator Die_Head_Move_Coroutine()
    {
        bool dir = true;
        int count = 1;
        int max = 10;

        die_Head.SetActive(true);
        die_Mouth.SetActive(true);
        GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 0);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < max; i++)
        {
            die_Head.transform.rotation = Quaternion.Euler(0, 0, count++);
            yield return new WaitForSeconds(0.01f);
        }
        dir = !dir;

        max -= 1;

        yield return new WaitForSeconds(0.1f);
        float time = 0.1f;
        for (int i = 0; i < 8; i++)
        {
            for (int k = 0; k < max * 2; k++)
            {
                if (dir)
                    die_Head.transform.rotation = Quaternion.Euler(0, 0, count++);
                else
                    die_Head.transform.rotation = Quaternion.Euler(0, 0, count--);

                yield return new WaitForSeconds(0.01f);
            }
            yield return new WaitForSeconds(time);
            time -= 0.02f;
            max -= 1;
            dir = !dir;
        }

        for (int i = 0; i < max + 3; i++)
        {
            die_Head.transform.rotation = Quaternion.Euler(0, 0, count--);
            yield return new WaitForSeconds(0.01f);
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            Die();

        if (Input.GetKeyDown(KeyCode.F1))
            Damaged();

        if (Input.GetKeyDown(KeyCode.F2))
            StartCoroutine(Die_Head_Move_Coroutine());
    }
}
