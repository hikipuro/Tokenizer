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
		public TokenType Type;

		/// <summary>
		/// マッチした文字列.
		/// BeforeAddToken イベントの中で修正できる.
		/// </summary>
		public string Text;

		/// <summary>
		/// マッチした文字列の位置.
		/// </summary>
		public int Index;

		/// <summary>
		/// マッチした文字列の行番号.
		/// </summary>
		public int LineNumber;

		/// <summary>
		/// 行の文字位置.
		/// </summary>
		public int LineIndex;

		/// <summary>
		/// マッチした時に使用した, 正規表現の Match オブジェクト.
		/// </summary>
		public Match Match;
	}
}
