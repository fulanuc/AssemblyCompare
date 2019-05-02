using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200033B RID: 827
	[RequireComponent(typeof(Interactor))]
	[RequireComponent(typeof(InputBankTest))]
	public class InteractionDriver : MonoBehaviour, ILifeBehavior
	{
		// Token: 0x17000176 RID: 374
		// (get) Token: 0x060010F4 RID: 4340 RVA: 0x0000CF04 File Offset: 0x0000B104
		// (set) Token: 0x060010F5 RID: 4341 RVA: 0x0000CF0C File Offset: 0x0000B10C
		public Interactor interactor { get; private set; }

		// Token: 0x060010F6 RID: 4342 RVA: 0x0000CF15 File Offset: 0x0000B115
		private void Awake()
		{
			this.networkIdentity = base.GetComponent<NetworkIdentity>();
			this.interactor = base.GetComponent<Interactor>();
			this.inputBank = base.GetComponent<InputBankTest>();
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x000644FC File Offset: 0x000626FC
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

		// Token: 0x060010F8 RID: 4344 RVA: 0x000645BC File Offset: 0x000627BC
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

		// Token: 0x060010F9 RID: 4345 RVA: 0x0000CF3B File Offset: 0x0000B13B
		static InteractionDriver()
		{
			OutlineHighlight.onPreRenderOutlineHighlight = (Action<OutlineHighlight>)Delegate.Combine(OutlineHighlight.onPreRenderOutlineHighlight, new Action<OutlineHighlight>(InteractionDriver.OnPreRenderOutlineHighlight));
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0006463C File Offset: 0x0006283C
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

		// Token: 0x060010FB RID: 4347 RVA: 0x0000A05D File Offset: 0x0000825D
		public void OnDeathStart()
		{
			base.enabled = false;
		}

		// Token: 0x04001527 RID: 5415
		public bool highlightInteractor;

		// Token: 0x04001528 RID: 5416
		private bool inputReceived;

		// Token: 0x04001529 RID: 5417
		private NetworkIdentity networkIdentity;

		// Token: 0x0400152B RID: 5419
		private InputBankTest inputBank;

		// Token: 0x0400152C RID: 5420
		[NonSerialized]
		public GameObject interactableOverride;

		// Token: 0x0400152D RID: 5421
		private const float interactableCooldownDuration = 0.25f;

		// Token: 0x0400152E RID: 5422
		private float interactableCooldown;
	}
}
