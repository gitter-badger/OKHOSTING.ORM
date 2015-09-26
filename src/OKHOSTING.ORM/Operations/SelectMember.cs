using OKHOSTING.Sql.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OKHOSTING.ORM.Operations
{
	/// <summary>
	/// Defines a field on a select query that can contain an alias
	/// </summary>
	public class SelectMember
	{
		/// <summary>
		/// Alias name of the resulting field
		/// </summary>
		public string Alias { get; set; }

		/// <summary>
		/// DataMember for build the field definition
		/// </summary>
		public DataMember DataMember { get; set; }

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		public SelectMember(DataMember member): this(member, null)
		{ 
		}

		/// <summary>
		/// Constructs the AggegateSelectField
		/// </summary>
		/// <param name="dataValue">
		/// DataMember for build the field definition
		/// </param>
		/// <param name="alias">
		/// Alias name of the resulting field
		/// </param>
		public SelectMember(DataMember member, string alias)
		{
			if (member == null)
			{
				throw new ArgumentNullException("member");
			}

			DataMember = member;
			Alias = alias;
		}

		public override string ToString()
		{
			return string.IsNullOrWhiteSpace(Alias)? DataMember.ToString() : Alias;
		}

		public static implicit operator SelectMember(DataMember member)
		{
			return new SelectMember(member);
		}
	}
}