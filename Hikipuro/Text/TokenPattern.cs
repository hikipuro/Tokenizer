using System.Text.RegularExpressions;

namespace Hikipuro.Text {
	/// <summary>
	/// トークンのマッチ用パターン.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class TokenPattern<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンの種類.
		/// </summary>
		public TokenType Type;

		/// <summary>
		/// マッチ用の正規表現文字列.
		/// </summary>
		public string Pattern;

		/// <summary>
		/// Pattern の正規表現.
		/// </summary>
		public Regex Regex;

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="pattern">マッチ用の正規表現文字列.</param>
		public TokenPattern(TokenType type, string pattern) {
			this.Type = type;
			this.Pattern = pattern;
			#if UNITY_5 || UNITY_5_3_OR_NEWER
			CompileRegex(RegexOptions.None);
			#else
			CompileRegex(RegexOptions.Compiled);
			#endif
		}

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="pattern">マッチ用の正規表現文字列.</param>
		/// <param name="options">正規表現のオプション.</param>
		public TokenPattern(TokenType type, string pattern, RegexOptions options) {
			this.Type = type;
			this.Pattern = pattern;
			CompileRegex(options);
		}

		/// <summary>
		/// Pattern 文字列を正規表現に変換する.
		/// </summary>
		/// <param name="options">正規表現のオプション.</param>
		private void CompileRegex(RegexOptions options) {
			Regex regex = new Regex(Pattern, options);
			this.Regex = regex;
		}
	}
}
