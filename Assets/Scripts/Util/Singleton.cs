using System;
using UnityEngine;

namespace Util
{
	public abstract class Singleton<T> : MonoBehaviour where T : class
	{
		public static T I { get; private set; }

		protected virtual void Awake()
		{
			ProcessSingleton();
		}
		
		private void  ProcessSingleton()
		{
			if (!Application.isPlaying) return;

			if (I != null)
			{
				Destroy(gameObject); 
				return;
			}

			I = this as T;
			if (I == null)
				throw new InvalidCastException($"Unable to cast {GetType()} to {typeof(T)}");
		}
	}
}