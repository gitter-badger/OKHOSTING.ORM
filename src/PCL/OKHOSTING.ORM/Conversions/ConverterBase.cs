namespace OKHOSTING.ORM.Conversions
{
	/// <summary>
	/// A converter usefull as a base class for your own converters
	/// </summary>
	public abstract class ConverterBase
	{
		public abstract object MemberToColumn(object memberValue);

		public abstract object ColumnToMember(object columnValue);
	}
	
	/// <summary>
	/// A generic converter usefull as a base class for your own converters
	/// </summary>
	/// <typeparam name="TType">Return type of the DataMember</typeparam>
	/// <typeparam name="TColumn">Type of the column</typeparam>
	public abstract class ConverterBase<TType, TColumn> : ConverterBase
	{
		public abstract TColumn MemberToColumn(TType memberValue);
		public abstract TType ColumnToMember(TColumn columnValue);
	}
}