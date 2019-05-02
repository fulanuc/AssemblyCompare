using System;
using System.Linq;
using UnityEngine;

namespace RoR2.Achievements
{
	// Token: 0x020006B2 RID: 1714
	[RegisterAchievement("StayAlive1", "Items.ExtraLife", null, null)]
	public class StayAlive1Achievement : BaseAchievement
	{
		// Token: 0x0600260A RID: 9738 RVA: 0x0001BD1F File Offset: 0x00019F1F
		public override void OnInstall()
		{
			base.OnInstall();
			RoR2Application.onUpdate += this.Check;
		}

		// Token: 0x0600260B RID: 9739 RVA: 0x0001BD38 File Offset: 0x00019F38
		public override void OnUninstall()
		{
			RoR2Application.onUpdate -= this.Check;
			base.OnUninstall();
		}

		// Token: 0x0600260C RID: 9740 RVA: 0x000B0B9C File Offset: 0x000AED9C
		private void Check()
		{
			NetworkUser networkUser = NetworkUser.readOnlyLocalPlayersList.FirstOrDefault((NetworkUser v) => v.localUser == this.localUser);
			if (networkUser)
			{
				GameObject masterObject = networkUser.masterObject;
				if (masterObject)
				{
					CharacterMaster component = masterObject.GetComponent<CharacterMaster>();
					if (component && component.currentLifeStopwatch >= 1800f)
					{
						base.Grant();
					}
				}
			}
		}

		// Token: 0x04002855 RID: 10325
		private const float requirement = 1800f;
	}
}
