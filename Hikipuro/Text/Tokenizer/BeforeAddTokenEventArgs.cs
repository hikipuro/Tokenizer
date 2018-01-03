using System;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Arguments of BeforeAddTokenEventHandler.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class BeforeAddTokenEventArgs<TokenType> : EventArgs where TokenType : struct {
		/// <summary>
		/// Position of token matched.
		/// </summary>
		public TokenMatch<TokenType> TokenMatch;

		/// <summary>
		/// if you don't want to add matched token to list,
		/// to set true this Cancel property.
		/// </summary>
		public bool Cancel = false;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="tokenMatch">Token match object.</param>
		public BeforeAddTokenEventArgs(TokenMatch<TokenType> tokenMatch)  {
			this.TokenMatch = tokenMatch;
		}
	}
}
