using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Object pool for gameobjects
public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject poolObject = null;
    private List<GameObject> active = new List<GameObject>();
    public int ActiveCount() { return active.Count; }
    private Queue<GameObject> deactive = new Queue<GameObject>();
    public int DeactiveCount() { return deactive.Count; }



    public GameObject Get()
    {
        GameObject obj = deactive.Count > 0 ? deactive.Dequeue() : Instantiate();
        active.Add(obj);
        obj.SetActive(true);
        return obj;
    }
    public void Return(GameObject obj)
    {
        if (!active.Contains(obj))
        {
            Debug.LogError("Card to be removed not in pool");
            return;
        }

        active.Remove(obj);
        deactive.Enqueue(obj);
        obj.SetActive(false);
    }
    public void ReturnAll()
    {
        foreach(var obj in active)
        {
            deactive.Enqueue(obj);
            obj.SetActive(false);
        }

        active.Clear();
    }

    public int objectCount = 0;
    private GameObject Instantiate()
    {
        GameObject obj = Instantiate(poolObject);
        obj.name = poolObject.name + " " + objectCount++.ToString();
        return obj;
    }
}
