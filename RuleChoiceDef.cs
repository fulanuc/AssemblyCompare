using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200048B RID: 1163
	public class RuleChoiceDef
	{
		// Token: 0x04001D37 RID: 7479
		public RuleDef ruleDef;

		// Token: 0x04001D38 RID: 7480
		public string spritePath;

		// Token: 0x04001D39 RID: 7481
		public string materialPath;

		// Token: 0x04001D3A RID: 7482
		public string tooltipNameToken;

		// Token: 0x04001D3B RID: 7483
		public Color tooltipNameColor = Color.white;

		// Token: 0x04001D3C RID: 7484
		public string tooltipBodyToken;

		// Token: 0x04001D3D RID: 7485
		public Color tooltipBodyColor = Color.white;

		// Token: 0x04001D3E RID: 7486
		public string localName;

		// Token: 0x04001D3F RID: 7487
		public string globalName;

		// Token: 0x04001D40 RID: 7488
		public int localIndex;

		// Token: 0x04001D41 RID: 7489
		public int globalIndex;

		// Token: 0x04001D42 RID: 7490
		public string unlockableName;

		// Token: 0x04001D43 RID: 7491
		public bool availableInSinglePlayer = true;

		// Token: 0x04001D44 RID: 7492
		public bool availableInMultiPlayer = true;

		// Token: 0x04001D45 RID: 7493
		public DifficultyIndex difficultyIndex = DifficultyIndex.Invalid;

		// Token: 0x04001D46 RID: 7494
		public ArtifactIndex artifactIndex = ArtifactIndex.None;

		// Token: 0x04001D47 RID: 7495
		public ItemIndex itemIndex = ItemIndex.None;

		// Token: 0x04001D48 RID: 7496
		public EquipmentIndex equipmentIndex = EquipmentIndex.None;

		// Token: 0x04001D49 RID: 7497
		public object extraData;

		// Token: 0x04001D4A RID: 7498
		public bool excludeByDefault;
	}
}
