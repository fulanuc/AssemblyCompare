using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x0200025A RID: 602
	public class AnimationEvents : MonoBehaviour
	{
		// Token: 0x06000B34 RID: 2868 RVA: 0x0004B558 File Offset: 0x00049758
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

		// Token: 0x06000B35 RID: 2869 RVA: 0x0004B5D8 File Offset: 0x000497D8
		public void UpdateIKState(AnimationEvent animationEvent)
		{
			IIKTargetBehavior component = this.childLocator.FindChild(animationEvent.stringParameter).GetComponent<IIKTargetBehavior>();
			if (component != null)
			{
				component.UpdateIKState(animationEvent.intParameter);
			}
		}

		// Token: 0x06000B36 RID: 2870 RVA: 0x00009008 File Offset: 0x00007208
		public void PlaySound(string soundString)
		{
			Util.PlaySound(soundString, this.soundCenter ? this.soundCenter : this.body);
		}

		// Token: 0x06000B37 RID: 2871 RVA: 0x0000902C File Offset: 0x0000722C
		public void NormalizeToFloor()
		{
			if (this.modelLocator)
			{
				this.modelLocator.normalizeToFloor = true;
			}
		}

		// Token: 0x06000B38 RID: 2872 RVA: 0x0004B60C File Offset: 0x0004980C
		public void CreateEffect(AnimationEvent animationEvent)
		{
			Transform transform = this.childLocator.FindChild(animationEvent.stringParameter);
			EffectData effectData = new EffectData();
			effectData.origin = transform.position;
			effectData.SetChildLocatorTransformReference(base.gameObject, this.childLocator.FindChildIndex(animationEvent.stringParameter));
			EffectManager.instance.SpawnEffect((GameObject)animationEvent.objectReferenceParameter, effectData, animationEvent.intParameter != 0);
		}

		// Token: 0x06000B39 RID: 2873 RVA: 0x0004B67C File Offset: 0x0004987C
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

		// Token: 0x06000B3A RID: 2874 RVA: 0x0004B73C File Offset: 0x0004993C
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

		// Token: 0x06000B3B RID: 2875 RVA: 0x0004B788 File Offset: 0x00049988
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

		// Token: 0x06000B3C RID: 2876 RVA: 0x00009047 File Offset: 0x00007247
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

		// Token: 0x06000B3D RID: 2877 RVA: 0x0004B7EC File Offset: 0x000499EC
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

		// Token: 0x06000B3E RID: 2878 RVA: 0x0004B83C File Offset: 0x00049A3C
		public void SwapMaterial(AnimationEvent animationEvent)
		{
			Material material = (Material)animationEvent.objectReferenceParameter;
			if (this.meshRenderer)
			{
				this.meshRenderer.material = material;
			}
		}

		// Token: 0x04000F44 RID: 3908
		public GameObject soundCenter;

		// Token: 0x04000F45 RID: 3909
		private GameObject body;

		// Token: 0x04000F46 RID: 3910
		private CharacterModel characterModel;

		// Token: 0x04000F47 RID: 3911
		private ChildLocator childLocator;

		// Token: 0x04000F48 RID: 3912
		private EntityLocator entityLocator;

		// Token: 0x04000F49 RID: 3913
		private Renderer meshRenderer;

		// Token: 0x04000F4A RID: 3914
		private ModelLocator modelLocator;

		// Token: 0x04000F4B RID: 3915
		private float printHeight;

		// Token: 0x04000F4C RID: 3916
		private float printTime;
	}
}
