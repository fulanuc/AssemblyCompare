using System;
using System.Globalization;

namespace RoR2.Networking
{
	// Token: 0x02000575 RID: 1397
	public struct AddressPortPair
	{
		// Token: 0x06001F33 RID: 7987 RVA: 0x00098F9C File Offset: 0x0009719C
		public static bool TryParse(string str, out AddressPortPair addressPortPair)
		{
			if (!string.IsNullOrEmpty(str))
			{
				int num = str.Length - 1;
				while (num >= 0 && str[num] != ':')
				{
					num--;
				}
				if (num >= 0)
				{
					string text = str.Substring(0, num);
					string s = str.Substring(num + 1, str.Length - num - 1);
					addressPortPair.address = text;
					ushort num2;
					addressPortPair.port = (TextSerialization.TryParseInvariant(s, out num2) ? num2 : 0);
					return true;
				}
			}
			addressPortPair.address = "";
			addressPortPair.port = 0;
			return false;
		}

		// Token: 0x170002B8 RID: 696
		// (get) Token: 0x06001F34 RID: 7988 RVA: 0x00016D45 File Offset: 0x00014F45
		public bool isValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.address);
			}
		}

		// Token: 0x06001F35 RID: 7989 RVA: 0x00016D55 File Offset: 0x00014F55
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.address, this.port);
		}

		// Token: 0x040021CD RID: 8653
		public string address;

		// Token: 0x040021CE RID: 8654
		public ushort port;
	}
}
