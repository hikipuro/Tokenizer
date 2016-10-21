using System.Text.RegularExpressions;

namespace Hikipuro.Text {
	/// <summary>
	/// トークンのマッチした場所.
	/// </summary>
	/// <typeparam name="TokenType"></typeparam>
	class TokenMatch<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンの種類.
		/// </summary>
		public TokenType type;

		/// <summary>
		/// 文字列の位置.
		/// </summary>
		public int index;

		/// <summary>
		/// 行番号.
		/// </summary>
		public int lineNumber;

		/// <summary>
		/// 正規表現の Match オブジェクト.
		/// マッチしなかった場合は null.
		/// </summary>
		public Match match;

		/// <summary>
		/// マッチした文字列.
		/// BeforeAddToken イベントの中で修正できる.
		/// </summary>
		public string value;
	}
}
