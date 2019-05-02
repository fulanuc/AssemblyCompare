using System;

namespace RoR2.Achievements
{
	// Token: 0x020006AB RID: 1707
	[RegisterAchievement("MaxHealingShrine", "Items.PassiveHealing", null, typeof(MaxHealingShrineAchievement.MaxHealingShrineServerAchievement))]
	public class MaxHealingShrineAchievement : BaseAchievement
	{
		// Token: 0x060025EE RID: 9710 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025EF RID: 9711 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006AC RID: 1708
		private class MaxHealingShrineServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025F1 RID: 9713 RVA: 0x0001BB97 File Offset: 0x00019D97
			public override void OnInstall()
			{
				base.OnInstall();
				ShrineHealingBehavior.onActivated += this.OnHealingShrineActivated;
			}

			// Token: 0x060025F2 RID: 9714 RVA: 0x0001BBB0 File Offset: 0x00019DB0
			public override void OnUninstall()
			{
				ShrineHealingBehavior.onActivated -= this.OnHealingShrineActivated;
				base.OnUninstall();
			}

			// Token: 0x060025F3 RID: 9715 RVA: 0x000B0A04 File Offset: 0x000AEC04
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

			// Token: 0x0400284F RID: 10319
			private const int requirement = 2;
		}
	}
}
