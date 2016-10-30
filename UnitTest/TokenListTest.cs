using Hikipuro.Text.Tokenizer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTest {
	[TestClass]
	public class TokenListTest {
		[TestMethod, TestCategory("TokenList")]
		public void Add() {
			TokenList<int> tokens = new TokenList<int>();
			Assert.AreEqual(0, tokens.Count);

			Token<int> token1 = new Token<int>("test1");
			token1.Index = 0;
			Assert.IsNull(token1.TokenList);
			tokens.Add(token1);
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual(tokens, token1.TokenList);

			Token<int> token2 = new Token<int>("test-2");
			token2.Index = token1.Length;
			Assert.IsNull(token2.TokenList);
			tokens.Add(token2);
			Assert.AreEqual(2, tokens.Count);
			Assert.AreEqual(tokens, token2.TokenList);

			tokens.Add((Token<int>)null);
			Assert.AreEqual(2, tokens.Count);

			Assert.AreEqual(token1, tokens[0]);
			Assert.AreEqual(token2, tokens[1]);

			bool catched = false;
			try {
				Token<int> token = tokens[2];
			} catch (ArgumentOutOfRangeException) {
				catched = true;
			}
			Assert.IsTrue(catched);
		}

		[TestMethod, TestCategory("TokenList")]
		public void Remove() {
			TokenList<int> tokens = new TokenList<int>();
			Assert.AreEqual(0, tokens.Count);

			Token<int> token1 = new Token<int>("test1");
			token1.Index = 0;
			tokens.Add(token1);
			Assert.AreEqual(tokens, token1.TokenList);

			Token<int> token2 = new Token<int>("test-2");
			token2.Index = token1.Length;
			tokens.Add(token2);
			Assert.AreEqual(tokens, token2.TokenList);
			Assert.AreEqual(2, tokens.Count);

			tokens.Remove(token1);
			Assert.AreEqual(1, tokens.Count);
			Assert.IsNull(token1.TokenList);
			Assert.AreEqual(tokens, token2.TokenList);

			tokens.Remove(token1);
			Assert.AreEqual(1, tokens.Count);
			Assert.AreEqual(token2, tokens[0]);

			tokens.Remove(null);
			Assert.AreEqual(1, tokens.Count);

			tokens.Remove(token2);
			Assert.AreEqual(0, tokens.Count);
			Assert.IsNull(token2.TokenList);

			bool catched = false;
			try {
				Token<int> token = tokens[0];
			} catch (ArgumentOutOfRangeException) {
				catched = true;
			}
			Assert.IsTrue(catched);
		}

		[TestMethod, TestCategory("TokenList")]
		public void RemoveAt() {
			TokenList<int> tokens = new TokenList<int>();
			Assert.AreEqual(0, tokens.Count);

			Token<int> token1 = new Token<int>("test1");
			token1.Index = 0;
			tokens.Add(token1);

			Token<int> token2 = new Token<int>("test-2");
			token2.Index = token1.Length;
			tokens.Add(token2);

			tokens.RemoveAt(0);
			Assert.AreEqual(1, tokens.Count);
			Assert.IsNull(token1.TokenList);
			Assert.AreEqual(token2, tokens[0]);

			tokens.RemoveAt(0);
			Assert.AreEqual(0, tokens.Count);
			Assert.IsNull(token2.TokenList);

			bool catched = false;
			try {
				Token<int> token = tokens[0];
			} catch (ArgumentOutOfRangeException) {
				catched = true;
			}
			Assert.IsTrue(catched);
		}

		[TestMethod, TestCategory("TokenList")]
		public void Last() {
			TokenList<int> tokens = new TokenList<int>();
			Assert.IsNull(tokens.Last());

			Token<int> token1 = new Token<int>("test1");
			token1.Index = 0;
			tokens.Add(token1);
			Assert.AreEqual(token1, tokens.Last());

			Token<int> token2 = new Token<int>("test-2");
			token2.Index = token1.Length;
			tokens.Add(token2);

			Assert.AreEqual(token2, tokens.Last());
			Assert.AreEqual(token2, tokens.Last(0));
			Assert.AreEqual(token1, tokens.Last(1));

			bool catched = false;
			try {
				Token<int> token = tokens.Last(2);
			} catch (ArgumentOutOfRangeException) {
				catched = true;
			}
			Assert.IsTrue(catched);

			catched = false;
			try {
				Token<int> token = tokens.Last(-1);
			} catch (ArgumentOutOfRangeException) {
				catched = true;
			}
			Assert.IsTrue(catched);

			tokens.Add((Token<int>)null);
			Assert.AreEqual(token2, tokens.Last());
		}

		[TestMethod, TestCategory("TokenList")]
		public void Next() {
			TokenList<int> tokens = new TokenList<int>();
			Assert.IsNull(tokens.Next(null));

			Token<int> token1 = new Token<int>("test1");
			token1.Index = 0;
			tokens.Add(token1);

			Token<int> token2 = new Token<int>("test-2");
			token2.Index = token1.Length;
			tokens.Add(token2);

			Assert.AreEqual(token2, tokens.Next(token1));
			Assert.IsNull(tokens.Next(token2));
			Assert.IsNull(tokens.Next(null));

			Token<int> token3 = new Token<int>("#3");
			Assert.IsNull(tokens.Next(token3));
		}

		[TestMethod, TestCategory("TokenList")]
		public void Prev() {
			TokenList<int> tokens = new TokenList<int>();
			Assert.IsNull(tokens.Prev(null));

			Token<int> token1 = new Token<int>("test1");
			token1.Index = 0;
			tokens.Add(token1);

			Token<int> token2 = new Token<int>("test-2");
			token2.Index = token1.Length;
			tokens.Add(token2);

			Assert.AreEqual(token1, tokens.Prev(token2));
			Assert.IsNull(tokens.Prev(token1));
			Assert.IsNull(tokens.Prev(null));

			Token<int> token3 = new Token<int>("#3");
			Assert.IsNull(tokens.Prev(token3));
		}
	}
}
