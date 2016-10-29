using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// ステップ実行用の Tokenizer.
	/// Tokenizer.CreateSteppingTokenizer() で作成する.
	/// </summary>
	/// <typeparam name="TokenType"></typeparam>
	public class SteppingTokenizer<TokenType> : IEnumerable<Token<TokenType>> where TokenType : struct {
		/// <summary>
		/// コンテキスト.
		/// </summary>
		Tokenizer<TokenType>.Context context;

		/// <summary>
		/// 次の項目があるかチェックする.
		/// true: 次の項目がある, false: ない.
		/// </summary>
		public bool HasNext {
			get { return context.Index < context.Text.Length; }
		}

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="context"></param>
		public SteppingTokenizer(Tokenizer<TokenType>.Context context) {
			this.context = context;
		}

		/// <summary>
		/// 次の項目を取得する.
		/// マッチしない場合は null を返す.
		/// </summary>
		/// <returns>トークン.</returns>
		public Token<TokenType> Next() {
			// 登録されたパターンでマッチを試す
			Token<TokenType> token = TryMatchToken(context);

			// マッチしなかった時は, null を返す
			if (token == null) {
				return null;
			}

			// マッチした場合

			// インデックスの位置を動かしておく
			int matchLength = token.Length;
			context.Index += matchLength;
			context.LineIndex += matchLength;
			
			// 改行位置に到達した時
			if (context.LineIndexList[context.LineNumber] == context.Index) {
				context.LineIndex = 1;
				context.LineNumber++;
			}

			return token;
		}

		/// <summary>
		/// 引数で指定した種類で, 次の項目を取得する.
		/// マッチしない場合は null を返す.
		/// </summary>
		/// <param name="tokenType">トークンの種類.</param>
		/// <returns>トークン.</returns>
		public Token<TokenType> Next(TokenType tokenType) {
			// 登録されたパターンでマッチを試す
			Token<TokenType> token = TryMatchToken(context, tokenType);

			// マッチしなかった時は, null を返す
			if (token == null) {
				return null;
			}

			// マッチした場合

			// インデックスの位置を動かしておく
			int matchLength = token.Length;
			context.Index += matchLength;
			context.LineIndex += matchLength;

			// 改行位置に到達した時
			if (context.LineIndexList[context.LineNumber] == context.Index) {
				context.LineIndex = 1;
				context.LineNumber++;
			}

			return token;
		}

		/// <summary>
		/// 次の要素が, 引数で指定した種類にマッチするかチェックする.
		/// </summary>
		/// <param name="tokenType">トークンの種類.</param>
		/// <returns>true: マッチした, false: マッチしない.</returns>
		public bool IsMatchNext(TokenType tokenType) {
			// すべてのパターンを巡回する
			foreach (TokenPattern<TokenType> pattern in context.Patterns) {
				// 種類が違う場合は飛ばす
				if (pattern.Type.Equals(tokenType) == false) {
					continue;
				}
				Match match = pattern.Regex.Match(context.Text, context.Index);

				// マッチに失敗した場合
				if (!match.Success) {
					return false;
				}
				// 先頭位置とマッチしなかった場合
				if (match.Index != context.Index) {
					return false;
				}
				// 長さ 0 でマッチする場合があるので回避
				if (match.Length <= 0) {
					return false;
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// 現在の行の文字列を取得する.
		/// </summary>
		/// <returns></returns>
		public string GetLine() {
			return GetLine(context);
		}

		/// <summary>
		/// トークンのマッチを試す.
		/// マッチしなかった場合, null を返す.
		/// </summary>
		/// <param name="context">コンテキスト.</param>
		/// <returns>トークン.</returns>
		private Token<TokenType> TryMatchToken(Tokenizer<TokenType>.Context context) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// すべてのパターンを試す
			foreach (TokenPattern<TokenType> pattern in context.Patterns) {
				match = pattern.Regex.Match(context.Text, context.Index);

				// マッチに失敗した場合
				if (!match.Success) {
					continue;
				}
				// 先頭位置とマッチしなかった場合
				if (match.Index != context.Index) {
					continue;
				}
				// 長さ 0 でマッチする場合があるので回避
				if (match.Length <= 0) {
					continue;
				}
				// マッチした
				tokenPattern = pattern;
				break;
			}

			// マッチしなかった場合
			if (tokenPattern == null) {
				return null;
			}

			// マッチした場合
			Token<TokenType> token = new Token<TokenType>(match.Value);
			token.Type = tokenPattern.Type;
			token.Index = context.Index;
			token.LineNumber = context.LineNumber;
			token.LineIndex = context.LineIndex;
			return token;
		}

		/// <summary>
		/// 引数で指定した種類で, トークンのマッチを試す.
		/// マッチしなかった場合, null を返す.
		/// </summary>
		/// <param name="context">コンテキスト.</param>
		/// <param name="tokenType">トークンの種類.</param>
		/// <returns>トークン.</returns>
		private Token<TokenType> TryMatchToken(Tokenizer<TokenType>.Context context, TokenType tokenType) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// すべてのパターンを試す
			foreach (TokenPattern<TokenType> pattern in context.Patterns) {
				// 種類が違う場合は飛ばす
				if (pattern.Type.Equals(tokenType) == false) {
					continue;
				}

				match = pattern.Regex.Match(context.Text, context.Index);

				// マッチに失敗した場合
				if (!match.Success) {
					continue;
				}
				// 先頭位置とマッチしなかった場合
				if (match.Index != context.Index) {
					continue;
				}
				// 長さ 0 でマッチする場合があるので回避
				if (match.Length <= 0) {
					continue;
				}
				// マッチした
				tokenPattern = pattern;
				break;
			}

			// マッチしなかった場合
			if (tokenPattern == null) {
				return null;
			}

			// マッチした場合
			Token<TokenType> token = new Token<TokenType>(match.Value);
			token.Type = tokenPattern.Type;
			token.Index = context.Index;
			token.LineNumber = context.LineNumber;
			token.LineIndex = context.LineIndex;
			return token;
		}

		/// <summary>
		/// 指定された行番号の文字列を取得する.
		/// </summary>
		/// <param name="context">コンテキスト.</param>
		/// <returns>指定された行の文字列.</returns>
		private string GetLine(Tokenizer<TokenType>.Context context) {
			int lineNumber = context.LineNumber;
			lineNumber--;
			if (lineNumber < 0 || lineNumber >= context.LineIndexList.Count) {
				return string.Empty;
			}
			int index = context.LineIndexList[lineNumber];
			if (index >= context.Text.Length) {
				return string.Empty;
			}
			Regex regex = new Regex("\r\n|\r|\n", RegexOptions.None);
			Match match = regex.Match(context.Text, index);
			if (match.Success) {
				string lineText = context.Text.Substring(index, match.Index - index);
				//lineText.Replace("\t", "    ");
				return lineText;
			}
			return context.Text.Substring(index);
		}

		/// <summary>
		/// IEnumerable の実装.
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Token<TokenType>> GetEnumerator() {
			while (HasNext) {
				Token<TokenType> token = Next();
				yield return token;
			}
		}

		/// <summary>
		/// IEnumerable の実装.
		/// </summary>
		/// <returns></returns>
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
