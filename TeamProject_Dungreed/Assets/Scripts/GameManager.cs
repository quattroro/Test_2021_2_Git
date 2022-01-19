using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    ObjectManager _objManager = new ObjectManager();
    ResourceManager _resourceManager = new ResourceManager();

    public static ObjectManager ObjManager { get { return Instance._objManager; } }
    public static ResourceManager Resource {  get { return Instance._resourceManager; } }

    [SerializeField]
    private Image[] ScreenBox; // 보스 등장시 위아래 검은색 박스
    [SerializeField]
    private Text[] BossName_Text; // 보스 이름, 별칭 텍스트

    private Boss cur_Boss;

    static void Init()
    {
        Instance._objManager.Init();
    }

    public static void Clear()
    {
        //ObjManager.Clear();
    }

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Boss_Start = true;
        cur_Boss = GameObject.FindGameObjectWithTag("Boss").GetComponent<Boss>();
    }

    public bool Boss_Start = false;
    void Screen_Effect()
    {
        if (!Boss_Start)
            return;

        foreach (Image img in ScreenBox)
        {
            Color alpha = img.color;
            alpha.a += Time.deltaTime * 1f;
            img.color = alpha;

            if (img.color.a >= 1)
            {
                Boss_Start = false;
            }
        }

        if (!Boss_Start)
        {
            StartCoroutine(Boss_Name_Coroutine());
            Debug.Log("실행");
        }
    }

    int name_Count = 0;
    IEnumerator Boss_Name_Coroutine()
    {
        if (name_Count < 2)
        {
            while (true)
            {
                Color alpha = BossName_Text[name_Count].color;
                alpha.a += Time.deltaTime * 1f;
                BossName_Text[name_Count].color = alpha;

                if (BossName_Text[name_Count].color.a >= 1)
                {
                    name_Count++;
                    break;
                }
                yield return null;
            }
        }

        yield return new WaitForSeconds(0.25f);

        if (name_Count < 2)
        {
            StartCoroutine(Boss_Name_Coroutine());
        }
        else
        {
            yield return new WaitForSeconds(1f);

            bool endflag = false;
            while (true)
            {
                yield return null;

                for (int i = 0; i < 2; i++)
                {
                    Color alphaA = ScreenBox[i].color;
                    Color alphaB = BossName_Text[i].color;

                    alphaA.a -= Time.deltaTime * 1f;
                    alphaB.a -= Time.deltaTime * 1f;

                    ScreenBox[i].color = alphaA;
                    BossName_Text[i].color = alphaB;

                    if (alphaA.a <= 0)
                    {
                        endflag = true;
                    }
                }

                if (endflag)
                {
                    StartCoroutine(cur_Boss.Pattern_Coroutine());
                    break;
                }
            }
        }
    }

    void Update()
    {
        Screen_Effect();
    }
}
