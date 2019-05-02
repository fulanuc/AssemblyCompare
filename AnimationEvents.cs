using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200025A RID: 602
	public class AnimationEvents : MonoBehaviour
	{
		// Token: 0x06000B31 RID: 2865 RVA: 0x0004B34C File Offset: 0x0004954C
		private void Start()
		{
			this.childLocator = base.GetComponent<ChildLocator>();
			this.entityLocator = base.GetComponent<EntityLocator>();
			this.meshRenderer = base.GetComponentInChildren<Renderer>();
			this.characterModel = base.GetComponent<CharacterModel>();
			if (this.characterModel)
			{
				this.body = this.characterModel.body.gameObject;
				if (this.body)
				{
					this.modelLocator = this.body.GetComponent<ModelLocator>();
				}
			}
		}

		// Token: 0x06000B32 RID: 2866 RVA: 0x0004B3CC File Offset: 0x000495CC
		public void UpdateIKState(AnimationEvent animationEvent)
		{
			IIKTargetBehavior component = this.childLocator.FindChild(animationEvent.stringParameter).GetComponent<IIKTargetBehavior>();
			if (component != null)
			{
				component.UpdateIKState(animationEvent.intParameter);
			}
		}

		// Token: 0x06000B33 RID: 2867 RVA: 0x00008FE3 File Offset: 0x000071E3
		public void PlaySound(string soundString)
		{
			Util.PlaySound(soundString, this.soundCenter ? this.soundCenter : this.body);
		}

		// Token: 0x06000B34 RID: 2868 RVA: 0x00009007 File Offset: 0x00007207
		public void NormalizeToFloor()
		{
			if (this.modelLocator)
			{
				this.modelLocator.normalizeToFloor = true;
			}
		}

		// Token: 0x06000B35 RID: 2869 RVA: 0x0004B400 File Offset: 0x00049600
		public void CreateEffect(AnimationEvent animationEvent)
		{
			Transform transform = this.childLocator.FindChild(animationEvent.stringParameter);
			EffectData effectData = new EffectData();
			effectData.origin = transform.position;
			effectData.SetChildLocatorTransformReference(base.gameObject, this.childLocator.FindChildIndex(animationEvent.stringParameter));
			EffectManager.instance.SpawnEffect((GameObject)animationEvent.objectReferenceParameter, effectData, animationEvent.intParameter != 0);
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x0004B470 File Offset: 0x00049670
		public void CreatePrefab(AnimationEvent animationEvent)
		{
			GameObject gameObject = (GameObject)animationEvent.objectReferenceParameter;
			string stringParameter = animationEvent.stringParameter;
			int intParameter = animationEvent.intParameter;
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild(stringParameter);
				if (transform)
				{
					if (intParameter == 0)
					{
						UnityEngine.Object.Instantiate<GameObject>(gameObject, transform.position, Quaternion.identity);
						return;
					}
					UnityEngine.Object.Instantiate<GameObject>(gameObject, transform.position, transform.rotation, transform);
					return;
				}
				else if (gameObject)
				{
					UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation);
					return;
				}
			}
			else if (gameObject)
			{
				UnityEngine.Object.Instantiate<GameObject>(gameObject, base.transform.position, base.transform.rotation);
			}
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0004B530 File Offset: 0x00049730
		public void ItemDrop()
		{
			if (NetworkServer.active && this.entityLocator)
			{
				ChestBehavior component = this.entityLocator.entity.GetComponent<ChestBehavior>();
				if (component)
				{
					component.ItemDrop();
					return;
				}
				Debug.Log("Parent has no item drops!");
			}
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0004B57C File Offset: 0x0004977C
		public void BeginPrint(AnimationEvent animationEvent)
		{
			if (this.meshRenderer)
			{
				Material material = (Material)animationEvent.objectReferenceParameter;
				float floatParameter = animationEvent.floatParameter;
				float maxPrintHeight = (float)animationEvent.intParameter;
				this.meshRenderer.material = material;
				this.printTime = 0f;
				MaterialPropertyBlock printPropertyBlock = new MaterialPropertyBlock();
				base.StartCoroutine(this.startPrint(floatParameter, maxPrintHeight, printPropertyBlock));
			}
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x00009022 File Offset: 0x00007222
		private IEnumerator startPrint(float maxPrintTime, float maxPrintHeight, MaterialPropertyBlock printPropertyBlock)
		{
			if (this.meshRenderer)
			{
				while (this.printHeight < maxPrintHeight)
				{
					this.printTime += Time.deltaTime;
					this.printHeight = this.printTime / maxPrintTime * maxPrintHeight;
					this.meshRenderer.GetPropertyBlock(printPropertyBlock);
					printPropertyBlock.Clear();
					printPropertyBlock.SetFloat("_SliceHeight", this.printHeight);
					this.meshRenderer.SetPropertyBlock(printPropertyBlock);
					yield return new WaitForEndOfFrame();
				}
			}
			yield break;
		}

		// Token: 0x06000B3A RID: 2874 RVA: 0x0004B5E0 File Offset: 0x000497E0
		public void SetChildEnable(AnimationEvent animationEvent)
		{
			string stringParameter = animationEvent.stringParameter;
			bool active = animationEvent.intParameter > 0;
			if (this.childLocator)
			{
				Transform transform = this.childLocator.FindChild(stringParameter);
				if (transform)
				{
					transform.gameObject.SetActive(active);
				}
			}
		}

		// Token: 0x06000B3B RID: 2875 RVA: 0x0004B630 File Offset: 0x00049830
		public void SwapMaterial(AnimationEvent animationEvent)
		{
			Material material = (Material)animationEvent.objectReferenceParameter;
			if (this.meshRenderer)
			{
				this.meshRenderer.material = material;
			}
		}

		// Token: 0x04000F3E RID: 3902
		public GameObject soundCenter;

		// Token: 0x04000F3F RID: 3903
		private GameObject body;

		// Token: 0x04000F40 RID: 3904
		private CharacterModel characterModel;

		// Token: 0x04000F41 RID: 3905
		private ChildLocator childLocator;

		// Token: 0x04000F42 RID: 3906
		private EntityLocator entityLocator;

		// Token: 0x04000F43 RID: 3907
		private Renderer meshRenderer;

		// Token: 0x04000F44 RID: 3908
		private ModelLocator modelLocator;

		// Token: 0x04000F45 RID: 3909
		private float printHeight;

		// Token: 0x04000F46 RID: 3910
		private float printTime;
	}
}
