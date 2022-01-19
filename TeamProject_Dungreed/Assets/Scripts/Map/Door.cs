using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//base
public class Door : MonoBehaviour
{
    public enum DoorType { Up, Down, Right, Left, DoorMax };

    //public GameObject[] DoorPrefabs;

    public Tilemap WallTilemap;
    public Tilemap BackTilemap;
    public Tilemap MoveableTilemap;


    public Tilemap MainWallTilemap;
    public Tilemap MainBackTilemap;
    public Tilemap MainMoveableTilemap;

    public Tile tile;

    public Transform StartPos;
    public Transform EndPos;

    public Vector3Int DoorStartIndex;
    public Vector3Int DoorEndIndex;

    public Vector3Int MainStartIndex;
    public Vector3Int MainEndIndex;


    public Vector2Int size;

    public GameObject NextRoom;

    public BoxCollider2D coll;
    public LayerMask PlayerMask;
    public DoorType type;

    public BoxCollider2D collision;
    public GameObject ParentStage;
    public Transform TeleportPos;


    //���� ������ Ÿ�ϸ��� �ε�����ȣ�� �Ѱ��ָ� �ش� �ε������� ���� �����Ѵ�.
    //public void CreateDoor(GameObject tilemaps, int i_x, int i_y, DoorType type)
    //{
    //    Tilemap[] temp = tilemaps.GetComponentsInChildren<Tilemap>();
    //    foreach (var a in temp)
    //    {
    //        if (a.gameObject.layer == LayerMask.NameToLayer("Wall"))
    //        {
    //            MainWallTilemap = a;
    //        }
    //        else if (a.gameObject.layer == LayerMask.NameToLayer("BackGround"))
    //        {
    //            MainBackTilemap = a;
    //        }
    //        else if(a.gameObject.layer == LayerMask.NameToLayer("Moveable"))
    //        {
    //            MainMoveableTilemap = a;
    //        }
    //    }
    //    GetTileInfo(type);

    //    for (int y = 0; y < size.y; y++)
    //    {
    //        for (int x = 0; x < size.x; x++)
    //        {
    //            Vector3Int index = new Vector3Int(x, y, 0);
    //            //index = StartIndex + index;
    //            TileBase tile = WallTilemap.GetTile(index);
    //            MainWallTilemap.SetTile(index, tile);
    //            tile = BackTilemap.GetTile(index);
    //            MainBackTilemap.SetTile(index, tile);
    //        }
    //    }

    //    BaseStage stagedata = tilemaps.GetComponent<BaseStage>();

    //    //if (type == DoorType.DOWN) NextRoom = stagedata.StageLinkedData.DownMap;
    //        //if (type == DoorType.DOWN)
    //           // if (type == DoorType.DOWN)
    //                //if (type == DoorType.DOWN)
    //}

