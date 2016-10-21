namespace Hikipuro.Text {
	/// <summary>
	/// トークン 1 つ分.
	/// </summary>
	/// <typeparam name="TokenType"></typeparam>
	class Token<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンの種類.
		/// </summary>
		public TokenType type;

		/// <summary>
		/// マッチしたテキスト.
		/// </summary>
		public string text;

		/// <summary>
		/// 文字列の位置.
		/// </summary>
		public int index;

		/// <summary>
		/// 行番号.
		/// </summary>
		public int lineNumber;

		/// <summary>
		/// 行の文字位置.
		/// </summary>
		public int lineIndex;

		/// <summary>
		/// TokenMatch オブジェクトから Token オブジェクトを作成する.
		/// </summary>
		/// <param name="tokenMatch">作成元の TokenMatch オブジェクト.</param>
		/// <returns>Token オブジェクト.</returns>
		public static Token<TokenType> FromTokenMatch(TokenMatch<TokenType> tokenMatch) {
			Token<TokenType> token = new Token<TokenType>();
			token.type = tokenMatch.type;
			token.text = tokenMatch.text;
			token.index = tokenMatch.index;
			token.lineNumber = tokenMatch.lineNumber;
			token.lineIndex = tokenMatch.lineIndex;
			return token;
		}
	}
}
