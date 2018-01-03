using System;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Tokenizer parse error.
	/// This object used in Tokenizer.Tokenize() method.
	/// </summary>
	public class ParseException : ApplicationException {
		/// <summary>
		/// Processing target Text.
		/// </summary>
		public string Text;

		/// <summary>
		/// A line text of location of the error.
		/// </summary>
		public string LineText;

		/// <summary>
		/// Processing target char index.
		/// </summary>
		public int Index;

		/// <summary>
		/// Processing target line number.
		/// </summary>
		public int LineNumber;

		/// <summary>
		/// Char index of the line.
		/// </summary>
		public int LineIndex;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="message">Error message.</param>
		public ParseException(string message) : base(message) {
		}
	}
}
