using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DUSCPackage
{
    internal class Hash128
    {
        public readonly ulong High;

        public readonly ulong Low;

        private string hashString_;

		public Hash128(string hash)
		{
			if (hash.Length != 32)
			{
				throw new ArgumentException("Invalid hash length");
			}
			High = Convert.ToUInt64(hash.Substring(0, 16), 16);
			Low = Convert.ToUInt64(hash.Substring(16, 16), 16);
			hashString_ = hash;
		}

		public Hash128(byte[] hash, int pos = 0, int len = 16)
		{
			if (hash.Length < pos + len)
			{
				throw new ArgumentException("Invalid hash array size");
			}
			High = 0uL;
			Low = 0uL;
			hashString_ = null;
			switch (len)
			{
				case 16:
					{
						for (int k = 0; k < 8; k++)
						{
							High |= (ulong)hash[pos + k] << (7 - k) * 8;
						}
						for (int l = 0; l < 8; l++)
						{
							Low |= (ulong)hash[pos + l + 8] << (7 - l) * 8;
						}
						break;
					}
				case 32:
					{
						for (int i = 0; i < 16; i++)
						{
							High |= (ulong)((long)hexToInt(hash[pos + i]) << (15 - i) * 4);
						}
						for (int j = 0; j < 16; j++)
						{
							Low |= (ulong)((long)hexToInt(hash[pos + j + 16]) << (15 - j) * 4);
						}
						break;
					}
				default:
					throw new ArgumentException("Invalid hash length");
			}
		}

		private int hexToInt(byte c)
		{
			if (c >= 48 && c <= 57)
			{
				return c - 48;
			}
			if (c >= 97 && c <= 102)
			{
				return c - 97 + 10;
			}
			if (c >= 65 && c <= 70)
			{
				return c - 65 + 10;
			}
			throw new ArgumentException($"Invalid character '{(char)c}'");
		}

		public override bool Equals(object obj)
		{
			if (obj is Hash128)
			{
				return Equals((Hash128)obj);
			}
			return false;
		}

		public bool Equals(Hash128 other)
		{
			if (other.High == High)
			{
				return other.Low == Low;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)(High ^ Low);
		}

		public static bool operator ==(Hash128 a, Hash128 b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(Hash128 a, Hash128 b)
		{
			return !a.Equals(b);
		}

		public void MakeStringHash()
		{
			if (hashString_ == null)
			{
				hashString_ = string.Format("{0,0:x16}{1,0:x16}", High, Low);
			}
		}

		public override string ToString()
		{
			MakeStringHash();
			return hashString_;
		}
	}
}
