using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000450 RID: 1104
	public static class InstanceTracker
	{
		// Token: 0x060018BE RID: 6334 RVA: 0x000128F3 File Offset: 0x00010AF3
		public static void Add<T>([NotNull] T instance) where T : MonoBehaviour
		{
			InstanceTracker.TypeData<T>.Add(instance);
		}

		// Token: 0x060018BF RID: 6335 RVA: 0x000128FB File Offset: 0x00010AFB
		public static void Remove<T>([NotNull] T instance) where T : MonoBehaviour
		{
			InstanceTracker.TypeData<T>.Remove(instance);
		}

		// Token: 0x060018C0 RID: 6336 RVA: 0x00012903 File Offset: 0x00010B03
		[NotNull]
		public static List<T> GetInstancesList<T>() where T : MonoBehaviour
		{
			return InstanceTracker.TypeData<T>.instancesList;
		}

		// Token: 0x060018C1 RID: 6337 RVA: 0x0007F4D4 File Offset: 0x0007D6D4
		[NotNull]
		public static IEnumerable<MonoBehaviour> FindInstancesEnumerable([NotNull] Type t)
		{
			IEnumerable<MonoBehaviour> result;
			if (!InstanceTracker.instancesLists.TryGetValue(t, out result))
			{
				return Enumerable.Empty<MonoBehaviour>();
			}
			return result;
		}

		// Token: 0x04001BCB RID: 7115
		private static readonly Dictionary<Type, IEnumerable<MonoBehaviour>> instancesLists = new Dictionary<Type, IEnumerable<MonoBehaviour>>();

		// Token: 0x02000451 RID: 1105
		private static class TypeData<T> where T : MonoBehaviour
		{
			// Token: 0x060018C3 RID: 6339 RVA: 0x00012916 File Offset: 0x00010B16
			static TypeData()
			{
				InstanceTracker.instancesLists[typeof(T)] = InstanceTracker.TypeData<T>.instancesList;
			}

			// Token: 0x060018C4 RID: 6340 RVA: 0x0001293B File Offset: 0x00010B3B
			public static void Add(T instance)
			{
				InstanceTracker.TypeData<T>.instancesList.Add(instance);
			}

			// Token: 0x060018C5 RID: 6341 RVA: 0x00012948 File Offset: 0x00010B48
			public static void Remove(T instance)
			{
				InstanceTracker.TypeData<T>.instancesList.Remove(instance);
			}

			// Token: 0x04001BCC RID: 7116
			public static readonly List<T> instancesList = new List<T>();
		}
	}
}
