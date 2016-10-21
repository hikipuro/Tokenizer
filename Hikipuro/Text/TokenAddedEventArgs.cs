using System;

namespace Hikipuro.Text {
	/// <summary>
	/// TokenAddedEventHandler の引数.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class TokenAddedEventArgs<TokenType> : EventArgs where TokenType : struct {
		/// <summary>
		/// 処理中のトークンのリスト.
		/// </summary>
		public TokenList<TokenType> tokenList;

		/// <summary>
		/// 追加されたトークン.
		/// </summary>
		public Token<TokenType> token;

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="tokenList">処理中のトークンのリスト.</param>
		/// <param name="token">追加されたトークン.</param>
		public TokenAddedEventArgs(TokenList<TokenType> tokenList, Token<TokenType> token) {
			this.tokenList = tokenList;
			this.token = token;
		}
	}
}
