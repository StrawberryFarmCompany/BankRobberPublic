using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class Grenade : MonoBehaviour
{
    public Action<Grenade> enqueue;
    public Action explosionDone;
    private ExplosionObject explosion;
    [SerializeField]private ParticleSystem particle;
    [SerializeField]private MeshRenderer mesh;
    [SerializeField]private Rigidbody rb;
    float goalTime = 2f;
    float explodeDoneTime = 7f;
    float currTime = 0f;
    bool throwed = false;
    bool explode = false;
    private void Init()
    {
        //초기화함수
        explosion = null;
        currTime = 0f;
        throwed = false;
        explode = false;
        rb.isKinematic = false;
    }
    private void Update()
    {
        if (throwed)
        {
            currTime += Time.deltaTime;
            if (!explode && currTime > goalTime)
            {
                explosionDone?.Invoke();
                OnExplosion();
            }
            else if(explode && currTime > explodeDoneTime)
            {
                enqueue?.Invoke(this);
            }
        }
    }
    private void OnEnable()
    {
        Init();
    }
    public void OnThrow(Vector3 dir,float power)
    {
        rb.AddForce(dir * power,ForceMode.Force);
        throwed = true;
    }
    private void OnExplosion()
    {
        explode = true;
        particle.Play();
        particle.transform.eulerAngles = Vector3.right*(-90);
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        explosion = new ExplosionObject(3,10,GameManager.GetInstance.GetVecInt(transform.position));
        explosion.Explosion();
    }
    private void OnDestroy()
    {
        enqueue = null;
    }
}
