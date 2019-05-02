using System;

namespace RoR2.Networking
{
	// Token: 0x02000596 RID: 1430
	public struct QosChannelIndex
	{
		// Token: 0x04002265 RID: 8805
		public int intVal;

		// Token: 0x04002266 RID: 8806
		public static QosChannelIndex defaultReliable = new QosChannelIndex
		{
			intVal = 0
		};

		// Token: 0x04002267 RID: 8807
		public static QosChannelIndex defaultUnreliable = new QosChannelIndex
		{
			intVal = 1
		};

		// Token: 0x04002268 RID: 8808
		public static QosChannelIndex characterTransformUnreliable = new QosChannelIndex
		{
			intVal = 2
		};

		// Token: 0x04002269 RID: 8809
		public static QosChannelIndex time = new QosChannelIndex
		{
			intVal = 3
		};

		// Token: 0x0400226A RID: 8810
		public static QosChannelIndex chat = new QosChannelIndex
		{
			intVal = 4
		};

		// Token: 0x0400226B RID: 8811
		public const int viewAnglesChannel = 5;

		// Token: 0x0400226C RID: 8812
		public static QosChannelIndex viewAngles = new QosChannelIndex
		{
			intVal = 5
		};

		// Token: 0x0400226D RID: 8813
		public static QosChannelIndex ping = new QosChannelIndex
		{
			intVal = 6
		};

		// Token: 0x0400226E RID: 8814
		public static QosChannelIndex effects = new QosChannelIndex
		{
			intVal = 7
		};
	}
}
