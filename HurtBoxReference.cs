using System;
using UnityEngine;
using UnityEngine.Networking;

namespace RoR2
{
	// Token: 0x02000328 RID: 808
	[Serializable]
	public struct HurtBoxReference
	{
		// Token: 0x060010B3 RID: 4275 RVA: 0x0006318C File Offset: 0x0006138C
		public static HurtBoxReference FromHurtBox(HurtBox hurtBox)
		{
			HurtBoxReference result;
			if (!hurtBox)
			{
				result = default(HurtBoxReference);
				return result;
			}
			result = new HurtBoxReference
			{
				rootObject = (hurtBox.healthComponent ? hurtBox.healthComponent.gameObject : null),
				hurtBoxIndexPlusOne = (byte)(hurtBox.indexInGroup + 1)
			};
			return result;
		}

		// Token: 0x060010B4 RID: 4276 RVA: 0x000631E8 File Offset: 0x000613E8
		public static HurtBoxReference FromRootObject(GameObject rootObject)
		{
			return new HurtBoxReference
			{
				rootObject = rootObject,
				hurtBoxIndexPlusOne = 0
			};
		}

		// Token: 0x060010B5 RID: 4277 RVA: 0x00063210 File Offset: 0x00061410
		public GameObject ResolveGameObject()
		{
			if (this.hurtBoxIndexPlusOne == 0)
			{
				return this.rootObject;
			}
			GameObject gameObject = this.rootObject;
			HurtBox[] array;
			if (gameObject == null)
			{
				array = null;
			}
			else
			{
				ModelLocator component = gameObject.GetComponent<ModelLocator>();
				if (component == null)
				{
					array = null;
				}
				else
				{
					Transform modelTransform = component.modelTransform;
					if (modelTransform == null)
					{
						array = null;
					}
					else
					{
						HurtBoxGroup component2 = modelTransform.GetComponent<HurtBoxGroup>();
						array = ((component2 != null) ? component2.hurtBoxes : null);
					}
				}
			}
			HurtBox[] array2 = array;
			if (array2 != null)
			{
				int num = (int)(this.hurtBoxIndexPlusOne - 1);
				if (num < array2.Length)
				{
					return array2[num].gameObject;
				}
			}
			return null;
		}

		// Token: 0x060010B6 RID: 4278 RVA: 0x00063280 File Offset: 0x00061480
		public HurtBox ResolveHurtBox()
		{
			GameObject gameObject = this.ResolveGameObject();
			if (!gameObject)
			{
				return null;
			}
			return gameObject.GetComponent<HurtBox>();
		}

		// Token: 0x060010B7 RID: 4279 RVA: 0x0000CCBC File Offset: 0x0000AEBC
		public void Write(NetworkWriter writer)
		{
			writer.Write(this.rootObject);
			writer.Write(this.hurtBoxIndexPlusOne);
		}

		// Token: 0x060010B8 RID: 4280 RVA: 0x0000CCD6 File Offset: 0x0000AED6
		public void Read(NetworkReader reader)
		{
			this.rootObject = reader.ReadGameObject();
			this.hurtBoxIndexPlusOne = reader.ReadByte();
		}

		// Token: 0x040014C3 RID: 5315
		public GameObject rootObject;

		// Token: 0x040014C4 RID: 5316
		public byte hurtBoxIndexPlusOne;
	}
}
