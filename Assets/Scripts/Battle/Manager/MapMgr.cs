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
    private int waveIndex = 1;

    private BattleMgr battleMgr;

    public void Init(BattleMgr battleMgr) {
        this.battleMgr = battleMgr;

        // 实例化怪物第一批
        battleMgr.LoadMonsterByWaveID(waveIndex);

        Common.Log("MapMgr init done");
    }
}
