using System;
using UnityEngine;

namespace RoR2
{
	// Token: 0x02000334 RID: 820
	public class IKSimpleChain : MonoBehaviour
	{
		// Token: 0x060010E8 RID: 4328 RVA: 0x0000CDEE File Offset: 0x0000AFEE
		private void Start()
		{
			this.ikTarget = base.GetComponent<IIKTargetBehavior>();
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x00063A98 File Offset: 0x00061C98
		private void LateUpdate()
		{
			if (this.firstRun)
			{
				this.tmpBone = this.boneList[this.startBone];
			}
			if (this.ikTarget != null)
			{
				this.ikTarget.UpdateIKTargetPosition();
			}
			this.targetPosition = base.transform.position;
			this.legLength = this.CalculateLegLength(this.boneList);
			this.Solve(this.boneList, this.targetPosition);
			this.firstRun = false;
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00063B10 File Offset: 0x00061D10
		public bool LegTooShort(float legScale = 1f)
		{
			bool result = false;
			if ((this.targetPosition - this.boneList[0].transform.position).sqrMagnitude >= this.legLength * this.legLength * legScale * legScale)
			{
				result = true;
			}
			return result;
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x00063B5C File Offset: 0x00061D5C
		private float CalculateLegLength(Transform[] bones)
		{
			float[] array = new float[bones.Length - 1];
			float num = 0f;
			for (int i = this.startBone; i < bones.Length - 1; i++)
			{
				array[i] = (bones[i + 1].position - bones[i].position).magnitude;
				num += array[i];
			}
			return num;
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x00063BB8 File Offset: 0x00061DB8
		public void Solve(Transform[] bones, Vector3 target)
		{
			Transform transform = bones[bones.Length - 1];
			Vector3[] array = new Vector3[bones.Length - 2];
			float[] array2 = new float[bones.Length - 2];
			Quaternion[] array3 = new Quaternion[bones.Length - 2];
			for (int i = this.startBone; i < bones.Length - 2; i++)
			{
				array[i] = Vector3.Cross(bones[i + 1].position - bones[i].position, bones[i + 2].position - bones[i + 1].position);
				array[i] = Quaternion.Inverse(bones[i].rotation) * array[i];
				array[i] = array[i].normalized;
				array2[i] = Vector3.Angle(bones[i + 1].position - bones[i].position, bones[i + 1].position - bones[i + 2].position);
				array3[i] = bones[i + 1].localRotation;
			}
			this.positionAccuracy = this.legLength * this.posAccuracy;
			float magnitude = (transform.position - bones[this.startBone].position).magnitude;
			float magnitude2 = (target - bones[this.startBone].position).magnitude;
			this.minIsFound = false;
			this.bendMore = false;
			if (magnitude2 >= magnitude)
			{
				this.minIsFound = true;
				this.bendingHigh = 1f;
				this.bendingLow = 0f;
			}
			else
			{
				this.bendMore = true;
				this.bendingHigh = 1f;
				this.bendingLow = 0f;
			}
			int num = array3.Length;
			int num2 = 0;
			while (Mathf.Abs(magnitude - magnitude2) > this.positionAccuracy && num2 < this.maxIterations)
			{
				num2++;
				float num3;
				if (!this.minIsFound)
				{
					num3 = this.bendingHigh;
				}
				else
				{
					num3 = (this.bendingLow + this.bendingHigh) / 2f;
				}
				for (int j = this.startBone; j < bones.Length - 2; j++)
				{
					float num4;
					if (!this.bendMore)
					{
						num4 = Mathf.Lerp(180f, array2[j], num3);
					}
					else
					{
						num4 = array2[j] * (1f - num3) + (array2[j] - 30f) * num3;
					}
					Quaternion localRotation = Quaternion.AngleAxis(array2[j] - num4, array[j]) * array3[j];
					bones[j + 1].localRotation = localRotation;
				}
				magnitude = (transform.position - bones[this.startBone].position).magnitude;
				if (magnitude2 > magnitude)
				{
					this.minIsFound = true;
				}
				if (this.minIsFound)
				{
					if (magnitude2 > magnitude)
					{
						this.bendingHigh = num3;
					}
					else
					{
						this.bendingLow = num3;
					}
					if (this.bendingHigh < 0.01f)
					{
						break;
					}
				}
				else
				{
					this.bendingLow = this.bendingHigh;
					this.bendingHigh += 1f;
				}
			}
			if (this.firstRun)
			{
				this.tmpBone.rotation = bones[this.startBone].rotation;
			}
			bones[this.startBone].rotation = Quaternion.AngleAxis(Vector3.Angle(transform.position - bones[this.startBone].position, target - bones[this.startBone].position), Vector3.Cross(transform.position - bones[this.startBone].position, target - bones[this.startBone].position)) * bones[this.startBone].rotation;
			if (this.ikPole)
			{
				Vector3 position = bones[this.startBone].position;
				Vector3 up = bones[this.startBone].transform.up;
				Vector3 position2 = transform.position;
				Vector3 position3 = this.ikPole.position;
				Vector3 vector = Vector3.Cross(position2 - position, position3 - position);
				Vector3 vector2 = Vector3.Cross(vector, up);
				Vector3 vecU = Vector3.zero;
				switch (this.innerAxis)
				{
				case IKSimpleChain.InnerAxis.Left:
					vecU = -bones[this.startBone].transform.right;
					break;
				case IKSimpleChain.InnerAxis.Right:
					vecU = bones[this.startBone].transform.right;
					break;
				case IKSimpleChain.InnerAxis.Forward:
					vecU = bones[this.startBone].transform.forward;
					break;
				case IKSimpleChain.InnerAxis.Backward:
					vecU = -bones[this.startBone].transform.forward;
					break;
				}
				float num5 = this.SignedAngle(vecU, vector2, up);
				num5 += this.poleAngle;
				bones[this.startBone].rotation = Quaternion.AngleAxis(num5, transform.position - bones[this.startBone].position) * bones[this.startBone].rotation;
				Debug.DrawLine(transform.position, bones[this.startBone].position, Color.red);
				Debug.DrawRay(bones[this.startBone].position, vector, Color.blue);
				Debug.DrawRay(bones[this.startBone].position, vector2, Color.yellow);
			}
			this.tmpBone = bones[this.startBone];
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x0006411C File Offset: 0x0006231C
		private float SignedAngle(Vector3 vecU, Vector3 vecV, Vector3 normal)
		{
			float num = Vector3.Angle(vecU, vecV);
			if (Vector3.Angle(Vector3.Cross(vecU, vecV), normal) < 1f)
			{
				num *= -1f;
			}
			return -num;
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x00064150 File Offset: 0x00062350
		private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
		{
			float num = Vector3.Dot(Vector3.Cross(fwd, targetDir), up);
			if (num > 0f)
			{
				return 1f;
			}
			if (num < 0f)
			{
				return -1f;
			}
			return 0f;
		}

		// Token: 0x040014E7 RID: 5351
		public float scale = 1f;

		// Token: 0x040014E8 RID: 5352
		public int maxIterations = 100;

		// Token: 0x040014E9 RID: 5353
		public float positionAccuracy = 0.001f;

		// Token: 0x040014EA RID: 5354
		private float posAccuracy = 0.001f;

		// Token: 0x040014EB RID: 5355
		public float bendingLow;

		// Token: 0x040014EC RID: 5356
		public float bendingHigh;

		// Token: 0x040014ED RID: 5357
		public int chainResolution;

		// Token: 0x040014EE RID: 5358
		private int startBone;

		// Token: 0x040014EF RID: 5359
		private bool minIsFound;

		// Token: 0x040014F0 RID: 5360
		private bool bendMore;

		// Token: 0x040014F1 RID: 5361
		private Vector3 targetPosition;

		// Token: 0x040014F2 RID: 5362
		public float legLength;

		// Token: 0x040014F3 RID: 5363
		public float poleAngle;

		// Token: 0x040014F4 RID: 5364
		public IKSimpleChain.InnerAxis innerAxis = IKSimpleChain.InnerAxis.Right;

		// Token: 0x040014F5 RID: 5365
		private Transform tmpBone;

		// Token: 0x040014F6 RID: 5366
		public Transform ikPole;

		// Token: 0x040014F7 RID: 5367
		public Transform[] boneList;

		// Token: 0x040014F8 RID: 5368
		private bool firstRun = true;

		// Token: 0x040014F9 RID: 5369
		private IIKTargetBehavior ikTarget;

		// Token: 0x02000335 RID: 821
		public enum InnerAxis
		{
			// Token: 0x040014FB RID: 5371
			Left,
			// Token: 0x040014FC RID: 5372
			Right,
			// Token: 0x040014FD RID: 5373
			Forward,
			// Token: 0x040014FE RID: 5374
			Backward
		}
	}
}
