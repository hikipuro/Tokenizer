namespace Hikipuro.Text {
	/// <summary>
	/// リストにトークンを追加する直前に呼ばれるイベントのデリゲート.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	/// <param name="sender">イベントの送信元.</param>
	/// <param name="e">イベントの引数.</param>
	public delegate void BeforeAddTokenEventHandler<TokenType>(object sender, BeforeAddTokenEventArgs<TokenType> e) where TokenType : struct;
}
