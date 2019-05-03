﻿using System;
using RoR2.Projectile;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000424 RID: 1060
	public class WinchControl : MonoBehaviour
	{
		// Token: 0x060017AF RID: 6063 RVA: 0x00011B50 File Offset: 0x0000FD50
		private void Start()
		{
			this.attachmentTransform = this.FindAttachmentTransform();
			if (this.attachmentTransform)
			{
				this.tailTransform.position = this.attachmentTransform.position;
			}
		}

		// Token: 0x060017B0 RID: 6064 RVA: 0x00011B81 File Offset: 0x0000FD81
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

		// Token: 0x060017B1 RID: 6065 RVA: 0x0007B438 File Offset: 0x00079638
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

		// Token: 0x04001AC8 RID: 6856
		public Transform tailTransform;

		// Token: 0x04001AC9 RID: 6857
		public string attachmentString;

		// Token: 0x04001ACA RID: 6858
		private ProjectileGhostController projectileGhostController;

		// Token: 0x04001ACB RID: 6859
		private Transform attachmentTransform;
	}
}
