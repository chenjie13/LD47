/****************************************************************************
 * Copyright (c) 2017 snowcold
 * Copyright (c) 2017 ~ 2018.5 liangxie
 * 
 * http://qframework.io
 * https://github.com/liangxiegame/QFramework
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/


namespace CGDK.Core
{
	using System.Reflection;
	using UnityEngine;
	using System.Linq;

	public static class MonoSingletonCreator
	{

		public static T CreateMonoSingleton<T>() where T : MonoBehaviour, ISingleton
		{
			T inst = null;

			inst = Object.FindObjectOfType<T>();

			if (inst == null)
			{
				var obj = new GameObject(typeof(T).Name);
				inst = obj.AddComponent<T>();
			}
			
			Object.DontDestroyOnLoad(inst.gameObject);

			var attribute = typeof(T).GetCustomAttribute<MonoSingletonPathAttribute>();
			if (attribute != null && !string.IsNullOrEmpty(attribute.PathInHierarchy))
			{
				var subPath = attribute.PathInHierarchy.Split('/');
				if (subPath.Length > 0)
				{
					inst.gameObject.name = subPath.Last();
					var parent = CreateOrFindPath(null, subPath, 0, subPath.Length - 1);
					if (parent != null)
					{
						inst.transform.SetParent(parent.transform);
					}
				}
			}
			
			inst.OnSingletonInit();
			return inst;
		}

		private static GameObject CreateOrFindPath(GameObject root, string[] subPath, int index, int length)
		{
			if (index >= length) return root;
			
			GameObject current = null;

			if (root == null)
			{
				current = GameObject.Find(subPath[index]);
			}
			else
			{
				var child = root.transform.Find(subPath[index]);
				if (child != null)
				{
					current = child.gameObject;
				}
			}

			if (current == null)
			{
				current = new GameObject(subPath[index]);
				if (root != null)
				{
					current.transform.SetParent(root.transform);
				}

				if (index == 0)
				{
					Object.DontDestroyOnLoad(current);
				}
			}

			return CreateOrFindPath(current, subPath, ++index, length);
		}
	}
}