/****************************************************
文件：BattleMgr.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/10/04 12:51:39
功能：战场管理器
*****************************************************/

using Protocal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMgr : MonoBehaviour
{
    private ResSvc resSvc;
    private AudioSvc audioSvc;

    private StateMgr stateMgr;
    private SkillMgr skillMgr;
    private MapMgr mapMgr;
    private EntityPlayer entityPlayer;
    private MapCfg mapCfg = null;

    private Dictionary<string, EntityMonster> monstersDic = new Dictionary<string, EntityMonster>();

    public void Init(int mapid) {
        resSvc = ResSvc.Instance;
        audioSvc = AudioSvc.Instance;

        // 初始化各个管理器
        stateMgr = gameObject.AddComponent<StateMgr>();
        stateMgr.Init();
        skillMgr = gameObject.AddComponent<SkillMgr>();
        skillMgr.Init();

        // 加载战场地图
        mapCfg = resSvc.GetMapCfgData(mapid);
        resSvc.AsyncLoadScene(mapCfg.sceneName,()=> {
            // 初始化地图数据
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            mapMgr = map.GetComponent<MapMgr>();
            mapMgr.Init(this);

            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.position = mapCfg.mainCameraPos;
            Camera.main.transform.localEulerAngles = mapCfg.mainCameraRot;

            LoadPlayer(mapCfg);
            entityPlayer.Idle();

            // 激活当前批次的怪物
            ActiveCurrentBatchMonsters();

            audioSvc.PlayBGAudio(Constants.BGHuangYe);
        });

        Common.Log("BattleMgr init done");
    }


    private void LoadPlayer(MapCfg mapCfg)
    {
        GameObject player = resSvc.LoadPrefab(PathDefine.AssissnBattlePlayerPrefab);
          
        player.transform.position = mapCfg.playerBornPos ;
        player.transform.localEulerAngles = mapCfg.playerBornRot;
        player.transform.localScale = Vector3.one;

        PlayerData pd = GameRoot.Instance.PlayerData;
        BattleProps props = new BattleProps {
            hp = pd.hp,
            ad = pd.ad,
            ap = pd.ap,
            addef = pd.addef,
            apdef = pd.apdef,
            dodge = pd.dodge,
            pierce = pd.pierce,
            critical = pd.critical
        };

        Animator ani = player.GetComponent<Animator>();
        StartCoroutine(AnimatorApplyRootMotion( ani));
        entityPlayer = new EntityPlayer {
            battleMgr = this,
            stateMgr = stateMgr,
            skillMgr = skillMgr
        };
        entityPlayer.SetBattleProps(props);

        PlayerController playerCtrl = player.GetComponent<PlayerController>();
        playerCtrl.Init();
        entityPlayer.controller = playerCtrl;
    }

    public void LoadMonsterByWaveID(int wave) {
        for (int i = 0; i < mapCfg.monsterLst.Count; i++)
        {
            MonsterData md = mapCfg.monsterLst[i];
            if (md.mWave == wave)
            {
                GameObject mst = resSvc.LoadPrefab(md.mCfg.resPath,true);
                mst.transform.localPosition = md.mBornPos;
                mst.transform.localEulerAngles = md.mBornRot;
                mst.transform.localScale = Vector3.one;
                mst.name = "mst" + md.mWave + "_" + md.mIndex;

                EntityMonster em = new EntityMonster {
                    battleMgr = this,
                    stateMgr = stateMgr,
                    skillMgr = skillMgr
                };
                em.md = md;
                em.SetBattleProps(md.mCfg.bps);

                MonsterController mc = mst.GetComponent<MonsterController>();
                mc.Init();
                em.controller = mc;

                mst.SetActive(false);

                monstersDic.Add(mst.name,em);

                // 添加对应怪物的血条UI
                GameRoot.Instance.dynamicWnd.AddHpItemInfo(mst.name,mc.hpRoot, em.HP);
            }


        }
    }

    public void ActiveCurrentBatchMonsters() {
        TimerSvc.Instance.AddTimeTask((int tid)=> {

            foreach (var item in monstersDic)
            {
                item.Value.controller.gameObject.SetActive(true);
                item.Value.Born();
                TimerSvc.Instance.AddTimeTask((id)=> {
                    // 根据出生动画的时间估算1秒之后切换为idle
                    item.Value.Idle();

                },1000);
            }
        },500);
    }

    public List<EntityMonster> GetEntityMonsters() {
        List<EntityMonster> monsters = new List<EntityMonster>();
        foreach (var item in monstersDic)
        {
            monsters.Add(item.Value);
        }

        return monsters;
    }


    public void SetSelfPlayerMoveDir(Vector2 dir) {
        //Debug.Log(GetType()+ "/SetSelfPlayerMoveDir()/ SetSelfPlayerMoveDir");

        if (entityPlayer.canControll == false)
        {
            return;
        }

        if (dir==Vector2.zero)
        {
            entityPlayer.Idle();
            
        }
        else
        {
            entityPlayer.Move();
            entityPlayer.SetDir(dir);
        }
    }

    public void ReqReleaseSkill(int index) {
        switch (index)
        {
            case 0:
                ReleaseNormalAtk();
                break;

            case 1:
                ReleaseSkill1();
                break;

            case 2:
                ReleaseSkill2();
                break;

            case 3:
                ReleaseSkill3();
                break;

        }
    }

    private void ReleaseNormalAtk()
    {
        Debug.Log(GetType() + "/ReleaseNormalAtk()/ ");
    }

    private void ReleaseSkill1()
    {
        //Debug.Log(GetType() + "/ReleaseSkill1()/ ");
        // 根据配置表传递参数
        entityPlayer.Attack(101);
    }

    private void ReleaseSkill2()
    {
        Debug.Log(GetType() + "/ReleaseSkill2()/ ");
    }
    private void ReleaseSkill3()
    {
        Debug.Log(GetType() + "/ReleaseSkill3()/ ");
    }


    public Vector2 GetCurDirInput() {

        return BattleSys.Instance.GetCurrentDir();
    }


    #region XAN Helper
    IEnumerator  AnimatorApplyRootMotion(Animator ani) {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);
        ani.applyRootMotion = true; // 避免人物一出生就从原点坠落（取消Animator 的 Apply Root Motion 也有类似效果）
    }
    #endregion
}
