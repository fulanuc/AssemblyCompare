using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200031D RID: 797
	[RequireComponent(typeof(Collider))]
	public class HurtBox : MonoBehaviour
	{
		// Token: 0x1700016C RID: 364
		// (get) Token: 0x06001084 RID: 4228 RVA: 0x0000CA99 File Offset: 0x0000AC99
		// (set) Token: 0x06001085 RID: 4229 RVA: 0x0000CAA1 File Offset: 0x0000ACA1
		public Collider collider { get; private set; }

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x06001086 RID: 4230 RVA: 0x0000CAAA File Offset: 0x0000ACAA
		// (set) Token: 0x06001087 RID: 4231 RVA: 0x0000CAB2 File Offset: 0x0000ACB2
		public float volume { get; private set; }

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x06001088 RID: 4232 RVA: 0x0000CABB File Offset: 0x0000ACBB
		public Vector3 randomVolumePoint
		{
			get
			{
				return Util.RandomColliderVolumePoint(this.collider);
			}
		}

		// Token: 0x06001089 RID: 4233 RVA: 0x00062C60 File Offset: 0x00060E60
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

		// Token: 0x0600108A RID: 4234 RVA: 0x0000CAC8 File Offset: 0x0000ACC8
		private void OnEnable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Add(this);
			}
		}

		// Token: 0x0600108B RID: 4235 RVA: 0x0000CADD File Offset: 0x0000ACDD
		private void OnDisable()
		{
			if (this.isBullseye)
			{
				HurtBox.bullseyesList.Remove(this);
			}
		}

		// Token: 0x04001490 RID: 5264
		[Tooltip("The health component to which this hurtbox belongs.")]
		public HealthComponent healthComponent;

		// Token: 0x04001491 RID: 5265
		[Tooltip("Whether or not this hurtbox is considered a bullseye. Do not change this at runtime!")]
		public bool isBullseye;

		// Token: 0x04001492 RID: 5266
		public HurtBox.DamageModifier damageModifier;

		// Token: 0x04001493 RID: 5267
		[NonSerialized]
		public TeamIndex teamIndex = TeamIndex.None;

		// Token: 0x04001494 RID: 5268
		[SerializeField]
		[HideInInspector]
		public HurtBoxGroup hurtBoxGroup;

		// Token: 0x04001495 RID: 5269
		[HideInInspector]
		[SerializeField]
		public short indexInGroup = -1;

		// Token: 0x04001498 RID: 5272
		private static readonly List<HurtBox> bullseyesList = new List<HurtBox>();

		// Token: 0x04001499 RID: 5273
		public static readonly ReadOnlyCollection<HurtBox> readOnlyBullseyesList = HurtBox.bullseyesList.AsReadOnly();

		// Token: 0x0200031E RID: 798
		public enum DamageModifier
		{
			// Token: 0x0400149B RID: 5275
			Normal,
			// Token: 0x0400149C RID: 5276
			SniperTarget,
			// Token: 0x0400149D RID: 5277
			Weak,
			// Token: 0x0400149E RID: 5278
			Barrier
		}
	}
}
