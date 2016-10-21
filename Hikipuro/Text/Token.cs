namespace Hikipuro.Text {
	/// <summary>
	/// トークン 1 つ分.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class Token<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンの種類.
		/// </summary>
		public TokenType Type;

		/// <summary>
		/// マッチしたテキスト.
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
		/// TokenMatch オブジェクトから Token オブジェクトを作成する.
		/// </summary>
		/// <param name="tokenMatch">作成元の TokenMatch オブジェクト.</param>
		/// <returns>Token オブジェクト.</returns>
		public static Token<TokenType> FromTokenMatch(TokenMatch<TokenType> tokenMatch) {
			Token<TokenType> token = new Token<TokenType>();
			token.Type = tokenMatch.Type;
			token.Text = tokenMatch.Text;
			token.Index = tokenMatch.Index;
			token.LineNumber = tokenMatch.LineNumber;
			token.LineIndex = tokenMatch.LineIndex;
			return token;
		}
	}
}
