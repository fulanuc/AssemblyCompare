using System;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000355 RID: 853
	public class MapZone : MonoBehaviour
	{
		// Token: 0x060011B2 RID: 4530 RVA: 0x0000D798 File Offset: 0x0000B998
		public void OnTriggerEnter(Collider other)
		{
			if (this.triggerType == MapZone.TriggerType.TriggerEnter)
			{
				this.TryZone(other);
			}
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x0000D7AA File Offset: 0x0000B9AA
		public void OnTriggerExit(Collider other)
		{
			if (this.triggerType == MapZone.TriggerType.TriggerExit)
			{
				this.TryZone(other);
			}
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00066C18 File Offset: 0x00064E18
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

		// Token: 0x060011B5 RID: 4533 RVA: 0x00066C9C File Offset: 0x00064E9C
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

		// Token: 0x040015AA RID: 5546
		public MapZone.TriggerType triggerType;

		// Token: 0x040015AB RID: 5547
		public MapZone.ZoneType zoneType;

		// Token: 0x02000356 RID: 854
		public enum TriggerType
		{
			// Token: 0x040015AD RID: 5549
			TriggerExit,
			// Token: 0x040015AE RID: 5550
			TriggerEnter
		}

		// Token: 0x02000357 RID: 855
		public enum ZoneType
		{
			// Token: 0x040015B0 RID: 5552
			OutOfBounds,
			// Token: 0x040015B1 RID: 5553
			KickOutPlayers
		}
	}
}
