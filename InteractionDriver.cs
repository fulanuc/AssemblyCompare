using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033D RID: 829
	[RequireComponent(typeof(Interactor))]
	[RequireComponent(typeof(InputBankTest))]
	public class InteractionDriver : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x1700017B RID: 379
		// (get) Token: 0x06001108 RID: 4360 RVA: 0x0000CFED File Offset: 0x0000B1ED
		// (set) Token: 0x06001109 RID: 4361 RVA: 0x0000CFF5 File Offset: 0x0000B1F5
		public Interactor interactor { get; private set; }

		// Token: 0x0600110A RID: 4362 RVA: 0x0000CFFE File Offset: 0x0000B1FE
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.interactor = base.GetComponent<Interactor>();
			this.inputBank = base.GetComponent<InputBankTest>();
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x0006473C File Offset: 0x0006293C
		private void FixedUpdate()
		{
			if (this.networkIdentity.hasAuthority)
			{
				this.interactableCooldown -= Time.fixedDeltaTime;
				this.inputReceived = (this.inputBank.interact.justPressed || (this.inputBank.interact.down && this.interactableCooldown <= 0f));
				if (this.inputBank.interact.justReleased)
				{
					this.inputReceived = false;
					this.interactableCooldown = 0f;
				}
			}
			if (this.inputReceived)
			{
				GameObject gameObject = this.FindBestInteractableObject();
				if (gameObject)
				{
					this.interactor.AttemptInteraction(gameObject);
					this.interactableCooldown = 0.25f;
				}
			}
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x000647FC File Offset: 0x000629FC
		public GameObject FindBestInteractableObject()
		{
			if (this.interactableOverride)
			{
				return this.interactableOverride;
			}
			float num = 0f;
			Ray originalAimRay = new Ray(this.inputBank.aimOrigin, this.inputBank.aimDirection);
			Ray raycastRay = CameraRigController.ModifyAimRayIfApplicable(originalAimRay, base.gameObject, out num);
			return this.interactor.FindBestInteractableObject(raycastRay, this.interactor.maxInteractionDistance + num, originalAimRay.origin, this.interactor.maxInteractionDistance);
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x0000D024 File Offset: 0x0000B224
		static InteractionDriver()
		{
			OutlineHighlight.onPreRenderOutlineHighlight = (Action<OutlineHighlight>)Delegate.Combine(OutlineHighlight.onPreRenderOutlineHighlight, new Action<OutlineHighlight>(InteractionDriver.OnPreRenderOutlineHighlight));
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x0006487C File Offset: 0x00062A7C
		private static void OnPreRenderOutlineHighlight(OutlineHighlight outlineHighlight)
		{
			if (!outlineHighlight.sceneCamera)
			{
				return;
			}
			if (!outlineHighlight.sceneCamera.cameraRigController)
			{
				return;
			}
			GameObject target = outlineHighlight.sceneCamera.cameraRigController.target;
			if (!target)
			{
				return;
			}
			InteractionDriver component = target.GetComponent<InteractionDriver>();
			if (!component)
			{
				return;
			}
			GameObject gameObject = component.FindBestInteractableObject();
			if (!gameObject)
			{
				return;
			}
			IInteractable component2 = gameObject.GetComponent<IInteractable>();
			Highlight component3 = gameObject.GetComponent<Highlight>();
			if (!component3)
			{
				return;
			}
			Color a = component3.GetColor();
			if (component2 != null && component2.GetInteractability(component.interactor) == Interactability.ConditionsNotMet)
			{
				a = ColorCatalog.GetColor(ColorCatalog.ColorIndex.Unaffordable);
			}
			outlineHighlight.highlightQueue.Enqueue(new OutlineHighlight.HighlightInfo
			{
				renderer = component3.targetRenderer,
				color = a * component3.strength
			});
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x0000A0C2 File Offset: 0x000082C2
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x0400153B RID: 5435
		public bool highlightInteractor;

		// Token: 0x0400153C RID: 5436
		private bool inputReceived;

		// Token: 0x0400153D RID: 5437
		private NetworkIdentity networkIdentity;

		// Token: 0x0400153F RID: 5439
		private InputBankTest inputBank;

		// Token: 0x04001540 RID: 5440
		[NonSerialized]
		public GameObject interactableOverride;

		// Token: 0x04001541 RID: 5441
		private const float interactableCooldownDuration = 0.25f;

		// Token: 0x04001542 RID: 5442
		private float interactableCooldown;
	}
}
