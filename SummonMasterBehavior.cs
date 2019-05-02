using System;
using RoR2.CharacterAI;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F1 RID: 1009
	public class SummonMasterBehavior : NetworkBehaviour
	{
		// Token: 0x0600160A RID: 5642 RVA: 0x0000913D File Offset: 0x0000733D
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x0600160B RID: 5643 RVA: 0x00075F28 File Offset: 0x00074128
		[Server]
		public void OpenSummon(Interactor activator)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.SummonMasterBehavior::OpenSummon(RoR2.Interactor)' called on client");
				return;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.masterPrefab, base.transform.position, base.transform.rotation);
			NetworkServer.Spawn(gameObject);
			CharacterMaster component = gameObject.GetComponent<CharacterMaster>();
			component.SpawnBody(component.bodyPrefab, base.transform.position + Vector3.up * 0.8f, base.transform.rotation);
			AIOwnership component2 = gameObject.GetComponent<AIOwnership>();
			if (component2)
			{
				CharacterBody component3 = activator.GetComponent<CharacterBody>();
				if (component3)
				{
					CharacterMaster master = component3.master;
					if (master)
					{
						component2.ownerMaster = master;
					}
				}
			}
			BaseAI component4 = gameObject.GetComponent<BaseAI>();
			if (component4)
			{
				component4.leader.gameObject = activator.gameObject;
			}
			UnityEngine.Object.Destroy(base.gameObject);
		}

		// Token: 0x0600160D RID: 5645 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x0600160E RID: 5646 RVA: 0x0004A818 File Offset: 0x00048A18
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600160F RID: 5647 RVA: 0x000025F6 File Offset: 0x000007F6
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001967 RID: 6503
		[Tooltip("The master to spawn")]
		public GameObject masterPrefab;
	}
}
