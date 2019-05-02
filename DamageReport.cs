using System;

namespace RoR2
{
	// Token: 0x02000239 RID: 569
	public class DamageReport
	{
		// Token: 0x170000BB RID: 187
		// (get) Token: 0x06000AC5 RID: 2757 RVA: 0x00008B17 File Offset: 0x00006D17
		// (set) Token: 0x06000AC4 RID: 2756 RVA: 0x00049244 File Offset: 0x00047444
		public HealthComponent victim
		{
			get
			{
				return this._victim;
			}
			set
			{
				this._victim = value;
				this.victimBody = (value ? value.body : null);
				this.victimMaster = (this.victimBody ? this.victimBody.master : null);
			}
		}

		// Token: 0x170000BC RID: 188
		// (get) Token: 0x06000AC6 RID: 2758 RVA: 0x00008B1F File Offset: 0x00006D1F
		// (set) Token: 0x06000AC7 RID: 2759 RVA: 0x00008B27 File Offset: 0x00006D27
		public CharacterBody victimBody { get; private set; }

		// Token: 0x170000BD RID: 189
		// (get) Token: 0x06000AC8 RID: 2760 RVA: 0x00008B30 File Offset: 0x00006D30
		// (set) Token: 0x06000AC9 RID: 2761 RVA: 0x00008B38 File Offset: 0x00006D38
		public CharacterMaster victimMaster { get; private set; }

		// Token: 0x04000E7A RID: 3706
		private HealthComponent _victim;

		// Token: 0x04000E7D RID: 3709
		public DamageInfo damageInfo;
	}
}
