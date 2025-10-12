using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using Unity.Mathematics;
using UnityEditor;

public class GameClearManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private Camera clearCamera;
    [SerializeField] private Transform player;
    [SerializeField] private Image fadeimage;
    [SerializeField] private GameObject clearPlayerobject;
    [SerializeField] private float zoomspeed = 2f;
    [SerializeField] private float zoomDistance = 1.5f;
    [SerializeField] private float fadespeed = 1f;
    [SerializeField] private float rotateSpeed = 180f;
    [SerializeField] private float rightOffset = 2.5f;

    private bool isClearing = false;

    private void Start()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;

        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        if(fadeimage != null )
        {
            var c = fadeimage.color;
            c.a = 0f;
            fadeimage.color = c;
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.G)) // デバック用キー　Gキー
        {
            StartCoroutine(StartGameClear());
        }
    }

    private IEnumerator StartGameClear()
    {
       isClearing = true;
        Time.timeScale = 0f; // ゲーム停止

        // ==== ズーム処理 ====

        Vector3 startPos = mainCamera.transform.position;
        Vector3 targetPos = player.transform.position + new Vector3(0,1.5f,-zoomDistance);

        float t = 0f;
        while(t < 1f)
        {
            t += Time.unscaledDeltaTime * fadespeed;
            mainCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.3f);

        // ==== フェード処理 ====
        float f = 0f;
        while (f < 1f)
        {
            f += Time.unscaledDeltaTime * fadespeed;

            if(fadeimage != null )
            {
                var c = fadeimage.color;
                c.a = Mathf.Clamp01(f);
                fadeimage.color = c;
            }

            yield return null;
        }

        // ==== SE部分 ====
        
        // クリアカメラ切り替え
        if(clearCamera != null )
        {
            mainCamera.enabled = false;
            clearCamera.enabled = true;
        }

        // ==== 新プレイヤー生成 ====
        if (clearPlayerobject != null && player != null)
        {
            Vector3 rightDir = player.right * rightOffset;
            Vector3 spawnPos = player.position + rightDir;
            quaternion spawanRot = player.rotation;
            GameObject newPlayer = Instantiate(clearPlayerobject, spawnPos, spawanRot);
            newPlayer.layer = LayerMask.NameToLayer("ClearPlayer");

            // ==== 回転演出 ====
            float rotTime = 0f;
            while (rotTime < 2f)
            {
                rotTime += Time.unscaledDeltaTime;
                newPlayer.transform.Rotate(Vector3.up, rotateSpeed * Time.unscaledDeltaTime);
                yield return null;
            }
        }

        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
        isClearing = false;
    }
}
