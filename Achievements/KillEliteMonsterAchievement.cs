using System;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006B6 RID: 1718
	[RegisterAchievement("KillEliteMonster", "Items.Medkit", null, typeof(KillEliteMonsterAchievement.KillEliteMonsterServerAchievement))]
	public class KillEliteMonsterAchievement : BaseAchievement
	{
		// Token: 0x06002666 RID: 9830 RVA: 0x0001BA4F File Offset: 0x00019C4F
		public override void OnInstall()
		{
			base.OnInstall();
			base.SetServerTracked(true);
		}

		// Token: 0x06002667 RID: 9831 RVA: 0x0001BA5E File Offset: 0x00019C5E
		public override void OnUninstall()
		{
			base.OnUninstall();
		}

		// Token: 0x020006B7 RID: 1719
		private class KillEliteMonsterServerAchievement : BaseServerAchievement
		{
			// Token: 0x06002669 RID: 9833 RVA: 0x0001C137 File Offset: 0x0001A337
			public override void OnInstall()
			{
				base.OnInstall();
				GlobalEventManager.onCharacterDeathGlobal += this.OnCharacterDeath;
			}

			// Token: 0x0600266A RID: 9834 RVA: 0x0001C150 File Offset: 0x0001A350
			public override void OnUninstall()
			{
				GlobalEventManager.onCharacterDeathGlobal -= this.OnCharacterDeath;
				base.OnUninstall();
			}

			// Token: 0x0600266B RID: 9835 RVA: 0x000B1F4C File Offset: 0x000B014C
			private void OnCharacterDeath(DamageReport damageReport)
			{
				if (!damageReport.victim)
				{
					return;
				}
				CharacterBody body = damageReport.victim.body;
				if (!body || !body.isElite)
				{
					return;
				}
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
				if (component.masterObject == this.serverAchievementTracker.networkUser.masterObject)
				{
					base.Grant();
				}
			}
		}
	}
}
