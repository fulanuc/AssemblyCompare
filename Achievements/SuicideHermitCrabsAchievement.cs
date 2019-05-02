using System;
using RoR2.Stats;

namespace RoR2.Achievements
{
	// Token: 0x020006C5 RID: 1733
	[RegisterAchievement("SuicideHermitCrabs", "Items.AutoCastEquipment", null, typeof(SuicideHermitCrabsAchievement.SuicideHermitCrabsServerAchievement))]
	public class SuicideHermitCrabsAchievement : BaseAchievement
	{
		// Token: 0x060026A6 RID: 9894 RVA: 0x0001C49C File Offset: 0x0001A69C
		public override void OnInstall()
		{
			base.OnInstall();
			this.userProfile.onStatsReceived += this.Check;
			base.SetServerTracked(true);
		}

		// Token: 0x060026A7 RID: 9895 RVA: 0x0001C4C2 File Offset: 0x0001A6C2
		public override void OnUninstall()
		{
			this.userProfile.onStatsReceived -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x060026A8 RID: 9896 RVA: 0x0001C4E1 File Offset: 0x0001A6E1
		private void Check()
		{
			if (this.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) >= 20UL)
			{
				base.Grant();
			}
		}

		// Token: 0x060026A9 RID: 9897 RVA: 0x0001C503 File Offset: 0x0001A703
		public override float ProgressForAchievement()
		{
			return this.userProfile.statSheet.GetStatValueULong(StatDef.suicideHermitCrabsAchievementProgress) / 20f;
		}

		// Token: 0x040028B2 RID: 10418
		private const int requirement = 20;

		// Token: 0x020006C6 RID: 1734
		private class SuicideHermitCrabsServerAchievement : BaseServerAchievement
		{
			// Token: 0x060026AB RID: 9899 RVA: 0x0001C523 File Offset: 0x0001A723
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x060026AC RID: 9900 RVA: 0x0001C53C File Offset: 0x0001A73C
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x060026AD RID: 9901 RVA: 0x000B22FC File Offset: 0x000B04FC
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victimBody)
				{
					return;
				}
				if (damageReport.damageInfo.attacker)
				{
					return;
				}
				if (damageReport.victim.name.Contains("HermitCrab") && damageReport.victimBody.teamComponent.teamIndex != TeamIndex.Player)
				{
					PlayerStatsComponent masterPlayerStatsComponent = base.networkUser.masterPlayerStatsComponent;
					if (masterPlayerStatsComponent)
					{
						masterPlayerStatsComponent.currentStats.PushStatValue(StatDef.suicideHermitCrabsAchievementProgress, 1UL);
					}
				}
			}
		}
	}
}
