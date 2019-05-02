using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000416 RID: 1046
	public class UnlockPickup : MonoBehaviour
	{
		// Token: 0x06001786 RID: 6022 RVA: 0x00011A6F File Offset: 0x0000FC6F
		private void FixedUpdate()
		{
			this.stopWatch += Time.fixedDeltaTime;
		}

		// Token: 0x06001787 RID: 6023 RVA: 0x0007A2A4 File Offset: 0x000784A4
		private void GrantPickup(GameObject activator)
		{
			if (Run.instance)
			{
				Util.PlaySound(UnlockPickup.itemPickupSoundString, activator);
				Run.instance.GrantUnlockToAllParticipatingPlayers(this.unlockableName);
				string pickupToken = "???";
				UnlockableDef unlockableDef = UnlockableCatalog.GetUnlockableDef(this.unlockableName);
				if (unlockableDef != null)
				{
					pickupToken = unlockableDef.nameToken;
				}
				Chat.SendBroadcastChat(new Chat.PlayerPickupChatMessage
				{
					subjectCharacterBodyGameObject = activator,
					baseToken = "PLAYER_PICKUP",
					pickupToken = pickupToken,
					pickupColor = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unlockable),
					pickupQuantity = 1u
				});
				this.consumed = true;
				UnityEngine.Object.Destroy(base.transform.root.gameObject);
			}
		}

		// Token: 0x06001788 RID: 6024 RVA: 0x0000C07C File Offset: 0x0000A27C
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06001789 RID: 6025 RVA: 0x0007A34C File Offset: 0x0007854C
		private void OnTriggerStay(Collider other)
		{
			if (NetworkServer.active && this.stopWatch >= this.waitDuration && !this.consumed)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					TeamComponent component2 = component.GetComponent<TeamComponent>();
					if (component2 && component2.teamIndex == TeamIndex.Player && component.inventory)
					{
						this.GrantPickup(component.gameObject);
					}
				}
			}
		}

		// Token: 0x04001A7C RID: 6780
		public static string itemPickupSoundString = "Play_UI_item_pickup";

		// Token: 0x04001A7D RID: 6781
		private bool consumed;

		// Token: 0x04001A7E RID: 6782
		private float stopWatch;

		// Token: 0x04001A7F RID: 6783
		public float waitDuration = 0.5f;

		// Token: 0x04001A80 RID: 6784
		public string displayNameToken;

		// Token: 0x04001A81 RID: 6785
		public string unlockableName;
	}
}
