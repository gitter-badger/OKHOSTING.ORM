using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using CoreConverter = OKHOSTING.Data.Convert;

namespace OKHOSTING.ORM
{
	/// <summary>
	/// Defines methods for serialization and deserialization of datatypes, datamembers and instances
	/// </summary>
	public static class Serializer
	{
		#region From object to string
		
		/// <summary>
		/// Converts a object to it's string representantion
		/// </summary>
		/// <param name="instance">
		/// object to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of object
		/// </returns>
		public static string ToString(object value)
		{
			if (value == null)
				return null;

			else if (value is DataType)
			{
				return ToString((DataType) value);
			}
			else if (value is DataMember)
			{
				return ToString((DataMember) value);
			}
			else if (DataType.IsMapped(value.GetType()))
			{
				DataType dtype = value.GetType();
				return ToString(value, dtype.AllDataMembers);
			}
			else
			{
				return CoreConverter.ToString(value);
			}
		}

		/// <summary>
		/// Converts an instance to it's string representantion
		/// </summary>
		/// <param name="instance">
		/// Instance to be converted to string
		/// </param>
		/// <param name="dmembers">
		/// Members that will be serialized
		/// </param>
		/// <returns>
		/// A string representation of instance
		/// </returns>
		public static string ToString(object instance, IEnumerable<DataMember> dmembers)
		{
			if (instance == null)
				return null;

			if (!DataType.IsMapped(instance.GetType()))
			{
				throw new ArgumentOutOfRangeException("instance", "Instance's Type is not mapped as a DataType");
			}

			DataType dtype = instance.GetType();
			string result = "DataType=" + ToString(dtype) + '&';

			foreach (DataMember dmember in dmembers)
			{
				var memberValue = dmember.Member.GetValue(instance);
				result += dmember.Member.Expression + '=' + ToString(memberValue);
			}

			return result.TrimEnd('&');
		}

		/// <summary>
		/// Converts a DataType to it's string representantion
		/// </summary>
		/// <param name="value">
		/// DataType to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of DataType
		/// </returns>
		public static string ToString(DataType value)
		{
			//null values
			if (value == null) return null;

			return value.InnerType.FullName;
		}

		/// <summary>
		/// Converts a DataMember to it's string representantion
		/// </summary>
		/// <param name="value">
		/// DataMember to be converted to string
		/// </param>
		/// <returns>
		/// A string representation of DataMember
		/// </returns>
		public static string ToString(DataMember value)
		{
			//null values
			if (value == null) return null;

			return "DataType=" + ToString(value.DataType) + "&DataMember=" + value.Member.Expression;
		}

		#endregion

		#region From string to object

		/// <summary>
		/// Converts a DataType string representantion into an actual DataType instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to DataType
		/// </param>
		/// <returns>
		/// A DataType object deserialized from the string
		/// </returns>
		public static DataType ToDataType(string value)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;

			//try to unparse as xml
			return Type.GetType(value, true);
		}

		/// <summary>
		/// Converts a DataMember string representantion into an actual DataMember instance
		/// </summary>
		/// <param name="value">
		/// Value to be converted to DataMember
		/// </param>
		/// <returns>
		/// A DataMember object deserialized from the string
		/// </returns>
		public static DataMember ToDataMember(string value)
		{
			//validate arguments
			if (string.IsNullOrWhiteSpace(value)) return null;

			DataType dtype = Type.GetType(CoreConverter.GetValueFromQueryString(value, "DataType"));
			DataMember dmember = dtype[CoreConverter.GetValueFromQueryString(value, "DataMember")];

			return dmember;
		}

		/// <summary>
		/// Creates and populates a persistent object based on a query string values
		/// </summary>
		/// <param name="value">
		/// Query string containing a serialized object
		/// </param>
		/// <returns>
		/// A populated object
		/// </returns>
		public static object ToInstance(string value)
		{
			return ToInstance(value, null);
		}

		/// <summary>
		/// Creates and populates a persistent object based on a query string values
		/// </summary>
		/// <param name="value">
		/// Query string containing a serialized object
		/// </param>
		/// <returns>
		/// A populated object
		/// </returns>
		public static object ToInstance(string value, IEnumerable<DataMember> dmembers)
		{
			DataType dtype;
			object dobj;

			//Validating if the sting is null
			if (string.IsNullOrWhiteSpace(value)) return null;

			//deserialize datatype
			dtype = ToDataType(CoreConverter.GetValueFromQueryString(value, "DataType"));

			//create new object
			dobj = Activator.CreateInstance(dtype.InnerType);

			//use all datamembers if no list is passed
			if (dmembers == null)
			{
				dmembers = dtype.AllDataMembers;
			}

			//deserialize values
			foreach (DataMember dmember in dmembers)
			{
				string dmemberValue = CoreConverter.GetValueFromQueryString(value, dmember.Member.Expression);

				//Validating if the value was specified on dicResult collection
				if (!string.IsNullOrWhiteSpace(dmemberValue))
				{
					dmember.Member.SetValue(dobj, CoreConverter.ChangeType(dmemberValue, dmember.Member.ReturnType));
				}
			}

			return dobj;
		}

		#endregion
	}
}