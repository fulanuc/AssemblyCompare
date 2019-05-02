using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031B RID: 795
	[RequireComponent(typeof(Collider))]
	public class HurtBox : MonoBehaviour
	{
		// Token: 0x17000167 RID: 359
		// (get) Token: 0x06001070 RID: 4208 RVA: 0x0000C9B5 File Offset: 0x0000ABB5
		// (set) Token: 0x06001071 RID: 4209 RVA: 0x0000C9BD File Offset: 0x0000ABBD
		public Collider collider { get; private set; }

		// Token: 0x17000168 RID: 360
		// (get) Token: 0x06001072 RID: 4210 RVA: 0x0000C9C6 File Offset: 0x0000ABC6
		// (set) Token: 0x06001073 RID: 4211 RVA: 0x0000C9CE File Offset: 0x0000ABCE
		public float volume { get; private set; }

		// Token: 0x17000169 RID: 361
		// (get) Token: 0x06001074 RID: 4212 RVA: 0x0000C9D7 File Offset: 0x0000ABD7
		public Vector3 randomVolumePoint
		{
			get
			{
				return Util.RandomColliderVolumePoint(this.collider);
			}
		}

		// Token: 0x06001075 RID: 4213 RVA: 0x000629C8 File Offset: 0x00060BC8
		private void Awake()
		{
			this.collider = base.GetComponent<Collider>();
			this.collider.isTrigger = false;
			Rigidbody rigidbody = base.GetComponent<Rigidbody>();
			if (!rigidbody)
			{
				rigidbody = base.gameObject.AddComponent<Rigidbody>();
			}
			rigidbody.isKinematic = true;
			Vector3 lossyScale = base.transform.lossyScale;
			this.volume = lossyScale.x * 2f * (lossyScale.y * 2f) * (lossyScale.z * 2f);
		}

		// Token: 0x06001076 RID: 4214 RVA: 0x0000C9E4 File Offset: 0x0000ABE4
		private void OnEnable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Add(this);
			}
		}

		// Token: 0x06001077 RID: 4215 RVA: 0x0000C9F9 File Offset: 0x0000ABF9
		private void OnDisable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Remove(this);
			}
		}

		// Token: 0x0400147C RID: 5244
		[Tooltip("The health component to which this hurtbox belongs.")]
		public HealthComponent healthComponent;

		// Token: 0x0400147D RID: 5245
		[Tooltip("Whether or not this hurtbox is considered a bullseye. Do not change this at runtime!")]
		public bool isBullseye;

		// Token: 0x0400147E RID: 5246
		public HurtBox.DamageModifier damageModifier;

		// Token: 0x0400147F RID: 5247
		[NonSerialized]
		public TeamIndex teamIndex = TeamIndex.None;

		// Token: 0x04001480 RID: 5248
		[SerializeField]
		[HideInInspector]
		public HurtBoxGroup hurtBoxGroup;

		// Token: 0x04001481 RID: 5249
		[HideInInspector]
		[SerializeField]
		public short indexInGroup = -1;

		// Token: 0x04001484 RID: 5252
		private static readonly List<HurtBox> bullseyesList = new List<HurtBox>();

		// Token: 0x04001485 RID: 5253
		public static readonly ReadOnlyCollection<HurtBox> readOnlyBullseyesList = HurtBox.bullseyesList.AsReadOnly();

		// Token: 0x0200031C RID: 796
		public enum DamageModifier
		{
			// Token: 0x04001487 RID: 5255
			Normal,
			// Token: 0x04001488 RID: 5256
			SniperTarget,
			// Token: 0x04001489 RID: 5257
			Weak,
			// Token: 0x0400148A RID: 5258
			Barrier
		}
	}
}
