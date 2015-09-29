using System;
using System.Collections.Generic;

namespace OKHOSTING.Authentication.Localization
{
	/// <summary>
	/// A multi-culture word dictionary for localization.
	/// Allows getting localized words with a key and a culture
	/// </summary>
	public class Translator
	{
		/// <summary>
		/// Dictionary of words
		/// </summary>
		public readonly Dictionary<string, string> Words = new Dictionary<string, string>();

		public Culture DefaultCulture { get; set; } = Culture.Default;

		public string GoogleApiKey { get; set; }

		/// <summary>
		/// Creates a new instance
		/// </summary>
		public Translator()
		{
		}

		/// <summary>
		/// Creates a new instance and initializes the word collection with a LocalizedDictionaryItem collection
		/// </summary>
		/// <param name="words">LocalizedDictionaryItem collection containing words and phrases in multiple cultures</param>
		public Translator(IEnumerable<Translation> words)
		{
			if (words == null) throw new ArgumentNullException("words", "Argument can't be null");

			foreach (Translation item in words)
			{
				Words.Add(item.Id + "." + item.Culture.Id, item.Value);
			}
		}

		/// <summary>
		/// Returns a localized word in the specified culture
		/// </summary>
		/// <param name="word">Word or phrase to localize</param>
		/// <param name="culture">Culture in which the word will be localized</param>
		/// <returns>Localized version of the word in the specified culture</returns>
		public string this[string word, Culture culture]
		{
			get
			{
				if (string.IsNullOrEmpty(word)) throw new ArgumentNullException("word");

				//word was found in the dictionary
				if (Words.ContainsKey(word + "." + culture.Id))
				{
					return Words[word + "." + culture.Id];
				}

				//word was not found
				else
				{
					//create a new item and store it in the database
					Translation item = new Translation();
					item.Id = word;
					item.Culture = culture;

					//look in the database to see if the item already exist
					if (!DataBase.Default.Select<Translation>(item))
					{
						//if item was not found in database, create a new item and insert it
						item.Value = word.Substring(word.LastIndexOf(".") + 1);

						if (!string.IsNullOrWhiteSpace(GoogleApiKey))
						{
						}

						item.Description = "Auto-generated at runtime";

						DataBase.Default.Insert<Translation>(item);
					}
					
					//finally add item to collection
					Words.Add(item.Id + "." + item.Culture.Id, item.Value);

					//search for a parent culture
					System.Globalization.CultureInfo cultureInfo = culture.CultureInfo;
					while (cultureInfo.Parent != null)
					{
						if (Words.ContainsKey(word + "." + cultureInfo.LCID))
						{
							return Words[word + "." + cultureInfo.LCID];
						}

						cultureInfo = cultureInfo.Parent;
					}

					//return the content after the last "."
					return word.Substring(word.LastIndexOf(".") + 1);
				}
			}
		}

		#region Support overrides

		/// <summary>
		/// Returns a localized word in the default culture
		/// </summary>
		/// <param name="word">Word or phrase to localize</param>
		/// <returns>Localized version of the word in the default culture</returns>
		public string this[string word]
		{
			get
			{
				return this[word, DefaultCulture];
			}
		}

		/// <summary>
		/// Returns a localized boolean value in the specified culture
		/// </summary>
		/// <param name="value">Boolean value to localize</param>
		/// <param name="culture">Culture in which the value will be localized</param>
		/// <returns>Localized version of the value in the specified culture</returns>
		public string this[bool value, Culture culture]
		{
			get
			{
				return this[value.ToString()];
			}
		}

		/// <summary>
		/// Returns a localized word in the default culture
		/// </summary>
		/// <param name="value">Boolean value to localize</param>
		/// <returns>Localized version of the value in the specified culture</returns>
		public string this[bool value]
		{
			get
			{
				return this[value, DefaultCulture];
			}
		}

		/// <summary>
		/// Returns a localized enum value in the specified culture
		/// </summary>
		/// <param name="value">Enum value to localize</param>
		/// <param name="culture">Culture in which the wird will be localized</param>
		/// <returns>Localized version of the value in the specified culture</returns>
		public string this[Enum value, Culture culture]
		{
			get
			{
				if(value == null) throw new ArgumentNullException("value");
				return this[value.GetType().FullName + "." + value.ToString()];
			}
		}

		/// <summary>
		/// Returns a localized enum value in the specified culture
		/// </summary>
		/// <param name="value">Enum value to localize</param>
		/// <returns>Localized version of the value in the specified culture</returns>
		public string this[Enum value]
		{
			get
			{
				return this[value, DefaultCulture];
			}
		}

		/// <summary>
		/// Returns a localized DataType name in the specified culture
		/// </summary>
		/// <param name="dtype">DataType to localize</param>
		/// <param name="culture">Culture in which the DataType will be localized</param>
		/// <returns>Localized version of the DataType's name in the specified culture</returns>
		public string this[DataType dtype, Culture culture]
		{
			get
			{
				if (dtype == null) throw new ArgumentNullException("dtype");
				return this[dtype.InnerType.FullName];
			}
		}

		/// <summary>
		/// Returns a localized DataType name in the specified culture
		/// </summary>
		/// <param name="dtype">DataType to localize</param>
		/// <returns>Localized version of the DataType's name in the specified culture</returns>
		public string this[DataType dtype]
		{
			get
			{
				return this[dtype, DefaultCulture];
			}
		}

		/// <summary>
		/// Returns a localized DataMember name in the specified culture
		/// </summary>
		/// <param name="dmember">DataMember to localize</param>
		/// <param name="culture">Culture in which the DataMember will be localized</param>
		/// <returns>Localized version of the DataMember's name in the specified culture</returns>
		public string this[DataMember dmember, Culture culture]
		{
			get
			{
				if (dmember == null) throw new ArgumentNullException("dmember");
				return this[dmember.DataType.InnerType.FullName + "." + dmember];
			}
		}

		/// <summary>
		/// Returns a localized DataMember name in the specified culture
		/// </summary>
		/// <param name="dmember">DataMember to localize</param>
		/// <returns>Localized version of the DataMember's name in the specified culture</returns>
		public string this[DataMember dmember]
		{
			get
			{
				return this[dmember, DefaultCulture];
			}
		}

		/// <summary>
		/// Returns a localized object in the specified culture
		/// </summary>
		/// <param name="dmember">Object to localize</param>
		/// <param name="culture">Culture in which the object will be localized</param>
		/// <returns>Localized version of the object in the specified culture</returns>
		public string this[object value, Culture culture]
		{
			get
			{
				//validation
				if (value == null) throw new ArgumentNullException("value");
				
				//casting
				if (value is bool) return this[(bool)value];
				if (value is Enum) return this[(Enum)value];
				
				//finally localize from object's string representation
				return this[value.ToString()];
			}
		}

		/// <summary>
		/// Returns a localized object in the specified culture
		/// </summary>
		/// <param name="dmember">Object to localize</param>
		/// <returns>Localized version of the object in the specified culture</returns>
		public string this[object value]
		{
			get
			{
				return this[value, DefaultCulture];
			}
		}

		#endregion
	}
}