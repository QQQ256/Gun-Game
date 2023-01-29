using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;
    public int seed = 10;

    [Range(0,1)]
    public float outlinePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;

    private void Start() {
        GenerateMap();
    }

    public void GenerateMap(){

        // 将所有坐标加入list之中
        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }

        // 将打乱顺序的坐标都加入queue当中
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), seed));

        // 若要生成新地图，则舍弃旧地图，再生成一个新地图，将所有生成的prefab归于其child
        // 由于在Editor中会一直调用这个函数，所有要生成的物体都应放在mapHolder之下
        string name = "Generated Map";
        if(transform.Find(name)){
            DestroyImmediate(transform.Find(name).gameObject);
        }
        Transform mapHolder = new GameObject(name).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                // 0.5f是设定起始位置，如果设成10就是从vector3(10,x,x)开始了
                // 其余的就是在当前坐标的右边生成，例如最开始的是0，设方块长度为1，那么下一个坐标的z位置应该是1.5
                // 原因是第一个方块的中心坐标是(0.5, 0, 0.5) -> 第二个应该就是 (0.5, 0, 1.5)
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = mapHolder;
                // Debug.Log(tilePosition + ", " + newTile.name);
            }
        }

        // 生成需要数量的障碍物
        // 思路是获取打乱后的坐标，将其转换成可生成位置的坐标，最后生成这个预制体
        int obstacleCount = 10;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
            Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * 0.5f, Quaternion.identity) as Transform;
            newObstacle.parent = mapHolder;
        }
    }

    private Vector3 CoordToPosition(int x, int y){
        return new Vector3(-mapSize.x / 2 + 0.5f + x, 0, -mapSize.y / 2 + 0.5f + y);
    }

    public Coord GetRandomCoord(){
        // 从队列头取出一个元素，再加入队尾
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    // 该结构体用于存储每个生成的方块的坐标
    public struct Coord{
        public int x;
        public int y;

        public Coord(int _x, int _y){
            x = _x;
            y = _y;
        }
    };
}
