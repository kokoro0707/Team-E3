using System;
using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public GameObject[] enemyPrefabs;  //複数の敵プレハブを配列で管理
    public float spawnInterval = 2f;     //出現間隔（秒）
    public float spawnXRange = 5f;       //左右の出現範囲
    public float spawnYOffset = 1f;            //出現する高さ（画面高さ）
    public float minDistanceBetweenEnemies = 2f;

    private float timer = 0f;
    private float gameTime = 0f;

    // 敵のタグを決めておく（Inspectorで敵に「Enemy」タグをつけておくことを推奨）
    public string enemyTag = "Enemy";


    void Update()
    {
        timer += Time.deltaTime;
        gameTime += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;

            //画面内にいる敵を数える
            int enemiesInscreen = CountEnemiesInScreen();

            if(enemiesInscreen >= 3)
            {
                //３たい以上ならスポーンしない
                return;

            }

            // カメラの左下（viewport座標(0,0)）と右上(1,1)のワールド座標を取得
            Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
            Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

            // 横の範囲は画面の左右に合わせる（bottomLeft.x ～ topRight.x）
            float randomX = UnityEngine.Random.Range(bottomLeft.x, topRight.x);

            // 縦の位置は画面上の少し外（topRight.y + spawnYOffset）
            float spawnY = topRight.y + spawnYOffset;

            Vector3 spawnPos = new Vector3(randomX, spawnY, 0);

            GameObject enemyToSpawn;

            if (gameTime < 30f)
            {
                enemyToSpawn = enemyPrefabs[0];
            }
            else
            {
                int index = UnityEngine.Random.Range(0, enemyPrefabs.Length);
                enemyToSpawn = enemyPrefabs[index];
            }

            //重ならない位置を見つける
            Vector3 spaenPos = GetSpawnPosition(bottomLeft, topRight);
            Instantiate(enemyToSpawn, spawnPos, Quaternion.identity);
        }

        
    }

    // 画面内にいる敵の数を数える関数
    int CountEnemiesInScreen()
    {
        int count = 0;

        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, Camera.main.nearClipPlane));
        Vector3 topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, Camera.main.nearClipPlane));

        foreach (var enemy in enemies)
        {
            Vector3 pos = enemy.transform.position;

            // 敵が画面内にいるか判定
            if (pos.x >= bottomLeft.x && pos.x <= topRight.x &&
                pos.y >= bottomLeft.y && pos.y <= topRight.y)
            {
                count++;
            }
        }

        return count;
    }

    Vector3 GetSpawnPosition(Vector3 bottomLeft, Vector3 topRight)
    {
        Vector3 spawnPos = Vector3.zero;
        bool validPosition = false;

        while (!validPosition)
        {
            // ランダムにスポーン位置を決める
            float randomX = UnityEngine.Random.Range(bottomLeft.x, topRight.x);
            float spawnY = topRight.y + spawnYOffset;  // 上から外にスポーン
            spawnPos = new Vector3(randomX, spawnY, 0);

            // 既存の敵との距離をチェック
            validPosition = IsPositionValid(spawnPos);
        }

        return spawnPos;
    }

    // スポーン位置が他の敵と重なっていないかチェック
    bool IsPositionValid(Vector3 spawnPos)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        foreach (var enemy in enemies)
        {
            // 既存の敵との距離をチェック
            if (Vector3.Distance(spawnPos, enemy.transform.position) < minDistanceBetweenEnemies)
            {
                return false;  // 距離が近ければ重なっているので無効
            }
        }

        return true;  // 重ならなければ有効
    }
}
