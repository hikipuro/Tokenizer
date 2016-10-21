using System;

namespace Hikipuro.Text {
	/// <summary>
	/// BeforeAddTokenEventHandler の引数.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class BeforeAddTokenEventArgs<TokenType> : EventArgs where TokenType : struct {
		/// <summary>
		/// トークンのマッチした場所を表すオブジェクト.
		/// </summary>
		public TokenMatch<TokenType> TokenMatch;

		/// <summary>
		/// リストに追加する処理をキャンセルする場合は true.
		/// </summary>
		public bool Cancel = false;

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="tokenMatch">トークンのマッチした場所を表すオブジェクト.</param>
		public BeforeAddTokenEventArgs(TokenMatch<TokenType> tokenMatch)  {
			this.TokenMatch = tokenMatch;
		}
	}
}
