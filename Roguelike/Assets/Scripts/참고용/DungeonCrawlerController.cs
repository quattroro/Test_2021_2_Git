﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCrawlerController : Singleton<DungeonCrawlerController>
{
    public List<Vector3Int> direction4 = new List<Vector3Int>
    {
        new Vector3Int( 0, 0,  1),       // down
        new Vector3Int( 1, 0,  0),       // right
        new Vector3Int(-1, 0,  0),       // left
        new Vector3Int( 0, 0, -1)        // up
    };
    public List<Vector3Int> direction8 = new List<Vector3Int>
    {   // Down                           // Up
        new Vector3Int( 0, 0,  1),        new Vector3Int( 0, 0, -1),        
        // Left                           // Right
        new Vector3Int(-1, 0,  0),        new Vector3Int( 1, 0,  0),        
        // UpLeft                         // UpRight
        new Vector3Int(-1, 0, -1),        new Vector3Int( 1, 0, -1),
        // DiwbLeft                       // DownRight
        new Vector3Int(-1, 0,  1),        new Vector3Int( 1, 0,  1)
    };
    public Dictionary<int, List<Vector3Int>> downPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, 1),   new Vector3Int(-1, 0, 1),   new Vector3Int(0, 0, 2),   new Vector3Int(-1, 0, 2) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, 1),   new Vector3Int(0, 0, 2),    new Vector3Int(-1, 0, 2)    } }, // ┓
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, 1),   new Vector3Int(0, 0, 2),    new Vector3Int(1, 0, 2)     } }, // ┏
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, 1),   new Vector3Int(0, 0, 2)                                 } }, // 아래 |
        {  4, new List<Vector3Int>      { new Vector3Int(0, 0, 1),   new Vector3Int(-1, 0, 1)                                } }, // 아래 |
        {  5, new List<Vector3Int>      { new Vector3Int(0, 0, 1),   new Vector3Int(1, 0, 1)                                 } }, // 아래 |
        {  6, new List<Vector3Int>      { new Vector3Int(0, 0, 1)                                                            } }, // 아래 |
    };
    public Dictionary<int, List<Vector3Int>> upPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(0, 0, -1),   new Vector3Int(0, 0, -2),  new Vector3Int(-1, 0, -1),  new Vector3Int(-1, 0, -2)} }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(0, 0, -1),   new Vector3Int(0, 0, -2),   new Vector3Int(1, 0, -2)    } }, // ┏
        {  2, new List<Vector3Int>      { new Vector3Int(0, 0, -1),   new Vector3Int(0, 0, -2),   new Vector3Int(-1, 0, -2)   } }, // ┐
        {  3, new List<Vector3Int>      { new Vector3Int(0, 0, -1),   new Vector3Int(0, 0, -2)                                } }, // 위 |
        {  4, new List<Vector3Int>      { new Vector3Int(0, 0, -1),   new Vector3Int(1, 0, -1)                                } }, // 위 |
        {  5, new List<Vector3Int>      { new Vector3Int(0, 0, -1),   new Vector3Int(-1, 0, -1)                               } }, // 위 |
        {  6, new List<Vector3Int>      { new Vector3Int(0, 0, -1)                                                            } }, // 위 |
    };
    public Dictionary<int, List<Vector3Int>> leftPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-1, 0, -1),  new Vector3Int(-2, 0, -1) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 0, -1)   } }, // └ 
        {  2, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0),   new Vector3Int(-2, 0, 1)    } }, // ┌
        {  3, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-2, 0, 0)                                } }, // 왼쪽  --
        {  4, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, 0, -1)                               } }, // 왼쪽  --
        {  5, new List<Vector3Int>      { new Vector3Int(-1, 0, 0),  new Vector3Int(-1, 0, 1)                                } }, // 왼쪽  --
        {  6, new List<Vector3Int>      { new Vector3Int(-1, 0, 0)                                                           } }, // 왼쪽  --
    };
    public Dictionary<int, List<Vector3Int>> rightPatten = new Dictionary<int, List<Vector3Int>>
    {
        {  0, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(1, 0, 1) ,   new Vector3Int(2, 0, 1) } }, // ㅁ
        {  1, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, 0, 1)     } }, // ┐
        {  2, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0),    new Vector3Int(2, 0, -1)    } }, // ┛ 
        {  3, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(2, 0, 0)                                 } }, // 오른쪽  --
        {  4, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, 0, 1)                                 } }, // 오른쪽  --
        {  5, new List<Vector3Int>      { new Vector3Int(1, 0, 0),   new Vector3Int(1, 0, -1)                                } }, // 오른쪽  --
        {  6, new List<Vector3Int>      { new Vector3Int(1, 0, 0)                                                            } },
    };

    public List<RoomInfo> mainRoot = new List<RoomInfo>();
    public List<Vector3Int> shortParentRoot = new List<Vector3Int>();

    public int minRoomCnt = 15;
    public int maxRoomCnt = 20;
    public int currRoomCnt = 0;
    public int maxDistance = 5;

    public int mainRoomCount = 0;

    public Vector3Int startPoint;
    public Vector3Int BossPoint;

    public RoomInfo[,] posArr = new RoomInfo[10, 10];

    public void CreatedRoom()
    {
        // 배열 ReSize
        posArr = (RoomInfo[,])ResizeArray(posArr, new int[] { (maxDistance * 2), (maxDistance * 2) });
        //초기화
        RealaseRoom();

        int x = Random.Range(0, maxDistance) + (int)(maxDistance / 2);
        int z = Random.Range(0, maxDistance) + (int)(maxDistance / 2);

        //시작방을 정해준다.
        startPoint = new Vector3Int(x, 0, z);
        
        //시작 방에 싱글방을 하나 만들어 준다.
        posArr[startPoint.z, startPoint.x] = singleRoom(new RoomInfo(), startPoint, "Single");
        posArr[startPoint.z, startPoint.x].weightCnt = 0;
        currRoomCnt++;

        //반복문으로 돌리면서 방들을 만들어 준다.
        while (true)
        {
            if (!(minRoomCnt <= currRoomCnt && currRoomCnt <= maxRoomCnt))
            {
                //매 반복마다 모든 만들어져 있는 방들을 돌면서 시작지점에서 얼마나 떨어져 있는지 확인한다.
                countRoomWeight(startPoint, startPoint);
                //매 반복마다 만들어진 모든 방을 메인 루트에 넣어준다.
                makeMainRoot();
                //매 반복마다 메인 루트를 소팅한다.
                mainRoomSort(mainRoot);
                //메인 루트 중에 하나를 뽑아준다.
                int Mainrand = Random.Range(0, mainRoot.Count - 1);
                //메인루트에서 랜덤으로 뽑아준 방의 인덱스를 받아와서
                Vector3Int position = new Vector3Int(mainRoot[Mainrand].currPos.x, 0, mainRoot[Mainrand].currPos.z);
                //방을 만들어 준다.
                makeRoomArray(position);
            }
            else
                break;
        }
        countRoomWeight(startPoint, startPoint);
        makeMainRoot();

        // 특수방 BOSS 방 생성
        selectBossRoom();

        setupPosition();
    }
    

    //방을 하나 만든다.
    public RoomInfo singleRoom(RoomInfo room, Vector3Int pos, string name)
    {
        RoomInfo single = room;
        single.roomID = name + "(" + pos.x + ", " + pos.y + ", " + pos.z + ")";
        single.roomName = name;
        single.currPos = pos;
        single.parentPos = pos;
        single.roomType = "Single";
        single.roomState = true;

        return single;
    }

    // 시작 방에서 해당 방까지 몇칸만큼 떨어져 있는지
    public void countRoomWeight(Vector3Int currentPos, Vector3Int prePos/*다시 돌아가는걸 막기 위함*/)
    {
        // currentPos = 현재 위치
        // prePos     = 이전 위치
        if (!possibleArr(currentPos))
            return;

        int weight = posArr[currentPos.z, currentPos.x].weightCnt;

        for (int i = 0; i < direction4.Count; i++)
        {
            Vector3Int newPosition = currentPos + direction4[i];//4방향을 모두 탐색함

            if (possibleArr(newPosition) && newPosition != prePos)//배열의 범위를 넘어가지 않으면 
            {
                // 새로운 위치가 활성화가 되었을 경우
                if (posArr[newPosition.z, newPosition.x].roomState)
                {
                    // 새로운 위치가 탐색했던 곳일 경우
                    if (posArr[newPosition.z, newPosition.x].weightCnt != -1)
                    {
                        if ((weight + 1) <= posArr[newPosition.z, newPosition.x].weightCnt)//새로운 위치의 거리가 현재 나의 위치 +1 이상일때
                        {
                            posArr[newPosition.z, newPosition.x].weightCnt = weight + 1;//새로운 위치의 거리를 하나 증가시킨다.
                            countRoomWeight(newPosition, currentPos);//해당 위치를 탐색
                        }
                    }// 새로운 위치가 탐색하지 않은 곳일 경우
                    else
                    {
                        posArr[newPosition.z, newPosition.x].weightCnt = weight + 1;//현재 나의 거리에서 +1;
                        countRoomWeight(newPosition, currentPos);//해당 위치를 탐색
                    }
                }
            }
        }
    }

    public void mainRoomSort(List<RoomInfo> root)
    {
        root.Sort(delegate (RoomInfo A, RoomInfo B)
        {
            if (A.weightCnt > B.weightCnt)
                return 1;
            else if (A.weightCnt < B.weightCnt)
                return -1;
            else
                return 0;
        });
    }

    public bool possibleArr(Vector3Int pos)
    {
        if ((0 <= (pos).x && (pos).x < (maxDistance * 2))
            && (0 <= (pos).z && (pos).z < (maxDistance * 2)))
        {
            return true;
        }
        else
            return false;
    }

    public void selectBossRoom()
    {
        mainRoomSort(mainRoot);

        bool selectBossRoom = false;

        for (int idx = mainRoot.Count - 1; 0 < idx; idx--)
        {
            if (!selectBossRoom)
            {
                int setLIstCnt = idx;
                Vector3Int pos = mainRoot[setLIstCnt].currPos;

                for (int i = 0; i < direction4.Count; i++)
                {
                    selectBossRoom = false;
                    Vector3Int bossMove = posArr[pos.z, pos.x].currPos + direction4[i];

                    if (possibleArr(bossMove))
                    {
                        if ((aroundRoomCount(bossMove) < 2)
                            && !posArr[bossMove.z, bossMove.x].roomState)
                        {
                            posArr[bossMove.z, bossMove.x].roomName = "Boss";
                            posArr[bossMove.z, bossMove.x].roomState = true;
                            posArr[bossMove.z, bossMove.x].currPos = bossMove;
                            posArr[bossMove.z, bossMove.x].parentPos = bossMove;
                            posArr[bossMove.z, bossMove.x].CenterPos = bossMove;
                            posArr[bossMove.z, bossMove.x].weightCnt = posArr[pos.z, pos.x].weightCnt + 1;
                            posArr[bossMove.z, bossMove.x].roomType = "Single";

                            BossPoint = bossMove;
                            selectBossRoom = true;

                            break;
                        }
                    }
                }
            }
        }
    }

    // 방의 배열을 초기화
    public void roomPosInit()
    {
        for (int i = 0; i < (maxDistance * 2); i++)
        {
            for (int j = 0; j < (maxDistance * 2); j++)
            {
                posArr[j, i] = new RoomInfo();
                posArr[j, i].roomState = false;
                posArr[j, i].weightCnt = -1;
            }
        }
    }
    // 모든 변수를 초기화
    public void RealaseRoom()
    {
        roomPosInit();
        mainRoot.Clear();
        shortParentRoot.Clear();

        currRoomCnt = 0;
    }

    // 배열의 방들을 RoomController의 List로 변환
    public void setupPosition()
    {
        List<RoomInfo> positions = new List<RoomInfo>();

        for (int i = 0; i < (maxDistance * 2); i++)
        {
            for (int j = 0; j < (maxDistance * 2); j++)
            {
                if (posArr[j, i].roomState)
                {
                    Vector3Int tmp = new Vector3Int(i, 0, j);

                    posArr[j, i] = singleRoom(posArr[j, i], posArr[j, i].roomName);

                    posArr[j, i].currPos = tmp - startPoint;
                    posArr[j, i].parentPos = posArr[j, i].parentPos - startPoint;
                    posArr[j, i].CenterPos = posArr[j, i].CenterPos - startPoint;

                    positions.Add(posArr[j, i]);
                }
            }
        }
        mainRoomCount = mainRoot.Count;
        for (int i = 0; i < positions.Count; i++)
            RoomController.Instance.LoadRoom(positions[i]);

    }

    //전체 배열은 10*10 (maxDistance가 5일때)
    public void makeMainRoot()
    {

        mainRoot.Clear();

        for (int i = 0; i < (maxDistance * 2); i++)
        {
            for (int j = 0; j < (maxDistance * 2); j++)
            {
                //모든 배열을 돌면서 방정보가 만들어진 방이면 메인 루트에 넣어준다.
                if (posArr[j, i].roomState)
                {
                    mainRoot.Add(posArr[j, i]);
                }
            }
        }
    }

    public RoomInfo singleRoom(RoomInfo pos, string name)
    {
        RoomInfo single = pos;
        single.roomID = name + "(" + pos.currPos.x + ", " + pos.currPos.y + ", " + pos.currPos.z + ")";
        single.roomName = name;
        single.currPos = pos.currPos;
        single.parentPos = pos.parentPos;
        single.roomType = pos.roomType;
        single.weightCnt = pos.weightCnt;

        return single;
    }

    public bool possiblePatten(Vector3Int pos, List<Vector3Int> move)
    {
        bool possible = true;
        for (int i = 0; i < move.Count; i++)
        {
            Vector3Int next = pos + move[i];

            if (possibleArr(next))
            {
                if (posArr[next.z, next.x].roomState)
                    return false;
            }
            else
                return false;

        }

        return possible;
    }

    public int aroundRoomCount(Vector3Int pos)
    {
        int count = 0;

        // LEFT
        if ((0 <= (pos.x - 1) && (pos.x - 1) < (maxDistance * 2)))
        {
            if (posArr[pos.z, pos.x - 1].roomState)
            {
                count += 1;
            }
        }

        // RIGHT
        if ((0 <= (pos.x + 1) && (pos.x + 1) < (maxDistance * 2)))
        {
            if (posArr[pos.z, pos.x + 1].roomState)
            {
                count += 1;
            }
        }

        // TOP
        if ((0 <= (pos.z - 1) && (pos.z - 1) < (maxDistance * 2)))
        {
            if (posArr[pos.z - 1, pos.x].roomState)
            {
                count += 1;
            }
        }
        // DOWN
        if ((0 <= (pos.z + 1) && (pos.z + 1) < (maxDistance * 2)))
        {
            if (posArr[pos.z + 1, pos.x].roomState)
            {
                count += 1;
            }
        }

        return count;
    }



    // Room Position 생성
    public void makeRoomArray(Vector3Int start)
    {
        //배열 넘어가는지 검사
        if (start.x >= (maxDistance * 2) || start.z >= (maxDistance * 2))
            return;
        //방 개수 조건 벗어나는지 검사
        if ((minRoomCnt <= currRoomCnt && currRoomCnt <= maxRoomCnt))
            return;

        // 사각형 방, 기억자, 니은자, -, |
        double[] persent = { 0.01, 0.03, 0.03, 0.1, 0.1, 0.1, 0.8 };

        // 각 방향 패턴을 List화 해당 위치에서 해당 방 모양을 만들기 위한 이동정보
        List<Dictionary<int, List<Vector3Int>>> allPatten
            = new List<Dictionary<int, List<Vector3Int>>> { downPatten, rightPatten, leftPatten, upPatten };

        //4방향을 모두 돌면서 작업
        for (int direction = 0; direction < direction4.Count; direction++)
        {
            //방향을 랜덤으로 뽑는다. 50%확률로 (Random.value=>0.0~1.0 까지 중에서 랜덤으로 뽑는다.)
            bool directionsRand = (Random.value > 0.5f);

            if (directionsRand)//50%확률로 실행
            {
                //랜덤으로 방 모양을 정한다.
                int thisPatten = (int)Choose(persent);

                //뽑힌 패턴이 가능한지 확인, 이미 방이 만들어져 있다던지 할때
                if (!possiblePatten(start, allPatten[direction][thisPatten]))
                    return;

                //만들어진 방 개수가 범위 안에 있는지 확인
                if (!roomCountCheck())
                {
                    
                    Vector3Int lastMove = start;

                    Vector3 currCenterPos = Vector3.zero;
                    //최대 방향 개수
                    int maxCount = allPatten[direction][thisPatten].Count;

                    //패턴의 크기만큼 돌면서
                    for (int i = 0; i < maxCount; i++)
                    {
                        //방향과 패턴이 정해졌으면 작업을 시작
                        Vector3Int firstPos = allPatten[direction][thisPatten][0];
                        //마지막위치
                        Vector3Int SecondPos = allPatten[direction][thisPatten][maxCount - 1];
                        //중심위치 
                        currCenterPos = new Vector3((float)(firstPos.x + SecondPos.x) / 2, 0, (float)(firstPos.z + SecondPos.z) / 2);

                        //움직임 
                        Vector3Int move = start + allPatten[direction][thisPatten][i];

                        //해당 방의 정보들을 넣어준다.
                        posArr[move.z, move.x].roomState = true;
                        //이름
                        posArr[move.z, move.x].roomName = "Single";
                        //현재 위치
                        posArr[move.z, move.x].currPos = start + allPatten[direction][thisPatten][i];
                        //거리
                        posArr[move.z, move.x].weightCnt = -1;

                        lastMove = move;

                        switch (allPatten[direction][thisPatten].Count)
                        {
                            case 2:
                                //해당 방의 패턴에따라 부모를 설정하고 중심좌표를 설정한다.
                                posArr[move.z, move.x].roomType = "Double";
                                posArr[move.z, move.x].parentPos = start + allPatten[direction][thisPatten][0];
                                posArr[move.z, move.x].CenterPos = start + currCenterPos;
                                break;
                            case 3:
                                posArr[move.z, move.x].roomType = "Triple";
                                posArr[move.z, move.x].parentPos = start + allPatten[direction][thisPatten][1];
                                posArr[move.z, move.x].CenterPos = start + allPatten[direction][thisPatten][1];

                                break;
                            case 4:
                                posArr[move.z, move.x].roomType = "Quad";
                                posArr[move.z, move.x].parentPos = start + allPatten[direction][thisPatten][0];
                                posArr[move.z, move.x].CenterPos = start + currCenterPos;
                                break;
                            default:
                                posArr[move.z, move.x].roomType = "Single";
                                posArr[move.z, move.x].parentPos = start + allPatten[direction][thisPatten][0];
                                posArr[move.z, move.x].CenterPos = start + currCenterPos;
                                break;
                        }
                    }
                    currRoomCnt++;
                    makeRoomArray(lastMove);
                }
            }
        }
    }
    public bool roomCountCheck()
    {
        return ((minRoomCnt <= currRoomCnt && currRoomCnt <= maxRoomCnt));
    }

    // 확률을 계산하여 패턴을 구성
    public double Choose(double[] probs)
    {
        double total = 0;

        foreach (double elem in probs)
            total += elem;

        double randomPoint = Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
                return i;
            else
                randomPoint -= probs[i];
        }
        return probs.Length - 1;
    }

    // 방의 갯수가 최소, 최대크기에 적합한지 체크

    private static System.Array ResizeArray(System.Array arr, int[] newSizes)
    {
        if (newSizes.Length != arr.Rank)
            return null;

        var temp = System.Array.CreateInstance(arr.GetType().GetElementType(), newSizes);
        int length = arr.Length <= temp.Length ? arr.Length : temp.Length;
        System.Array.ConstrainedCopy(arr, 0, temp, 0, length);
        return temp;
    }
}
