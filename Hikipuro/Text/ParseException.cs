using System;

namespace Hikipuro.Text {
	/// <summary>
	/// Tokenizer のパースエラー.
	/// Tokenizer.Tokenize() で使用する.
	/// </summary>
	public class ParseException : ApplicationException {
		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="message">エラーメッセージ.</param>
		public ParseException(string message) : base(message) {
		}
	}
}
