using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200062E RID: 1582
	[RequireComponent(typeof(RectTransform))]
	public class PregameArtifactBarController : MonoBehaviour
	{
		// Token: 0x060023C3 RID: 9155 RVA: 0x000AA4CC File Offset: 0x000A86CC
		private void Start()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.artifactTogglePrefab, component).GetComponent<ArtifactToggleController>().artifactIndex = artifactIndex;
			}
		}

		// Token: 0x04002679 RID: 9849
		public GameObject artifactTogglePrefab;
	}
}
