using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200047B RID: 1147
	public class ProjectIssueChecker
	{
		// Token: 0x060019C6 RID: 6598 RVA: 0x00013381 File Offset: 0x00011581
		private static IEnumerable<Assembly> GetAssemblies()
		{
			List<string> list = new List<string>();
			Stack<Assembly> stack = new Stack<Assembly>();
			stack.Push(Assembly.GetEntryAssembly());
			do
			{
				Assembly asm = stack.Pop();
				yield return asm;
				foreach (AssemblyName assemblyName in asm.GetReferencedAssemblies())
				{
					if (!list.Contains(assemblyName.FullName))
					{
						stack.Push(Assembly.Load(assemblyName));
						list.Add(assemblyName.FullName);
					}
				}
				asm = null;
			}
			while (stack.Count > 0);
			yield break;
		}

		// Token: 0x060019C7 RID: 6599 RVA: 0x00083FC8 File Offset: 0x000821C8
		private ProjectIssueChecker()
		{
			this.assetCheckMethods = new Dictionary<Type, List<MethodInfo>>();
			this.allChecks = new List<MethodInfo>();
			this.enabledChecks = new Dictionary<MethodInfo, bool>();
			Assembly[] source = new Assembly[]
			{
				typeof(RoR2Application).Assembly,
				typeof(TMP_Text).Assembly
			};
			ProjectIssueChecker.<>c__DisplayClass7_0 CS$<>8__locals1;
			CS$<>8__locals1.types = source.SelectMany((Assembly a) => a.GetTypes()).ToArray<Type>();
			Type[] types = CS$<>8__locals1.types;
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					foreach (object obj in methodInfo.GetCustomAttributes(true))
					{
						if (obj is AssetCheckAttribute)
						{
							Type assetType = ((AssetCheckAttribute)obj).assetType;
							this.<.ctor>g__AddMethodForTypeDescending|7_1(assetType, methodInfo, ref CS$<>8__locals1);
							this.allChecks.Add(methodInfo);
							this.enabledChecks.Add(methodInfo, true);
						}
					}
				}
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x0008413C File Offset: 0x0008233C
		private string GetCurrentAssetFullPath()
		{
			GameObject gameObject = null;
			string arg = "";
			if (this.currentAsset is GameObject)
			{
				gameObject = (GameObject)this.currentAsset;
			}
			else if (this.currentAsset is Component)
			{
				gameObject = ((Component)this.currentAsset).gameObject;
			}
			string arg2 = this.currentAsset ? this.currentAsset.name : "NULL ASSET";
			if (gameObject)
			{
				arg2 = Util.GetGameObjectHierarchyName(gameObject);
			}
			string arg3 = this.currentAsset ? this.currentAsset.GetType().Name : "VOID";
			return string.Format("{0}:{1}({2})", arg, arg2, arg3);
		}

		// Token: 0x060019C9 RID: 6601 RVA: 0x000841EC File Offset: 0x000823EC
		public void Log(string message, UnityEngine.Object context = null)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = false,
				message = message,
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x060019CA RID: 6602 RVA: 0x00084234 File Offset: 0x00082434
		public void LogError(string message, UnityEngine.Object context = null)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = true,
				message = message,
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x060019CB RID: 6603 RVA: 0x0008427C File Offset: 0x0008247C
		public void LogFormat(UnityEngine.Object context, string format, params object[] args)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = false,
				message = string.Format(format, args),
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x060019CC RID: 6604 RVA: 0x000842C8 File Offset: 0x000824C8
		public void LogErrorFormat(UnityEngine.Object context, string format, params object[] args)
		{
			this.log.Add(new ProjectIssueChecker.LogMessage
			{
				error = true,
				message = string.Format(format, args),
				assetPath = this.GetCurrentAssetFullPath(),
				context = context
			});
		}

		// Token: 0x060019CD RID: 6605 RVA: 0x00084314 File Offset: 0x00082514
		private void FlushLog()
		{
			bool flag = false;
			for (int i = 0; i < this.log.Count; i++)
			{
				if (this.log[i].error)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				foreach (ProjectIssueChecker.LogMessage logMessage in this.log)
				{
					if (logMessage.error)
					{
						Debug.LogErrorFormat(logMessage.context, "[\"{0}\"] {1}", new object[]
						{
							logMessage.assetPath,
							logMessage.message
						});
					}
					else
					{
						Debug.LogFormat(logMessage.context, "[\"{0}\"] {1}", new object[]
						{
							logMessage.assetPath,
							logMessage.message
						});
					}
				}
			}
			this.log.Clear();
		}

		// Token: 0x04001D01 RID: 7425
		private Dictionary<Type, List<MethodInfo>> assetCheckMethods;

		// Token: 0x04001D02 RID: 7426
		private List<MethodInfo> allChecks;

		// Token: 0x04001D03 RID: 7427
		private Dictionary<MethodInfo, bool> enabledChecks;

		// Token: 0x04001D04 RID: 7428
		private bool checkScenes = true;

		// Token: 0x04001D05 RID: 7429
		private List<string> scenesToCheck = new List<string>();

		// Token: 0x04001D06 RID: 7430
		private string currentAssetPath = "";

		// Token: 0x04001D07 RID: 7431
		private readonly Stack<UnityEngine.Object> assetStack = new Stack<UnityEngine.Object>();

		// Token: 0x04001D08 RID: 7432
		private UnityEngine.Object currentAsset;

		// Token: 0x04001D09 RID: 7433
		private List<ProjectIssueChecker.LogMessage> log = new List<ProjectIssueChecker.LogMessage>();

		// Token: 0x04001D0A RID: 7434
		private string currentSceneName = "";

		// Token: 0x0200047C RID: 1148
		// (Invoke) Token: 0x060019D1 RID: 6609
		private delegate void ObjectCheckDelegate(ProjectIssueChecker issueChecker, UnityEngine.Object obj);

		// Token: 0x0200047D RID: 1149
		private struct LogMessage
		{
			// Token: 0x04001D0B RID: 7435
			public bool error;

			// Token: 0x04001D0C RID: 7436
			public string message;

			// Token: 0x04001D0D RID: 7437
			public UnityEngine.Object context;

			// Token: 0x04001D0E RID: 7438
			public string assetPath;
		}
	}
}
