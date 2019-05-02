using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200029A RID: 666
	public class ChestRevealer : NetworkBehaviour
	{
		// Token: 0x06000D9F RID: 3487 RVA: 0x000551E8 File Offset: 0x000533E8
		[RuntimeInitializeOnLoadMethod]
		private static void Init()
		{
			RoR2Application.onFixedUpdate += ChestRevealer.StaticFixedUpdate;
			ChestRevealer.typesToCheck = (from t in typeof(ChestRevealer).Assembly.GetTypes()
			where typeof(IInteractable).IsAssignableFrom(t)
			select t).ToArray<Type>();
		}

		// Token: 0x06000DA0 RID: 3488 RVA: 0x00055248 File Offset: 0x00053448
		private static void StaticFixedUpdate()
		{
			ChestRevealer.pendingReveals.Sort();
			while (ChestRevealer.pendingReveals.Count > 0)
			{
				ChestRevealer.PendingReveal pendingReveal = ChestRevealer.pendingReveals[0];
				if (!pendingReveal.time.hasPassed)
				{
					break;
				}
				if (pendingReveal.gameObject)
				{
					ChestRevealer.RevealedObject.RevealObject(pendingReveal.gameObject, pendingReveal.duration);
				}
				ChestRevealer.pendingReveals.RemoveAt(0);
			}
		}

		// Token: 0x06000DA1 RID: 3489 RVA: 0x000552B4 File Offset: 0x000534B4
		public void Pulse()
		{
			ChestRevealer.<>c__DisplayClass12_0 CS$<>8__locals1;
			CS$<>8__locals1.<>4__this = this;
			CS$<>8__locals1.origin = base.transform.position;
			CS$<>8__locals1.radiusSqr = this.radius * this.radius;
			CS$<>8__locals1.invPulseTravelSpeed = 1f / this.pulseTravelSpeed;
			Type[] array = ChestRevealer.typesToCheck;
			for (int i = 0; i < array.Length; i++)
			{
				foreach (MonoBehaviour monoBehaviour in InstanceTracker.FindInstancesEnumerable(array[i]))
				{
					if (((IInteractable)monoBehaviour).ShouldShowOnScanner())
					{
						this.<Pulse>g__TryAddRevealable|12_0(monoBehaviour.transform, ref CS$<>8__locals1);
					}
				}
			}
			EffectManager.instance.SpawnEffect(this.pulseEffectPrefab, new EffectData
			{
				origin = CS$<>8__locals1.origin,
				scale = this.radius * this.pulseEffectScale
			}, false);
		}

		// Token: 0x06000DA2 RID: 3490 RVA: 0x0000A9EC File Offset: 0x00008BEC
		private void FixedUpdate()
		{
			if (this.nextPulse.hasPassed)
			{
				this.Pulse();
				this.nextPulse = Run.FixedTimeStamp.now + this.pulseInterval;
			}
		}

		// Token: 0x06000DA6 RID: 3494 RVA: 0x000025DA File Offset: 0x000007DA
		private void UNetVersion()
		{
		}

		// Token: 0x1700012A RID: 298
		// (get) Token: 0x06000DA7 RID: 3495 RVA: 0x00055428 File Offset: 0x00053628
		// (set) Token: 0x06000DA8 RID: 3496 RVA: 0x0000AA62 File Offset: 0x00008C62
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

		// Token: 0x06000DA9 RID: 3497 RVA: 0x0005543C File Offset: 0x0005363C
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

		// Token: 0x06000DAA RID: 3498 RVA: 0x000554A8 File Offset: 0x000536A8
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

		// Token: 0x0400118C RID: 4492
		[SyncVar]
		public float radius;

		// Token: 0x0400118D RID: 4493
		public float pulseTravelSpeed = 10f;

		// Token: 0x0400118E RID: 4494
		public float revealDuration = 10f;

		// Token: 0x0400118F RID: 4495
		public float pulseInterval = 1f;

		// Token: 0x04001190 RID: 4496
		private Run.FixedTimeStamp nextPulse = Run.FixedTimeStamp.negativeInfinity;

		// Token: 0x04001191 RID: 4497
		public GameObject pulseEffectPrefab;

		// Token: 0x04001192 RID: 4498
		public float pulseEffectScale = 1f;

		// Token: 0x04001193 RID: 4499
		private static readonly List<ChestRevealer.PendingReveal> pendingReveals = new List<ChestRevealer.PendingReveal>();

		// Token: 0x04001194 RID: 4500
		private static Type[] typesToCheck;

		// Token: 0x0200029B RID: 667
		private struct PendingReveal : IComparable<ChestRevealer.PendingReveal>
		{
			// Token: 0x06000DAB RID: 3499 RVA: 0x0000AA76 File Offset: 0x00008C76
			public int CompareTo(ChestRevealer.PendingReveal other)
			{
				return this.time.CompareTo(other.time);
			}

			// Token: 0x04001195 RID: 4501
			public GameObject gameObject;

			// Token: 0x04001196 RID: 4502
			public Run.FixedTimeStamp time;

			// Token: 0x04001197 RID: 4503
			public float duration;
		}

		// Token: 0x0200029C RID: 668
		private class RevealedObject : MonoBehaviour
		{
			// Token: 0x06000DAC RID: 3500 RVA: 0x000554EC File Offset: 0x000536EC
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

			// Token: 0x06000DAD RID: 3501 RVA: 0x00055520 File Offset: 0x00053720
			private void OnEnable()
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/PositionIndicators/PoiPositionIndicator"), base.transform.position, base.transform.rotation);
				this.positionIndicator = gameObject.GetComponent<PositionIndicator>();
				this.positionIndicator.targetTransform = base.transform;
				ChestRevealer.RevealedObject.currentlyRevealedObjects[base.gameObject] = this;
			}

			// Token: 0x06000DAE RID: 3502 RVA: 0x0000AA89 File Offset: 0x00008C89
			private void OnDisable()
			{
				ChestRevealer.RevealedObject.currentlyRevealedObjects.Remove(base.gameObject);
				UnityEngine.Object.Destroy(this.positionIndicator.gameObject);
				this.positionIndicator = null;
			}

			// Token: 0x06000DAF RID: 3503 RVA: 0x0000AAB3 File Offset: 0x00008CB3
			private void FixedUpdate()
			{
				this.lifetime -= Time.fixedDeltaTime;
				if (this.lifetime <= 0f)
				{
					UnityEngine.Object.Destroy(this);
				}
			}

			// Token: 0x04001198 RID: 4504
			private float lifetime;

			// Token: 0x04001199 RID: 4505
			private static readonly Dictionary<GameObject, ChestRevealer.RevealedObject> currentlyRevealedObjects = new Dictionary<GameObject, ChestRevealer.RevealedObject>();

			// Token: 0x0400119A RID: 4506
			private PositionIndicator positionIndicator;
		}
	}
}
