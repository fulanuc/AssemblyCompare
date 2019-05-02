using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000352 RID: 850
	[RequireComponent(typeof(TeamFilter))]
	public class MapZone : MonoBehaviour
	{
		// Token: 0x0600119B RID: 4507 RVA: 0x0000D6AF File Offset: 0x0000B8AF
		public void OnTriggerEnter(Collider other)
		{
			if (this.triggerType == MapZone.TriggerType.TriggerEnter)
			{
				this.TryZone(other);
			}
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x0000D6C1 File Offset: 0x0000B8C1
		public void OnTriggerExit(Collider other)
		{
			if (this.triggerType == MapZone.TriggerType.TriggerExit)
			{
				this.TryZone(other);
			}
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x000668E0 File Offset: 0x00064AE0
		private void TryZone(Collider other)
		{
			CharacterBody component = other.GetComponent<CharacterBody>();
			if (component)
			{
				TeamComponent component2 = component.GetComponent<TeamComponent>();
				MapZone.ZoneType zoneType = this.zoneType;
				if (zoneType != MapZone.ZoneType.OutOfBounds)
				{
					if (zoneType != MapZone.ZoneType.KickOutPlayers)
					{
						return;
					}
					if (component2.teamIndex == TeamIndex.Player)
					{
						this.TeleportBody(component);
					}
				}
				else if (Util.HasEffectiveAuthority(component.gameObject))
				{
					if (component2.teamIndex == TeamIndex.Player)
					{
						this.TeleportBody(component);
						return;
					}
					if (NetworkServer.active)
					{
						HealthComponent healthComponent = component.healthComponent;
						if (healthComponent)
						{
							healthComponent.Suicide(null);
							return;
						}
					}
				}
			}
		}

		// Token: 0x0600119E RID: 4510 RVA: 0x00066964 File Offset: 0x00064B64
		private void TeleportBody(CharacterBody characterBody)
		{
			if (!Physics.GetIgnoreLayerCollision(base.gameObject.layer, characterBody.gameObject.layer))
			{
				SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
				spawnCard.hullSize = characterBody.hullClassification;
				spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
				spawnCard.prefab = Resources.Load<GameObject>("SpawnCards/HelperPrefab");
				GameObject gameObject = DirectorCore.instance.TrySpawnObject(spawnCard, new DirectorPlacementRule
				{
					placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
					position = characterBody.transform.position
				}, RoR2Application.rng);
				if (gameObject)
				{
					Debug.Log("tp back");
					if (Util.HasEffectiveAuthority(characterBody.gameObject))
					{
						TeleportHelper.TeleportBody(characterBody, gameObject.transform.position);
					}
					GameObject teleportEffectPrefab = Run.instance.GetTeleportEffectPrefab(characterBody.gameObject);
					if (teleportEffectPrefab)
					{
						UnityEngine.Object.Instantiate<GameObject>(teleportEffectPrefab, characterBody.transform.position, Quaternion.identity);
					}
					UnityEngine.Object.Destroy(gameObject);
				}
				UnityEngine.Object.Destroy(spawnCard);
			}
		}

		// Token: 0x04001591 RID: 5521
		public MapZone.TriggerType triggerType;

		// Token: 0x04001592 RID: 5522
		public MapZone.ZoneType zoneType;

		// Token: 0x02000353 RID: 851
		public enum TriggerType
		{
			// Token: 0x04001594 RID: 5524
			TriggerExit,
			// Token: 0x04001595 RID: 5525
			TriggerEnter
		}

		// Token: 0x02000354 RID: 852
		public enum ZoneType
		{
			// Token: 0x04001597 RID: 5527
			OutOfBounds,
			// Token: 0x04001598 RID: 5528
			KickOutPlayers
		}
	}
}
