using System;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Tokenizer のパースエラー.
	/// Tokenizer.Tokenize() で使用する.
	/// </summary>
	public class ParseException : ApplicationException {
		/// <summary>
		/// 処理対象の文字列.
		/// </summary>
		public string Text;

		/// <summary>
		/// エラーの発生した行の文字列.
		/// </summary>
		public string LineText;

		/// <summary>
		/// 処理中の文字位置.
		/// </summary>
		public int Index;

		/// <summary>
		/// 処理中の行番号.
		/// </summary>
		public int LineNumber;

		/// <summary>
		/// 処理中の行の文字位置.
		/// </summary>
		public int LineIndex;

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="message">エラーメッセージ.</param>
		public ParseException(string message) : base(message) {
		}
	}
}
