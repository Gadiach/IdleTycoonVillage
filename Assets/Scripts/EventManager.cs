using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EventManager : MonoBehaviour
{
    public bool LimitQueueProcessing = false;
    public float QueueProcessTime = 0.0f;
    private static EventManager s_Instance = null;
    private Queue<GameEvent> m_eventQueue = new Queue<GameEvent>();

    public delegate void EventDelegate<T>(T e) where T : GameEvent;
    private delegate void EventDelegate(GameEvent e);

    private Dictionary<System.Type, EventDelegate> delegates = new Dictionary<System.Type, EventDelegate>();
    private Dictionary<System.Delegate, EventDelegate> delegateLookup = new Dictionary<System.Delegate, EventDelegate>();
    private Dictionary<System.Delegate, System.Delegate> onceLookups = new Dictionary<System.Delegate, System.Delegate>();

    //private void Awake()
    //{
    //    if (s_Instance == null)
    //    {
    //        s_Instance = this;
    //        DontDestroyOnLoad(gameObject); 
    //    }
    //    else
    //    {
    //        Debug.LogError("Удаляется EventManager: " + gameObject.name);
    //        Destroy(gameObject);
    //    }
    //}

    public static EventManager Instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = GameObject.FindObjectOfType<EventManager>();
            }
            return s_Instance;
        }
    }

    private EventDelegate AddDelegate<T>(EventDelegate<T> del) where T : GameEvent
    {
        if (delegateLookup.ContainsKey(del))
            return null;

        EventDelegate internalDelegate = (e) => del((T)e);
        delegateLookup[del] = internalDelegate;

        EventDelegate tempDel;
        if (delegates.TryGetValue(typeof(T), out tempDel))
        {
            delegates[typeof(T)] = tempDel += internalDelegate;
        }
        else
        {
            delegates[typeof(T)] = internalDelegate;
        }

        return internalDelegate;
    }

    public void AddListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        AddDelegate<T>(del);
    }

    public void AddListenerOnce<T>(EventDelegate<T> del) where T : GameEvent
    {
        EventDelegate result = AddDelegate<T>(del);

        if (result != null)
        {
            onceLookups[result] = del;
        }
    }

    public void RemoveListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        EventDelegate internalDelegate;
        if (delegateLookup.TryGetValue(del, out internalDelegate))
        {
            EventDelegate tempDel;
            if (delegates.TryGetValue(typeof(T), out tempDel))
            {
                tempDel -= internalDelegate;
                if (tempDel == null)
                {
                    delegates.Remove(typeof(T));
                }
                else
                {
                    delegates[typeof(T)] = tempDel;
                }
            }

            delegateLookup.Remove(del);
        }
    }

    public void RemoveAll()
    {
        delegates.Clear();
        delegateLookup.Clear();
        onceLookups.Clear();
    }

    public bool HasListener<T>(EventDelegate<T> del) where T : GameEvent
    {
        return delegateLookup.ContainsKey(del);
    }

    public void TriggerEvent(GameEvent e)
    {
        EventDelegate del;
        if (delegates.TryGetValue(e.GetType(), out del))
        {
            var invocationList = del.GetInvocationList();
            foreach (var handler in invocationList)
            {
                ((EventDelegate)handler).Invoke(e);
            }

            foreach (var handler in invocationList)
            {
                if (onceLookups.ContainsKey(handler))
                {
                    delegates[e.GetType()] -= (EventDelegate)handler;

                    if (delegates[e.GetType()] == null)
                    {
                        delegates.Remove(e.GetType());
                    }

                    delegateLookup.Remove(onceLookups[handler]);
                    onceLookups.Remove(handler);
                }
            }
        }
        else
        {
            Debug.LogWarning("Event: " + e.GetType() + " has no listeners");
        }
    }

    public bool QueueEvent(GameEvent evt)
    {
        if (!delegates.ContainsKey(evt.GetType()))
        {
            Debug.LogWarning("EventManager: QueueEvent failed due to no listeners for event: " + evt.GetType());
            return false;
        }

        m_eventQueue.Enqueue(evt);
        return true;
    }

    void Update()
    {
        float timer = 0.0f;
        while (m_eventQueue.Count > 0)
        {
            if (LimitQueueProcessing)
            {
                if (timer > QueueProcessTime)
                    return;
            }

            GameEvent evt = m_eventQueue.Dequeue();
            TriggerEvent(evt);

            if (LimitQueueProcessing)
                timer += Time.deltaTime;
        }
    }

    private void OnApplicationQuit()
    {
        RemoveAll();
        m_eventQueue.Clear();
        s_Instance = null;
    }
}