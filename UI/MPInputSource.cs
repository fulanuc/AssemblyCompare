using System;
using Rewired.UI;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000604 RID: 1540
	public class MPInputSource : IMouseInputSource
	{
		// Token: 0x060022BF RID: 8895 RVA: 0x00013CDA File Offset: 0x00011EDA
		public bool GetButtonDown(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060022C0 RID: 8896 RVA: 0x00013CDA File Offset: 0x00011EDA
		public bool GetButtonUp(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x060022C1 RID: 8897 RVA: 0x00013CDA File Offset: 0x00011EDA
		public bool GetButton(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000303 RID: 771
		// (get) Token: 0x060022C2 RID: 8898 RVA: 0x000195A1 File Offset: 0x000177A1
		public int playerId { get; }

		// Token: 0x17000304 RID: 772
		// (get) Token: 0x060022C3 RID: 8899 RVA: 0x000195A9 File Offset: 0x000177A9
		public bool enabled { get; }

		// Token: 0x17000305 RID: 773
		// (get) Token: 0x060022C4 RID: 8900 RVA: 0x000195B1 File Offset: 0x000177B1
		public bool locked { get; }

		// Token: 0x17000306 RID: 774
		// (get) Token: 0x060022C5 RID: 8901 RVA: 0x000195B9 File Offset: 0x000177B9
		public int buttonCount { get; }

		// Token: 0x17000307 RID: 775
		// (get) Token: 0x060022C6 RID: 8902 RVA: 0x000195C1 File Offset: 0x000177C1
		public Vector2 screenPosition { get; }

		// Token: 0x17000308 RID: 776
		// (get) Token: 0x060022C7 RID: 8903 RVA: 0x000195C9 File Offset: 0x000177C9
		public Vector2 screenPositionDelta { get; }

		// Token: 0x17000309 RID: 777
		// (get) Token: 0x060022C8 RID: 8904 RVA: 0x000195D1 File Offset: 0x000177D1
		public Vector2 wheelDelta { get; }
	}
}
