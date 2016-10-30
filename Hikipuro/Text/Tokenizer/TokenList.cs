using System;
using System.Collections.Generic;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// トークンのリスト.
	/// 分解された文字列全体が, ここにリストとして保存される.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class TokenList<TokenType> : List<Token<TokenType>> where TokenType : struct {
		/// <summary>
		/// Token オブジェクトを追加する.
		/// </summary>
		/// <param name="token">トークン.</param>
		public new void Add(Token<TokenType> token) {
			if (token == null) {
				return;
			}
			token.TokenList = this;
			base.Add(token);
		}

		/// <summary>
		/// TokenMatch オブジェクトを Token オブジェクトに変換してから追加する.
		/// </summary>
		/// <param name="tokenMatch">トークンのマッチした場所を表すオブジェクト.</param>
		public void Add(TokenMatch<TokenType> tokenMatch) {
			if (tokenMatch == null) {
				return;
			}
			Token<TokenType> token = Token<TokenType>.FromTokenMatch(tokenMatch);
			token.TokenList = this;
			base.Add(token);
		}

		/// <summary>
		/// Token オブジェクトを取り除く.
		/// </summary>
		/// <param name="token">トークン.</param>
		public new void Remove(Token<TokenType> token) {
			if (token == null) {
				return;
			}
			if (Contains(token) == false) {
				return;
			}
			token.TokenList = null;
			base.Remove(token);
		}

		/// <summary>
		/// Token オブジェクトを取り除く.
		/// </summary>
		/// <param name="index">インデックス番号.</param>
		public new void RemoveAt(int index) {
			Token<TokenType> token = this[index];
			Remove(token);
		}

		/// <summary>
		/// リストの最後から, インデックス値を指定して要素を取得する.
		/// </summary>
		/// <param name="index">最後の要素からのインデックス番号.</param>
		/// <returns>トークン.</returns>
		public Token<TokenType> Last(int index = 0) {
			if (Count <= 0) {
				return null;
			}
			int lastIndex = Count - 1;
			return this[lastIndex - index];
		}

		/// <summary>
		/// 引数で指定されたトークンの 1 つ次の要素を取得する.
		/// リストの端に達した場合は巡回する.
		/// 要素の数が 0 の場合は null を返す.
		/// </summary>
		/// <param name="token">トークン.</param>
		/// <returns>1 つ次の要素.</returns>
		public Token<TokenType> Next(Token<TokenType> token) {
			if (token == null || Count <= 0) {
				return null;
			}
			if (Contains(token) == false) {
				return null;
			}
			int index = IndexOf(token) + 1;
			if (index >= Count) {
				return null;
			}
			return this[index];
		}

		/// <summary>
		/// 引数で指定されたトークンの 1 つ前の要素を取得する.
		/// リストの端に達した場合は巡回する.
		/// 要素の数が 0 の場合は null を返す.
		/// </summary>
		/// <param name="token">トークン.</param>
		/// <returns>1 つ前の要素.</returns>
		public Token<TokenType> Prev(Token<TokenType> token) {
			if (token == null || Count <= 0) {
				return null;
			}
			if (Contains(token) == false) {
				return null;
			}
			int index = IndexOf(token) - 1;
			if (index < 0) {
				return null;
			}
			return this[index];
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
