using System;

namespace RoR2
{
	// Token: 0x02000202 RID: 514
	public static class ArtifactCatalog
	{
		// Token: 0x06000A08 RID: 2568 RVA: 0x000081C7 File Offset: 0x000063C7
		private static void RegisterArtifact(ArtifactIndex artifactIndex, ArtifactDef artifactDef)
		{
			ArtifactCatalog.artifactDefs[(int)artifactIndex] = artifactDef;
		}

		// Token: 0x06000A09 RID: 2569 RVA: 0x0004659C File Offset: 0x0004479C
		static ArtifactCatalog()
		{
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Command, new ArtifactDef
			{
				nameToken = "ARTIFACT_COMMAND_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texCommandSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texCommandSmallDeselected",
				unlockableName = "artifact_command"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Bomb, new ArtifactDef
			{
				nameToken = "ARTIFACT_BOMB_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSpiteSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSpiteSmallDeselected",
				unlockableName = "artifact_bomb"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Sacrifice, new ArtifactDef
			{
				nameToken = "ARTIFACT_SACRIFICE_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSacrificeSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSacrificeSmallDeselected",
				unlockableName = "artifact_sacrifice"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Enigma, new ArtifactDef
			{
				nameToken = "ARTIFACT_ENIGMA_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texEnigmaSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texEnigmaSmallDeselected",
				unlockableName = "artifact_enigma"
			});
			ArtifactCatalog.RegisterArtifact(ArtifactIndex.Spirit, new ArtifactDef
			{
				nameToken = "ARTIFACT_SPIRIT_NAME",
				smallIconSelectedPath = "Textures/ArtifactIcons/texSpiritSmallSelected",
				smallIconDeselectedPath = "Textures/ArtifactIcons/texSpiritSmallDeselected",
				unlockableName = "artifact_spirit"
			});
		}

		// Token: 0x06000A0A RID: 2570 RVA: 0x000081D1 File Offset: 0x000063D1
		public static ArtifactDef GetArtifactDef(ArtifactIndex artifactIndex)
		{
			if (artifactIndex < ArtifactIndex.Command || artifactIndex >= ArtifactIndex.Count)
			{
				return null;
			}
			return ArtifactCatalog.artifactDefs[(int)artifactIndex];
		}

		// Token: 0x04000D48 RID: 3400
		private static ArtifactDef[] artifactDefs = new ArtifactDef[5];
	}
}
