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
    public ChatWnd chatWnd;
    public BuyWnd buyWnd;
    public TaskWnd taskWnd;

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

    #region EnterMainCity And Player

   
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
    #endregion

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
                EnterFuben();
                break;

            case 2:

                // TODO 进入强化界面
                OpenStrongWnd();
                break;

            case 3:

                // TODO 进入体力购买
                OpenBuyWnd(BuyType.BuyPower);
                break;


            case 4:

                // TODO 进入金币铸造
                OpenBuyWnd(BuyType.MKCoin);
                break;

            case 5:

                // TODO 进入世界聊天
                OpenChatWnd();
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

        StopNavTask();
        strongWnd.SetWndState();
    }

    public void RspStrong(GameMsg msg) {
        int zhanliPre = Common.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByStrong(msg.rspStrong);
        int zhanliNow = Common.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.AddTips(Constants.Color("战力提升 +"+(zhanliNow-zhanliPre),TxtColor.Blue));

        strongWnd.UpdateUI();
        mainCityWnd.RefreshUI();

    }
    #endregion

    #region chat
    public void OpenChatWnd() {

        StopNavTask();
        chatWnd.SetWndState();
    }

    public void PshChat(GameMsg msg) {
        chatWnd.AddChatMsg(msg.pshChat.name,msg.pshChat.chat);
    }
    #endregion

    #region Buy
    public void OpenBuyWnd(BuyType buyType) {

        StopNavTask();
        buyWnd.SetWndState();
        buyWnd.SetBuyType(buyType);
        
    }

    public void RspBuy(GameMsg msg) {
        RspBuy data = msg.rspBuy;
        GameRoot.Instance.SetPlayerDataByBuy(data);

        GameRoot.AddTips("购买成功");

        mainCityWnd.RefreshUI();

        buyWnd.SetWndState(false);
    }

    #endregion

    #region Power
    public void PshPower(GameMsg msg)
    {
        PshPower data = msg.pshPower;
        GameRoot.Instance.SetPlayerDataByPower(data);
        if (mainCityWnd.GetWndState() ==true)
        {
            mainCityWnd.RefreshUI();

        }

    }
    #endregion

    #region Task 
    public void OpenTaskWnd()
    {
        StopNavTask();
        taskWnd.SetWndState();
       

    }

    public void RspTakeTaskReward(GameMsg msg)
    {
        RspTakeTaskReward data = msg.rspTakeTaskReward;
        GameRoot.Instance.SetPlayerDataByTask(data);
        taskWnd.RefreshUI();
        mainCityWnd.RefreshUI();

        // 服务器并包处理
        if (msg.pshTaskPrgs != null)
        {
            PshTaskPrgs(msg);
        }
    }

    public void PshTaskPrgs(GameMsg msg) {
        PshTaskPrgs data = msg.pshTaskPrgs;
        GameRoot.Instance.SetPlayerDataByTaskPsh(data);

        if (taskWnd.GetWndState()==true)
        {
            taskWnd.RefreshUI();
        }
    }

    #endregion

    #region Enter Fuben
    public void EnterFuben() {
        StopNavTask();
        FubenSys.Instance.EnterFubenWnd();
    }

    #endregion
}

