using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EEffect
{
    Explode_Block_Red_Eff,    //기존 빨강 블록 폭발
    Explode_Block_Blue_Eff,    //기존 파랑 블록 폭발
    Explode_Block_Green_Eff,    //기존 초록 블록 폭발
    Explode_Block_Yellow_Eff,    //기존 노랑 블록 폭발
    Hit_Spark_Eff,      //광선이 블록에 닿았을때 튀는 이펙트

}
[Serializable]
public class EffectData
{
    public EEffect effectType;
    public GameObject prefab;
    public int poolSize = 5;
    [Header("파티클 유지 시간")]
    public float duration = 1.5f;
    [HideInInspector] public Queue<GameObject> pool=new Queue<GameObject>();
    [HideInInspector] public Transform rootTrs;
}
public class EffectManager : MonoBehaviour
{
    public static EffectManager Instance { get; private set; }
    [Header("Effect 목록")]
    [SerializeField] private List<EffectData> _effectDataList;

    private Dictionary<EEffect, EffectData> _effDataDic = new Dictionary<EEffect, EffectData>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var data in _effectDataList)
        {
            GameObject rootObj = new GameObject(data.effectType.ToString() + "_Pool");
            rootObj.transform.SetParent(transform);
            data.rootTrs = rootObj.transform;

            for (int i = 0; i < data.poolSize; i++)
            {
                GameObject obj = Instantiate(data.prefab, data.rootTrs);
                obj.SetActive(false);
                data.pool.Enqueue(obj);
            }
            _effDataDic.Add(data.effectType, data);
        }
    }


    public void PlayEffect(EEffect effect, Vector3 position)
    {
        // 등록되지 않은 이펙트를 불렀다면 무시
        if (!_effDataDic.ContainsKey(effect)) return;

        EffectData data= _effDataDic[effect];
        GameObject fx = null;

        // 해당 종류의 창고에서 꺼내오기 (창고가 비었으면 하나 더 만들기)
        if (data.pool.Count > 0)
        {
            fx = data.pool.Dequeue();
        }
        else
        {
            fx = Instantiate(data.prefab, data.rootTrs);
        }

        fx.transform.position = position;
        fx.SetActive(true);
        StartCoroutine(ReturnToPoolRoutine(data, fx));
    }

    private IEnumerator ReturnToPoolRoutine(EffectData data, GameObject fx)
    {
        yield return new WaitForSeconds(data.duration);
        fx.SetActive(false);
        fx.transform.SetParent(data.rootTrs);
        data.pool.Enqueue(fx);
    }

}
