/****************************************************
文件：PlayerController.cs
作者：仙魁Xan
邮箱：1272200579@qq.com 
日期：2020/09/27 16:25:19
功能：角色控制器
*****************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    public Animator ani;
    public CharacterController ctrl;

    private Transform camTrans;
    private Vector2 dir = Vector2.zero;
    private bool isMove = false;
    private Vector3 camOffset;
    private float targetBlend;
    private float currentBlend;

    public Vector2 Dir { get => dir;
        set {
            dir = value;
            if (value != Vector2.zero)
            {
                isMove = true;
            }
            else {
                isMove = false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Init();
    }


    // Update is called once per frame
    void Update()
    {
        #region 输入方向
        /*
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector2 _dir = new Vector2(h, v).normalized;
        if (_dir != Vector2.zero)
        {
            Dir = _dir;
            SetBlend(Constants.BlendWalk);
        }
        else {
            Dir = Vector2.zero;
            SetBlend(Constants.BlendIdle);
        }
        */
        #endregion

        if (currentBlend != targetBlend)
        {
            UpdateMixBlend();
        }

        if (isMove == true)
        {
            //设置方向
            SetDir();

            // 移动
            SetMove();

            // 相机跟随
            SetCamera();
        }

        
    }

    public void Init() {
        camTrans = Camera.main.transform;
        camOffset = transform.position - camTrans.position;
    }

    private void SetDir() {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0,1)) +camTrans.localEulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    private void SetMove() {
        ctrl.Move(transform.forward * Time.deltaTime * Constants.PlayerMoveSpeed);
    }
    private void SetCamera() {
        if (camTrans !=null)
        {
            camTrans.position = transform.position - camOffset;
        }
    }

    public void SetBlend(float blend) {
        targetBlend = blend;
    }

    private void UpdateMixBlend() {
        if (Mathf.Abs(currentBlend - targetBlend) < Constants.AccelerSpeed * Time.deltaTime)
        {
            currentBlend = targetBlend;
        }
        else if (currentBlend > targetBlend)
        {
            currentBlend -= Constants.AccelerSpeed * Time.deltaTime;
        }
        else {
            currentBlend += Constants.AccelerSpeed * Time.deltaTime;
        }

        ani.SetFloat("Blend",currentBlend);
    }
}
