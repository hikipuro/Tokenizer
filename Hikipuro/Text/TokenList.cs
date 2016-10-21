using System;
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

		/// <summary>
		/// 2 つのトークン間の距離を, トークンの数で取得する.
		/// 同じトークン: 0, 隣接しているトークン: 1.
		/// TODO: IndexOf() が遅そうなので, 高速化する.
		/// </summary>
		/// <param name="tokenA">トークン A.</param>
		/// <param name="tokenB">トークン B.</param>
		/// <returns>トークン数.</returns>
		public int GetDistance(Token<TokenType> tokenA, Token<TokenType> tokenB) {
			int indexA = IndexOf(tokenA);
			int indexB = IndexOf(tokenB);
			return Math.Abs(indexA - indexB);
		}

		/// <summary>
		/// 2 つのトークンの位置を入れ替える.
		/// TODO: IndexOf() が遅そうなので, 高速化する.
		/// </summary>
		/// <param name="tokenA">トークン A.</param>
		/// <param name="tokenB">トークン B.</param>
		public void Swap(Token<TokenType> tokenA, Token<TokenType> tokenB) {
			int indexA = IndexOf(tokenA);
			int indexB = IndexOf(tokenB);
			Swap(indexA, indexB);
		}

		/// <summary>
		/// 2 つのトークンの位置を入れ替える.
		/// </summary>
		/// <param name="indexA">トークン A のインデックス.</param>
		/// <param name="indexB">トークン B のインデックス.</param>
		public void Swap(int indexA, int indexB) {
			Token<TokenType> token = this[indexA];
			this[indexA] = this[indexB];
			this[indexB] = token;
		}
	}
}
