using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200028C RID: 652
	public class CharacterEmoteDefinitions : MonoBehaviour
	{
		// Token: 0x06000CD6 RID: 3286 RVA: 0x000526A8 File Offset: 0x000508A8
		public int FindEmoteIndex(string name)
		{
			for (int i = 0; i < this.emoteDefinitions.Length; i++)
			{
				if (this.emoteDefinitions[i].name == name)
				{
					return i;
				}
			}
			return -1;
		}

		// Token: 0x040010F8 RID: 4344
		public CharacterEmoteDefinitions.EmoteDef[] emoteDefinitions;

		// Token: 0x0200028D RID: 653
		[Serializable]
		public struct EmoteDef
		{
			// Token: 0x040010F9 RID: 4345
			public string name;

			// Token: 0x040010FA RID: 4346
			public string displayName;

			// Token: 0x040010FB RID: 4347
			public EntityStateMachine targetStateMachine;

			// Token: 0x040010FC RID: 4348
			public SerializableEntityStateType state;
		}
	}
}
