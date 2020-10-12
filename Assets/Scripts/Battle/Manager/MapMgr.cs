/****************************************************
文件：MapMgr.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/04 12:55:47
功能：地图管理器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    public TriggerData[] triggerArr;

    private int waveIndex = 1;

    private BattleMgr battleMgr;

    public void Init(BattleMgr battleMgr) {
        this.battleMgr = battleMgr;

        // 实例化怪物第一批
        battleMgr.LoadMonsterByWaveID(waveIndex);

        Common.Log("MapMgr init done");
    }

    public void TriggerMonsterBorn(TriggerData trigger, int waveIndex) {
        if (battleMgr != null)
        {
            BoxCollider co = trigger.GetComponent<BoxCollider>();
            co.isTrigger = false;

            battleMgr.LoadMonsterByWaveID(waveIndex);
            battleMgr.ActiveCurrentBatchMonsters();
            battleMgr.triggerCheck = true;
            
        }
    }

    public bool SetNextTriggerOn() {
        waveIndex += 1;
        for (int i = 0; i < triggerArr.Length; i++)
        {
            if (triggerArr[i].triggerMstWave == waveIndex) {
                BoxCollider co = triggerArr[i].GetComponent<BoxCollider>();
                co.isTrigger = true;


                return true;
            }
        }

        return false;
    }
}
