using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Timers;

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
		/// タイムアウト時間 (ミリ秒).
		/// デフォルト値: 0.
		/// 処理が長時間に及ぶ場合はタイムアウト例外を発生させる.
		/// 0 以下の値を入れるとタイムアウトしないようになる.
		/// </summary>
		public int Timeout = 0;

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
		/// 行の開始位置のインデックス番号のリスト.
		/// </summary>
		List<int> lineIndexList;

		/// <summary>
		/// 改行にマッチする正規表現.
		/// コンストラクタで作成する.
		/// </summary>
		Regex newLineRegex;

		/// <summary>
		/// 処理中の文字位置.
		/// </summary>
		int index = 0;

		/// <summary>
		/// 処理中の行番号.
		/// </summary>
		int lineNumber = 1;

		/// <summary>
		/// 処理中の行の文字位置.
		/// </summary>
		int lineIndex = 0;

		/// <summary>
		/// タイムアウトの処理用.
		/// 正規表現で無限ループになってしまう時があるため.
		/// TODO: Timer クラスを System.Threading.Timer にするかどうか考える.
		/// </summary>
		Timer timer;

		/// <summary>
		/// 処理中の文字位置.
		/// </summary>
		public int Index {
			get { return index; }
		}

		/// <summary>
		/// 処理中の行番号.
		/// </summary>
		public int LineNumber {
			get { return lineNumber; }
		}

		/// <summary>
		/// 処理中の行の文字位置.
		/// </summary>
		public int LineIndex {
			get { return lineIndex; }
		}

		/// <summary>
		/// コンストラクタ.
		/// </summary>
		public Tokenizer() {
			patterns = new List<TokenPattern<TokenType>>();
			lineIndexList = new List<int>();

			#if UNITY_5 || UNITY_5_3_OR_NEWER
			newLineRegex = new Regex("\r\n|\r|\n", RegexOptions.None);
			#else
			newLineRegex = new Regex("\r\n|\r|\n", RegexOptions.Compiled);
			#endif
			timer = CreateTimeoutTimer();
		}

		/// <summary>
		/// デストラクタ.
		/// </summary>
		~Tokenizer() {
			if (timer != null) {
				timer.Dispose();
				timer = null;
			}
			if (patterns != null) {
				patterns.Clear();
				patterns = null;
			}
			if (lineIndexList != null) {
				lineIndexList.Clear();
				lineIndexList = null;
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
		/// スペース文字列を削除する.
		/// (調整中)
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public string TrimSpaces(string text) {
			return Regex.Replace(text, @"[\f\t\v\x85\p{Z}]", string.Empty);
		}

		/// <summary>
		/// 正規表現のマッチパターンを追加する.
		/// </summary>
		/// <param name="type">トークンの種類.</param>
		/// <param name="patternText">マッチ用の正規表現文字列.</param>
		/// <returns>追加されたパターン.</returns>
		public TokenPattern<TokenType> AddPattern(TokenType type, string patternText) {
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
		/// マッチに失敗した場合, "Parse Error" を例外として投げる (Exception クラス).
		/// 処理が長時間に及ぶ場合, "Timeout" を例外として投げる (Exception クラス).
		/// タイムアウト時間は, Timeout 変数で変更する.
		/// TODO: 独自の Parse Error を作るか検討する.
		/// </summary>
		/// <param name="text">処理対象の文字列.</param>
		/// <returns>トークンのリスト.</returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public TokenList<TokenType> Tokenize(string text) {
			if (text == null || text == string.Empty) {
				return null;
			}

			// 戻り値
			TokenList<TokenType> tokens = new TokenList<TokenType>();

			// メンバ変数の初期化
			index = 0;
			lineIndex = 0;
			lineNumber = 1;
			lineIndexList.Clear();
			lineIndexList.Add(0);

			// Sleep() を定期的に入れる用
			int loopCount = 0;

			// タイムアウトのチェック
			if (Timeout > 0) {
				timer.Interval = Timeout;
				timer.Start();
			}

			// テキストの終わりまでマッチを試す
			while (index < text.Length) {
				// 改行チェック
				bool isMatchNewLine = false;
				Match match = newLineRegex.Match(text, index, 2);
				if (match.Success && match.Index == index) {
					isMatchNewLine = true;
					lineIndexList.Add(index + match.Length);
				}

				// 登録されたパターンでマッチを試す
				TokenMatch<TokenType> tokenMatch = TryMatchToken(text);

				// マッチしなかった時は, 例外を投げる
				if (tokenMatch == null) {
					ThrowParseError(text, lineNumber, lineIndex);
				}

				// マッチした場合

				// インデックスの位置を動かしておく
				int matchLength = tokenMatch.Match.Length;
				index += matchLength;
				lineIndex += matchLength;

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

				// ループの開始位置で改行文字が見つかった時
				if (isMatchNewLine) {
					lineIndex = 0;
					lineNumber++;
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

			// タイムアウトチェック用のタイマーを停止する
			timer.Stop();

			return tokens;
		}

		/// <summary>
		/// トークンのマッチを試す.
		/// マッチしなかった場合, null を返す.
		/// </summary>
		/// <param name="text">処理対象の文字列.</param>
		/// <returns></returns>
		private TokenMatch<TokenType> TryMatchToken(string text) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// すべてのパターンを試す
			foreach (TokenPattern<TokenType> pattern in patterns) {
				match = pattern.Regex.Match(text, index);
				// マッチに失敗した場合
				if (!match.Success) {
					continue;
				}
				// 先頭位置とマッチしなかった場合
				if (match.Index != index) {
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
			tokenMatch.Index = index;
			tokenMatch.LineNumber = lineNumber;
			tokenMatch.LineIndex = lineIndex;
			tokenMatch.Match = match;
			//tokenMatch.Text = match.Value;

			return tokenMatch;
		}

		/// <summary>
		/// タイムアウトチェック用のタイマーを作成する.
		/// </summary>
		/// <returns>タイマーオブジェクト.</returns>
		private Timer CreateTimeoutTimer() {
			Timer timer = new Timer();
			timer.Elapsed += (object sender, ElapsedEventArgs e) => {
				ThrowTimeout();
			};
			return timer;
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
		/// <param name="text">処理対象の文字列.</param>
		/// <param name="lineNumber">行番号.</param>
		/// <returns>指定された行の文字列.</returns>
		private string GetLine(string text, int lineNumber) {
			lineNumber--;
			if (lineNumber < 0 || lineNumber >= lineIndexList.Count) {
				return string.Empty;
			}
			int index = lineIndexList[lineNumber];
			Match match = newLineRegex.Match(text, index);
			if (match.Success) {
				string lineText = text.Substring(index, match.Index - index);
				//lineText.Replace("\t", "    ");
				return lineText;
			}
			return string.Empty;
		}

		/// <summary>
		/// ParseError 例外を発生させる.
		/// </summary>
		/// <param name="text">処理対象の文字列.</param>
		/// <param name="lineNumber">行番号.</param>
		/// <param name="lineIndex">行の位置.</param>
		private void ThrowParseError(string text, int lineNumber, int lineIndex) {
			string lineText = GetLine(text, lineNumber);
			throw new Exception(string.Format(
				"Parse Error (Line:{0}, Index:{1}){2}{3}",
				lineNumber,
				lineIndex,
				Environment.NewLine,
				lineText
			));
		}

		/// <summary>
		/// Timeout 例外を発生させる.
		/// </summary>
		private void ThrowTimeout() {
			throw new TimeoutException(string.Format(
				"Timeout: Tokenizer.Tokenize() (Timeout:{0}ms)",
				Timeout
			));
		}
	}
}
