using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020004CA RID: 1226
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class SystemInitializerAttribute : Attribute
	{
		// Token: 0x06001BCB RID: 7115 RVA: 0x00014964 File Offset: 0x00012B64
		public SystemInitializerAttribute(params Type[] dependencies)
		{
			if (dependencies != null)
			{
				this.dependencies = dependencies;
			}
		}

		// Token: 0x06001BCC RID: 7116 RVA: 0x00089394 File Offset: 0x00087594
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

		// Token: 0x04001E25 RID: 7717
		public Type[] dependencies = Array.Empty<Type>();

		// Token: 0x04001E26 RID: 7718
		private MethodInfo methodInfo;

		// Token: 0x04001E27 RID: 7719
		private Type associatedType;
	}
}
