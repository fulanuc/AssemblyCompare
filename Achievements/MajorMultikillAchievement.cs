using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006A9 RID: 1705
	[RegisterAchievement("MajorMultikill", "Items.BurnNearby", null, typeof(MajorMultikillAchievement.MajorMultikillServerAchievement))]
	public class MajorMultikillAchievement : BaseAchievement
	{
		// Token: 0x060025E7 RID: 9703 RVA: 0x0001B314 File Offset: 0x00019514
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x060025E8 RID: 9704 RVA: 0x0001B323 File Offset: 0x00019523
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006AA RID: 1706
		private class MajorMultikillServerAchievement : BaseServerAchievement
		{
			// Token: 0x060025EA RID: 9706 RVA: 0x0001BB65 File Offset: 0x00019D65
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x060025EB RID: 9707 RVA: 0x0001BB7E File Offset: 0x00019D7E
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x060025EC RID: 9708 RVA: 0x000B09A4 File Offset: 0x000AEBA4
			private void OnCharacterDeath(DamageReport damageReport)
			{
				GameObject attacker = damageReport.damageInfo.attacker;
				if (!attacker)
				{
					return;
				}
				CharacterBody component = attacker.GetComponent<CharacterBody>();
				if (!component)
				{
					return;
				}
				if (component.multiKillCount >= 15 && component.masterObject == this.serverAchievementTracker.networkUser.masterObject)
				{
					base.Grant();
				}
			}

			// Token: 0x0400284E RID: 10318
			private const int multiKillThreshold = 15;
		}
	}
}
