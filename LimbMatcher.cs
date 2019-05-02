using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034B RID: 843
	public class LimbMatcher : MonoBehaviour
	{
		// Token: 0x06001182 RID: 4482 RVA: 0x00066494 File Offset: 0x00064694
		public void SetChildLocator(ChildLocator childLocator)
		{
			for (int i = 0; i < this.limbPairs.Length; i++)
			{
				LimbMatcher.LimbPair limbPair = this.limbPairs[i];
				Transform targetTransform = childLocator.FindChild(limbPair.targetChildLimb);
				this.limbPairs[i].targetTransform = targetTransform;
			}
		}

		// Token: 0x06001183 RID: 4483 RVA: 0x0000D5DF File Offset: 0x0000B7DF
		private void LateUpdate()
		{
			this.UpdateLimbs();
		}

		// Token: 0x06001184 RID: 4484 RVA: 0x000664E0 File Offset: 0x000646E0
		private void UpdateLimbs()
		{
			for (int i = 0; i < this.limbPairs.Length; i++)
			{
				LimbMatcher.LimbPair limbPair = this.limbPairs[i];
				Transform targetTransform = limbPair.targetTransform;
				if (targetTransform && limbPair.originalTransform)
				{
					limbPair.originalTransform.position = targetTransform.position;
					limbPair.originalTransform.rotation = targetTransform.rotation;
					if (i < this.limbPairs.Length - 1)
					{
						float num = Vector3.Magnitude(this.limbPairs[i + 1].targetTransform.position - targetTransform.position);
						float originalLimbLength = limbPair.originalLimbLength;
						Vector3 localScale = limbPair.originalTransform.localScale;
						localScale.y = num / originalLimbLength;
						limbPair.originalTransform.localScale = localScale;
					}
				}
			}
		}

		// Token: 0x04001578 RID: 5496
		public LimbMatcher.LimbPair[] limbPairs;

		// Token: 0x0200034C RID: 844
		[Serializable]
		public struct LimbPair
		{
			// Token: 0x04001579 RID: 5497
			public Transform originalTransform;

			// Token: 0x0400157A RID: 5498
			public string targetChildLimb;

			// Token: 0x0400157B RID: 5499
			public float originalLimbLength;

			// Token: 0x0400157C RID: 5500
			[NonSerialized]
			public Transform targetTransform;
		}
	}
}
