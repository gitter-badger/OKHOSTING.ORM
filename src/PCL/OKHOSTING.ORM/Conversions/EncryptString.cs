namespace OKHOSTING.ORM.Conversions
{
	public class EncryptString: ConverterBase<string, string>
	{
		public readonly string Password;

		public EncryptString(string password)
		{
			Password = password;
		}

		public override string MemberToColumn(string memberValue)
		{
			return OKHOSTING.Cryptography.SimpleEncryption.Encrypt(memberValue, Password);
		}

		public override string ColumnToMember(string columnValue)
		{
			return OKHOSTING.Cryptography.SimpleEncryption.Decrypt(columnValue, Password);
		}

		public override object MemberToColumn(object memberValue)
		{
			return MemberToColumn((string)memberValue);
		}

		public override object ColumnToMember(object columnValue)
		{
			return ColumnToMember((string)columnValue);
		}
	}
}