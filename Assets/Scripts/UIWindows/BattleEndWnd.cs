/****************************************************
文件：BattleEndWnd.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/12 22:37:45
功能：战斗结算界面
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleEndWnd : WindowRoot
{
    #region UI Define

    public Transform rewardTrans;

    public Button btnClose;
    public Button btnExit;
    public Button btnSure;

    public Text txtTime;
    public Text txtRestHp;
    public Text txtReward;

    #endregion

    private FBEndType endType = FBEndType.None;

    private void Start()
    {
        btnClose.onClick.AddListener(ClickCloseBtn);
        btnExit.onClick.AddListener(ClickExitBtn);
        btnSure.onClick.AddListener(ClickSureBtn);
    }

    protected override void InitWnd()
    {
        base.InitWnd();
        RefreshUI();
    }

    private void RefreshUI() {
        switch (endType)
        {
            case FBEndType.None:
                break;
            case FBEndType.Pause:
                SetActive(rewardTrans,false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject);
                break;
            case FBEndType.Win:

                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject,false);
                SetActive(btnClose.gameObject, false);

                MapCfg cfg = resSvc.GetMapCfgData(fbid);
                // 毫秒
                int min = costtime/1000/60 ;
                int sec = costtime/1000%60 ;
                int coin = cfg.coin;
                int exp = cfg.exp;
                int crystal = cfg.crystal;

                SetText(txtTime,"通关时间："+min+":"+sec);
                SetText(txtRestHp,"剩余血量："+resthp);
                SetText(txtReward,"关卡奖励："+Constants.Color(coin+"金币", TxtColor.Yellow)+
                                                Constants.Color(exp + "经验", TxtColor.Red)+
                                                Constants.Color(crystal + "水晶", TxtColor.Blue));

                // 延时界面出来
                timerSvc.AddTimeTask((tid)=> {
                    SetActive(rewardTrans);

                    timerSvc.AddTimeTask((tid1) => {
                        audioSvc.PlayUIAudio(Constants.FBItemEnter);
                        timerSvc.AddTimeTask((tid2) => {
                            audioSvc.PlayUIAudio(Constants.FBItemEnter);
                            timerSvc.AddTimeTask((tid3) => {
                                audioSvc.PlayUIAudio(Constants.FBItemEnter);
                                timerSvc.AddTimeTask((tid4) => {
                                    audioSvc.PlayUIAudio(Constants.FBWin);
                                }, 250);
                            }, 250);
                        }, 250);
                    }, 250);
                },1000);

                break;
            case FBEndType.Lose:
                SetActive(rewardTrans, false);
                SetActive(btnExit.gameObject);
                SetActive(btnClose.gameObject,false);
                audioSvc.PlayUIAudio(Constants.FBLose);
                break;
            default:
                break;
        }
    }

    public void SetWndType(FBEndType endType) {
        this.endType = endType;
    }

    private int fbid;
    private int costtime;
    private int resthp;
    public void SetBattleEndData(int fbid,int costtime, int resthp) {
        this.fbid = fbid;
        this.costtime = costtime;
        this.resthp = resthp;
    }

    #region Click Event

    private void ClickCloseBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        BattleSys.Instance.battleMgr.isGamePause = false;
        SetWndState(false);
    }

    private void ClickExitBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        // 进入主城，销毁当前场景
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();

    }

    private void ClickSureBtn() {
        audioSvc.PlayUIAudio(Constants.UIClickBtn);
        //进入主城，打开副本界面
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        FubenSys.Instance.EnterFubenWnd();
    }

    #endregion
}

public enum FBEndType {
    None,
    Pause,
    Win,
    Lose
}
