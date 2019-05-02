using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000470 RID: 1136
	public class ProjectIssueChecker
	{
		// Token: 0x06001969 RID: 6505 RVA: 0x00012E67 File Offset: 0x00011067
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

		// Token: 0x0600196A RID: 6506 RVA: 0x00083620 File Offset: 0x00081820
		private ProjectIssueChecker()
		{
			this.assetCheckMethods = new Dictionary<Type, List<MethodInfo>>();
			this.allChecks = new List<MethodInfo>();
			this.enabledChecks = new Dictionary<MethodInfo, bool>();
			Type[] types = typeof(RoR2Application).Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					foreach (object obj in methodInfo.GetCustomAttributes(true))
					{
						if (obj is AssetCheckAttribute)
						{
							List<MethodInfo> list = null;
							Type assetType = ((AssetCheckAttribute)obj).assetType;
							this.assetCheckMethods.TryGetValue(assetType, out list);
							if (list == null)
							{
								list = new List<MethodInfo>();
								this.assetCheckMethods[assetType] = list;
							}
							list.Add(methodInfo);
							this.allChecks.Add(methodInfo);
							this.enabledChecks.Add(methodInfo, true);
						}
					}
				}
			}
		}

		// Token: 0x0600196B RID: 6507 RVA: 0x0008376C File Offset: 0x0008196C
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

		// Token: 0x0600196C RID: 6508 RVA: 0x0008381C File Offset: 0x00081A1C
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

		// Token: 0x0600196D RID: 6509 RVA: 0x00083864 File Offset: 0x00081A64
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

		// Token: 0x0600196E RID: 6510 RVA: 0x000838AC File Offset: 0x00081AAC
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

		// Token: 0x0600196F RID: 6511 RVA: 0x000838F8 File Offset: 0x00081AF8
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

		// Token: 0x06001970 RID: 6512 RVA: 0x00083944 File Offset: 0x00081B44
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

		// Token: 0x04001CCD RID: 7373
		private Dictionary<Type, List<MethodInfo>> assetCheckMethods;

		// Token: 0x04001CCE RID: 7374
		private List<MethodInfo> allChecks;

		// Token: 0x04001CCF RID: 7375
		private Dictionary<MethodInfo, bool> enabledChecks;

		// Token: 0x04001CD0 RID: 7376
		private bool checkScenes = true;

		// Token: 0x04001CD1 RID: 7377
		private List<string> scenesToCheck = new List<string>();

		// Token: 0x04001CD2 RID: 7378
		private string currentAssetPath = "";

		// Token: 0x04001CD3 RID: 7379
		private readonly Stack<UnityEngine.Object> assetStack = new Stack<UnityEngine.Object>();

		// Token: 0x04001CD4 RID: 7380
		private UnityEngine.Object currentAsset;

		// Token: 0x04001CD5 RID: 7381
		private List<ProjectIssueChecker.LogMessage> log = new List<ProjectIssueChecker.LogMessage>();

		// Token: 0x04001CD6 RID: 7382
		private string currentSceneName = "";

		// Token: 0x02000471 RID: 1137
		// (Invoke) Token: 0x06001972 RID: 6514
		private delegate void ObjectCheckDelegate(ProjectIssueChecker issueChecker, UnityEngine.Object obj);

		// Token: 0x02000472 RID: 1138
		private struct LogMessage
		{
			// Token: 0x04001CD7 RID: 7383
			public bool error;

			// Token: 0x04001CD8 RID: 7384
			public string message;

			// Token: 0x04001CD9 RID: 7385
			public UnityEngine.Object context;

			// Token: 0x04001CDA RID: 7386
			public string assetPath;
		}
	}
}
