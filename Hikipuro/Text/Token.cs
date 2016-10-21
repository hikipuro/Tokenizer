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
