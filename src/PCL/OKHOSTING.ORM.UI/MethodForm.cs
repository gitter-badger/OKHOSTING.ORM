using OKHOSTING.Core;
using OKHOSTING.UI.Controls.Forms;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace OKHOSTING.ORM.UI
{
	/// <summary>
	/// A form used to retrieve all parameters necessary for executing a DataMethod
	/// </summary>
	public class MethodForm: OKHOSTING.UI.Controls.Forms.MethodForm
	{
		public MethodForm(MethodInfo method): base(method)
		{
		}

		/// <summary>
		/// Adds a field for every argument that the DataMethos needs in order to be invoked
		/// </summary>
		/// <param name="method">DataMethod which parameters will be used as fields</param>
		public override void AddFields(MethodInfo method)
		{
			if (method == null) throw new ArgumentNullException("dmethod");
			uint order = 0;

			//add a field for each parameter
			foreach (ParameterInfo param in method.GetParameters())
			{
				FormField field;

				field = ObjectForm.CreateField(param.ParameterType);
				
				//set common values
				field.Container = this;
				field.Name = param.Name;
				field.Required = !param.IsOptional && !param.IsOut;
				field.CaptionControl.Text = new System.Resources.ResourceManager(method.DeclaringType).GetString(method.GetFriendlyFullName().Replace('.', '_') + '_' + param.Name);
				field.SortOrder = order++;

				Fields.Add(field);
			}
		}
	}
}