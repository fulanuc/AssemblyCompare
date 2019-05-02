using System;

namespace RoR2.Networking
{
	// Token: 0x02000583 RID: 1411
	public struct QosChannelIndex
	{
		// Token: 0x0400220E RID: 8718
		public int intVal;

		// Token: 0x0400220F RID: 8719
		public static QosChannelIndex defaultReliable = new QosChannelIndex
		{
			intVal = 0
		};

		// Token: 0x04002210 RID: 8720
		public static QosChannelIndex defaultUnreliable = new QosChannelIndex
		{
			intVal = 1
		};

		// Token: 0x04002211 RID: 8721
		public static QosChannelIndex characterTransformUnreliable = new QosChannelIndex
		{
			intVal = 2
		};

		// Token: 0x04002212 RID: 8722
		public static QosChannelIndex time = new QosChannelIndex
		{
			intVal = 3
		};

		// Token: 0x04002213 RID: 8723
		public static QosChannelIndex chat = new QosChannelIndex
		{
			intVal = 4
		};

		// Token: 0x04002214 RID: 8724
		public const int viewAnglesChannel = 5;

		// Token: 0x04002215 RID: 8725
		public static QosChannelIndex viewAngles = new QosChannelIndex
		{
			intVal = 5
		};

		// Token: 0x04002216 RID: 8726
		public static QosChannelIndex ping = new QosChannelIndex
		{
			intVal = 6
		};

		// Token: 0x04002217 RID: 8727
		public static QosChannelIndex effects = new QosChannelIndex
		{
			intVal = 7
		};
	}
}
