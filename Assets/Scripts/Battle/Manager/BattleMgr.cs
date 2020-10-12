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
    internal EntityPlayer entityPlayer;
    private MapCfg mapCfg = null;

    public bool triggerCheck = true;

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

    private void Update()
    {
        foreach (var item in monstersDic)
        {
            EntityMonster monster = item.Value;
            monster.TickAILogic();
        }

        if (mapMgr != null)
        {
            if (monstersDic.Count == 0&& triggerCheck ==true)
            {
                triggerCheck = false;
                bool isExit = mapMgr.SetNextTriggerOn();

                if (isExit == false)
                {
                    // 关卡结束，结算界面
                }
            }
        }
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
        entityPlayer.SetCtrl(playerCtrl);
        entityPlayer.Name = "AssassinBattle";
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
                em.SetCtrl( mc);
                em.Name = mst.name;
                mst.SetActive(false);

                monstersDic.Add(mst.name,em);

                if (md.mCfg.mType == MonsterType.Normal)
                {
                    // 添加对应怪物的血条UI
                    GameRoot.Instance.dynamicWnd.AddHpItemInfo(mst.name, mc.hpRoot, em.HP);
                }
                else if (md.mCfg.mType == MonsterType.Boss)
                {
                    BattleSys.Instance.playerCtrlWnd.SetBossHpBarState(true);
                }
                
            }


        }
    }

    public void ActiveCurrentBatchMonsters() {
        TimerSvc.Instance.AddTimeTask((int tid)=> {

            foreach (var item in monstersDic)
            {
                item.Value.SetActive(true);
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

    public void RmvMonster(string key) {
        EntityMonster entityMonster;
        if (monstersDic.TryGetValue(key, out entityMonster)==true)
        {
            monstersDic.Remove(key);
            GameRoot.Instance.dynamicWnd.RmvHpItemInfo(key);
        }
    }


    #region 技能释放和角色控制

    
    public void SetSelfPlayerMoveDir(Vector2 dir) {
        //Debug.Log(GetType()+ "/SetSelfPlayerMoveDir()/ SetSelfPlayerMoveDir");

        if (entityPlayer.canControll == false)
        {
            return;
        }
        if (entityPlayer.currentAniState == AniState.Idle
            || entityPlayer.currentAniState == AniState.Move
            )
        {
            if (dir == Vector2.zero)
            {
                entityPlayer.Idle();

            }
            else
            {
                entityPlayer.Move();
                entityPlayer.SetDir(dir);
            }
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

    internal double lastAtkTime = 0;
    private int[] comboArr = new int[] {111,112,113,114,115 };
    internal int comboIndex = 0;

    private void ReleaseNormalAtk()
    {
        //Debug.Log(GetType() + "/ReleaseNormalAtk()/ ");
        // 连招判定
        if (entityPlayer.currentAniState == AniState.Attack)
        {
            // 在规定时间内进行二次点击，存数据
            double nowAtkTime = TimerSvc.Instance.GetNowTime();
            if (nowAtkTime-lastAtkTime<Constants.ComboSpace && lastAtkTime !=0)
            {
                if (comboArr[comboIndex] != comboArr[comboArr.Length - 1])
                {
                    comboIndex += 1;
                    entityPlayer.comboQue.Enqueue(comboArr[comboIndex]);
                    lastAtkTime = nowAtkTime;
                }
                else {
                    lastAtkTime = 0;
                    comboIndex = 0;
                }
                
            }
        }
        else if(entityPlayer.currentAniState == AniState.Idle 
            || entityPlayer.currentAniState == AniState.Move)
        {
            lastAtkTime = TimerSvc.Instance.GetNowTime();
            comboIndex = 0;
            entityPlayer.Attack(comboArr[comboIndex]);

        }
    }

    private void ReleaseSkill1()
    {
        //Debug.Log(GetType() + "/ReleaseSkill1()/ ");
        // 根据配置表传递参数
        entityPlayer.Attack(101);
    }

    private void ReleaseSkill2()
    {
        //Debug.Log(GetType() + "/ReleaseSkill2()/ ");
        // 根据配置表传递参数
        entityPlayer.Attack(102);
    }
    private void ReleaseSkill3()
    {
        //Debug.Log(GetType() + "/ReleaseSkill3()/ ");
        // 根据配置表传递参数
        entityPlayer.Attack(103);
    }


    public Vector2 GetCurDirInput() {

        return BattleSys.Instance.GetCurrentDir();
    }

    #endregion


    public bool CanRlsSkill() {
        return entityPlayer.canRlsSkill;
    }

    #region XAN Helper
    IEnumerator AnimatorApplyRootMotion(Animator ani) {
        //yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.05f);
        ani.applyRootMotion = true; // 避免人物一出生就从原点坠落（取消Animator 的 Apply Root Motion 也有类似效果）
    }
    #endregion
}
