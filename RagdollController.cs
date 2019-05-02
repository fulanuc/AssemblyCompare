using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x020003A7 RID: 935
	public class RagdollController : MonoBehaviour
	{
		// Token: 0x060013E3 RID: 5091 RVA: 0x0006E480 File Offset: 0x0006C680
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			foreach (Transform transform in this.bones)
			{
				Collider component = transform.GetComponent<Collider>();
				if (!component)
				{
					Debug.LogFormat("Bone {0} is missing a collider!", new object[]
					{
						transform
					});
				}
				else
				{
					component.enabled = false;
				}
			}
		}

		// Token: 0x060013E4 RID: 5092 RVA: 0x0006E4E0 File Offset: 0x0006C6E0
		public void BeginRagdoll(Vector3 force)
		{
			if (this.animator)
			{
				Debug.Log("animator disabled");
				this.animator.enabled = false;
			}
			foreach (Transform transform in this.bones)
			{
				transform.parent = base.transform;
				Rigidbody component = transform.GetComponent<Rigidbody>();
				transform.GetComponent<Collider>().enabled = true;
				component.isKinematic = false;
				component.interpolation = RigidbodyInterpolation.Interpolate;
				component.collisionDetectionMode = CollisionDetectionMode.Continuous;
				component.AddForce(force * UnityEngine.Random.Range(0.9f, 1.2f), ForceMode.VelocityChange);
			}
			MonoBehaviour[] array2 = this.componentsToDisableOnRagdoll;
			for (int i = 0; i < array2.Length; i++)
			{
				array2[i].enabled = false;
			}
		}

		// Token: 0x04001770 RID: 6000
		public Transform[] bones;

		// Token: 0x04001771 RID: 6001
		public MonoBehaviour[] componentsToDisableOnRagdoll;

		// Token: 0x04001772 RID: 6002
		private Animator animator;
	}
}
