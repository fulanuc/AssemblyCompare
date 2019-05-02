using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x020002C7 RID: 711
	public class DebuffZone : MonoBehaviour
	{
		// Token: 0x06000E74 RID: 3700 RVA: 0x000025F6 File Offset: 0x000007F6
		private void Awake()
		{
		}

		// Token: 0x06000E75 RID: 3701 RVA: 0x00058DFC File Offset: 0x00056FFC
		private void OnTriggerEnter(Collider other)
		{
			if (NetworkServer.active)
			{
				CharacterBody component = other.GetComponent<CharacterBody>();
				if (component)
				{
					component.AddTimedBuff(this.buffType, this.buffDuration);
					Util.PlaySound(this.buffApplicationSoundString, component.gameObject);
					if (this.buffApplicationEffectPrefab)
					{
						EffectManager.instance.SpawnEffect(this.buffApplicationEffectPrefab, new EffectData
						{
							origin = component.mainHurtBox.transform.position,
							scale = component.radius
						}, true);
					}
				}
			}
		}

		// Token: 0x04001255 RID: 4693
		[Tooltip("The buff type to grant")]
		public BuffIndex buffType;

		// Token: 0x04001256 RID: 4694
		[Tooltip("The buff duration")]
		public float buffDuration;

		// Token: 0x04001257 RID: 4695
		public string buffApplicationSoundString;

		// Token: 0x04001258 RID: 4696
		public GameObject buffApplicationEffectPrefab;
	}
}
