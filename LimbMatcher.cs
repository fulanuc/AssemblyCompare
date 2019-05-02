using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200034E RID: 846
	public class LimbMatcher : MonoBehaviour
	{
		// Token: 0x06001199 RID: 4505 RVA: 0x000667CC File Offset: 0x000649CC
		public void SetChildLocator(ChildLocator childLocator)
		{
			for (int i = 0; i < this.limbPairs.Length; i++)
			{
				LimbMatcher.LimbPair limbPair = this.limbPairs[i];
				Transform targetTransform = childLocator.FindChild(limbPair.targetChildLimb);
				this.limbPairs[i].targetTransform = targetTransform;
			}
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x0000D6C8 File Offset: 0x0000B8C8
		private void LateUpdate()
		{
			this.UpdateLimbs();
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00066818 File Offset: 0x00064A18
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

		// Token: 0x04001591 RID: 5521
		public LimbMatcher.LimbPair[] limbPairs;

		// Token: 0x0200034F RID: 847
		[Serializable]
		public struct LimbPair
		{
			// Token: 0x04001592 RID: 5522
			public Transform originalTransform;

			// Token: 0x04001593 RID: 5523
			public string targetChildLimb;

			// Token: 0x04001594 RID: 5524
			public float originalLimbLength;

			// Token: 0x04001595 RID: 5525
			[NonSerialized]
			public Transform targetTransform;
		}
	}
}
