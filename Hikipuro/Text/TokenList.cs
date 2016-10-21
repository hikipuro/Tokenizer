using System.Collections.Generic;

namespace Hikipuro.Text {
	/// <summary>
	/// トークンのリスト.
	/// ソースコード全体がここにリストとして保存される.
	/// </summary>
	/// <typeparam name="TokenType"></typeparam>
	class TokenList<TokenType> : List<Token<TokenType>> where TokenType : struct {
	}
}
