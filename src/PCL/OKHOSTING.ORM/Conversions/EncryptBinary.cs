namespace OKHOSTING.ORM.Conversions
{
	public class EncryptBinary : ConverterBase<byte[], byte[]>
	{
		public readonly string Password;

		public EncryptBinary(string password)
		{
			Password = password;
		}

		public override byte[] MemberToColumn(byte[] memberValue)
		{
			return OKHOSTING.Cryptography.SimpleEncryption.Encrypt(memberValue, Password);
		}

		public override byte[] ColumnToMember(byte[] columnValue)
		{
			return OKHOSTING.Cryptography.SimpleEncryption.Decrypt(columnValue, Password);
		}

		public override object MemberToColumn(object memberValue)
		{
			return MemberToColumn((byte[]) memberValue);
		}

		public override object ColumnToMember(object columnValue)
		{
			return ColumnToMember((byte[]) columnValue);
		}
	}
}