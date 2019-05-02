using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004BC RID: 1212
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SystemInitializerAttribute : Attribute
	{
		// Token: 0x06001B67 RID: 7015 RVA: 0x00014497 File Offset: 0x00012697
		public SystemInitializerAttribute(params Type[] dependencies)
		{
			if (dependencies != null)
			{
				this.dependencies = dependencies;
			}
		}

		// Token: 0x06001B68 RID: 7016 RVA: 0x0008881C File Offset: 0x00086A1C
		public static void Execute()
		{
			Queue<SystemInitializerAttribute> queue = new Queue<SystemInitializerAttribute>();
			foreach (Type type in typeof(SystemInitializerAttribute).Assembly.GetTypes())
			{
				foreach (MethodInfo element in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					SystemInitializerAttribute customAttribute = element.GetCustomAttribute<SystemInitializerAttribute>();
					if (customAttribute != null)
					{
						queue.Enqueue(customAttribute);
						customAttribute.methodInfo = element;
						customAttribute.associatedType = type;
					}
				}
			}
			SystemInitializerAttribute.<>c__DisplayClass4_0 CS$<>8__locals1;
			CS$<>8__locals1.initializedTypes = new HashSet<Type>();
			int num = 0;
			while (queue.Count > 0)
			{
				SystemInitializerAttribute systemInitializerAttribute = queue.Dequeue();
				if (!SystemInitializerAttribute.<Execute>g__InitializerDependenciesMet|4_0(systemInitializerAttribute, ref CS$<>8__locals1))
				{
					queue.Enqueue(systemInitializerAttribute);
					num++;
					if (num >= queue.Count)
					{
						Debug.LogFormat("SystemInitializerAttribute infinite loop detected. currentMethod={0}", new object[]
						{
							systemInitializerAttribute.associatedType.FullName + systemInitializerAttribute.methodInfo.Name
						});
						return;
					}
				}
				else
				{
					systemInitializerAttribute.methodInfo.Invoke(null, Array.Empty<object>());
					CS$<>8__locals1.initializedTypes.Add(systemInitializerAttribute.associatedType);
					num = 0;
				}
			}
		}

		// Token: 0x04001DEB RID: 7659
		public Type[] dependencies = Array.Empty<Type>();

		// Token: 0x04001DEC RID: 7660
		private MethodInfo methodInfo;

		// Token: 0x04001DED RID: 7661
		private Type associatedType;
	}
}
