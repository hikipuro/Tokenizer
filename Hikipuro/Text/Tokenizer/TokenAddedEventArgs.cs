using System;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Arguments of TokenAddedEventHandler.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class TokenAddedEventArgs<TokenType> : EventArgs where TokenType : struct {
		/// <summary>
		/// TokenList in procesing.
		/// </summary>
		public TokenList<TokenType> TokenList;

		/// <summary>
		/// Added token.
		/// </summary>
		public Token<TokenType> Token;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tokenList">TokenList in procesing.</param>
		/// <param name="token">Added token.</param>
		public TokenAddedEventArgs(TokenList<TokenType> tokenList, Token<TokenType> token) {
			this.TokenList = tokenList;
			this.Token = token;
		}
	}
}
