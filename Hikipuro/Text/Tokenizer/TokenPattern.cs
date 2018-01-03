using System.Text.RegularExpressions;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// A token matching pattern.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class TokenPattern<TokenType> where TokenType : struct {
		/// <summary>
		/// Token type.
		/// </summary>
		public TokenType Type;

		/// <summary>
		/// Regex pattern for matching.
		/// </summary>
		public string Pattern;

		/// <summary>
		/// Regex object for matching.
		/// </summary>
		public Regex Regex;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="type">Token type.</param>
		/// <param name="pattern">Regex patern text for matching.</param>
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
		/// Constructor.
		/// </summary>
		/// <param name="type">Token type.</param>
		/// <param name="pattern">Regex patern text for matching.</param>
		/// <param name="options">Regex options.</param>
		public TokenPattern(TokenType type, string pattern, RegexOptions options) {
			this.Type = type;
			this.Pattern = pattern;
			CompileRegex(options);
		}

		/// <summary>
		/// Convet pattern text to Regex object.
		/// </summary>
		/// <param name="options">Regex options.</param>
		private void CompileRegex(RegexOptions options) {
			Regex regex = new Regex(Pattern, options);
			this.Regex = regex;
		}
	}
}
