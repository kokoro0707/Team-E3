using UnityEngine;

public class enemy : MonoBehaviour
{
    public float verticalSpeed = 2f;      //下方向のスピード
    public float horizontalAmplitude = 5f;//左右の揺れの幅
    public float horizontalFrequency = 5f;//揺れの速さ

    private float startX; //初期X座標
    private float timeOffset;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startX = transform.position.x;
        timeOffset = Random.Range(1f, 100f); // 揺れをずらすためにランダム値を加える
    }

    // Update is called once per frame
    void Update()
    {
        //下方向に常に移動
        transform.Translate(Vector2.down * verticalSpeed * Time.deltaTime);

        //左右にスイングする動き（sin波を使う）
        float horizontalOffset = Mathf.Sin((Time.time + timeOffset) * horizontalFrequency) * horizontalAmplitude;
        Vector3 newPosition = transform.position;
        transform.position = newPosition;

    }
}