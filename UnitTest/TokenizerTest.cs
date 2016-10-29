using Hikipuro.Text.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace UnitTest {
	[TestClass]
	public class TokenizerTest {
		/// <summary>
		/// JSON ファイルで使用するトークンの種類.
		/// </summary>
		public enum TokenType {
			NewLine,
			Comma,
			Colon,
			OpenBrace,
			CloseBrace,
			OpenBracket,
			CloseBracket,
			Null,
			True,
			False,
			Number,
			String,
			Space
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void Tokenize() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, ",");
			tokenizer.AddPattern(TokenType.Colon, ":");
			tokenizer.AddPattern(TokenType.OpenBrace, "{");
			tokenizer.AddPattern(TokenType.CloseBrace, "}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\]");
			tokenizer.AddPattern(TokenType.Null, "null");
			tokenizer.AddPattern(TokenType.True, "true");
			tokenizer.AddPattern(TokenType.False, "false");
			tokenizer.AddPattern(TokenType.Number, @"\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\s+");

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			Assert.AreEqual(123, tokens.Count, "incorrect token count");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void TokenizeWithPatternG() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			Assert.AreEqual(123, tokens.Count, "incorrect token count");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void TokenizeWithNoPattern() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンに分解する
			bool catched = false;
			try {
				TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			} catch (ParseException e) {
				catched = true;
				Assert.AreEqual(0, e.Index, "incorrect index");
				Assert.AreEqual(1, e.LineNumber, "incorrect line number");
				Assert.AreEqual(1, e.LineIndex, "incorrect line index");
			}

			Assert.AreEqual(true, catched, "no throw ParseException");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void TokenizeWithInvalidPattern() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.OpenBrace, "{");
			tokenizer.AddPattern(TokenType.Space, @"\s+");

			// トークンに分解する
			bool catched = false;
			try {
				TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			} catch (ParseException e) {
				catched = true;
				Assert.AreEqual(7, e.Index, "incorrect index");
				Assert.AreEqual(2, e.LineNumber, "incorrect line number");
				Assert.AreEqual(5, e.LineIndex, "incorrect line index");
			}

			Assert.AreEqual(true, catched, "no throw ParseException");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void BeforeAddTokenEvent() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// リストにトークンを追加する直前に発生するイベント
			// - e.Cancel = true; で追加しない
			int count = 0;
			TokenList<TokenType> tokenList = new TokenList<TokenType>();
			tokenizer.BeforeAddToken += (object sender, BeforeAddTokenEventArgs<TokenType> e) => {
				count++;
				tokenList.Add(e.TokenMatch);
				if (e.TokenMatch.Type == TokenType.NewLine) {
					e.Cancel = true;
					return;
				}
				if (e.TokenMatch.Type == TokenType.Space) {
					e.Cancel = true;
					return;
				}
			};

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			Assert.AreEqual(65, tokens.Count, "incorrect token count");
			Assert.AreEqual(123, count, "incorrect event count");
			Assert.AreEqual(123, tokenList.Count, "incorrect token count");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void TokenAddedEvent() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// リストにトークンを追加した直後に発生するイベント
			int lineNumber = 1;
			int count = 0;
			TokenList<TokenType> tokenList = new TokenList<TokenType>();
			tokenizer.TokenAdded += (object sender, TokenAddedEventArgs<TokenType> e) => {
				count++;
				tokenList.Add(e.Token);
				Assert.AreEqual(lineNumber, e.Token.LineNumber, "incorrect token line number");
				if (e.Token.Type == TokenType.NewLine) {
					lineNumber++;
				}
			};

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			Assert.AreEqual(123, count, "incorrect event count");
			Assert.AreEqual(123, tokens.Count, "incorrect token count");
			Assert.AreEqual(123, tokenList.Count, "incorrect token count");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void Regex() {
			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			bool catched = false;
			try {
				tokenizer.AddPattern(TokenType.NewLine, "[");
			} catch (ArgumentException) {
				catched = true;
			}
			Assert.AreEqual(true, catched);
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void AddPatternWithOption() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n", RegexOptions.None);
			tokenizer.AddPattern(TokenType.Comma, "\\G,", RegexOptions.Compiled);
			tokenizer.AddPattern(TokenType.Colon, "\\G:", RegexOptions.Singleline);
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{", RegexOptions.None);
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}", RegexOptions.None);
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[", RegexOptions.None);
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]", RegexOptions.None);
			tokenizer.AddPattern(TokenType.Null, "\\Gnull", RegexOptions.None);
			tokenizer.AddPattern(TokenType.True, "\\Gtrue", RegexOptions.None);
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			Assert.AreEqual(123, tokens.Count, "incorrect token count");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void RemovePattern() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// RemovePattern のテスト
			tokenizer.RemovePattern(TokenType.OpenBracket);

			// リストにトークンを追加した直後に発生するイベント
			TokenList<TokenType> tokenList = new TokenList<TokenType>();
			tokenizer.TokenAdded += (object sender, TokenAddedEventArgs<TokenType> e) => {
				tokenList.Add(e.Token);
			};

			// トークンに分解する
			TokenList<TokenType> tokens = null;
			bool catched = false;
			try {
				tokens = tokenizer.Tokenize(text);
			} catch (ParseException e) {
				catched = true;
				string lineText = @"						""GlossSeeAlso"": [""GML"", ""XML""]";
				Assert.AreEqual(lineText, e.LineText);
				Assert.AreEqual(15, e.LineNumber, "incorrect line number");
				Assert.AreEqual(23, e.LineIndex, "incorrect line index");
				Assert.AreEqual(483, e.Index, "incorrect index");
			}
			Assert.AreEqual(true, catched, "no throw ParseException");
			Assert.AreEqual(null, tokens, "no throw ParseException");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void HasPatternType() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			//tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// RemovePattern のテスト
			tokenizer.RemovePattern(TokenType.OpenBracket);

			// HasPatternType のテスト
			bool result = false;
			result = tokenizer.HasPatternType(TokenType.NewLine);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.Comma);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.Colon);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.OpenBrace);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.CloseBrace);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.OpenBracket);
			Assert.AreEqual(false, result);
			result = tokenizer.HasPatternType(TokenType.CloseBracket);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.Null);
			Assert.AreEqual(false, result);
			result = tokenizer.HasPatternType(TokenType.True);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.False);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.Number);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.String);
			Assert.AreEqual(true, result);
			result = tokenizer.HasPatternType(TokenType.Space);
			Assert.AreEqual(true, result);

			// リストにトークンを追加した直後に発生するイベント
			TokenList<TokenType> tokenList = new TokenList<TokenType>();
			tokenizer.TokenAdded += (object sender, TokenAddedEventArgs<TokenType> e) => {
				tokenList.Add(e.Token);
			};

			// トークンに分解する
			TokenList<TokenType> tokens = null;
			bool catched = false;
			try {
				tokens = tokenizer.Tokenize(text);
			} catch (ParseException e) {
				catched = true;
				Assert.AreEqual(15, e.LineNumber, "incorrect line number");
				Assert.AreEqual(23, e.LineIndex, "incorrect line index");
				Assert.AreEqual(483, e.Index, "incorrect index");
			}
			Assert.AreEqual(true, catched, "no throw ParseException");
			Assert.AreEqual(null, tokens, "no throw ParseException");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void SleepWait() {
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			// SleepWait のテスト
			tokenizer.SleepWait = 3;

			// リストにトークンを追加した直後に発生するイベント
			TokenList<TokenType> tokenList = new TokenList<TokenType>();
			tokenizer.TokenAdded += (object sender, TokenAddedEventArgs<TokenType> e) => {
				tokenList.Add(e.Token);
			};

			// トークンに分解する
			TokenList<TokenType> tokens = tokenizer.Tokenize(text);
			Assert.AreEqual(123, tokens.Count, "incorrect token count");
			Assert.AreEqual(123, tokenList.Count, "incorrect token count");
		}

		[TestMethod, TestCategory("Tokenizer")]
		public void Concurrency() {
			const int threadCount = 10;
			string text = TestUtility.ReadTextFile("JSON/Test1.json");

			// Tokenizer オブジェクトを準備する
			Tokenizer<TokenType> tokenizer = new Tokenizer<TokenType>();

			// トークンの分解規則を追加する
			tokenizer.AddPattern(TokenType.NewLine, "\\G\r\n|\r|\n");
			tokenizer.AddPattern(TokenType.Comma, "\\G,");
			tokenizer.AddPattern(TokenType.Colon, "\\G:");
			tokenizer.AddPattern(TokenType.OpenBrace, "\\G{");
			tokenizer.AddPattern(TokenType.CloseBrace, "\\G}");
			tokenizer.AddPattern(TokenType.OpenBracket, @"\G\[");
			tokenizer.AddPattern(TokenType.CloseBracket, @"\G\]");
			tokenizer.AddPattern(TokenType.Null, "\\Gnull");
			tokenizer.AddPattern(TokenType.True, "\\Gtrue");
			tokenizer.AddPattern(TokenType.False, "\\Gfalse");
			tokenizer.AddPattern(TokenType.Number, @"\G\d+[.]?\d*");
			tokenizer.AddPattern(TokenType.String, @"\G""((?<=\\)""|[^\r\n""])*""");
			tokenizer.AddPattern(TokenType.Space, @"\G\s+");

			TokenList<TokenType> correctTokens = tokenizer.Tokenize(text);
			Assert.AreEqual(123, correctTokens.Count, "incorrect token count");

			List<Thread> threads = new List<Thread>();
			List<TokenList<TokenType>> tokensList = new List<TokenList<TokenType>>();
			for (int i = 0; i < threadCount; i++) {
				// スレッドを作成する
				Thread thread = new Thread(new ThreadStart(() => {
					//Thread.Sleep(100);
					TokenList<TokenType> tokens = tokenizer.Tokenize(text);
					tokensList.Add(tokens);
				}));
				threads.Add(thread);
			}

			// 実行する
			foreach (Thread thread in threads) {
				thread.Start();
			}

			// 全てのスレッドの終了を待つ
			foreach (Thread thread in threads) {
				thread.Join();
			}
			Assert.AreEqual(threadCount, tokensList.Count, "incorrect thread count");
			foreach (TokenList<TokenType> tokens in tokensList) {
				Assert.AreEqual(123, tokens.Count, "incorrect token count");
			}

			for (int i = 0; i < correctTokens.Count; i++) {
				Token<TokenType> correctToken = correctTokens[i];

				List<Token<TokenType>> tokenList = new List<Token<TokenType>>();
				for (int n = 0; n < threadCount; n++) {
					Token<TokenType> token = tokensList[n][i];
					Assert.AreEqual(correctToken.Text, token.Text, "incorrect token text");
				}
			}
		}
	}
}
