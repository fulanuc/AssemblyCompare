using System;

namespace RoR2.Achievements
{
	// Token: 0x020006BD RID: 1725
	[RegisterAchievement("MaxHealingShrine", "Items.PassiveHealing", null, typeof(MaxHealingShrineAchievement.MaxHealingShrineServerAchievement))]
	public class MaxHealingShrineAchievement : BaseAchievement
	{
		// Token: 0x06002685 RID: 9861 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002686 RID: 9862 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006BE RID: 1726
		private class MaxHealingShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002688 RID: 9864 RVA: 0x0001C2D2 File Offset: 0x0001A4D2
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineHealingBehavior.onActivated += this.OnHealingShrineActivated;
			}

			// Token: 0x06002689 RID: 9865 RVA: 0x0001C2EB File Offset: 0x0001A4EB
			public override void OnUninstall()
			{
				ShrineHealingBehavior.onActivated -= this.OnHealingShrineActivated;
				base.OnUninstall();
			}

			// Token: 0x0600268A RID: 9866 RVA: 0x000B20FC File Offset: 0x000B02FC
			private void OnHealingShrineActivated(ShrineHealingBehavior shrine, Interactor activator)
			{
				if (shrine.purchaseCount >= shrine.maxPurchaseCount)
				{
					CharacterBody currentBody = base.GetCurrentBody();
					if (currentBody && currentBody.gameObject == activator.gameObject)
					{
						base.Grant();
					}
				}
			}

			// Token: 0x040028AB RID: 10411
			private const int requirement = 2;
		}
	}
}
