using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020003A6 RID: 934
	public class RadiotowerTerminal : NetworkBehaviour
	{
		// Token: 0x060013D9 RID: 5081 RVA: 0x0000F28B File Offset: 0x0000D48B
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
			}
		}

		// Token: 0x060013DA RID: 5082 RVA: 0x0000F29D File Offset: 0x0000D49D
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.FindStageLogUnlockable();
			}
			bool active = NetworkClient.active;
		}

		// Token: 0x060013DB RID: 5083 RVA: 0x0006E2BC File Offset: 0x0006C4BC
		private void FindStageLogUnlockable()
		{
			this.unlockableName = SceneCatalog.GetUnlockableLogFromSceneName(SceneManager.GetActiveScene().name);
		}

		// Token: 0x060013DC RID: 5084 RVA: 0x0006E2E4 File Offset: 0x0006C4E4
		[Server]
		public void GrantUnlock(Interactor interactor)
		{
			if (!NetworkServer.active)
			{
				Debug.LogWarning("[Server] function 'System.Void RoR2.RadiotowerTerminal::GrantUnlock(RoR2.Interactor)' called on client");
				return;
			}
			EffectManager.instance.SpawnEffect(this.unlockEffect, new EffectData
			{
				origin = base.transform.position
			}, true);
			this.SetHasBeenPurchased(true);
			if (Run.instance)
			{
				Util.PlaySound(this.unlockSoundString, interactor.gameObject);
				Run.instance.GrantUnlockToAllParticipatingPlayers(this.unlockableName);
				string pickupToken = "???";
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(this.unlockableName);
				if (unlockableDef != null)
				{
					pickupToken = unlockableDef.nameToken;
				}
				Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
				{
					subjectCharacterBodyGameObject = interactor.gameObject,
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1u
				});
			}
		}

		// Token: 0x060013DE RID: 5086 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x170001BE RID: 446
		// (get) Token: 0x060013DF RID: 5087 RVA: 0x0006E3BC File Offset: 0x0006C5BC
		// (set) Token: 0x060013E0 RID: 5088 RVA: 0x0000F2B2 File Offset: 0x0000D4B2
		public bool NetworkhasBeenPurchased
		{
			get
			{
				return this.hasBeenPurchased;
			}
			set
			{
				uint dirtyBit = 1u;
				if (NetworkServer.localClientActive && !base.syncVarHookGuard)
				{
					base.syncVarHookGuard = true;
					this.SetHasBeenPurchased(value);
					base.syncVarHookGuard = false;
				}
				base.SetSyncVar<bool>(value, ref this.hasBeenPurchased, dirtyBit);
			}
		}

		// Token: 0x060013E1 RID: 5089 RVA: 0x0006E3D0 File Offset: 0x0006C5D0
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.hasBeenPurchased);
				return true;
			}
			bool flag = false;
			if ((base.syncVarDirtyBits & 1u) != 0u)
			{
				if (!flag)
				{
					writer.WritePackedUInt32(base.syncVarDirtyBits);
					flag = true;
				}
				writer.Write(this.hasBeenPurchased);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x060013E2 RID: 5090 RVA: 0x0006E43C File Offset: 0x0006C63C
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.hasBeenPurchased = reader.ReadBoolean();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.SetHasBeenPurchased(reader.ReadBoolean());
			}
		}

		// Token: 0x0400176C RID: 5996
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x0400176D RID: 5997
		private string unlockableName;

		// Token: 0x0400176E RID: 5998
		public string unlockSoundString;

		// Token: 0x0400176F RID: 5999
		public GameObject unlockEffect;
	}
}
