using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShotEffect
{
    public ParticlePool muzzlePool;
    public TransformPool trailPool;

    // Start is called before the first frame update
    public ShotEffect()
    {
        muzzlePool = new ParticlePool("GunFIreVFX");
        trailPool = new TransformPool("BulletTrail");
    }
    public void Reset()
    {
        muzzlePool.Reset(); 
        trailPool.Reset(); 
    }

}

public abstract class EffectPool<T> where T : Component
{
    protected Queue<T> queue = new Queue<T>();
    protected GameObject prefab;
    protected Transform folder;
    public EffectPool(string name)
    {
        prefab = (GameObject)ResourceManager.GetInstance.GetPreLoad[name];
    }
    protected abstract void Enqueue(T target);
    protected abstract T Dequeue();
    public abstract void PlayEffect(Vector3 start,Vector3 target);
    public abstract void Reset();
    protected abstract T Instantiate();
}
public class TransformPool : EffectPool<Transform>
{
    public TransformPool(string name) : base(name)
    {
        prefab = (GameObject)ResourceManager.GetInstance.GetPreLoad[name];
        folder = new GameObject(name).transform;
        Enqueue(Instantiate());
    }
    protected override Transform Dequeue()
    {
        queue.TryDequeue(out Transform tr);
        if (DOTween.IsTweening(tr))
        {
            queue.Enqueue(tr);
            return Instantiate();
        }
        else
        {
            tr.gameObject.SetActive(true);
            return tr;
        }
    }

    protected override void Enqueue(Transform target)
    {
        target.gameObject.SetActive(false);
        queue.Enqueue(target);
    }

    public override void PlayEffect(Vector3 start, Vector3 target)
    {
        Transform tr = Dequeue();
        tr.transform.position = start;
        float dist = Vector3.Distance(start, target);
        
        tr.DOMove(target,dist/300f );
        Enqueue(tr);
    }
    public override void Reset()
    {
        queue.Clear();
    }
    protected override Transform Instantiate()
    {
        GameObject obj = GameObject.Instantiate(prefab);
        obj.SetActive(true);
        obj.transform.parent = folder;
        return GameObject.Instantiate(prefab).transform;
    }
}
public class ParticlePool : EffectPool<ParticleSystem>
{
    public ParticlePool(string name) : base(name)
    {
        prefab = (GameObject)ResourceManager.GetInstance.GetPreLoad[name];
        folder = new GameObject(name).transform;
        Enqueue(Instantiate());
    }
    protected override ParticleSystem Dequeue()
    {
        queue.TryDequeue(out ParticleSystem particle);
        if (particle.isPlaying)
        {
            queue.Enqueue(particle);
            return Instantiate();
        }
        else
        {
            particle.gameObject.SetActive(true);
            return particle;
        }
    }

    protected override void Enqueue(ParticleSystem target)
    {
        target.gameObject.SetActive(false);
        queue.Enqueue(target);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="start">이펙트 위치</param>
    /// <param name="eulerAngle">이펙트 각도</param>
    /// <param name="time">미사용 매개변수</param>
    public override void PlayEffect(Vector3 start, Vector3 eulerAngle)
    {
        ParticleSystem particle = Dequeue();
        particle.transform.position = start;
        particle.transform.eulerAngles = eulerAngle;
        Enqueue(particle);
    }
    public override void Reset()
    {
        queue.Clear();
    }
    protected override ParticleSystem Instantiate()
    {
        GameObject obj = GameObject.Instantiate(prefab);
        obj.SetActive(true);
        obj.transform.parent = folder;
        return obj.GetComponent<ParticleSystem>();
    }
}