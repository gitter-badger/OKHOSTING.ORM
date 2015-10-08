using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using OKHOSTING.Data;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Compares two Member's on the same object
	/// </summary>
	public class MemberCompareFilter : CompareFilter
	{
		/// <summary>
		/// Member to compare with the Member defined in the 
		/// Field with the same name
		/// </summary>
		public DataMember MemberToCompare;

		/// <summary>
		/// Optional alias used for the table
		/// </summary>
		public string MemberToCompareTypeAlias;
	}
}