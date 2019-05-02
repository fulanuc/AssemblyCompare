using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000327 RID: 807
	public class HurtBoxGroup : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x060010A5 RID: 4261 RVA: 0x0000CC0C File Offset: 0x0000AE0C
		public void OnDeathStart()
		{
			this.SetHurtboxesActive(false);
		}

		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060010A6 RID: 4262 RVA: 0x0000CC15 File Offset: 0x0000AE15
		// (set) Token: 0x060010A7 RID: 4263 RVA: 0x00063018 File Offset: 0x00061218
		public int hurtBoxesDeactivatorCounter
		{
			get
			{
				return this._hurtBoxesDeactivatorCounter;
			}
			set
			{
				bool flag = this._hurtBoxesDeactivatorCounter <= 0;
				bool flag2 = value <= 0;
				this._hurtBoxesDeactivatorCounter = value;
				if (flag != flag2)
				{
					this.SetHurtboxesActive(flag2);
				}
			}
		}

		// Token: 0x060010A8 RID: 4264 RVA: 0x0006304C File Offset: 0x0006124C
		private void SetHurtboxesActive(bool active)
		{
			for (int i = 0; i < this.hurtBoxes.Length; i++)
			{
				this.hurtBoxes[i].gameObject.SetActive(active);
			}
		}

		// Token: 0x060010A9 RID: 4265 RVA: 0x00063080 File Offset: 0x00061280
		public void OnValidate()
		{
			int num = 0;
			short num2 = 0;
			while ((int)num2 < this.hurtBoxes.Length)
			{
				this.hurtBoxes[(int)num2].hurtBoxGroup = this;
				this.hurtBoxes[(int)num2].indexInGroup = num2;
				if (this.hurtBoxes[(int)num2].isBullseye)
				{
					num++;
				}
				num2 += 1;
			}
			if (this.bullseyeCount != num)
			{
				this.bullseyeCount = num;
			}
			if (!this.mainHurtBox)
			{
				IEnumerable<HurtBox> source = from v in this.hurtBoxes
				where v.isBullseye
				select v;
				IEnumerable<HurtBox> source2 = from v in source
				where v.transform.parent.name.ToLower() == "chest"
				select v;
				this.mainHurtBox = (source2.FirstOrDefault<HurtBox>() ?? source.FirstOrDefault<HurtBox>());
			}
		}

		// Token: 0x060010AA RID: 4266 RVA: 0x0000CC1D File Offset: 0x0000AE1D
		public HurtBoxGroup.VolumeDistribution GetVolumeDistribution()
		{
			return new HurtBoxGroup.VolumeDistribution(this.hurtBoxes);
		}

		// Token: 0x040014B1 RID: 5297
		[Tooltip("The hurtboxes in this group. This really shouldn't be set manually.")]
		public HurtBox[] hurtBoxes;

		// Token: 0x040014B2 RID: 5298
		[Tooltip("The most important hurtbox in this group, usually a good center-of-mass target like the chest.")]
		public HurtBox mainHurtBox;

		// Token: 0x040014B3 RID: 5299
		[HideInInspector]
		public int bullseyeCount;

		// Token: 0x040014B4 RID: 5300
		private int _hurtBoxesDeactivatorCounter;

		// Token: 0x02000328 RID: 808
		public class VolumeDistribution
		{
			// Token: 0x1700016E RID: 366
			// (get) Token: 0x060010AC RID: 4268 RVA: 0x0000CC2A File Offset: 0x0000AE2A
			// (set) Token: 0x060010AD RID: 4269 RVA: 0x0000CC32 File Offset: 0x0000AE32
			public float totalVolume { get; private set; }

			// Token: 0x060010AE RID: 4270 RVA: 0x00063158 File Offset: 0x00061358
			public VolumeDistribution(HurtBox[] hurtBoxes)
			{
				this.totalVolume = 0f;
				for (int i = 0; i < hurtBoxes.Length; i++)
				{
					this.totalVolume += hurtBoxes[i].volume;
				}
				this.hurtBoxes = (HurtBox[])hurtBoxes.Clone();
			}

			// Token: 0x1700016F RID: 367
			// (get) Token: 0x060010AF RID: 4271 RVA: 0x000631AC File Offset: 0x000613AC
			public Vector3 randomVolumePoint
			{
				get
				{
					float num = UnityEngine.Random.Range(0f, this.totalVolume);
					HurtBox hurtBox = this.hurtBoxes[0];
					float num2 = 0f;
					for (int i = 0; i < this.hurtBoxes.Length; i++)
					{
						num2 += this.hurtBoxes[i].volume;
						if (num2 <= num)
						{
							hurtBox = this.hurtBoxes[i];
							break;
						}
					}
					return hurtBox.randomVolumePoint;
				}
			}

			// Token: 0x040014B6 RID: 5302
			private HurtBox[] hurtBoxes;
		}
	}
}
