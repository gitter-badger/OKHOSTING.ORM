using NUnit.Framework;
using OKHOSTING.ORM.Tests.Model;
using System;
using System.Collections.Generic;

namespace OKHOSTING.ORM.Tests
{
	public static class CodeGeneration
	{
		[Test]
		public static void GenerateWebForms()
		{
			Type[] types = new Type[] { typeof(Person), typeof(Employee), typeof(Customer), typeof(CustomerContact), typeof(Address), typeof(Country) };

			var dtypes = DataType.DefaultMap(types);
			OKHOSTING.ORM.UI.Web.Forms.Templates.CodeGenerator.Generate(@"C:\Pruebas\Test1", dtypes);
		}
	}
}