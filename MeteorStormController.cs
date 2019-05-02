using System;
using System.Collections.Generic;
using System.Linq;
using RoR2.Navigation;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200035C RID: 860
	public class MeteorStormController : MonoBehaviour
	{
		// Token: 0x060011C6 RID: 4550 RVA: 0x0000D845 File Offset: 0x0000BA45
		private void Start()
		{
			if (NetworkServer.active)
			{
				this.meteorList = new List<MeteorStormController.Meteor>();
				this.waveList = new List<MeteorStormController.MeteorWave>();
			}
		}

		// Token: 0x060011C7 RID: 4551 RVA: 0x00066F34 File Offset: 0x00065134
		private void FixedUpdate()
		{
			if (!NetworkServer.active)
			{
				return;
			}
			this.waveTimer -= Time.fixedDeltaTime;
			if (this.waveTimer <= 0f && this.wavesPerformed < this.waveCount)
			{
				this.wavesPerformed++;
				this.waveTimer = UnityEngine.Random.Range(this.waveMinInterval, this.waveMaxInterval);
				MeteorStormController.MeteorWave item = new MeteorStormController.MeteorWave(CharacterBody.readOnlyInstancesList.ToArray<CharacterBody>(), base.transform.position);
				this.waveList.Add(item);
			}
			for (int i = this.waveList.Count - 1; i >= 0; i--)
			{
				MeteorStormController.MeteorWave meteorWave = this.waveList[i];
				meteorWave.timer -= Time.fixedDeltaTime;
				if (meteorWave.timer <= 0f)
				{
					meteorWave.timer = UnityEngine.Random.Range(0.05f, 1f);
					MeteorStormController.Meteor nextMeteor = meteorWave.GetNextMeteor();
					if (nextMeteor == null)
					{
						this.waveList.RemoveAt(i);
					}
					else if (nextMeteor.valid)
					{
						this.meteorList.Add(nextMeteor);
						EffectManager.instance.SpawnEffect(this.warningEffectPrefab, new EffectData
						{
							origin = nextMeteor.impactPosition,
							scale = this.blastRadius
						}, true);
					}
				}
			}
			float num = Run.instance.time - this.impactDelay;
			float num2 = num - this.travelEffectDuration;
			for (int j = this.meteorList.Count - 1; j >= 0; j--)
			{
				MeteorStormController.Meteor meteor = this.meteorList[j];
				if (meteor.startTime < num2 && !meteor.didTravelEffect)
				{
					this.DoMeteorEffect(meteor);
				}
				if (meteor.startTime < num)
				{
					this.meteorList.RemoveAt(j);
					this.DetonateMeteor(meteor);
				}
			}
			if (this.wavesPerformed == this.waveCount && this.meteorList.Count == 0)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		// Token: 0x060011C8 RID: 4552 RVA: 0x0000D864 File Offset: 0x0000BA64
		private void OnDestroy()
		{
			this.onDestroyEvents.Invoke();
		}

		// Token: 0x060011C9 RID: 4553 RVA: 0x0000D871 File Offset: 0x0000BA71
		private void DoMeteorEffect(MeteorStormController.Meteor meteor)
		{
			meteor.didTravelEffect = true;
			if (this.travelEffectPrefab)
			{
				EffectManager.instance.SpawnEffect(this.travelEffectPrefab, new EffectData
				{
					origin = meteor.impactPosition
				}, true);
			}
		}

		// Token: 0x060011CA RID: 4554 RVA: 0x00067128 File Offset: 0x00065328
		private void DetonateMeteor(MeteorStormController.Meteor meteor)
		{
			EffectData effectData = new EffectData
			{
				origin = meteor.impactPosition
			};
			EffectManager.instance.SpawnEffect(this.impactEffectPrefab, effectData, true);
			new BlastAttack
			{
				inflictor = base.gameObject,
				baseDamage = this.blastDamageCoefficient * this.ownerDamage,
				baseForce = this.blastForce,
				canHurtAttacker = true,
				crit = this.isCrit,
				falloffModel = BlastAttack.FalloffModel.Linear,
				attacker = this.owner,
				bonusForce = Vector3.zero,
				damageColorIndex = DamageColorIndex.Item,
				position = meteor.impactPosition,
				procChainMask = default(ProcChainMask),
				procCoefficient = 1f,
				teamIndex = TeamIndex.None,
				radius = this.blastRadius
			}.Fire();
		}

		// Token: 0x040015BF RID: 5567
		public int waveCount;

		// Token: 0x040015C0 RID: 5568
		public float waveMinInterval;

		// Token: 0x040015C1 RID: 5569
		public float waveMaxInterval;

		// Token: 0x040015C2 RID: 5570
		public GameObject warningEffectPrefab;

		// Token: 0x040015C3 RID: 5571
		public GameObject travelEffectPrefab;

		// Token: 0x040015C4 RID: 5572
		public float travelEffectDuration;

		// Token: 0x040015C5 RID: 5573
		public GameObject impactEffectPrefab;

		// Token: 0x040015C6 RID: 5574
		public float impactDelay;

		// Token: 0x040015C7 RID: 5575
		public float blastDamageCoefficient;

		// Token: 0x040015C8 RID: 5576
		public float blastRadius;

		// Token: 0x040015C9 RID: 5577
		public float blastForce;

		// Token: 0x040015CA RID: 5578
		[NonSerialized]
		public GameObject owner;

		// Token: 0x040015CB RID: 5579
		[NonSerialized]
		public float ownerDamage;

		// Token: 0x040015CC RID: 5580
		[NonSerialized]
		public bool isCrit;

		// Token: 0x040015CD RID: 5581
		public UnityEvent onDestroyEvents;

		// Token: 0x040015CE RID: 5582
		private List<MeteorStormController.Meteor> meteorList;

		// Token: 0x040015CF RID: 5583
		private List<MeteorStormController.MeteorWave> waveList;

		// Token: 0x040015D0 RID: 5584
		private int wavesPerformed;

		// Token: 0x040015D1 RID: 5585
		private float waveTimer;

		// Token: 0x0200035D RID: 861
		private class Meteor
		{
			// Token: 0x040015D2 RID: 5586
			public Vector3 impactPosition;

			// Token: 0x040015D3 RID: 5587
			public float startTime;

			// Token: 0x040015D4 RID: 5588
			public bool didTravelEffect;

			// Token: 0x040015D5 RID: 5589
			public bool valid = true;
		}

		// Token: 0x0200035E RID: 862
		private class MeteorWave
		{
			// Token: 0x060011CD RID: 4557 RVA: 0x000671FC File Offset: 0x000653FC
			public MeteorWave(CharacterBody[] targets, Vector3 center)
			{
				this.targets = new CharacterBody[targets.Length];
				targets.CopyTo(this.targets, 0);
				Util.ShuffleArray<CharacterBody>(targets);
				this.center = center;
				this.nodeGraphSpider = new NodeGraphSpider(SceneInfo.instance.groundNodes, HullMask.Human);
				this.nodeGraphSpider.AddNodeForNextStep(SceneInfo.instance.groundNodes.FindClosestNode(center, HullClassification.Human));
				int num = 0;
				int num2 = 20;
				while (num < num2 && this.nodeGraphSpider.PerformStep())
				{
					num++;
				}
			}

			// Token: 0x060011CE RID: 4558 RVA: 0x00067290 File Offset: 0x00065490
			public MeteorStormController.Meteor GetNextMeteor()
			{
				if (this.currentStep >= this.targets.Length)
				{
					return null;
				}
				CharacterBody characterBody = this.targets[this.currentStep];
				MeteorStormController.Meteor meteor = new MeteorStormController.Meteor();
				if (characterBody && UnityEngine.Random.value < this.hitChance)
				{
					meteor.impactPosition = characterBody.corePosition;
					Vector3 origin = meteor.impactPosition + Vector3.up * 6f;
					Vector3 onUnitSphere = UnityEngine.Random.onUnitSphere;
					onUnitSphere.y = -1f;
					RaycastHit raycastHit;
					if (Physics.Raycast(origin, onUnitSphere, out raycastHit, 12f, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
					{
						meteor.impactPosition = raycastHit.point;
					}
					else if (Physics.Raycast(meteor.impactPosition, Vector3.down, out raycastHit, float.PositiveInfinity, LayerIndex.world.mask, QueryTriggerInteraction.Ignore))
					{
						meteor.impactPosition = raycastHit.point;
					}
				}
				else if (this.nodeGraphSpider.collectedSteps.Count != 0)
				{
					int index = UnityEngine.Random.Range(0, this.nodeGraphSpider.collectedSteps.Count);
					SceneInfo.instance.groundNodes.GetNodePosition(this.nodeGraphSpider.collectedSteps[index].node, out meteor.impactPosition);
				}
				else
				{
					meteor.valid = false;
				}
				meteor.startTime = Run.instance.time;
				this.currentStep++;
				return meteor;
			}

			// Token: 0x040015D6 RID: 5590
			private readonly CharacterBody[] targets;

			// Token: 0x040015D7 RID: 5591
			private int currentStep;

			// Token: 0x040015D8 RID: 5592
			private float hitChance = 0.4f;

			// Token: 0x040015D9 RID: 5593
			private readonly Vector3 center;

			// Token: 0x040015DA RID: 5594
			public float timer;

			// Token: 0x040015DB RID: 5595
			private NodeGraphSpider nodeGraphSpider;
		}
	}
}
