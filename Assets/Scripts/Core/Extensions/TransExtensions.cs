using System;
using UnityEngine;

namespace CGDK.Core
{
    public static class TransExtensions
    {
        public static void Reset(this Transform trans)
        {
            trans.localPosition = Vector3.zero;
            trans.localScale = Vector3.one;
            trans.rotation = Quaternion.identity;
        }

        public static Transform CreateChildren(this Transform trans, string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                // LogUtil.Warning($"CreateChildren path is null or empty");
                return trans;
            }
            
            var childNames = path.Split('\\', '/');
            var result = trans;
            foreach (var name in childNames)
            {
                var go = new GameObject(name);
                go.transform.SetParent(result);
                result = go.transform;
            }

            return trans;
        }
    }
}