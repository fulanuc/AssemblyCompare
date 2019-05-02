using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000304 RID: 772
	public class GoldshoresMissionController : MonoBehaviour
	{
		// Token: 0x17000153 RID: 339
		// (get) Token: 0x06000FEA RID: 4074 RVA: 0x0000C300 File Offset: 0x0000A500
		// (set) Token: 0x06000FEB RID: 4075 RVA: 0x0000C307 File Offset: 0x0000A507
		public static GoldshoresMissionController instance { get; private set; }

		// Token: 0x06000FEC RID: 4076 RVA: 0x0000C30F File Offset: 0x0000A50F
		private void OnEnable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Assign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x06000FED RID: 4077 RVA: 0x0000C321 File Offset: 0x0000A521
		private void OnDisable()
		{
			GoldshoresMissionController.instance = SingletonHelper.Unassign<GoldshoresMissionController>(GoldshoresMissionController.instance, this);
		}

		// Token: 0x06000FEE RID: 4078 RVA: 0x0000C333 File Offset: 0x0000A533
		private void Start()
		{
			this.rng = new Xoroshiro128Plus((ulong)Run.instance.stageRng.nextUint);
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06000FEF RID: 4079 RVA: 0x0006044C File Offset: 0x0005E64C
		public void SpawnBeacons()
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

		// Token: 0x06000FF0 RID: 4080 RVA: 0x0000C368 File Offset: 0x0000A568
		public void BeginTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(true);
			this.exitTransitionIntoBossFightEffect.SetActive(false);
		}

		// Token: 0x06000FF1 RID: 4081 RVA: 0x0000C382 File Offset: 0x0000A582
		public void ExitTransitionIntoBossfight()
		{
			this.beginTransitionIntoBossFightEffect.SetActive(false);
			this.exitTransitionIntoBossFightEffect.SetActive(true);
		}

		// Token: 0x040013DD RID: 5085
		public Xoroshiro128Plus rng;

		// Token: 0x040013DE RID: 5086
		public EntityStateMachine entityStateMachine;

		// Token: 0x040013DF RID: 5087
		public GameObject beginTransitionIntoBossFightEffect;

		// Token: 0x040013E0 RID: 5088
		public GameObject exitTransitionIntoBossFightEffect;

		// Token: 0x040013E1 RID: 5089
		public Transform bossSpawnPosition;

		// Token: 0x040013E2 RID: 5090
		public List<GameObject> beaconInstanceList = new List<GameObject>();

		// Token: 0x040013E3 RID: 5091
		public int beaconsActive;

		// Token: 0x040013E4 RID: 5092
		public int beaconsRequiredToSpawnBoss;

		// Token: 0x040013E5 RID: 5093
		public int beaconsToSpawnOnMap;

		// Token: 0x040013E6 RID: 5094
		public InteractableSpawnCard beaconSpawnCard;
	}
}
