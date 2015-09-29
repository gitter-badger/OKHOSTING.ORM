using System;
using OKHOSTING.Core.Data.Validation;

namespace OKHOSTING.Authentication.Localization
{
	/// <summary>
	/// A multilingual item that represnets a word or phrase in a particular language (culture)
	/// </summary>
	public class Translation : IComparable
	{
		/// <summary>
		/// Unique id
		/// </summary>
		[StringLengthValidator(255)]
		public string Id
		{
			get; set;
		}

		/// <summary>
		/// Culture which the current item is localized to
		/// </summary>
		public Culture Culture
		{
			get; set;
		}

		/// <summary>
		/// The actual localized word or phrase
		/// </summary>
		public string Value
		{
			get; set;
		}

		/// <summary>
		/// Optional notes on this item
		/// </summary>
		[StringLengthValidator(255)]
		public string Description
		{
			get; set;
		}

		/// <summary>
		/// Returns the key of this item
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Id;
		}

		/// <summary>
		/// Compares to another LocalizedDictionaryItem using the Key and the Culture as primary key
		/// </summary>
		/// <param name="obj">LocalizedDictionaryItem to be compared with</param>
		/// <returns>IComparable result of the comparission</returns>
		public int CompareTo(object obj)
		{
			return CompareTo((Translation)obj);
		}

		/// <summary>
		/// Compares to another LocalizedDictionaryItem using the Key and the Culture as primary key
		/// </summary>
		/// <param name="obj">LocalizedDictionaryItem to be compared with</param>
		/// <returns>IComparable result of the comparission</returns>
		public int CompareTo(Translation obj)
		{
			string comparable_1;
			string comparable_2;

			comparable_1 = Id + "_" + Culture.Id;
			comparable_2 = obj.Id + "_" + obj.Culture.Id;

			return comparable_1.CompareTo(comparable_2);
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public Translation()
		{
		}

		/// <summary>
		/// Creates a new instance
		/// </summary>
		/// <param name="key">Unique id</param>
		/// <param name="culture">Culture which the current item is localized to</param>
		public Translation(string key, Culture culture)
		{
			this.Id = key;
			this.Culture = culture;
		}

		/// <summary>
		/// Returns the initial collection of DictionaryWord that should be created on system setup
		/// </summary>
		public static System.Collections.Generic.IEnumerable<Translation> CreateDefaultValues(Culture culture)
		{
			Translation word;

			//Look in every datatype and add datatype name and all of it's members in the default culture
			foreach (DataType dtype in DataType.AllDataTypes)
			{
				//Add word for DataType
				word = new Translation();
				word.Id = dtype.InnerType.FullName;
				word.Value = dtype.InnerType.Name;
				word.Description = "Added automatically on system setup";
				word.Culture = culture;

				yield return word;

				//Add word for every datatype member
				foreach (DataMember member in dtype.DataMembers)
				{
					word = new Translation();
					word.Id = member.DataType.InnerType.FullName + "." + member.Member.Expression;
					word.Value = member.Member.Expression;
					word.Description = "Added automatically on system setup";
					word.Culture = culture;

					yield return word;
				}
			}
		}
	}
}