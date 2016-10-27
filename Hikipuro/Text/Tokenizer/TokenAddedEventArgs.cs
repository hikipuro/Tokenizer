using System;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// TokenAddedEventHandler の引数.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class TokenAddedEventArgs<TokenType> : EventArgs where TokenType : struct {
		/// <summary>
		/// 処理中のトークンのリスト.
		/// </summary>
		public TokenList<TokenType> TokenList;

		/// <summary>
		/// 追加されたトークン.
		/// </summary>
		public Token<TokenType> Token;

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="tokenList">処理中のトークンのリスト.</param>
		/// <param name="token">追加されたトークン.</param>
		public TokenAddedEventArgs(TokenList<TokenType> tokenList, Token<TokenType> token) {
			this.TokenList = tokenList;
			this.Token = token;
		}
	}
}
