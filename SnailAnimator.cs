using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003E9 RID: 1001
	public class SnailAnimator : MonoBehaviour
	{
		// Token: 0x060015DB RID: 5595 RVA: 0x00010668 File Offset: 0x0000E868
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			this.characterModel = base.GetComponentInParent<CharacterModel>();
		}

		// Token: 0x060015DC RID: 5596 RVA: 0x00075194 File Offset: 0x00073394
		private void FixedUpdate()
		{
			if (this.characterModel)
			{
				CharacterBody body = this.characterModel.body;
				if (body)
				{
					bool outOfDanger = body.outOfDanger;
					if (outOfDanger && !this.lastOutOfDanger)
					{
						this.animator.SetBool("spawn", true);
						this.animator.SetBool("hide", false);
						Util.PlaySound("Play_item_proc_slug_emerge", this.characterModel.gameObject);
						this.healEffectSystem.main.loop = true;
						this.healEffectSystem.Play();
					}
					else if (!outOfDanger && this.lastOutOfDanger)
					{
						this.animator.SetBool("hide", true);
						this.animator.SetBool("spawn", false);
						Util.PlaySound("Play_item_proc_slug_hide", this.characterModel.gameObject);
						this.healEffectSystem.main.loop = false;
					}
					this.lastOutOfDanger = outOfDanger;
				}
			}
		}

		// Token: 0x0400192E RID: 6446
		public ParticleSystem healEffectSystem;

		// Token: 0x0400192F RID: 6447
		private bool lastOutOfDanger;

		// Token: 0x04001930 RID: 6448
		private Animator animator;

		// Token: 0x04001931 RID: 6449
		private CharacterModel characterModel;
	}
}
