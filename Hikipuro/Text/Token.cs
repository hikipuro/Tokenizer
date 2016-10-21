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
		/// 行番号.
		/// </summary>
		public int lineNumber;

		/// <summary>
		/// 行の文字位置.
		/// </summary>
		public int lineIndex;
	}
}
