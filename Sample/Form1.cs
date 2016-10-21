using Hikipuro.Text;
using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using static Tokenizer.Sample.CSVTokenizer;

namespace Tokenizer.Sample {
	public partial class Form1 : Form {
		public Form1() {
			InitializeComponent();
		}

		/// <summary>
		/// CSV ファイルのロードボタンが押された時.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e) {
			// 
			// Sample/CSV/13TOKYO.CSV を読み込む
			// 
			// - このファイルは郵便局のサイトからお借りしました
			//   http://www.post.japanpost.jp/zipcode/dl/oogaki-zip.html
			// 
			StreamReader reader = new StreamReader(
				"Sample/CSV/13TOKYO.CSV",
				Encoding.GetEncoding("Shift_JIS")
			);
			string text = reader.ReadToEnd();
			reader.Close();
			
			// CSV ファイルを分解する
			TokenList<TokenType> tokens = CSVTokenizer.Tokenize(text);

			// 分解されたトークンを巡回する
			StringBuilder csv = new StringBuilder();
			foreach (Token<TokenType> token in tokens) {
				// 改行文字
				if (token.type == TokenType.NewLine) {
					csv.AppendLine();
					continue;
				}
				// カンマ
				if (token.type == TokenType.Comma) {
					csv.Append("\t");
					continue;
				}
				// 数値と文字列
				csv.Append(token.text);
			}

			// 画面に表示する
			textBox1.Text = csv.ToString();
		}
	}
}
