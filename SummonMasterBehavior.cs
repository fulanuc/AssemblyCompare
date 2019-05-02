using System;
using RoR2.CharacterAI;
using RoR2.Networking;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020003F7 RID: 1015
	public class SummonMasterBehavior : NetworkBehaviour
	{
		// Token: 0x06001647 RID: 5703 RVA: 0x00009162 File Offset: 0x00007362
		public override int GetNetworkChannel()
		{
			return QosChannelIndex.defaultReliable.intVal;
		}

		// Token: 0x06001648 RID: 5704 RVA: 0x00076560 File Offset: 0x00074760
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

		// Token: 0x0600164A RID: 5706 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x0600164B RID: 5707 RVA: 0x0004AA24 File Offset: 0x00048C24
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			bool result;
			return result;
		}

		// Token: 0x0600164C RID: 5708 RVA: 0x000025DA File Offset: 0x000007DA
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
		}

		// Token: 0x04001990 RID: 6544
		[Tooltip("The master to spawn")]
		public GameObject masterPrefab;
	}
}
