namespace Hikipuro.Text {
	/// <summary>
	/// リストにトークンが追加された直後に呼ばれるイベントのデリゲート.
	/// </summary>
	/// <param name="tokens">処理中のトークンのリスト.</param>
	/// <param name="token">追加されたトークンオブジェクト.</param>
	public delegate void TokenAddedEventHandler<TokenType>(object sender, TokenAddedEventArgs<TokenType> e) where TokenType : struct;
}
