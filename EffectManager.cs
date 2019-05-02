using System;
using System.Collections.Generic;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002E0 RID: 736
	public class EffectManager : MonoBehaviour
	{
		// Token: 0x1700013E RID: 318
		// (get) Token: 0x06000EC0 RID: 3776 RVA: 0x0000B5D8 File Offset: 0x000097D8
		// (set) Token: 0x06000EBF RID: 3775 RVA: 0x0000B5D0 File Offset: 0x000097D0
		public static EffectManager instance { get; private set; }

		// Token: 0x06000EC1 RID: 3777 RVA: 0x0000B5DF File Offset: 0x000097DF
		private void OnEnable()
		{
			if (EffectManager.instance)
			{
				Debug.LogError("Only one EffectManager can exist at a time.");
				return;
			}
			EffectManager.instance = this;
		}

		// Token: 0x06000EC2 RID: 3778 RVA: 0x0000B5FE File Offset: 0x000097FE
		private void OnDisable()
		{
			if (EffectManager.instance == this)
			{
				EffectManager.instance = null;
			}
		}

		// Token: 0x06000EC3 RID: 3779 RVA: 0x0005A0AC File Offset: 0x000582AC
		private void Awake()
		{
			this.effectPrefabsList = new List<GameObject>(Resources.LoadAll<GameObject>("Prefabs/Effects/"));
			uint num = 0u;
			while ((ulong)num < (ulong)((long)this.effectPrefabsList.Count))
			{
				this.effectPrefabToIndexMap[this.effectPrefabsList[(int)num]] = num;
				num += 1u;
			}
		}

		// Token: 0x06000EC4 RID: 3780 RVA: 0x0000B613 File Offset: 0x00009813
		[NetworkMessageHandler(msgType = 52, server = true)]
		private static void HandleEffectServer(NetworkMessage netMsg)
		{
			if (EffectManager.instance)
			{
				EffectManager.instance.HandleEffectServerInternal(netMsg);
			}
		}

		// Token: 0x06000EC5 RID: 3781 RVA: 0x0000B62C File Offset: 0x0000982C
		[NetworkMessageHandler(msgType = 52, client = true)]
		private static void HandleEffectClient(NetworkMessage netMsg)
		{
			if (EffectManager.instance)
			{
				EffectManager.instance.HandleEffectClientInternal(netMsg);
			}
		}

		// Token: 0x06000EC6 RID: 3782 RVA: 0x0005A100 File Offset: 0x00058300
		public void SpawnEffect(GameObject effectPrefab, EffectData effectData, bool transmit)
		{
			if (transmit)
			{
				this.TransmitEffect(effectPrefab, effectData, null);
				if (NetworkServer.active)
				{
					return;
				}
			}
			if (NetworkClient.active)
			{
				if (!VFXBudget.CanAffordSpawn(effectPrefab))
				{
					return;
				}
				EffectData effectData2 = effectData.Clone();
				EffectComponent component = UnityEngine.Object.Instantiate<GameObject>(effectPrefab, effectData2.origin, effectData2.rotation).GetComponent<EffectComponent>();
				if (component)
				{
					component.effectData = effectData2.Clone();
				}
			}
		}

		// Token: 0x06000EC7 RID: 3783 RVA: 0x0005A164 File Offset: 0x00058364
		private void TransmitEffect(GameObject effectPrefab, EffectData effectData, NetworkConnection netOrigin = null)
		{
			uint effectPrefabIndex;
			if (!this.LookupEffectPrefabIndex(effectPrefab, out effectPrefabIndex))
			{
				return;
			}
			this.TransmitEffect(effectPrefabIndex, effectData, netOrigin);
		}

		// Token: 0x06000EC8 RID: 3784 RVA: 0x0005A188 File Offset: 0x00058388
		private void TransmitEffect(uint effectPrefabIndex, EffectData effectData, NetworkConnection netOrigin = null)
		{
			EffectManager.outgoingEffectMessage.effectPrefabIndex = effectPrefabIndex;
			EffectData.Copy(effectData, EffectManager.outgoingEffectMessage.effectData);
			if (NetworkServer.active)
			{
				using (IEnumerator<NetworkConnection> enumerator = NetworkServer.connections.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						NetworkConnection networkConnection = enumerator.Current;
						if (networkConnection != null && networkConnection != netOrigin)
						{
							networkConnection.SendByChannel(52, EffectManager.outgoingEffectMessage, QosChannelIndex.effects.intVal);
						}
					}
					return;
				}
			}
			if (ClientScene.readyConnection != null)
			{
				ClientScene.readyConnection.SendByChannel(52, EffectManager.outgoingEffectMessage, QosChannelIndex.effects.intVal);
			}
		}

		// Token: 0x06000EC9 RID: 3785 RVA: 0x0000B645 File Offset: 0x00009845
		private bool LookupEffectPrefabIndex(GameObject effectPrefab, out uint effectPrefabIndex)
		{
			if (!this.effectPrefabToIndexMap.TryGetValue(effectPrefab, out effectPrefabIndex))
			{
				Debug.LogErrorFormat("Attempted to find effect index for prefab \"{0}\" which is not in Resources/Prefabs/Effects.", new object[]
				{
					effectPrefab
				});
				return false;
			}
			return true;
		}

		// Token: 0x06000ECA RID: 3786 RVA: 0x0000B66D File Offset: 0x0000986D
		private GameObject LookupEffectPrefab(uint effectPrefabIndex)
		{
			if ((ulong)effectPrefabIndex >= (ulong)((long)this.effectPrefabsList.Count))
			{
				Debug.LogErrorFormat("Attempted to find effect prefab for bad index #{0}.", new object[]
				{
					effectPrefabIndex
				});
				return null;
			}
			return this.effectPrefabsList[(int)effectPrefabIndex];
		}

		// Token: 0x06000ECB RID: 3787 RVA: 0x0005A234 File Offset: 0x00058434
		private void HandleEffectClientInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<EffectManager.EffectMessage>(EffectManager.incomingEffectMessage);
			GameObject gameObject = this.LookupEffectPrefab(EffectManager.incomingEffectMessage.effectPrefabIndex);
			if (gameObject == null)
			{
				return;
			}
			this.SpawnEffect(gameObject, EffectManager.incomingEffectMessage.effectData, false);
		}

		// Token: 0x06000ECC RID: 3788 RVA: 0x0005A27C File Offset: 0x0005847C
		private void HandleEffectServerInternal(NetworkMessage netMsg)
		{
			netMsg.ReadMessage<EffectManager.EffectMessage>(EffectManager.incomingEffectMessage);
			GameObject gameObject = this.LookupEffectPrefab(EffectManager.incomingEffectMessage.effectPrefabIndex);
			if (gameObject == null)
			{
				return;
			}
			this.TransmitEffect(gameObject, EffectManager.incomingEffectMessage.effectData, netMsg.conn);
		}

		// Token: 0x06000ECD RID: 3789 RVA: 0x0005A2C8 File Offset: 0x000584C8
		public void SimpleMuzzleFlash(GameObject effectPrefab, GameObject obj, string muzzleName, bool transmit)
		{
			if (!obj)
			{
				return;
			}
			ModelLocator component = obj.GetComponent<ModelLocator>();
			if (component && component.modelTransform)
			{
				ChildLocator component2 = component.modelTransform.GetComponent<ChildLocator>();
				if (component2)
				{
					int childIndex = component2.FindChildIndex(muzzleName);
					Transform transform = component2.FindChild(childIndex);
					if (transform)
					{
						EffectData effectData = new EffectData
						{
							origin = transform.position
						};
						effectData.SetChildLocatorTransformReference(obj, childIndex);
						EffectManager.instance.SpawnEffect(effectPrefab, effectData, transmit);
					}
				}
			}
		}

		// Token: 0x06000ECE RID: 3790 RVA: 0x0000B6A6 File Offset: 0x000098A6
		public void SimpleImpactEffect(GameObject effectPrefab, Vector3 hitPos, Vector3 normal, bool transmit)
		{
			this.SpawnEffect(effectPrefab, new EffectData
			{
				origin = hitPos,
				rotation = ((normal == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(normal))
			}, transmit);
		}

		// Token: 0x06000ECF RID: 3791 RVA: 0x0005A354 File Offset: 0x00058554
		public void SimpleImpactEffect(GameObject effectPrefab, Vector3 hitPos, Vector3 normal, Color color, bool transmit)
		{
			this.SpawnEffect(effectPrefab, new EffectData
			{
				origin = hitPos,
				rotation = ((normal == Vector3.zero) ? Quaternion.identity : Util.QuaternionSafeLookRotation(normal)),
				color = color
			}, transmit);
		}

		// Token: 0x06000ED0 RID: 3792 RVA: 0x0000B6DD File Offset: 0x000098DD
		public void SimpleEffect(GameObject effectPrefab, Vector3 position, Quaternion rotation, bool transmit)
		{
			this.SpawnEffect(effectPrefab, new EffectData
			{
				origin = position,
				rotation = rotation
			}, transmit);
		}

		// Token: 0x040012CA RID: 4810
		private List<GameObject> effectPrefabsList;

		// Token: 0x040012CB RID: 4811
		private readonly Dictionary<GameObject, uint> effectPrefabToIndexMap = new Dictionary<GameObject, uint>();

		// Token: 0x040012CC RID: 4812
		private static readonly EffectManager.EffectMessage outgoingEffectMessage = new EffectManager.EffectMessage();

		// Token: 0x040012CD RID: 4813
		private static readonly EffectManager.EffectMessage incomingEffectMessage = new EffectManager.EffectMessage();

		// Token: 0x020002E1 RID: 737
		private class EffectMessage : MessageBase
		{
			// Token: 0x06000ED3 RID: 3795 RVA: 0x0000B724 File Offset: 0x00009924
			public override void Serialize(NetworkWriter writer)
			{
				writer.WritePackedUInt32(this.effectPrefabIndex);
				writer.Write(this.effectData);
			}

			// Token: 0x06000ED4 RID: 3796 RVA: 0x0000B73E File Offset: 0x0000993E
			public override void Deserialize(NetworkReader reader)
			{
				this.effectPrefabIndex = reader.ReadPackedUInt32();
				reader.ReadEffectData(this.effectData);
			}

			// Token: 0x040012CE RID: 4814
			public uint effectPrefabIndex;

			// Token: 0x040012CF RID: 4815
			public readonly EffectData effectData = new EffectData();
		}
	}
}
