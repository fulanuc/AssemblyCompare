using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003EF RID: 1007
	public class SnailAnimator : MonoBehaviour
	{
		// Token: 0x06001618 RID: 5656 RVA: 0x00010A71 File Offset: 0x0000EC71
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			this.characterModel = base.GetComponentInParent<CharacterModel>();
		}

		// Token: 0x06001619 RID: 5657 RVA: 0x000757CC File Offset: 0x000739CC
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

		// Token: 0x04001957 RID: 6487
		public ParticleSystem healEffectSystem;

		// Token: 0x04001958 RID: 6488
		private bool lastOutOfDanger;

		// Token: 0x04001959 RID: 6489
		private Animator animator;

		// Token: 0x0400195A RID: 6490
		private CharacterModel characterModel;
	}
}
