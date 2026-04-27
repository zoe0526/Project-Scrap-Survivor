using System.Collections.Generic;
using UnityEngine;

public abstract class GenericPool<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    [Header("Pool Settings")]
    [SerializeField] protected GameObject prefab;       // 풀링할 프리팹
    [SerializeField] protected int poolSize = 50;       // 초기 생성 개수
    [SerializeField] protected Transform poolParent;    // 창고 폴더

    protected Queue<GameObject> pool = new Queue<GameObject>();

    protected virtual void Awake()
    {
        // 싱글톤 초기화
        if (Instance == null)
        {
            Instance = this as T;
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateNewObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject newObj = Instantiate(prefab);
        newObj.transform.SetParent(poolParent, false); // false 필수!
        newObj.SetActive(false);
        pool.Enqueue(newObj);
        return newObj;
    }

    // 오브젝트 빌려가기
    public GameObject Get()
    {
        if (pool.Count == 0) CreateNewObject();

        GameObject obj = pool.Dequeue();
        obj.transform.SetParent(null); // 일단 부모 종속을 풀고 내보냄 (스포너가 알아서 부모 설정하도록)
        obj.SetActive(true);
        return obj;
    }

    // 오브젝트 반납하기
    public void ReturnToPool(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(poolParent, false); // 다시 창고로 쏙
        pool.Enqueue(obj);
    }
}