    //���� ������ Ÿ�ϸ��� �ε�����ȣ�� �Ѱ��ָ� �ش� �ε������� ���� �����Ѵ�.
    public void CreateDoor(GameObject tilemaps)
    {
        ParentStage = tilemaps;
        Tilemap[] temp = tilemaps.GetComponentsInChildren<Tilemap>();
        foreach (var a in temp)
        {
            if (a.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                MainWallTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("BackGround"))
            {
                MainBackTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("Moveable"))
            {
                MainMoveableTilemap = a;
            }
        }

        if(this.type==DoorType.Up)
        {
            string name = tilemaps.GetComponent<BaseStage>().StageLinkedData.UpMap.gameObject.name;
            NextRoom = GameObject.Find(name);
        }
        else if(this.type == DoorType.Down)
        {
            //NextRoom = tilemaps.GetComponent<BaseStage>().StageLinkedData.DownMap;
            string name = tilemaps.GetComponent<BaseStage>().StageLinkedData.DownMap.gameObject.name;
            NextRoom = GameObject.Find(name);
        }
        else if (this.type == DoorType.Right)
        {
            //NextRoom = tilemaps.GetComponent<BaseStage>().StageLinkedData.RightMap;
            string name = tilemaps.GetComponent<BaseStage>().StageLinkedData.RightMap.gameObject.name;
            NextRoom = GameObject.Find(name);
        }
        else if (this.type == DoorType.Left)
        {
            //NextRoom = tilemaps.GetComponent<BaseStage>().StageLinkedData.LeftMap;
            string name = tilemaps.GetComponent<BaseStage>().StageLinkedData.LeftMap.gameObject.name;
            NextRoom = GameObject.Find(name);
        }

        GetTileInfo(type);

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3Int index = new Vector3Int(x, y * -1, 0);
                index = MainStartIndex + index;

                //TileBase tile = WallTilemap.GetTile(index);
                MainWallTilemap.SetTile(index, null);

                //tile = BackTilemap.GetTile(index);
                MainBackTilemap.SetTile(index, null);

                if(MoveableTilemap!=null)
                {
                    //tile = MoveableTilemap.GetTile(index);
                    MainMoveableTilemap.SetTile(index, null);
                }
            }
        }

        BaseStage stagedata = tilemaps.GetComponent<BaseStage>();

        //if (type == DoorType.DOWN) NextRoom = stagedata.StageLinkedData.DownMap;
        //if (type == DoorType.DOWN)
        // if (type == DoorType.DOWN)
        //if (type == DoorType.DOWN)
    }


    //�� Ÿ�Կ� ���� �������� �޾Ƽ� �ش�. ���μ��� ũ��, ������ �ִ� ���� ���� �ε��� ��ȣ ���
    public void GetTileInfo(DoorType type)
    {
        //Tilemap[] temp = DoorPrefabs[(int)type].GetComponentsInChildren<Tilemap>();
        Tilemap[] temp = GetComponentsInChildren<Tilemap>();//�ڱ� �ڽ��� Ÿ�ϸ� ������ �޾ƿ´�.
        foreach (var a in temp)
        {
            if (a.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                WallTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("BackGround"))
            {
                BackTilemap = a;
            }
            else if (a.gameObject.layer == LayerMask.NameToLayer("Moveable"))
            {
                MoveableTilemap = a;
            }
        }

        StartPos = transform.Find("StartPos");
        EndPos= transform.Find("EndPos");

        DoorStartIndex = WallTilemap.WorldToCell(StartPos.position);
        DoorEndIndex= WallTilemap.WorldToCell(EndPos.position);
        
        MainStartIndex=MainWallTilemap.WorldToCell(StartPos.position);
        MainEndIndex = MainWallTilemap.WorldToCell(EndPos.position);

        size.x = DoorEndIndex.x - DoorStartIndex.x + 1;
        size.y = DoorStartIndex.y - DoorEndIndex.y + 1;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("�ݸ��� ����");


    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Ʈ���� ����");
        if(collision.gameObject.layer==LayerMask.NameToLayer("Player"))
        {
            Debug.Log("NameToLayer ����");
            GoNextMap(collision.gameObject);
        }
        //if(collision.gameObject.layer == PlayerMask)
        //{
        //    Debug.Log("GoNextMap ����");
            
        //}
    }


    public void GoNextMap(GameObject Player)
    {
        Door pos = null;
        GameObject temp = null;
        if (this.type==DoorType.Up)
        {
            //string name = ParentStage.GetComponent<BaseStage>().StageLinkedData.UpMap.gameObject.name;
            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.UpMap;
            string name = temp.gameObject.name;

            NextRoom = GameObject.Find(name);
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Down];
        }
        else if (this.type == DoorType.Down)
        {
            //string name = ParentStage.GetComponent<BaseStage>().StageLinkedData.DownMap.gameObject.name;

            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.DownMap;
            string name = temp.gameObject.name;

            NextRoom = GameObject.Find(name);
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Up];
        }
        else if (this.type == DoorType.Right)
        {
            //string name = ParentStage.GetComponent<BaseStage>().StageLinkedData.RightMap.gameObject.name;

            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.RightMap;
            string name = temp.gameObject.name;

            NextRoom = GameObject.Find(name);
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Left];
        }
        else if (this.type == DoorType.Left)
        {
            //string name = ParentStage.GetComponent<BaseStage>().StageLinkedData.LeftMap.gameObject.name;

            temp = ParentStage.GetComponent<BaseStage>().StageLinkedData.LeftMap;
            string name = temp.name;

            NextRoom = GameObject.Find(name);
            pos = NextRoom.GetComponent<BaseStage>().door[(int)DoorType.Right];
        }
        Player.transform.position = pos.TeleportPos.position;
    }

    // Start is called before the first frame update
    void Start()
    {
        //DoorPrefabs = new GameObject[(int)(DoorType.Left + 1)];
        //for(DoorType i=DoorType.Up;i<=DoorType.Left;i++)
        //{
        //    DoorPrefabs[(int)i] = Resources.Load<GameObject>($"Prefabs/DoorPrefabs/{i.ToString()}Door");
        //}
        WallTilemap = null;
        BackTilemap = null;
        MoveableTilemap = null;


        MainWallTilemap = null;
        MainBackTilemap = null;
        MainMoveableTilemap = null;



    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
