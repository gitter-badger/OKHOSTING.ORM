using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OKHOSTING.ORM.Converters
{
	/// <summary>
	/// Represents a conversion applied when saving a persisten object in a database.
	/// </summary>
	/// <typeparam name="TMember">Return type of the DataMember</typeparam>
	/// <typeparam name="TColumn">Type of the DataBases column</typeparam>
	public interface IConverter
	{
		/// <summary>
		/// Converts a value coming from a DataMember into a value to be stored in a DataBase column
		/// </summary>
		object MemberToColumn(object memberValue);

		/// <summary>
		/// Converts a value coming from a DataBase column into a value to be stored in a DataMember
		/// </summary>
		object ColumnToMember(object columnValue);
	}
}