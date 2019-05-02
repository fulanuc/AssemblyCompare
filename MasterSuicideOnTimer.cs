using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000356 RID: 854
	public class MasterSuicideOnTimer : MonoBehaviour
	{
		// Token: 0x060011A3 RID: 4515 RVA: 0x0000D6E0 File Offset: 0x0000B8E0
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x060011A4 RID: 4516 RVA: 0x00066AD4 File Offset: 0x00064CD4
		private void FixedUpdate()
		{
			if (NetworkServer.active)
			{
				this.timer += Time.fixedDeltaTime;
				if (this.timer >= this.lifeTimer && !this.hasDied)
				{
					GameObject bodyObject = this.characterMaster.GetBodyObject();
					if (bodyObject)
					{
						HealthComponent component = bodyObject.GetComponent<HealthComponent>();
						if (component)
						{
							component.Suicide(null);
							this.hasDied = true;
						}
					}
				}
			}
		}

		// Token: 0x0400159B RID: 5531
		public float lifeTimer;

		// Token: 0x0400159C RID: 5532
		private float timer;

		// Token: 0x0400159D RID: 5533
		private bool hasDied;

		// Token: 0x0400159E RID: 5534
		private CharacterMaster characterMaster;
	}
}
