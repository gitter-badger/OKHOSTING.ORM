using Microsoft.VisualStudio.TestTools.UnitTesting;
using OKHOSTING.ORM.Tests.Model;
using System;
using System.Collections.Generic;

namespace OKHOSTING.ORM.Tests
{
    [TestClass]
    public class CodeGeneration
	{
        public class MyTestClass
        {
        }

        [TestMethod]
		public void GenerateWebForms()
		{
			Type[] types = new Type[] { typeof(Person), typeof(Employee), typeof(Customer), typeof(CustomerContact), typeof(Address), typeof(Country) };

			var dtypes = DataType.DefaultMap(types);
			OKHOSTING.ORM.UI.Web.Forms.Templates.CodeGenerator.Generate(@"C:\Pruebas\", dtypes);
		}
	}
}