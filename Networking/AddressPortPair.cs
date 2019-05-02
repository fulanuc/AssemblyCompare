using System;
using System.Globalization;

namespace RoR2.Networking
{
	// Token: 0x02000584 RID: 1412
	public struct AddressPortPair : IEquatable<AddressPortPair>
	{
		// Token: 0x06001F9D RID: 8093 RVA: 0x00017224 File Offset: 0x00015424
		public AddressPortPair(string address, ushort port)
		{
			this.address = address;
			this.port = port;
		}

		// Token: 0x06001F9E RID: 8094 RVA: 0x00099CB8 File Offset: 0x00097EB8
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

		// Token: 0x170002C5 RID: 709
		// (get) Token: 0x06001F9F RID: 8095 RVA: 0x00017234 File Offset: 0x00015434
		public bool isValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.address);
			}
		}

		// Token: 0x06001FA0 RID: 8096 RVA: 0x00017244 File Offset: 0x00015444
		public override string ToString()
		{
			return string.Format(CultureInfo.InvariantCulture, "{0}:{1}", this.address, this.port);
		}

		// Token: 0x06001FA1 RID: 8097 RVA: 0x00017266 File Offset: 0x00015466
		public bool Equals(AddressPortPair other)
		{
			return string.Equals(this.address, other.address) && this.port == other.port;
		}

		// Token: 0x06001FA2 RID: 8098 RVA: 0x00099D3C File Offset: 0x00097F3C
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is AddressPortPair)
			{
				AddressPortPair other = (AddressPortPair)obj;
				return this.Equals(other);
			}
			return false;
		}

		// Token: 0x06001FA3 RID: 8099 RVA: 0x0001728B File Offset: 0x0001548B
		public override int GetHashCode()
		{
			return ((this.address != null) ? this.address.GetHashCode() : 0) * 397 ^ this.port.GetHashCode();
		}

		// Token: 0x0400220B RID: 8715
		public string address;

		// Token: 0x0400220C RID: 8716
		public ushort port;
	}
}
