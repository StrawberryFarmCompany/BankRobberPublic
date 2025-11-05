using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ShotEffect : MonoBehaviour
{
    public ParticleSystem particle;
    public Transform tr;
    public ParticlePool muzzlePool;
    public TransformPool trailPool;
    // Start is called before the first frame update
    void Start()
    {
        muzzlePool = new ParticlePool("muzzleVFX");
        trailPool = new TransformPool("bulletTrail");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

        }
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
}
public class TransformPool : EffectPool<Transform>
{
    public TransformPool(string name) : base(name)
    {
        prefab = (GameObject)ResourceManager.GetInstance.GetPreLoad[name];
        folder = new GameObject(name).transform;
    }
    protected override Transform Dequeue()
    {
        Transform tr = queue.Dequeue();
        if (DOTween.IsTweening(tr))
        {
            queue.Enqueue(tr);

            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(true);
            obj.transform.parent = folder;
            return GameObject.Instantiate(prefab).transform;
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
}
public class ParticlePool : EffectPool<ParticleSystem>
{
    public ParticlePool(string name) : base(name)
    {
        prefab = (GameObject)ResourceManager.GetInstance.GetPreLoad[name];
        folder = new GameObject(name).transform;
    }
    protected override ParticleSystem Dequeue()
    {
        ParticleSystem particle = queue.Dequeue();
        if (particle.isPlaying)
        {
            queue.Enqueue(particle);

            GameObject obj = GameObject.Instantiate(prefab);
            obj.SetActive(true);
            obj.transform.parent = folder;
            return obj.GetComponent<ParticleSystem>();
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
}