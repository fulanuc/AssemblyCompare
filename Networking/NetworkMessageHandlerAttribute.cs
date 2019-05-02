using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2.Networking
{
	// Token: 0x0200058F RID: 1423
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class NetworkMessageHandlerAttribute : Attribute
	{
		// Token: 0x0600201A RID: 8218 RVA: 0x0009C09C File Offset: 0x0009A29C
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void CollectHandlers()
		{
			NetworkMessageHandlerAttribute.clientMessageHandlers.Clear();
			NetworkMessageHandlerAttribute.serverMessageHandlers.Clear();
			HashSet<short> hashSet = new HashSet<short>();
			Type[] types = typeof(NetworkMessageHandlerAttribute).Assembly.GetTypes();
			for (int i = 0; i < types.Length; i++)
			{
				foreach (MethodInfo methodInfo in types[i].GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					object[] customAttributes = methodInfo.GetCustomAttributes(false);
					for (int k = 0; k < customAttributes.Length; k++)
					{
						NetworkMessageHandlerAttribute networkMessageHandlerAttribute = ((Attribute)customAttributes[k]) as NetworkMessageHandlerAttribute;
						if (networkMessageHandlerAttribute != null)
						{
							networkMessageHandlerAttribute.messageHandler = (NetworkMessageDelegate)Delegate.CreateDelegate(typeof(NetworkMessageDelegate), methodInfo);
							if (networkMessageHandlerAttribute.messageHandler != null)
							{
								if (networkMessageHandlerAttribute.client)
								{
									NetworkMessageHandlerAttribute.clientMessageHandlers.Add(networkMessageHandlerAttribute);
									hashSet.Add(networkMessageHandlerAttribute.msgType);
								}
								if (networkMessageHandlerAttribute.server)
								{
									NetworkMessageHandlerAttribute.serverMessageHandlers.Add(networkMessageHandlerAttribute);
									hashSet.Add(networkMessageHandlerAttribute.msgType);
								}
							}
							if (networkMessageHandlerAttribute.messageHandler == null)
							{
								Debug.LogWarningFormat("Could not register message handler for {0}. The function signature is likely incorrect.", new object[]
								{
									methodInfo.Name
								});
							}
							if (!networkMessageHandlerAttribute.client && !networkMessageHandlerAttribute.server)
							{
								Debug.LogWarningFormat("Could not register message handler for {0}. It is marked as neither server nor client.", new object[]
								{
									methodInfo.Name
								});
							}
						}
					}
				}
			}
			for (short num = 48; num < 71; num += 1)
			{
				if (!hashSet.Contains(num))
				{
					Debug.LogWarningFormat("Network message MsgType.Highest + {0} is unregistered.", new object[]
					{
						(int)(num - 47)
					});
				}
			}
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x0009C248 File Offset: 0x0009A448
		public static void RegisterServerMessages()
		{
			foreach (NetworkMessageHandlerAttribute networkMessageHandlerAttribute in NetworkMessageHandlerAttribute.serverMessageHandlers)
			{
				NetworkServer.RegisterHandler(networkMessageHandlerAttribute.msgType, networkMessageHandlerAttribute.messageHandler);
			}
		}

		// Token: 0x0600201C RID: 8220 RVA: 0x0009C2A4 File Offset: 0x0009A4A4
		public static void RegisterClientMessages(NetworkClient client)
		{
			foreach (NetworkMessageHandlerAttribute networkMessageHandlerAttribute in NetworkMessageHandlerAttribute.clientMessageHandlers)
			{
				client.RegisterHandler(networkMessageHandlerAttribute.msgType, networkMessageHandlerAttribute.messageHandler);
			}
		}

		// Token: 0x0400222B RID: 8747
		public short msgType;

		// Token: 0x0400222C RID: 8748
		public bool server;

		// Token: 0x0400222D RID: 8749
		public bool client;

		// Token: 0x0400222E RID: 8750
		private NetworkMessageDelegate messageHandler;

		// Token: 0x0400222F RID: 8751
		private static List<NetworkMessageHandlerAttribute> clientMessageHandlers = new List<NetworkMessageHandlerAttribute>();

		// Token: 0x04002230 RID: 8752
		private static List<NetworkMessageHandlerAttribute> serverMessageHandlers = new List<NetworkMessageHandlerAttribute>();
	}
}
