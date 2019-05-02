using System;
using EntityStates;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200028C RID: 652
	public class CharacterEmoteDefinitions : MonoBehaviour
	{
		// Token: 0x06000CE3 RID: 3299 RVA: 0x00052A50 File Offset: 0x00050C50
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

		// Token: 0x04001103 RID: 4355
		public CharacterEmoteDefinitions.EmoteDef[] emoteDefinitions;

		// Token: 0x0200028D RID: 653
		[Serializable]
		public struct EmoteDef
		{
			// Token: 0x04001104 RID: 4356
			public string name;

			// Token: 0x04001105 RID: 4357
			public string displayName;

			// Token: 0x04001106 RID: 4358
			public EntityStateMachine targetStateMachine;

			// Token: 0x04001107 RID: 4359
			public SerializableEntityStateType state;
		}
	}
}
