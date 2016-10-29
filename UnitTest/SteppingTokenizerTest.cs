using Hikipuro.Text.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace UnitTest {
	[TestClass]
	public class SteppingTokenizerTest {
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

		[TestMethod, TestCategory("SteppingTokenizer")]
		public void Create() {
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

			// 作成する
			SteppingTokenizer<TokenType> stepping = tokenizer.CreateSteppingTokenizer(text);
			Assert.AreNotEqual(null, stepping);
			Assert.AreEqual(null, stepping.Current, "incorrect token");
		}

		[TestMethod, TestCategory("SteppingTokenizer")]
		public void Next() {
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

			// 作成する
			SteppingTokenizer<TokenType> stepping = tokenizer.CreateSteppingTokenizer(text);
			Assert.AreNotEqual(null, stepping);
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			// while と HasNext で要素を巡回
			int lineNumber = 1;
			TokenList<TokenType> tokens = new TokenList<TokenType>();
			while (stepping.HasNext) {
				Token<TokenType> token = stepping.Next();
				Assert.AreNotEqual(null, stepping.Current, "incorrect token");
				switch (token.Type) {
				case TokenType.NewLine:
					Assert.AreEqual(lineNumber, token.LineNumber, "incorrect token line number");
					lineNumber++;
					break;
				case TokenType.Space:
					break;
				default:
					tokens.Add(token);
					break;
				}
			}
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");
			Assert.AreEqual(65, tokens.Count, "incorrect token count");
		}

		[TestMethod, TestCategory("SteppingTokenizer")]
		public void NextWithTokenType() {
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

			// 作成する
			SteppingTokenizer<TokenType> stepping = tokenizer.CreateSteppingTokenizer(text);
			Assert.AreNotEqual(null, stepping);
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			// パラメータ付き Next() の実行
			Token<TokenType> token = null;
			token = stepping.Next(TokenType.Number);
			Assert.AreEqual(null, token, "incorrect token type");
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next(TokenType.String);
			Assert.AreEqual(null, token, "incorrect token type");
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next();
			Assert.AreEqual(TokenType.OpenBrace, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next(TokenType.Colon);
			Assert.AreEqual(null, token, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next();
			Assert.AreEqual(TokenType.NewLine, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			int count = 0;
			while (stepping.HasNext) {
				token = stepping.Next();
				switch (token.Type) {
				case TokenType.NewLine:
				case TokenType.Space:
					break;
				default:
					count++;
					break;
				}
			}
			Assert.AreEqual(64, count, "incorrect token count");

			count = 0;
			while (stepping.HasNext) {
				token = stepping.Next();
				count++;
			}
			Assert.AreEqual(0, count, "incorrect count");
		}

		[TestMethod, TestCategory("SteppingTokenizer")]
		public void Back() {
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

			// 作成する
			SteppingTokenizer<TokenType> stepping = tokenizer.CreateSteppingTokenizer(text);
			Assert.AreNotEqual(null, stepping);
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			// パラメータ付き Next() の実行
			Token<TokenType> token = null;
			token = stepping.Back();
			Assert.AreEqual(null, token, "incorrect token type");
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next(TokenType.OpenBrace);
			Assert.AreEqual(TokenType.OpenBrace, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Back();
			Assert.AreEqual(null, token, "incorrect token type");
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next(TokenType.OpenBrace);
			Assert.AreEqual(TokenType.OpenBrace, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next(TokenType.NewLine);
			Assert.AreEqual(TokenType.NewLine, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next();
			Assert.AreEqual(TokenType.Space, token.Type, "incorrect token type");

			token = stepping.Back();
			Assert.AreEqual(TokenType.NewLine, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Back();
			Assert.AreEqual(TokenType.OpenBrace, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Back();
			Assert.AreEqual(null, token, "incorrect token type");
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next(TokenType.OpenBrace);
			Assert.AreEqual(TokenType.OpenBrace, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			int count = 0;
			while (stepping.HasNext) {
				token = stepping.Next();
				switch (token.Type) {
				case TokenType.NewLine:
				case TokenType.Space:
					break;
				default:
					count++;
					break;
				}
			}
			Assert.AreEqual(TokenType.NewLine, token.Type, "incorrect token type");
			Assert.AreEqual(64, count, "incorrect token count");
			Assert.AreEqual(false, stepping.HasNext);
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Back();
			Assert.AreEqual(TokenType.CloseBrace, token.Type, "incorrect token type");
			Assert.AreEqual(true, stepping.HasNext);
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Back();
			Assert.AreEqual(TokenType.NewLine, token.Type, "incorrect token type");
			Assert.AreEqual(true, stepping.HasNext);
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next();
			Assert.AreEqual(TokenType.CloseBrace, token.Type, "incorrect token type");
			Assert.AreEqual(true, stepping.HasNext);
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			count = 0;
			while (stepping.HasNext) {
				token = stepping.Next();
				count++;
			}
			Assert.AreEqual(1, count, "incorrect token count");
			Assert.AreEqual(false, stepping.HasNext);
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");
		}

		[TestMethod, TestCategory("SteppingTokenizer")]
		public void IsMatchNext() {
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

			// 作成する
			SteppingTokenizer<TokenType> stepping = tokenizer.CreateSteppingTokenizer(text);
			Assert.AreNotEqual(null, stepping);
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			// パラメータ付き Next() の実行
			Token<TokenType> token = null;
			bool result = false;

			result = stepping.IsMatchNext(TokenType.Number);
			Assert.AreEqual(false, result, "incorrect match");
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			result = stepping.IsMatchNext(TokenType.OpenBrace);
			Assert.AreEqual(true, result, "incorrect match");

			token = stepping.Next(TokenType.Number);
			Assert.AreEqual(null, token, "incorrect token type");
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			token = stepping.Next();
			Assert.AreEqual(TokenType.OpenBrace, token.Type, "incorrect token type");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			result = stepping.IsMatchNext(TokenType.OpenBrace);
			Assert.AreEqual(false, result, "incorrect match");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			result = stepping.IsMatchNext(TokenType.NewLine);
			Assert.AreEqual(true, result, "incorrect match");

			token = stepping.Next();
			Assert.AreEqual(TokenType.NewLine, token.Type, "incorrect token type");

			token = stepping.Back();
			Assert.AreEqual(TokenType.OpenBrace, token.Type, "incorrect token type");

			result = stepping.IsMatchNext(TokenType.NewLine);
			Assert.AreEqual(true, result, "incorrect match");
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");

			int count = 0;
			while (stepping.HasNext) {
				token = stepping.Next();
				switch (token.Type) {
				case TokenType.NewLine:
				case TokenType.Space:
					break;
				default:
					count++;
					break;
				}
			}
			Assert.AreEqual(64, count, "incorrect token count");

			result = stepping.IsMatchNext(TokenType.NewLine);
			Assert.AreEqual(false, result, "incorrect match");
			result = stepping.IsMatchNext(TokenType.Space);
			Assert.AreEqual(false, result, "incorrect match");
		}

		[TestMethod, TestCategory("SteppingTokenizer")]
		public void Current() {
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

			// 作成する
			SteppingTokenizer<TokenType> stepping = tokenizer.CreateSteppingTokenizer(text);
			Assert.AreNotEqual(null, stepping);
			Assert.AreEqual(null, stepping.Current, "incorrect token");

			Token<TokenType> token = stepping.Next();
			Assert.AreNotEqual(null, stepping.Current, "incorrect token");
			Assert.AreEqual(TokenType.OpenBrace, stepping.Current.Type, "incorrect token");

			stepping.Reset();

			// while と HasNext で要素を巡回
			TokenList<TokenType> tokens = new TokenList<TokenType>();
			TokenList<TokenType> currentList = new TokenList<TokenType>();
			while (stepping.HasNext) {
				token = stepping.Next();
				switch (token.Type) {
				case TokenType.NewLine:
				case TokenType.Space:
					break;
				default:
					tokens.Add(token);
					currentList.Add(stepping.Current);
					break;
				}
				Assert.AreEqual(token.Text, stepping.Current.Text, "incorrect token");
			}
			Assert.AreEqual(TokenType.NewLine, stepping.Current.Type, "incorrect token");

			Assert.AreEqual(65, tokens.Count);
			Assert.AreEqual(65, currentList.Count);
			for (int i = 0; i < tokens.Count; i++) {
				Assert.AreEqual(tokens[i].Type, currentList[i].Type, "incorrect token");
			}

			stepping.Reset();
			Assert.AreEqual(null, stepping.Current, "incorrect token");
		}

		[TestMethod, TestCategory("SteppingTokenizer")]
		public void Reset() {
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

			// 作成する
			SteppingTokenizer<TokenType> stepping = tokenizer.CreateSteppingTokenizer(text);
			Assert.AreNotEqual(null, stepping);

			// while と HasNext で要素を巡回
			List<int> indices = new List<int>();
			List<int> lineIndices = new List<int>();
			List<int> lineNumbers = new List<int>();
			TokenList<TokenType> tokens = new TokenList<TokenType>();
			while (stepping.HasNext) {
				Token<TokenType> token = stepping.Next();
				switch (token.Type) {
				case TokenType.NewLine:
				case TokenType.Space:
					break;
				default:
					tokens.Add(token);
					break;
				}
				indices.Add(token.Index);
				lineIndices.Add(token.LineIndex);
				lineNumbers.Add(token.LineNumber);
			}
			Assert.AreEqual(65, tokens.Count, "incorrect token count");

			// Reset した後に再度実行する
			int count = 0;
			tokens.Clear();
			stepping.Reset();
			while (stepping.HasNext) {
				Token<TokenType> token = stepping.Next();
				switch (token.Type) {
				case TokenType.NewLine:
				case TokenType.Space:
					break;
				default:
					tokens.Add(token);
					break;
				}
				Assert.AreEqual(indices[count], token.Index, "incorrect token index");
				Assert.AreEqual(lineIndices[count], token.LineIndex, "incorrect token line index");
				Assert.AreEqual(lineNumbers[count], token.LineNumber, "incorrect token line number");
				count++;
			}
			Assert.AreEqual(65, tokens.Count, "incorrect token count");
		}
	}
}
