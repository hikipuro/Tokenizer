namespace Hikipuro.Text {
	/// <summary>
	/// リストにトークンを追加する直前に呼ばれるイベントのデリゲート.
	/// </summary>
	/// <param name="tokenMatch">トークンのマッチした場所を表すオブジェクト.</param>
	/// <returns>true: リストに追加する, false: リストに追加しない.</returns>
	public delegate bool BeforeAddTokenEventHandler<TokenType>(TokenMatch<TokenType> tokenMatch) where TokenType : struct;
}
