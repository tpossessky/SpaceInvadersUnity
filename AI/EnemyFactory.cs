using UnityEngine;

public class EnemyFactory : MonoBehaviour {
    [SerializeField] private GameObject[] enemyPrefabs;

    public int rowSize = 3;
    public int colSize = 8;
    public float xStep = 0.4f;
    public float yStep = .5f;
    public GameObject[,] enemyGrid;

    private readonly Vector2 startingTopLeftXY = new(-2.1f, 1.8f);

    public void createEnemyGrid() {
        enemyGrid = new GameObject[rowSize, colSize];
        var curXY = startingTopLeftXY;
        for (var i = 0; i < rowSize; i++) {
            for (var j = 0; j < colSize; j++) {
                var enemy = Instantiate(enemyPrefabs[i]);
                enemy.transform.position = curXY;
                curXY.x += xStep;
                enemyGrid[i, j] = enemy.gameObject;
            }

            curXY.y -= yStep;
            curXY.x = startingTopLeftXY.x;
        }
    }

    public void clearEnemyGrid() {
        for (var i = 0; i < rowSize; i++) {
            for (var j = 0; j < colSize; j++) {
                Destroy(enemyGrid[i, j]);
            }
        }
    }
}