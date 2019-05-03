using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000410 RID: 1040
	public class UnlockPickup : MonoBehaviour
	{
		// Token: 0x06001743 RID: 5955 RVA: 0x00011643 File Offset: 0x0000F843
		private void FixedUpdate()
		{
			this.stopWatch += Time.fixedDeltaTime;
		}

		// Token: 0x06001744 RID: 5956 RVA: 0x00079CE4 File Offset: 0x00077EE4
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

		// Token: 0x06001745 RID: 5957 RVA: 0x0000BFA2 File Offset: 0x0000A1A2
		private static bool BodyHasPickupPermission(CharacterBody body)
		{
			return (body.masterObject ? body.masterObject.GetComponent<PlayerCharacterMasterController>() : null) && body.inventory;
		}

		// Token: 0x06001746 RID: 5958 RVA: 0x00079D8C File Offset: 0x00077F8C
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

		// Token: 0x04001A53 RID: 6739
		public static string itemPickupSoundString = "Play_UI_item_pickup";

		// Token: 0x04001A54 RID: 6740
		private bool consumed;

		// Token: 0x04001A55 RID: 6741
		private float stopWatch;

		// Token: 0x04001A56 RID: 6742
		public float waitDuration = 0.5f;

		// Token: 0x04001A57 RID: 6743
		public string displayNameToken;

		// Token: 0x04001A58 RID: 6744
		public string unlockableName;
	}
}
