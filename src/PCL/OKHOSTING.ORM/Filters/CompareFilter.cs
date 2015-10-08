using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Reflection;
using OKHOSTING.Data;

namespace OKHOSTING.ORM.Filters
{
	/// <summary>
	/// Base class for value-to-compare filters
	/// </summary>
	public abstract class CompareFilter : MemberFilter
	{
		/// <summary>
		/// Operator of the comparison
		/// </summary>
		public CompareOperator Operator;
	}
}