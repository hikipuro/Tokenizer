using System;

namespace Hikipuro.Text {
	/// <summary>
	/// トークン 1 つ分.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class Token<TokenType> where TokenType : struct {
		/// <summary>
		/// トークンの種類.
		/// </summary>
		public TokenType Type;

		/// <summary>
		/// マッチしたテキスト.
		/// </summary>
		public string Text;

		/// <summary>
		/// マッチした文字列の位置.
		/// </summary>
		public int Index;

		/// <summary>
		/// マッチした文字列の行番号.
		/// </summary>
		public int LineNumber;

		/// <summary>
		/// 行の文字位置.
		/// </summary>
		public int LineIndex;

		/// <summary>
		/// 追加されたリスト.
		/// </summary>
		public TokenList<TokenType> TokenList;

		/// <summary>
		/// タグ.
		/// ユーザー側でトークンに目印を付けるために使用する.
		/// ライブラリ側では値をセットしない.
		/// </summary>
		public object Tag;

		/// <summary>
		/// 加工前のマッチした文字列.
		/// </summary>
		string rawText;

		/// <summary>
		/// 加工前のマッチした文字列 (読み取り専用).
		/// </summary>
		public string RawText {
			get { return rawText; }
		}

		/// <summary>
		/// マッチしたテキストの長さ.
		/// </summary>
		public int Length {
			get { return Text.Length; }
		}

		/// <summary>
		/// Index + Length の位置.
		/// </summary>
		public int RightIndex {
			get { return Index + Text.Length; }
		}

		/// <summary>
		/// LineIndex + Length の位置.
		/// </summary>
		public int RightLineIndex {
			get { return LineIndex + Text.Length; }
		}

		/// <summary>
		/// このトークンが, リスト内の最後の要素かチェックする.
		/// </summary>
		public bool IsLast {
			get {
				if (TokenList == null) {
					return false;
				}
				return TokenList.Last(0) == this;
			}
		}

		/// <summary>
		/// 1 つ次の要素を取得する.
		/// </summary>
		/// <returns>1 つ次の要素.</returns>
		public Token<TokenType> Next {
			get {
				if (TokenList == null) {
					return null;
				}
				return TokenList.Next(this);
			}
		}

		/// <summary>
		/// 1 つ前の要素を取得する.
		/// </summary>
		/// <returns>1 つ前の要素.</returns>
		public Token<TokenType> Prev {
			get {
				if (TokenList == null) {
					return null;
				}
				return TokenList.Prev(this);
			}
		}

		/// <summary>
		/// TokenMatch オブジェクトから Token オブジェクトを作成する.
		/// </summary>
		/// <param name="tokenMatch">作成元の TokenMatch オブジェクト.</param>
		/// <returns>Token オブジェクト.</returns>
		public static Token<TokenType> FromTokenMatch(TokenMatch<TokenType> tokenMatch) {
			Token<TokenType> token = new Token<TokenType>(tokenMatch.RawText);
			token.Type = tokenMatch.Type;
			token.Text = tokenMatch.Text;
			token.Index = tokenMatch.Index;
			token.LineNumber = tokenMatch.LineNumber;
			token.LineIndex = tokenMatch.LineIndex;
			return token;
		}

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		public Token() {
		}

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		/// <param name="text">マッチした文字列.</param>
		public Token(string text) {
			rawText = text;
			this.Text = text;
		}

		/// <summary>
		/// デストラクタ.
		/// </summary>
		~Token() {
			TokenList = null;
			Tag = null;
		}

		/// <summary>
		/// このトークンが, 引数で指定された種類と一致するかチェックする.
		/// </summary>
		/// <param name="tokenType">トークンの種類.</param>
		/// <returns>true: 一致, false: 一致しない.</returns>
		public bool IsTypeOf(TokenType tokenType) {
			return Type.Equals(tokenType);
		}

		/// <summary>
		/// このトークンが, 引数で指定されたグループに属するかチェックする.
		/// </summary>
		/// <param name="tokenTypeGroup">チェックするグループ.</param>
		/// <returns>true: 属している, false: 属していない.</returns>
		public bool IsMemberOf(TokenTypeGroup<TokenType> tokenTypeGroup) {
			if (tokenTypeGroup == null) {
				return false;
			}
			return tokenTypeGroup.Contains(Type);
		}

		/// <summary>
		/// 開始位置同士の距離を文字数で取得する.
		/// </summary>
		/// <param name="token">比較用のトークン.</param>
		/// <returns>文字数.</returns>
		public int GetDistance(Token<TokenType> token) {
			return Math.Abs(Index - token.Index);
		}

		/// <summary>
		/// このトークンの位置が, 引数で指定されたトークンよりも前にあるかチェックする.
		/// </summary>
		/// <param name="token">比較用のトークン.</param>
		/// <returns>true: 前にある, false: 後ろにある.</returns>
		public bool IsBefore(Token<TokenType> token) {
			return Index < token.Index;
		}

		/// <summary>
		/// このトークンの位置が, 引数で指定されたトークンよりも後にあるかチェックする.
		/// </summary>
		/// <param name="token">比較用のトークン.</param>
		/// <returns>true: 後ろにある, false: 前にある.</returns>
		public bool IsAfter(Token<TokenType> token) {
			return Index > token.Index;
		}

		/// <summary>
		/// このトークンの位置が, 引数で指定されたトークンと隣接しているかチェックする.
		/// </summary>
		/// <param name="token">比較用のトークン.</param>
		/// <returns>true: 隣接している, false: 隣接していない.</returns>
		public bool IsNeighbor(Token<TokenType> token) {
			if (this.IsBefore(token)) {
				return Index + RawText.Length == token.Index;
			}
			return token.Index + token.RawText.Length == Index;
		}
	}
}
