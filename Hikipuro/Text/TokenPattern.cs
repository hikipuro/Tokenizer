using System.Text.RegularExpressions;

namespace Hikipuro.Text {
	/// <summary>
	/// トークンのマッチ用パターン.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	class TokenPattern<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンの種類.
		/// </summary>
		public TokenType type;

		/// <summary>
		/// マッチ用の正規表現文字列.
		/// </summary>
		public string pattern;

		/// <summary>
		/// pattern の正規表現.
		/// </summary>
		public Regex regex;

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="pattern">マッチ用の正規表現文字列.</param>
		public TokenPattern(TokenType type, string pattern) {
			this.type = type;
			this.pattern = pattern;
			CompileRegex();
		}

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="pattern">マッチ用の正規表現文字列.</param>
		/// <param name="options">正規表現のオプション.</param>
		public TokenPattern(TokenType type, string pattern, RegexOptions options) {
			this.type = type;
			this.pattern = pattern;
			CompileRegex(options);
		}

		/// <summary>
		/// pattern 文字列を正規表現に変換する.
		/// </summary>
		/// <param name="options">正規表現のオプション.</param>
		private void CompileRegex(RegexOptions options = RegexOptions.Compiled) {
			Regex regex = new Regex(pattern, options);
			this.regex = regex;
		}
	}
}
