using System;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200042A RID: 1066
	public class WinchControl : MonoBehaviour
	{
		// Token: 0x060017F3 RID: 6131 RVA: 0x00011F82 File Offset: 0x00010182
		private void Start()
		{
			this.attachmentTransform = this.FindAttachmentTransform();
			if (this.attachmentTransform)
			{
				this.tailTransform.position = this.attachmentTransform.position;
			}
		}

		// Token: 0x060017F4 RID: 6132 RVA: 0x00011FB3 File Offset: 0x000101B3
		private void Update()
		{
			if (!this.attachmentTransform)
			{
				this.attachmentTransform = this.FindAttachmentTransform();
			}
			if (this.attachmentTransform)
			{
				this.tailTransform.position = this.attachmentTransform.position;
			}
		}

		// Token: 0x060017F5 RID: 6133 RVA: 0x0007B9F8 File Offset: 0x00079BF8
		private Transform FindAttachmentTransform()
		{
			this.projectileGhostController = base.GetComponent<ProjectileGhostController>();
			if (this.projectileGhostController)
			{
				Transform authorityTransform = this.projectileGhostController.authorityTransform;
				if (authorityTransform)
				{
					ProjectileController component = authorityTransform.GetComponent<ProjectileController>();
					if (component)
					{
						GameObject owner = component.owner;
						if (owner)
						{
							ModelLocator component2 = owner.GetComponent<ModelLocator>();
							if (component2)
							{
								Transform modelTransform = component2.modelTransform;
								if (modelTransform)
								{
									ChildLocator component3 = modelTransform.GetComponent<ChildLocator>();
									if (component3)
									{
										return component3.FindChild(this.attachmentString);
									}
								}
							}
						}
					}
				}
			}
			return null;
		}

		// Token: 0x04001AF1 RID: 6897
		public Transform tailTransform;

		// Token: 0x04001AF2 RID: 6898
		public string attachmentString;

		// Token: 0x04001AF3 RID: 6899
		private ProjectileGhostController projectileGhostController;

		// Token: 0x04001AF4 RID: 6900
		private Transform attachmentTransform;
	}
}
