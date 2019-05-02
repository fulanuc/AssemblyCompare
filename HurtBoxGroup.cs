using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000329 RID: 809
	public class HurtBoxGroup : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x060010B9 RID: 4281 RVA: 0x0000CCF0 File Offset: 0x0000AEF0
		public void OnDeathStart()
		{
			this.SetHurtboxesActive(false);
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060010BA RID: 4282 RVA: 0x0000CCF9 File Offset: 0x0000AEF9
		// (set) Token: 0x060010BB RID: 4283 RVA: 0x000632A4 File Offset: 0x000614A4
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

		// Token: 0x060010BC RID: 4284 RVA: 0x000632D8 File Offset: 0x000614D8
		private void SetHurtboxesActive(bool active)
		{
			for (int i = 0; i < this.hurtBoxes.Length; i++)
			{
				this.hurtBoxes[i].gameObject.SetActive(active);
			}
		}

		// Token: 0x060010BD RID: 4285 RVA: 0x0006330C File Offset: 0x0006150C
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
				where v.transform.parent.name.ToLower(CultureInfo.InvariantCulture) == "chest"
				select v;
				this.mainHurtBox = (source2.FirstOrDefault<HurtBox>() ?? source.FirstOrDefault<HurtBox>());
			}
		}

		// Token: 0x060010BE RID: 4286 RVA: 0x0000CD01 File Offset: 0x0000AF01
		public HurtBoxGroup.VolumeDistribution GetVolumeDistribution()
		{
			return new HurtBoxGroup.VolumeDistribution(this.hurtBoxes);
		}

		// Token: 0x040014C5 RID: 5317
		[Tooltip("The hurtboxes in this group. This really shouldn't be set manually.")]
		public HurtBox[] hurtBoxes;

		// Token: 0x040014C6 RID: 5318
		[Tooltip("The most important hurtbox in this group, usually a good center-of-mass target like the chest.")]
		public HurtBox mainHurtBox;

		// Token: 0x040014C7 RID: 5319
		[HideInInspector]
		public int bullseyeCount;

		// Token: 0x040014C8 RID: 5320
		private int _hurtBoxesDeactivatorCounter;

		// Token: 0x0200032A RID: 810
		public class VolumeDistribution
		{
			// Token: 0x17000173 RID: 371
			// (get) Token: 0x060010C0 RID: 4288 RVA: 0x0000CD0E File Offset: 0x0000AF0E
			// (set) Token: 0x060010C1 RID: 4289 RVA: 0x0000CD16 File Offset: 0x0000AF16
			public float totalVolume { get; private set; }

			// Token: 0x060010C2 RID: 4290 RVA: 0x000633E4 File Offset: 0x000615E4
			public VolumeDistribution(HurtBox[] hurtBoxes)
			{
				this.totalVolume = 0f;
				for (int i = 0; i < hurtBoxes.Length; i++)
				{
					this.totalVolume += hurtBoxes[i].volume;
				}
				this.hurtBoxes = (HurtBox[])hurtBoxes.Clone();
			}

			// Token: 0x17000174 RID: 372
			// (get) Token: 0x060010C3 RID: 4291 RVA: 0x00063438 File Offset: 0x00061638
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

			// Token: 0x040014CA RID: 5322
			private HurtBox[] hurtBoxes;
		}
	}
}
