using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RoR2
{
	// Token: 0x020003A1 RID: 929
	public class RadiotowerTerminal : NetworkBehaviour
	{
		// Token: 0x060013BC RID: 5052 RVA: 0x0000F0E7 File Offset: 0x0000D2E7
		private void SetHasBeenPurchased(bool newHasBeenPurchased)
		{
			if (this.hasBeenPurchased != newHasBeenPurchased)
			{
				this.NetworkhasBeenPurchased = newHasBeenPurchased;
			}
		}

		// Token: 0x060013BD RID: 5053 RVA: 0x0000F0F9 File Offset: 0x0000D2F9
		public void Start()
		{
			if (NetworkServer.active)
			{
				this.FindStageLogUnlockable();
			}
			bool active = NetworkClient.active;
		}

		// Token: 0x060013BE RID: 5054 RVA: 0x0006E0B4 File Offset: 0x0006C2B4
		private void FindStageLogUnlockable()
		{
			this.unlockableName = SceneCatalog.GetUnlockableLogFromSceneName(SceneManager.GetActiveScene().name);
		}

		// Token: 0x060013BF RID: 5055 RVA: 0x0006E0DC File Offset: 0x0006C2DC
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

		// Token: 0x060013C1 RID: 5057 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x170001B9 RID: 441
		// (get) Token: 0x060013C2 RID: 5058 RVA: 0x0006E1B4 File Offset: 0x0006C3B4
		// (set) Token: 0x060013C3 RID: 5059 RVA: 0x0000F10E File Offset: 0x0000D30E
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

		// Token: 0x060013C4 RID: 5060 RVA: 0x0006E1C8 File Offset: 0x0006C3C8
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

		// Token: 0x060013C5 RID: 5061 RVA: 0x0006E234 File Offset: 0x0006C434
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

		// Token: 0x04001752 RID: 5970
		[SyncVar(hook = "SetHasBeenPurchased")]
		private bool hasBeenPurchased;

		// Token: 0x04001753 RID: 5971
		private string unlockableName;

		// Token: 0x04001754 RID: 5972
		public string unlockSoundString;

		// Token: 0x04001755 RID: 5973
		public GameObject unlockEffect;
	}
}
