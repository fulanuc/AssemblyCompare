using System;
using JetBrains.Annotations;

namespace RoR2
{
	// Token: 0x020001FA RID: 506
	[MeansImplicitUse]
	public class RegisterAchievementAttribute : Attribute
	{
		// Token: 0x060009F3 RID: 2547 RVA: 0x00008047 File Offset: 0x00006247
		public RegisterAchievementAttribute([NotNull] string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, Type serverTrackerType = null)
		{
			this.identifier = identifier;
			this.unlockableRewardIdentifier = unlockableRewardIdentifier;
			this.prerequisiteAchievementIdentifier = prerequisiteAchievementIdentifier;
			this.serverTrackerType = serverTrackerType;
		}

		// Token: 0x04000D27 RID: 3367
		public readonly string identifier;

		// Token: 0x04000D28 RID: 3368
		public readonly string unlockableRewardIdentifier;

		// Token: 0x04000D29 RID: 3369
		public readonly string prerequisiteAchievementIdentifier;

		// Token: 0x04000D2A RID: 3370
		public readonly Type serverTrackerType;
	}
}
