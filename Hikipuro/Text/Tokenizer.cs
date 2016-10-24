using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hikipuro.Text {
	/// <summary>
	/// 文字列をトークンに分割するためのクラス.
	/// </summary>
	/// <typeparam name="TokenType">トークンの種類.</typeparam>
	public class Tokenizer<TokenType> where TokenType : struct {
		/// <summary>
		/// リストにトークンを追加する直前に呼ばれるイベント.
		/// イベントハンドラ内で e.Cancel = true; を設定したトークンは追加しない.
		/// </summary>
		public event BeforeAddTokenEventHandler<TokenType> BeforeAddToken;

		/// <summary>
		/// リストにトークンが追加された直後に呼ばれるイベント.
		/// </summary>
		public event TokenAddedEventHandler<TokenType> TokenAdded;

		/// <summary>
		/// Tokenize() の呼び出しごとに作られるコンテキスト.
		/// </summary>
		class Context {
			/// <summary>
			/// 処理対象の文字列.
			/// </summary>
			public string Text;

			/// <summary>
			/// 行の開始位置のインデックス番号のリスト.
			/// </summary>
			public List<int> LineIndexList;

			/// <summary>
			/// 処理中の文字位置.
			/// </summary>
			public int Index = 0;

			/// <summary>
			/// 処理中の行番号.
			/// </summary>
			public int LineNumber = 1;

			/// <summary>
			/// 処理中の行の文字位置.
			/// </summary>
			public int LineIndex = 0;

			/// <summary>
			/// コンストラクタ.
			/// </summary>
			public Context() {
			}
		}

		/// <summary>
		/// System.Threading.Sleep() を定期的に入れるための値.
		/// デフォルト値: 0.
		/// SleepWait で指定された回数分ループするごとに 1 回スリープする (トークンの個数).
		/// 0 以下の値を入れるとスリープしないようになる.
		/// </summary>
		public int SleepWait = 0;

		/// <summary>
		/// トークンのマッチ用パターン.
		/// AddPattern() で追加する.
		/// </summary>
		List<TokenPattern<TokenType>> patterns;

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		public Tokenizer() {
			patterns = new List<TokenPattern<TokenType>>();
		}

		/// <summary>
		/// デストラクタ.
		/// </summary>
		~Tokenizer() {
			if (patterns != null) {
				patterns.Clear();
				patterns = null;
			}
			if (BeforeAddToken != null) {
				foreach (Delegate d in BeforeAddToken.GetInvocationList()) {
					BeforeAddToken -= (BeforeAddTokenEventHandler<TokenType>)d;
				}
			}
			if (TokenAdded != null) {
				foreach (Delegate d in TokenAdded.GetInvocationList()) {
					TokenAdded -= (TokenAddedEventHandler<TokenType>)d;
				}
			}
		}

		/// <summary>
		/// 正規表現のマッチパターンを追加する.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="patternText">マッチ用の正規表現文字列.</param>
		/// <returns>追加されたパターン.</returns>
		public TokenPattern<TokenType> AddPattern(TokenType type, string patternText) {
			if (patternText == null || patternText == string.Empty) {
				return null;
			}
			if (HasPatternType(type)) {
				return null;
			}
			TokenPattern<TokenType> pattern;
			pattern = new TokenPattern<TokenType>(type, patternText);
			patterns.Add(pattern);
			return pattern;
		}

		/// <summary>
		/// 正規表現のマッチパターンを追加する.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="patternText">マッチ用の正規表現文字列.</param>
		/// <param name="options">正規表現のオプション.</param>
		/// <returns>追加されたパターン.</returns>
		public TokenPattern<TokenType> AddPattern(TokenType type, string patternText, RegexOptions options) {
			if (patternText == null || patternText == string.Empty) {
				return null;
			}
			if (HasPatternType(type)) {
				return null;
			}
			TokenPattern<TokenType> pattern;
			pattern = new TokenPattern<TokenType>(type, patternText, options);
			patterns.Add(pattern);
			return pattern;
		}

		/// <summary>
		/// 引数で指定したトークンの種類が, パターンに追加されているかチェックする.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <returns>true: すでに追加されている, false: 追加されていない.</returns>
		public bool HasPatternType(TokenType type) {
			foreach (TokenPattern<TokenType> pattern in patterns) {
				if (pattern.Type.Equals(type)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 登録されたパターンを使ってトークンに分割する.
		/// マッチに失敗した場合, ParseException 例外を発生させる.
		/// </summary>
		/// <param name="text">処理対象の文字列.</param>
		/// <returns>トークンのリスト.</returns>
		public TokenList<TokenType> Tokenize(string text) {
			if (text == null || text == string.Empty) {
				return null;
			}

			// 戻り値
			TokenList<TokenType> tokens = new TokenList<TokenType>();

			// コンテキストオブジェクトを作成する
			Context context = new Context();
			context.Text = text;

			// Sleep() を定期的に入れる用
			int loopCount = 0;

			// 改行位置をチェックしておく
			context.LineIndexList = CreateLineIndexList(text);

			// テキストの終わりまでマッチを試す
			while (context.Index < text.Length) {
				// 登録されたパターンでマッチを試す
				TokenMatch<TokenType> tokenMatch = TryMatchToken(context);

				// マッチしなかった時は, 例外を投げる
				if (tokenMatch == null) {
					ThrowParseException(context);
				}

				// マッチした場合

				// インデックスの位置を動かしておく
				int matchLength = tokenMatch.Match.Length;
				context.Index += matchLength;
				context.LineIndex += matchLength;

				if (BeforeAddToken != null) {
					// 追加前イベントが登録されている場合は実行する
					// false が返ってきた場合はトークンリストに追加しない
					BeforeAddTokenEventArgs<TokenType> args
						= new BeforeAddTokenEventArgs<TokenType>(tokenMatch);
					BeforeAddToken(this, args);
					if (args.Cancel == false) {
						AddToken(tokens, tokenMatch);
					}
				} else {
					// トークンをリストに追加する
					AddToken(tokens, tokenMatch);
				}

				// 改行位置に到達した時
				if (context.LineIndexList[context.LineNumber] == context.Index) {
					context.LineIndex = 0;
					context.LineNumber++;
				}

				// Sleep() を定期的に入れる
				if (SleepWait > 0) {
					loopCount++;
					if (loopCount > SleepWait) {
						loopCount = 0;
						System.Threading.Thread.Sleep(1);
					}
				}
			}

			return tokens;
		}

		/// <summary>
		/// 改行位置のインデックス番号のリストを返す.
		/// </summary>
		/// <param name="text">処理対象の文字列.</param>
		/// <returns>改行位置のリスト.</returns>
		private List<int> CreateLineIndexList(string text) {
			List<int> lineIndexList = new List<int>();
			lineIndexList.Add(0);

			Regex regex = new Regex("\r\n|\r|\n", RegexOptions.None);
			MatchCollection matches = regex.Matches(text);
			foreach (Match match in matches) {
				if (match.Success == false) {
					continue;
				}
				lineIndexList.Add(match.Index + match.Length);
			}

			lineIndexList.Add(0);
			return lineIndexList;
		}

		/// <summary>
		/// トークンのマッチを試す.
		/// マッチしなかった場合, null を返す.
		/// </summary>
		/// <param name="context">コンテキスト.</param>
		/// <returns>マッチオブジェクト.</returns>
		private TokenMatch<TokenType> TryMatchToken(Context context) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// すべてのパターンを試す
			foreach (TokenPattern<TokenType> pattern in patterns) {
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
			TokenMatch<TokenType> tokenMatch = new TokenMatch<TokenType>(match.Value);
			tokenMatch.Type = tokenPattern.Type;
			tokenMatch.Index = context.Index;
			tokenMatch.LineNumber = context.LineNumber;
			tokenMatch.LineIndex = context.LineIndex;
			tokenMatch.Match = match;
			//tokenMatch.Text = match.Value;

			return tokenMatch;
		}

		/// <summary>
		/// リストにトークンを追加する.
		/// </summary>
		/// <param name="tokenList">追加先のリスト.</param>
		/// <param name="tokenMatch">追加するマッチオブジェクト.</param>
		private void AddToken(TokenList<TokenType> tokenList, TokenMatch<TokenType> tokenMatch) {
			tokenList.Add(tokenMatch);

			// 追加後イベントを実行する
			if (TokenAdded != null) {
				Token<TokenType> token = tokenList.Last();
				TokenAddedEventArgs<TokenType> args
					= new TokenAddedEventArgs<TokenType>(tokenList, token);
				TokenAdded(this, args);
			}
		}

		/// <summary>
		/// 指定された行番号の文字列を取得する.
		/// </summary>
		/// <param name="context">コンテキスト.</param>
		/// <returns>指定された行の文字列.</returns>
		private string GetLine(Context context) {
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
			} else if (index != 0) {
				return context.Text.Substring(index);
			}
			return string.Empty;
		}

		/// <summary>
		/// ParseException 例外を発生させる.
		/// </summary>
		/// <param name="context">コンテキスト.</param>
		private void ThrowParseException(Context context) {
			string lineText = GetLine(context);
			ParseException exception = new ParseException(string.Format(
				"Parse Error (Line:{0}, Index:{1}){2}{3}",
				context.LineNumber,
				context.LineIndex,
				Environment.NewLine,
				lineText
			));

			exception.Text = context.Text;
			exception.LineText = lineText;
			exception.Index = context.Index;
			exception.LineNumber = context.LineNumber;
			exception.LineIndex = context.LineIndex;

			throw exception;
		}
	}
}
