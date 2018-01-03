namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Delegate for event, a matched token will add to TokenList.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	/// <param name="sender">Event sender object.</param>
	/// <param name="e">Event args.</param>
	public delegate void BeforeAddTokenEventHandler<TokenType>(object sender, BeforeAddTokenEventArgs<TokenType> e) where TokenType : struct;
}
