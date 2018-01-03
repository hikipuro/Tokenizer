using System;
using System.Collections.Generic;

namespace Hikipuro.Text.Tokenizer {
	/// <summary>
	/// Token list.
	/// This object has all tokenized text list.
	/// </summary>
	/// <typeparam name="TokenType">Token type.</typeparam>
	public class TokenList<TokenType> : List<Token<TokenType>> where TokenType : struct {
		/// <summary>
		/// Add Token object to list.
		/// </summary>
		/// <param name="token">Token.</param>
		public new void Add(Token<TokenType> token) {
			if (token == null) {
				return;
			}
			token.TokenList = this;
			base.Add(token);
		}

		/// <summary>
		/// Add Token object to list, before convert TokenMatch object to Token object.
		/// </summary>
		/// <param name="tokenMatch">Token match object.</param>
		public void Add(TokenMatch<TokenType> tokenMatch) {
			if (tokenMatch == null) {
				return;
			}
			Token<TokenType> token = Token<TokenType>.FromTokenMatch(tokenMatch);
			token.TokenList = this;
			base.Add(token);
		}

		/// <summary>
		/// Remove Token object from list.
		/// </summary>
		/// <param name="token">トークン.</param>
		public new void Remove(Token<TokenType> token) {
			if (token == null) {
				return;
			}
			if (Contains(token) == false) {
				return;
			}
			token.TokenList = null;
			base.Remove(token);
		}

		/// <summary>
		/// Remove Token object from list.
		/// </summary>
		/// <param name="index">Index number of target Token.</param>
		public new void RemoveAt(int index) {
			Token<TokenType> token = this[index];
			Remove(token);
		}

		/// <summary>
		/// Get Token object from last index of list.
		/// </summary>
		/// <param name="index">Last index number of list.</param>
		/// <returns>Token.</returns>
		public Token<TokenType> Last(int index = 0) {
			if (Count <= 0) {
				return null;
			}
			int lastIndex = Count - 1;
			return this[lastIndex - index];
		}

		/// <summary>
		/// Get next token of specified in the argument.
		/// Rewind position when reached last item.
		/// Return null when TokenList doesn't have any items.
		/// </summary>
		/// <param name="token">Token.</param>
		/// <returns>Next token.</returns>
		public Token<TokenType> Next(Token<TokenType> token) {
			if (token == null || Count <= 0) {
				return null;
			}
			if (Contains(token) == false) {
				return null;
			}
			int index = IndexOf(token) + 1;
			if (index >= Count) {
				return null;
			}
			return this[index];
		}

		/// <summary>
		/// Get previous token of specified in the argument.
		/// Return last token when reached first item.
		/// Return null when TokenList doesn't have any items.
		/// </summary>
		/// <param name="token">Token.</param>
		/// <returns>Previous token.</returns>
		public Token<TokenType> Prev(Token<TokenType> token) {
			if (token == null || Count <= 0) {
				return null;
			}
			if (Contains(token) == false) {
				return null;
			}
			int index = IndexOf(token) - 1;
			if (index < 0) {
				return null;
			}
			return this[index];
		}

		/// <summary>
		/// Return distance in token count.
		/// This method compares Token A and B position.
		/// [Return] Same token: 0, Adjacent token: 1.
		/// TODO: optimize IndexOf() method.
		/// </summary>
		/// <param name="tokenA">Token A.</param>
		/// <param name="tokenB">Token B.</param>
		/// <returns>Token count.</returns>
		public int GetDistance(Token<TokenType> tokenA, Token<TokenType> tokenB) {
			int indexA = IndexOf(tokenA);
			int indexB = IndexOf(tokenB);
			return Math.Abs(indexA - indexB);
		}

		/// <summary>
		/// Swap two tokens position.
		/// TODO: optimize IndexOf() method.
		/// </summary>
		/// <param name="tokenA">Token A.</param>
		/// <param name="tokenB">Token B.</param>
		public void Swap(Token<TokenType> tokenA, Token<TokenType> tokenB) {
			int indexA = IndexOf(tokenA);
			int indexB = IndexOf(tokenB);
			Swap(indexA, indexB);
		}

		/// <summary>
		/// Swap two tokens position.
		/// </summary>
		/// <param name="indexA">Index of Token A.</param>
		/// <param name="indexB">Index of Token B.</param>
		public void Swap(int indexA, int indexB) {
			Token<TokenType> token = this[indexA];
			this[indexA] = this[indexB];
			this[indexB] = token;
		}
	}
}
