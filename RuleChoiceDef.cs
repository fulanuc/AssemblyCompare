using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200047E RID: 1150
	public class RuleChoiceDef
	{
		// Token: 0x04001D00 RID: 7424
		public RuleDef ruleDef;

		// Token: 0x04001D01 RID: 7425
		public string spritePath;

		// Token: 0x04001D02 RID: 7426
		public string materialPath;

		// Token: 0x04001D03 RID: 7427
		public string tooltipNameToken;

		// Token: 0x04001D04 RID: 7428
		public Color tooltipNameColor = Color.white;

		// Token: 0x04001D05 RID: 7429
		public string tooltipBodyToken;

		// Token: 0x04001D06 RID: 7430
		public Color tooltipBodyColor = Color.white;

		// Token: 0x04001D07 RID: 7431
		public string localName;

		// Token: 0x04001D08 RID: 7432
		public string globalName;

		// Token: 0x04001D09 RID: 7433
		public int localIndex;

		// Token: 0x04001D0A RID: 7434
		public int globalIndex;

		// Token: 0x04001D0B RID: 7435
		public string unlockableName;

		// Token: 0x04001D0C RID: 7436
		public bool availableInSinglePlayer = true;

		// Token: 0x04001D0D RID: 7437
		public bool availableInMultiPlayer = true;

		// Token: 0x04001D0E RID: 7438
		public DifficultyIndex difficultyIndex = DifficultyIndex.Invalid;

		// Token: 0x04001D0F RID: 7439
		public ArtifactIndex artifactIndex = ArtifactIndex.None;

		// Token: 0x04001D10 RID: 7440
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x04001D11 RID: 7441
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x04001D12 RID: 7442
		public object extraData;

		// Token: 0x04001D13 RID: 7443
		public bool excludeByDefault;
	}
}
