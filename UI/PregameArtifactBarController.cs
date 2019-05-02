using System;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x0200061C RID: 1564
	[RequireComponent(typeof(RectTransform))]
	public class PregameArtifactBarController : MonoBehaviour
	{
		// Token: 0x06002333 RID: 9011 RVA: 0x000A8E50 File Offset: 0x000A7050
		private void Start()
		{
			RectTransform component = base.GetComponent<RectTransform>();
			for (ArtifactIndex artifactIndex = ArtifactIndex.Command; artifactIndex < ArtifactIndex.Count; artifactIndex++)
			{
				UnityEngine.Object.Instantiate<GameObject>(this.artifactTogglePrefab, component).GetComponent<ArtifactToggleController>().artifactIndex = artifactIndex;
			}
		}

		// Token: 0x0400261E RID: 9758
		public GameObject artifactTogglePrefab;
	}
}
