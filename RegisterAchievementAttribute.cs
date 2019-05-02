using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020001FA RID: 506
	public class RegisterAchievementAttribute : Attribute
	{
		// Token: 0x060009F0 RID: 2544 RVA: 0x00008038 File Offset: 0x00006238
		public RegisterAchievementAttribute([NotNull] string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, Type serverTrackerType = null)
		{
			this.identifier = identifier;
			this.unlockableRewardIdentifier = unlockableRewardIdentifier;
			this.prerequisiteAchievementIdentifier = prerequisiteAchievementIdentifier;
			this.serverTrackerType = serverTrackerType;
		}

		// Token: 0x04000D23 RID: 3363
		public readonly string identifier;

		// Token: 0x04000D24 RID: 3364
		public readonly string unlockableRewardIdentifier;

		// Token: 0x04000D25 RID: 3365
		public readonly string prerequisiteAchievementIdentifier;

		// Token: 0x04000D26 RID: 3366
		public readonly Type serverTrackerType;
	}
}
