namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Delegate for event, a matched token added to TokenList.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	/// <param name="sender">Event sender object.</param>
	/// <param name="e">Event args.</param>
	public delegate void TokenAddedEventHandler<TokenType>(object sender, TokenAddedEventArgs<TokenType> e) where TokenType : struct;
}
