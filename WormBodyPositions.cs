﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace RoR2
{
	// Token: 0x0200042E RID: 1070
	public class WormBodyPositions : MonoBehaviour
	{
		// Token: 0x060017FF RID: 6143 RVA: 0x0007BCA4 File Offset: 0x00079EA4
		private void Start()
		{
			this.positionHistory.Add(new WormBodyPositions.Keyframe
			{
				rotation = this.segments[0].rotation,
				position = this.segments[0].position,
				fromPreviousNormal = Vector3.zero,
				fromPreviousLength = 0f
			});
		}

		// Token: 0x06001800 RID: 6144 RVA: 0x0007BD08 File Offset: 0x00079F08
		private void FixedUpdate()
		{
			Vector3 position = this.segments[0].position;
			Vector3 a = position - this.positionHistory[this.positionHistory.Count - 1].position;
			float magnitude = a.magnitude;
			if (magnitude != 0f)
			{
				Quaternion rotation = this.segments[0].rotation;
				this.segments[0].up = -a;
				Quaternion rotation2 = this.segments[0].rotation;
				this.segments[0].rotation = Quaternion.RotateTowards(rotation, rotation2, 360f * Time.fixedDeltaTime);
				this.positionHistory.Add(new WormBodyPositions.Keyframe
				{
					rotation = this.segments[0].rotation,
					position = position,
					fromPreviousNormal = a * (1f / magnitude),
					fromPreviousLength = magnitude
				});
			}
			float num = this.segmentRadius * 2f;
			float num2 = num;
			Vector3 a2 = position;
			int num3 = 1;
			for (int i = this.positionHistory.Count - 1; i >= 1; i--)
			{
				Vector3 position2 = this.positionHistory[i - 1].position;
				float fromPreviousLength = this.positionHistory[i].fromPreviousLength;
				if (num2 < fromPreviousLength)
				{
					float t = num2 / fromPreviousLength;
					this.segments[num3].position = Vector3.Lerp(a2, position2, t);
					num3++;
					if (num3 >= this.segments.Length)
					{
						this.positionHistory.RemoveRange(0, i - 1);
						break;
					}
					num2 += num;
				}
				num2 -= fromPreviousLength;
				a2 = position2;
			}
			if (this.segments.Length > 1)
			{
				Quaternion rotation3 = this.segments[0].rotation;
				Vector3 b = this.segments[0].position;
				Vector3 vector = this.segments[1].position;
				for (int j = 1; j < this.segments.Length - 1; j++)
				{
					Vector3 position3 = this.segments[j + 1].position;
					Vector3 vector2 = position3 - b;
					if (vector2 != Vector3.zero)
					{
						this.segments[j].rotation = rotation3;
						this.segments[j].up = vector2;
					}
					b = vector;
					vector = position3;
				}
			}
		}

		// Token: 0x04001B01 RID: 6913
		public Vector3 headVelocity = Vector3.zero;

		// Token: 0x04001B02 RID: 6914
		public Transform[] segments;

		// Token: 0x04001B03 RID: 6915
		public float segmentRadius = 1f;

		// Token: 0x04001B04 RID: 6916
		private List<WormBodyPositions.Keyframe> positionHistory = new List<WormBodyPositions.Keyframe>();

		// Token: 0x0200042F RID: 1071
		private struct Keyframe
		{
			// Token: 0x04001B05 RID: 6917
			public Vector3 position;

			// Token: 0x04001B06 RID: 6918
			public Quaternion rotation;

			// Token: 0x04001B07 RID: 6919
			public Vector3 fromPreviousNormal;

			// Token: 0x04001B08 RID: 6920
			public float fromPreviousLength;
		}
	}
}
