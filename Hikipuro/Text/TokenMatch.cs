using System.Text.RegularExpressions;

namespace Hikipuro.Text {
	/// <summary>
	/// トークンのマッチした場所.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class TokenMatch<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンの種類.
		/// </summary>
		public TokenType type;

		/// <summary>
		/// マッチした文字列.
		/// BeforeAddToken イベントの中で修正できる.
		/// </summary>
		public string text;

		/// <summary>
		/// マッチした文字列の位置.
		/// </summary>
		public int index;

		/// <summary>
		/// マッチした文字列の行番号.
		/// </summary>
		public int lineNumber;

		/// <summary>
		/// 行の文字位置.
		/// </summary>
		public int lineIndex;

		/// <summary>
		/// マッチした時に使用した, 正規表現の Match オブジェクト.
		/// </summary>
		public Match match;
	}
}
