using System.Collections.Generic;

namespace Hikipuro.Text {
	/// <summary>
	/// トークンのリスト.
	/// ソースコード全体がここにリストとして保存される.
	/// </summary>
	/// <typeparam name="TokenType"></typeparam>
	class TokenList<TokenType> : List<Token<TokenType>> where TokenType : struct {
		/// <summary>
		/// TokenMatch オブジェクトを Token オブジェクトに変換してから追加する.
		/// </summary>
		/// <param name="tokenMatch"></param>
		public void Add(TokenMatch<TokenType> tokenMatch) {
			Add(Token<TokenType>.FromTokenMatch(tokenMatch));
		}
	}
}
