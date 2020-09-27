/****************************************************
文件：MainCitySys.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 10:05:59
功能：主城业务系统
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCitySys : SystemRoot
{
    public static MainCitySys Instance = null;

    public MainCityWnd mainCityWnd;

    private PlayerController playerCtrl;

    public override void InitSys()
    {
        base.InitSys();
        Debug.Log(GetType() + "/InitSys()/Init MainCitySys...");

        Instance = this;
    }


    public void EnterMainCity() {
        MapCfg mapData = resSvc.GetMapCfgData(Constants.MainCityID);
        resSvc.AsyncLoadScene(mapData.sceneName,()=> {
            Common.Log(GetType()+ "/EnterMainCity()/Enter MainCity...");

            // 加载游戏主角
            LoadPlayer(mapData);

            // 打开主城的UI
            mainCityWnd.SetWndState(true);

            //播放主城背景音乐
            audioSvc.PlayBGAudio(Constants.BGMainCity);

            // TODO 设置任务展示相机
        });
    }

    private void LoadPlayer(MapCfg mapData) {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnCityPlayerPrefab,true);
        player.transform.position = mapData.playerBornPos;
        player.transform.localEulerAngles = mapData.playerBornRot;
        player.transform.localScale = Vector3.one * 1.5f;

        // 相机初始化
        Transform camTrans = Camera.main.transform;
        camTrans.position = mapData.mainCameraPos;
        camTrans.localEulerAngles = mapData.mainCameraRot;

        // 角色控制
        playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
    }

    public void SetMoveDir(Vector2 dir) {
        if (dir == Vector2.zero)
        {
            playerCtrl.SetBlend(Constants.BlendIdle);
        }
        else
        {
            playerCtrl.SetBlend(Constants.BlendWalk);
        }

        playerCtrl.Dir = dir;
    }
}

