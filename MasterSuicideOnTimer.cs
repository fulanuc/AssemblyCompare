using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000359 RID: 857
	public class MasterSuicideOnTimer : MonoBehaviour
	{
		// Token: 0x060011BA RID: 4538 RVA: 0x0000D7C9 File Offset: 0x0000B9C9
		private void Start()
		{
			this.characterMaster = base.GetComponent<CharacterMaster>();
		}

		// Token: 0x060011BB RID: 4539 RVA: 0x00066E0C File Offset: 0x0006500C
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

		// Token: 0x040015B4 RID: 5556
		public float lifeTimer;

		// Token: 0x040015B5 RID: 5557
		private float timer;

		// Token: 0x040015B6 RID: 5558
		private bool hasDied;

		// Token: 0x040015B7 RID: 5559
		private CharacterMaster characterMaster;
	}
}
