/****************************************************
文件：MainCitySys.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 10:05:59
功能：主城业务系统
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MainCitySys : SystemRoot
{
    public static MainCitySys Instance = null;

    public MainCityWnd mainCityWnd;
    public InfoWnd infoWnd;
    public GuideWnd guideWnd;
    public StrongWnd strongWnd;

    private Transform CharCamTrans;

    private PlayerController playerCtrl;
    private AutoGuideCfg curTaskData;
    private Transform[] npcPosTrans;
    private NavMeshAgent nav;

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

            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            MainCityMap mcm = map.GetComponent<MainCityMap>();
            Debug.Log(GetType()+ "/EnterMainCity()/ map : " + map.gameObject.name);
            
            npcPosTrans = mcm.NpcPosTrans;

            // 设置任务展示相机
            if (CharCamTrans != null)
            {
                CharCamTrans.gameObject.SetActive(false);
            }
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
        nav = playerCtrl.gameObject.GetComponent<NavMeshAgent>();

    }

    public void SetMoveDir(Vector2 dir) {
        // 手动控制角色，导航终端
        StopNavTask();

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

    #region Info

    
    public void OpenInfoWnd() {
        StopNavTask();
        if (CharCamTrans == null)
        {
            CharCamTrans = GameObject.FindGameObjectWithTag("CharShowCam").transform;
        }

        CharCamTrans.localPosition = playerCtrl.transform.position + playerCtrl.transform.forward * 2.8f + new Vector3(0,1.2f,0);
        CharCamTrans.localEulerAngles = new Vector3(0,180 +playerCtrl.transform.localEulerAngles.y,0);
        CharCamTrans.localScale = Vector3.one;
        CharCamTrans.gameObject.SetActive(true);

        infoWnd.SetWndState();
    }

    public void CloseInfoWnd() {
        if (CharCamTrans != null)
        {
            CharCamTrans.gameObject.SetActive(false);
        }

        infoWnd.SetWndState(false);
    }

    private float startRotate = 0;
    public void SetStartRotate() {
        startRotate = playerCtrl.transform.localEulerAngles.y;
    }

    public void SetPlayerRotate(float rotate) {
        playerCtrl.transform.localEulerAngles = new Vector3(0,startRotate + rotate,0) ;
    }
    #endregion

    #region Guide


    private bool isNavGuide = false;
    public void RunTask(AutoGuideCfg task) {
        if (task != null)
        {
            curTaskData = task;
        }

        // 解析任务系统数据
        nav.enabled = true;
        if (curTaskData.npcID != -1)
        {
            float dis = Vector3.Distance(playerCtrl.transform.position,npcPosTrans[task.npcID].position);
            if (dis < 0.5f)
            {
                isNavGuide = false;
                nav.isStopped = true;
                playerCtrl.SetBlend(Constants.BlendIdle);
                nav.enabled = false;

                OpenGuideWnd();
            }
            else {
                nav.enabled = true;
                isNavGuide = true;
                nav.speed = Constants.PlayerMoveSpeed;
                nav.SetDestination(npcPosTrans[task.npcID].position);
                playerCtrl.SetBlend(Constants.BlendWalk);
            }
        }
        else {
            OpenGuideWnd();
        }
    }

    private void Update()
    {
        if (isNavGuide == true)
        {
            IsArriveNavPos();
            playerCtrl.SetCamera();
        }
    }

    private void IsArriveNavPos() {
        float dis = Vector3.Distance(playerCtrl.transform.position, npcPosTrans[curTaskData.npcID].position);
        if (dis < 0.5f)
        {
            isNavGuide = false;
            nav.isStopped = true;
            playerCtrl.SetBlend(Constants.BlendIdle);
            nav.enabled = false;

            OpenGuideWnd();
        }
    }

    private void StopNavTask() {
        if (isNavGuide == true)
        {
            isNavGuide = false;
        nav.isStopped = true;
        playerCtrl.SetBlend(Constants.BlendIdle);
        nav.enabled = false;
        }
        
    }

    private void OpenGuideWnd() {
        // TODO

        Debug.Log(GetType()+ "/OpenGuideWnd()/ Open Guide Wnd");
        guideWnd.SetWndState();
    }

    public AutoGuideCfg GetAutoGuideCfg() {
        return curTaskData;
    }

    public void RspGuide(GameMsg msg) {
        RspGuide data = msg.rspGuide;

        GameRoot.AddTips(Constants.Color( string.Format("任务奖励 金币 {0} ，经验 {1}", curTaskData.coin,curTaskData.exp),TxtColor.Blue));
        switch (curTaskData.actID)
        {
            case 0:

                // 与智者对话
                break;

            case 1:

                // TODO 进入副本
                break;

            case 2:

                // TODO 进入强化界面
                break;

            case 3:

                // TODO 进入体力购买
                break;


            case 4:

                // TODO 进入金币铸造
                break;

            case 5:

                // TODO 进入世界聊天
                break;
            default:
                break;
        }

        GameRoot.Instance.SetPlayerDataByGuide(data);
        mainCityWnd.RefreshUI();
    }

    #endregion

    #region Strong
    public void OpenStrongWnd() {
        strongWnd.SetWndState();
    }
    #endregion
}

