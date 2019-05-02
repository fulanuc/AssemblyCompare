using System;
using Rewired.UI;
using UnityEngine;

namespace RoR2.UI
{
	// Token: 0x02000616 RID: 1558
	public class MPInputSource : IMouseInputSource
	{
		// Token: 0x0600234F RID: 9039 RVA: 0x000141EC File Offset: 0x000123EC
		public bool GetButtonDown(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002350 RID: 9040 RVA: 0x000141EC File Offset: 0x000123EC
		public bool GetButtonUp(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06002351 RID: 9041 RVA: 0x000141EC File Offset: 0x000123EC
		public bool GetButton(int button)
		{
			throw new NotImplementedException();
		}

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06002352 RID: 9042 RVA: 0x00019C58 File Offset: 0x00017E58
		public int playerId { get; }

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06002353 RID: 9043 RVA: 0x00019C60 File Offset: 0x00017E60
		public bool enabled { get; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06002354 RID: 9044 RVA: 0x00019C68 File Offset: 0x00017E68
		public bool locked { get; }

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06002355 RID: 9045 RVA: 0x00019C70 File Offset: 0x00017E70
		public int buttonCount { get; }

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06002356 RID: 9046 RVA: 0x00019C78 File Offset: 0x00017E78
		public Vector2 screenPosition { get; }

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06002357 RID: 9047 RVA: 0x00019C80 File Offset: 0x00017E80
		public Vector2 screenPositionDelta { get; }

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06002358 RID: 9048 RVA: 0x00019C88 File Offset: 0x00017E88
		public Vector2 wheelDelta { get; }
	}
}
