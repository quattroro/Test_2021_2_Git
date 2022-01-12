using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


//base
public class Door : MonoBehaviour
{
    public enum DoorType { UP, DOWN, RIGHT, LEFT };

    public GameObject[] DoorPrefabs;

    public Tilemap WallTilemap;
    public Tilemap BackTilemap;
    public Tilemap MoveableTilemap;


    public Tilemap MainWallTilemap;
    public Tilemap MainBackTilemap;
    public Tilemap MainMoveableTilemap;

    public Tile tile;

    public Transform StartPos;
    public Transform EndPos;

    public Vector3Int StartIndex;
    public Vector3Int EndIndex;
    
    public Vector2Int size;

    public GameObject NextRoom;


    //문이 생성될 타일맵의 인덱스번호를 넘겨주면 해당 인덱스부터 문을 생성한다.
    public void CreateDoor(GameObject tilemaps, int i_x, int i_y, DoorType type)
    {
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
            else if(a.gameObject.layer == LayerMask.NameToLayer("Moveable"))
            {
                MoveableTilemap = a;
            }
        }
        GetTileInfo(type);

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                Vector3Int index = new Vector3Int(x, y, 0);
                index = StartIndex + index;
                TileBase tile = WallTilemap.GetTile(index);
                MainWallTilemap.SetTile(index, tile);
                tile = BackTilemap.GetTile(index);
                MainBackTilemap.SetTile(index, tile);
            }
        }

        BaseStage stagedata = tilemaps.GetComponent<BaseStage>();

        //if (type == DoorType.DOWN) NextRoom = stagedata.StageLinkedData.DownMap;
            //if (type == DoorType.DOWN)
               // if (type == DoorType.DOWN)
                    //if (type == DoorType.DOWN)
    }




    //문 타입에 따른 정보들을 받아서 준다. 가로세로 크기, 정보가 있는 셀의 시작 인덱스 번호 등등
    public void GetTileInfo(DoorType type)
    {
        Tilemap[] temp = DoorPrefabs[(int)type].GetComponentsInChildren<Tilemap>();
        foreach(var a in temp)
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

        StartPos = DoorPrefabs[(int)type].transform.Find("StartPos");
        EndPos= DoorPrefabs[(int)type].transform.Find("EndPos");

        StartIndex = WallTilemap.WorldToCell(StartPos.position);
        EndIndex= WallTilemap.WorldToCell(EndPos.position);

        size.x = EndIndex.x - StartIndex.x + 1;
        size.y = EndIndex.y - StartIndex.y + 1;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
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
