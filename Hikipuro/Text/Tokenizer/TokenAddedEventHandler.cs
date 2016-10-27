namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// リストにトークンが追加された直後に呼ばれるイベントのデリゲート.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	/// <param name="sender">イベントの送信元.</param>
	/// <param name="e">イベントの引数.</param>
	public delegate void TokenAddedEventHandler<TokenType>(object sender, TokenAddedEventArgs<TokenType> e) where TokenType : struct;
}
