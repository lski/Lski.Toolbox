using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;

namespace Lski.IO {
	
	/// <summary>
	/// Offers simple static functions for exporting and importing an object to xml from and into a file
	/// </summary>
	public class Objects {

		/// <summary>
		/// Exports passed object to the file stated using a DataContractSerialiser to convert it into XML. REQUIRED A serializable attribute type recongnised by DataContractSerializer
		/// E.g. Serializable, XmlSerializable, and DataContract
		/// </summary>
		/// <param name="filename">The file to export to</param>
		/// <remarks></remarks>
		public static void ExportToFile<T>(string filename, T obj) {

			FileInfo file = new FileInfo(filename);

			// 1. If the file exists the directory must also exist, so delete the file ready for the new one
			if (file.Exists)
				file.Delete();
			// 2. Ensure the directory exists, before attempting to write a new settings file
			else
				file.Directory.Create();

			XmlWriterSettings xmlSettings = new XmlWriterSettings();
			xmlSettings.Indent = true;
			xmlSettings.CloseOutput = true;
			XmlWriter writer = XmlWriter.Create(file.FullName, xmlSettings);

			DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
			serializer.WriteObject(writer, obj);

			writer.Close();
		}

		/// <summary>
		/// Imports an object from a file in an XML format that was created using ExportToFile
		/// </summary>
		/// <param name="filename">The file to import from.</param>
		/// <exception cref="IO.FileNotFoundException">If the file to read from, simply does not exist.</exception>
		/// <exception cref="XmlException">If the file is a malformed xml document</exception>
		/// <remarks></remarks>
		public static T ImportFromFile<T>(string filename) {

			XmlReaderSettings xmlSettings = new XmlReaderSettings();
			xmlSettings.IgnoreWhitespace = true;
			xmlSettings.CloseInput = true;
			XmlReader reader = XmlReader.Create(filename, xmlSettings);

			// Get the type of this object so that we can create a blank instance of the object
			DataContractSerializer serializer = new DataContractSerializer(typeof(T));
			T obj = (T)serializer.ReadObject(reader);

			reader.Close();

			return obj;
		}
	}
}
