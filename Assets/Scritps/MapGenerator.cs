using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public Map[] maps;
    public int mapIndex;

    public Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform navMeshFloor;
    public Transform navMeshPrefab;
    public Vector2 maxMapSize; // set navmesh quad size
    public float tileSize;

    [Range(0,1)]
    public float outlinePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords; // 所有方块，被打乱的方块
    Queue<Coord> shuffledOpenTileCoords; // 除了障碍物的所有方块，玩家和敌人都能踩的方块
    Transform[,] tileMap;

    Map currentMap;

    Coord mapCenter;

    private void Start() {
        GenerateMap();
    }

    public void GenerateMap(){
        // 设置障碍物的高度
        currentMap = maps[mapIndex];
        System.Random prng = new System.Random(currentMap.seed);
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];

        // 设置碰撞体大小
        GetComponent<BoxCollider>().size = new Vector3(currentMap.mapSize.x * tileSize, .05f, currentMap.mapSize.y * tileSize);

        mapCenter = new Coord((int)(currentMap.mapSize.x / 2), (int)(currentMap.mapSize.y / 2));

        // 将所有坐标加入list之中
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }

        // 将打乱顺序的坐标都加入queue当中
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // 若要生成新地图，则舍弃旧地图，再生成一个新地图，将所有生成的prefab归于其child
        // 由于在Editor中会一直调用这个函数，所有要生成的物体都应放在mapHolder之下
        // 创建mapHolder
        string name = "Generated Map";
        if(transform.Find(name)){
            DestroyImmediate(transform.Find(name).gameObject);
        }
        Transform mapHolder = new GameObject(name).transform;
        mapHolder.parent = transform;

        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                // 0.5f是设定起始位置，如果设成10就是从vector3(10,x,x)开始了
                // 其余的就是在当前坐标的右边生成，例如最开始的是0，设方块长度为1，那么下一个坐标的z位置应该是1.5
                // 原因是第一个方块的中心坐标是(0.5, 0, 0.5) -> 第二个应该就是 (0.5, 0, 1.5)
                Vector3 tilePosition = CoordToPosition(x, y);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                newTile.parent = mapHolder;
                
                tileMap[x, y] = newTile;
            }
        }

        // 生成需要数量的障碍物
        // 思路是获取打乱后的坐标，将其转换成可生成位置的坐标，最后生成这个预制体
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y]; // 记录哪些地方已经有障碍物了
        int obstacleCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y * currentMap.obstaclePercent); // 通过比率计算出要生成的障碍物个数
        int currentObstacleCount = 0;
        List<Coord> openTileLists = new List<Coord>(allTileCoords); // 复制所有的coords进来，将是障碍物的coord从list内移除

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            if(randomCoord != mapCenter && MapIsFullyAccessible(obstacleMap, currentObstacleCount)){
                // 设定障碍物高度和大小
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                currentObstacleCount++;

                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);
                newObstacle.parent = mapHolder;

                // 设定障碍物颜色
                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                // 计算其在整体的位置来获取一个百分比
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                // 移除是障碍物的coord
                openTileLists.Remove(randomCoord);
            }
            else{
                // 回溯!?
                obstacleMap[randomCoord.x, randomCoord.y] = false;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(openTileLists.ToArray(), currentMap.seed));

        // 设置四个mask，分别围住地图，以此去除一些寻路
        // 宽度的计算：最长的边是maxMapSize.x，中间地图的边是mapSize.x，获取的点是左边或右边的一个差值的一半，也就是差值的中点，所以 / 4
        Transform maskLeft = Instantiate(navMeshPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.parent = mapHolder;        
        // 再设置大小
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        // 右边同理
        Transform maskRight = Instantiate(navMeshPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.parent = mapHolder;        
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskUp = Instantiate(navMeshPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskUp.parent = mapHolder;
        maskUp.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskDown = Instantiate(navMeshPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskDown.parent = mapHolder;
        maskDown.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navMeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
    }


    // 此方法用于检测从正中间点出发的所有方块（除了障碍物）数量是否等于总数 - 障碍物数量
    // 说白了就是个BFS，如果有几个障碍物将方块包围在一起，则说明无法填充满所有非障碍物方块
    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount){
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0), obstacleMap.GetLength(1)]; // 开辟一个新的bool数组用于判断当前方块是否已经被访问
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        int accessibleTileNumber = 1;

        while(queue.Count > 0){
            Coord tile = queue.Dequeue();
            // 往上下左右四个方向进行BFS
            for(int x = -1; x <= 1; x++){
                for(int y = -1; y <= 1; y++){
                    int neighborX = tile.x + x;
                    int neighborY = tile.y + y;
                    if(x == 0 || y == 0){ // 当 x 的值为 0 或 y 的值为 0 时，它们所代表的邻居才在上下左右四个方向。
                        if(neighborX >= 0 && neighborX < obstacleMap.GetLength(0) && neighborY >= 0 && neighborY < obstacleMap.GetLength(1)){
                            if(!mapFlags[neighborX, neighborY] && !obstacleMap[neighborX, neighborY]){ // 若从未访问过
                                mapFlags[neighborX, neighborY] = true;
                                queue.Enqueue(new Coord(neighborX, neighborY));
                                accessibleTileNumber++;
                            }
                        }
                    }
                }
            }
        }
        int targetAccessibleTileNumber = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileNumber == accessibleTileNumber;
    }

    public Transform PositionToCoord(Vector3 position){
        //  对CoordToPosition的运算进行方程计算，求x
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f); // Mathf.RoundToInt会四舍五入，而(int)则不会，会损失精度
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

        // 设置x和y的范围，确保在地图内，[0...(len - 1)]
        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);
        return tileMap[x, y];
    }

    private Vector3 CoordToPosition(int x, int y){
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    public Coord GetRandomCoord(){
        // 从队列头取出一个元素，再加入队尾
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile(){
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    // 该结构体用于存储每个生成的方块的坐标
    [System.Serializable]
    public struct Coord{
        public int x;
        public int y;

        public Coord(int _x, int _y){
            x = _x;
            y = _y;
        }

        // 重载，用于比较两个coord是否相同
        public static bool operator == (Coord c1, Coord c2){
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator != (Coord c1, Coord c2){
            return !(c1 == c2);
        } 
    };

    [System.Serializable]
    public class Map{
        public Coord mapSize;
        [Range(0,1)]
        public float obstaclePercent;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;
        public int seed;
    
        public Coord mapCenter{
            get {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }
    }
}
