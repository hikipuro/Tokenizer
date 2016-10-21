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
		/// イベントハンドラ内で true を返すと追加, false を返すと追加しない.
		/// </summary>
		public event BeforeAddTokenEventHandler<TokenType> BeforeAddToken;

		/// <summary>
		/// リストにトークンが追加された直後に呼ばれるイベント.
		/// </summary>
		public event TokenAddedEventHandler<TokenType> TokenAdded;

		/// <summary>
		/// タイムアウト時間 (ミリ秒).
		/// デフォルト値: 10 秒.
		/// 処理が長時間に及ぶ場合はタイムアウト例外を発生させる.
		/// 0 以下の値を入れるとタイムアウトしないようになる.
		/// </summary>
		public int timeout = 10000;

		/// <summary>
		/// System.Threading.Sleep() を定期的に入れるための値.
		/// sleepWait で指定された回数分ループするごとに 1 回スリープする.
		/// 0 以下の値を入れるとスリープしないようになる.
		/// </summary>
		public int sleepWait = 1000;

		/// <summary>
		/// トークンのマッチ用パターン.
		/// AddPattern() で追加する.
		/// </summary>
		List<TokenPattern<TokenType>> patterns;

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
		/// コンストラクタ.
		/// </summary>
		public Tokenizer() {
			patterns = new List<TokenPattern<TokenType>>();
			newLineRegex = new Regex("\r\n|\r|\n", RegexOptions.Compiled);
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
			foreach (Delegate d in BeforeAddToken.GetInvocationList()) {
				BeforeAddToken -= (BeforeAddTokenEventHandler<TokenType>)d;
			}
			foreach (Delegate d in TokenAdded.GetInvocationList()) {
				TokenAdded -= (TokenAddedEventHandler<TokenType>)d;
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
				if (pattern.type.Equals(type)) {
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 登録されたパターンを使ってトークンに分割する.
		/// マッチに失敗した場合, "Parse Error" を例外として投げる (Exception クラス).
		/// 処理が長時間に及ぶ場合, "Timeout" を例外として投げる (Exception クラス).
		/// タイムアウト時間は, timeout 変数で変更する.
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

			// Sleep() を定期的に入れる用
			int loopCount = 0;

			// タイムアウトのチェック
			if (timeout > 0) {
				timer.Interval = timeout;
				timer.Start();
			}

			// テキストの終わりまでマッチを試す
			while (index < text.Length) {
				// 改行チェック
				bool matchNewLine = MatchNewLine(text, index);

				// 登録されたパターンでマッチを試す
				TokenMatch<TokenType> tokenMatch = TryMatchToken(text);

				// マッチしなかった時は, 例外を投げる
				if (tokenMatch == null) {
					ThrowParseError(lineNumber, lineIndex);
				}

				// マッチした場合
				if (BeforeAddToken != null) {
					// 追加前イベントが登録されている場合は実行する
					// false が返ってきた場合はトークンリストに追加しない
					if (BeforeAddToken(tokenMatch) == true) {
						AddToken(tokens, tokenMatch);
					}
				} else {
					// トークンをリストに追加する
					AddToken(tokens, tokenMatch);
				}

				// ループの開始位置で改行文字が見つかった時
				if (matchNewLine) {
					lineIndex = 0;
					lineNumber++;
				}

				// Sleep() を定期的に入れる
				if (sleepWait > 0) {
					loopCount++;
					if (loopCount > sleepWait) {
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
		/// <param name="text"></param>
		/// <returns></returns>
		private TokenMatch<TokenType> TryMatchToken(string text) {
			TokenPattern<TokenType> tokenPattern = null;
			Match match = null;

			// すべてのパターンを試す
			foreach (TokenPattern<TokenType> pattern in patterns) {
				match = pattern.regex.Match(text, index);
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
			TokenMatch<TokenType> tokenMatch = new TokenMatch<TokenType>();
			tokenMatch.type = tokenPattern.type;
			tokenMatch.index = index;
			tokenMatch.lineNumber = lineNumber;
			tokenMatch.lineIndex = lineIndex;
			tokenMatch.match = match;
			tokenMatch.text = match.Value;

			// インデックスの位置を動かしておく
			index += match.Length;
			lineIndex += match.Length;

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
		/// 指定した位置の先頭が, 改行文字とマッチするかチェックする.
		/// </summary>
		/// <param name="text">チェックする文字列.</param>
		/// <param name="index">チェックする場所.</param>
		/// <returns>true: マッチした, false: マッチしなかった.</returns>
		private bool MatchNewLine(string text, int index) {
			Match match = newLineRegex.Match(text, index);
			if (!match.Success) {
				return false;
			}
			return match.Index == index;
		}

		/// <summary>
		/// リストにトークンを追加する.
		/// </summary>
		/// <param name="tokens">追加先のリスト.</param>
		/// <param name="tokenMatch">追加するマッチオブジェクト.</param>
		private void AddToken(TokenList<TokenType> tokens, TokenMatch<TokenType> tokenMatch) {
			tokens.Add(tokenMatch);

			// 追加後イベントを実行する
			if (TokenAdded != null) {
				TokenAdded(tokens, tokens[tokens.Count - 1]);
			}
		}

		/// <summary>
		/// ParseError 例外を発生させる.
		/// </summary>
		/// <param name="lineNumber"></param>
		/// <param name="lineIndex"></param>
		private void ThrowParseError(int lineNumber, int lineIndex) {
			throw new Exception(string.Format(
				"Parse Error (Line:{0}, Index:{1})",
				lineNumber,
				lineIndex
			));
		}

		/// <summary>
		/// Timeout 例外を発生させる.
		/// </summary>
		private void ThrowTimeout() {
			throw new TimeoutException("Timeout: Tokenizer.Tokenize()");
		}
	}
}
