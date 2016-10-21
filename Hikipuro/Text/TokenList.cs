using System.Collections.Generic;

namespace Hikipuro.Text {
	/// <summary>
	/// トークンのリスト.
	/// 分解された文字列全体が, ここにリストとして保存される.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class TokenList<TokenType> : List<Token<TokenType>> where TokenType : struct {
		/// <summary>
		/// TokenMatch オブジェクトを Token オブジェクトに変換してから追加する.
		/// </summary>
		/// <param name="tokenMatch">トークンのマッチした場所を表すオブジェクト.</param>
		public void Add(TokenMatch<TokenType> tokenMatch) {
			Add(Token<TokenType>.FromTokenMatch(tokenMatch));
		}

		/// <summary>
		/// リストの最後から, インデックス値を指定して要素を取得する.
		/// </summary>
		/// <param name="index">最後の要素からのインデックス番号.</param>
		/// <returns>トークン.</returns>
		public Token<TokenType> Last(int index = 0) {
			int lastIndex = Count - 1;
			return this[lastIndex - index];
		}
	}
}
