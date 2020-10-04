using System;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

namespace CGDK.Core
{
    /// <summary>
    /// 简易的事件管理器
    /// 按监听事件类型来触发事件
    /// </summary>
    public static class EventBus
    {
        private static Dictionary<Type, List<object>> _subscribeDic;

        static EventBus()
        {
            _subscribeDic = new Dictionary<Type, List<object>>();
        }

        public static void Subscribe<T>(Action<T> cb)
        {
            if (cb == null) return;
            var eventType = typeof(T);
            if (!_subscribeDic.ContainsKey(eventType)) _subscribeDic[eventType] = new List<object>();
            if (!IsSubscribe(cb)) _subscribeDic[eventType].Add(cb);
        }

        public static void Unsubscribe<T>(Action<T> cb)
        {
            var eventType = typeof(T);

            if (!_subscribeDic.ContainsKey(eventType)) return;

            var subscriberList = _subscribeDic[eventType];

            for (var i = 0; i < subscriberList.Count; i++)
            {
                if ((Action<T>) subscriberList[i] == cb)
                {
                    subscriberList.Remove(subscriberList[i]);
                    if (subscriberList.Count == 0) _subscribeDic.Remove(eventType);
                    return;
                }
            }
        }

        public static void Trigger<T>(T @event)
        {
            if (!_subscribeDic.TryGetValue(typeof(T), out var list))
            {
                LogUtil.Warning($"Trigger a no listener event, type: {typeof(T)}");
                return;
            }

            for (var i = 0; i < list.Count; i++)
            {
                ((Action<T>) list[i])(@event);
            }
        }

        public static bool IsSubscribe<T>(Action<T> cb)
        {
            if (!_subscribeDic.TryGetValue(typeof(T), out var receivers)) return false;

            for (var i = 0; i < receivers.Count; i++)
            {
                if ((Action<T>) receivers[i] == cb) return true;
            }

            return false;
        }
    }
}