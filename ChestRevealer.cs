using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029A RID: 666
	public class ChestRevealer : NetworkBehaviour
	{
		// Token: 0x06000D9C RID: 3484 RVA: 0x0000A9A5 File Offset: 0x00008BA5
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += ChestRevealer.StaticFixedUpdate;
		}

		// Token: 0x06000D9D RID: 3485 RVA: 0x00055348 File Offset: 0x00053548
		private static void StaticFixedUpdate()
		{
			ChestRevealer.pendingReveals.Sort();
			while (ChestRevealer.pendingReveals.Count > 0 && ChestRevealer.pendingReveals[0].time.hasPassed)
			{
				if (ChestRevealer.pendingReveals[0].gameObject)
				{
					ChestRevealer.RevealedObject.RevealObject(ChestRevealer.pendingReveals[0].gameObject, ChestRevealer.pendingReveals[0].duration);
				}
				ChestRevealer.pendingReveals.RemoveAt(0);
			}
		}

		// Token: 0x06000D9E RID: 3486 RVA: 0x000553D0 File Offset: 0x000535D0
		public void Pulse()
		{
			Vector3 position = base.transform.position;
			float num = this.radius * this.radius;
			foreach (PurchaseInteraction purchaseInteraction in PurchaseInteraction.readOnlyInstancesList)
			{
				float sqrMagnitude = (purchaseInteraction.transform.position - position).sqrMagnitude;
				if (sqrMagnitude <= num && purchaseInteraction.available)
				{
					float b = Mathf.Sqrt(sqrMagnitude) / this.pulseTravelSpeed;
					ChestRevealer.PendingReveal item = new ChestRevealer.PendingReveal
					{
						gameObject = purchaseInteraction.gameObject,
						time = Run.FixedTimeStamp.now + b,
						duration = this.revealDuration
					};
					ChestRevealer.pendingReveals.Add(item);
				}
			}
			EffectManager.instance.SpawnEffect(this.pulseEffectPrefab, new EffectData
			{
				origin = position,
				scale = this.radius * this.pulseEffectScale
			}, false);
		}

		// Token: 0x06000D9F RID: 3487 RVA: 0x0000A9B8 File Offset: 0x00008BB8
		private void FixedUpdate()
		{
			if (this.nextPulse.hasPassed)
			{
				this.Pulse();
				this.nextPulse = Run.FixedTimeStamp.now + this.pulseInterval;
			}
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x000025F6 File Offset: 0x000007F6
		private void UNetVersion()
		{
		}

		// Token: 0x17000126 RID: 294
		// (get) Token: 0x06000DA3 RID: 3491 RVA: 0x000554E4 File Offset: 0x000536E4
		// (set) Token: 0x06000DA4 RID: 3492 RVA: 0x0000AA2E File Offset: 0x00008C2E
		public float Networkradius
		{
			get
			{
				return this.radius;
			}
			set
			{
				base.SetSyncVar<float>(value, ref this.radius, 1u);
			}
		}

		// Token: 0x06000DA5 RID: 3493 RVA: 0x000554F8 File Offset: 0x000536F8
		public override bool OnSerialize(NetworkWriter writer, bool forceAll)
		{
			if (forceAll)
			{
				writer.Write(this.radius);
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
				writer.Write(this.radius);
			}
			if (!flag)
			{
				writer.WritePackedUInt32(base.syncVarDirtyBits);
			}
			return flag;
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x00055564 File Offset: 0x00053764
		public override void OnDeserialize(NetworkReader reader, bool initialState)
		{
			if (initialState)
			{
				this.radius = reader.ReadSingle();
				return;
			}
			int num = (int)reader.ReadPackedUInt32();
			if ((num & 1) != 0)
			{
				this.radius = reader.ReadSingle();
			}
		}

		// Token: 0x04001181 RID: 4481
		[SyncVar]
		public float radius;

		// Token: 0x04001182 RID: 4482
		public float pulseTravelSpeed = 10f;

		// Token: 0x04001183 RID: 4483
		public float revealDuration = 10f;

		// Token: 0x04001184 RID: 4484
		public float pulseInterval = 1f;

		// Token: 0x04001185 RID: 4485
		private Run.FixedTimeStamp nextPulse = Run.FixedTimeStamp.negativeInfinity;

		// Token: 0x04001186 RID: 4486
		public GameObject pulseEffectPrefab;

		// Token: 0x04001187 RID: 4487
		public float pulseEffectScale = 1f;

		// Token: 0x04001188 RID: 4488
		private static readonly List<ChestRevealer.PendingReveal> pendingReveals = new List<ChestRevealer.PendingReveal>();

		// Token: 0x0200029B RID: 667
		private struct PendingReveal : IComparable<ChestRevealer.PendingReveal>
		{
			// Token: 0x06000DA7 RID: 3495 RVA: 0x0000AA42 File Offset: 0x00008C42
			public int CompareTo(ChestRevealer.PendingReveal other)
			{
				return this.time.CompareTo(other.time);
			}

			// Token: 0x04001189 RID: 4489
			public GameObject gameObject;

			// Token: 0x0400118A RID: 4490
			public Run.FixedTimeStamp time;

			// Token: 0x0400118B RID: 4491
			public float duration;
		}

		// Token: 0x0200029C RID: 668
		private class RevealedObject : MonoBehaviour
		{
			// Token: 0x06000DA8 RID: 3496 RVA: 0x000555A8 File Offset: 0x000537A8
			public static void RevealObject(GameObject gameObject, float duration)
			{
				ChestRevealer.RevealedObject revealedObject;
				if (!ChestRevealer.RevealedObject.currentlyRevealedObjects.TryGetValue(gameObject, out revealedObject))
				{
					revealedObject = gameObject.AddComponent<ChestRevealer.RevealedObject>();
				}
				if (revealedObject.lifetime < duration)
				{
					revealedObject.lifetime = duration;
				}
			}

			// Token: 0x06000DA9 RID: 3497 RVA: 0x000555DC File Offset: 0x000537DC
			private void OnEnable()
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PositionIndicators/PoiPositionIndicator"), base.transform.position, base.transform.rotation);
				this.positionIndicator = gameObject.GetComponent<PositionIndicator>();
				this.positionIndicator.targetTransform = base.transform;
				ChestRevealer.RevealedObject.currentlyRevealedObjects[base.gameObject] = this;
			}

			// Token: 0x06000DAA RID: 3498 RVA: 0x0000AA55 File Offset: 0x00008C55
			private void OnDisable()
			{
				ChestRevealer.RevealedObject.currentlyRevealedObjects.Remove(base.gameObject);
				UnityEngine.Object.Destroy(this.positionIndicator.gameObject);
				this.positionIndicator = null;
			}

			// Token: 0x06000DAB RID: 3499 RVA: 0x0000AA7F File Offset: 0x00008C7F
			private void FixedUpdate()
			{
				this.lifetime -= Time.fixedDeltaTime;
				if (this.lifetime <= 0f)
				{
					UnityEngine.Object.Destroy(this);
				}
			}

			// Token: 0x0400118C RID: 4492
			private float lifetime;

			// Token: 0x0400118D RID: 4493
			private static readonly Dictionary<GameObject, ChestRevealer.RevealedObject> currentlyRevealedObjects = new Dictionary<GameObject, ChestRevealer.RevealedObject>();

			// Token: 0x0400118E RID: 4494
			private PositionIndicator positionIndicator;
		}
	}
}
