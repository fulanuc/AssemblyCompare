using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000307 RID: 775
	public class GoldshoresMissionController : MonoBehaviour
	{
		// Token: 0x17000158 RID: 344
		// (get) Token: 0x06001000 RID: 4096 RVA: 0x0000C3EA File Offset: 0x0000A5EA
		// (set) Token: 0x06001001 RID: 4097 RVA: 0x0000C3F1 File Offset: 0x0000A5F1
		public static GoldshoresMissionController instance { get; private set; }

		// Token: 0x06001002 RID: 4098 RVA: 0x0000C3F9 File Offset: 0x0000A5F9
		private void OnEnable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Assign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x06001003 RID: 4099 RVA: 0x0000C40B File Offset: 0x0000A60B
		private void OnDisable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Unassign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x06001004 RID: 4100 RVA: 0x0000C41D File Offset: 0x0000A61D
		private void Start()
		{
			this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06001005 RID: 4101 RVA: 0x000606C8 File Offset: 0x0005E8C8
		public void SpawnBeacons()
		{
			if (NetworkServer.active)
			{
				for (int i = 0; i < this.beaconsToSpawnOnMap; i++)
				{
					GameObject gameObject = DirectorCore.instance.TrySpawnObject(this.beaconSpawnCard, new DirectorPlacementRule
					{
						placementMode = DirectorPlacementRule.PlacementMode.Random
					}, this.rng);
					if (gameObject)
					{
						this.beaconInstanceList.Add(gameObject);
					}
				}
				this.beaconsToSpawnOnMap = this.beaconInstanceList.Count;
			}
		}

		// Token: 0x06001006 RID: 4102 RVA: 0x0000C452 File Offset: 0x0000A652
		public void BeginTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(true);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06001007 RID: 4103 RVA: 0x0000C46C File Offset: 0x0000A66C
		public void ExitTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(true);
		}

		// Token: 0x040013F5 RID: 5109
		public Xoroshiro128Plus rng;

		// Token: 0x040013F6 RID: 5110
		public EntityStateMachine entityStateMachine;

		// Token: 0x040013F7 RID: 5111
		public GameObject beginTransitionIntoBossFightEffect;

		// Token: 0x040013F8 RID: 5112
		public GameObject exitTransitionIntoBossFightEffect;

		// Token: 0x040013F9 RID: 5113
		public Transform bossSpawnPosition;

		// Token: 0x040013FA RID: 5114
		public List<GameObject> beaconInstanceList = new List<GameObject>();

		// Token: 0x040013FB RID: 5115
		public int beaconsActive;

		// Token: 0x040013FC RID: 5116
		public int beaconsRequiredToSpawnBoss;

		// Token: 0x040013FD RID: 5117
		public int beaconsToSpawnOnMap;

		// Token: 0x040013FE RID: 5118
		public InteractableSpawnCard beaconSpawnCard;
	}
}
