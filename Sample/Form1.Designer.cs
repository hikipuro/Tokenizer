namespace Tokenizer.Sample {
	partial class Form1 {
		/// <summary>
		/// 必要なデザイナー変数です。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 使用中のリソースをすべてクリーンアップします。
		/// </summary>
		/// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows フォーム デザイナーで生成されたコード

		/// <summary>
		/// デザイナー サポートに必要なメソッドです。このメソッドの内容を
		/// コード エディターで変更しないでください。
		/// </summary>
		private void InitializeComponent() {
			this.buttonLoadCsv = new System.Windows.Forms.Button();
			this.textBox = new System.Windows.Forms.TextBox();
			this.buttonLoadJson = new System.Windows.Forms.Button();
			this.textBoxTime = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// buttonLoadCsv
			// 
			this.buttonLoadCsv.Location = new System.Drawing.Point(12, 12);
			this.buttonLoadCsv.Name = "buttonLoadCsv";
			this.buttonLoadCsv.Size = new System.Drawing.Size(130, 23);
			this.buttonLoadCsv.TabIndex = 0;
			this.buttonLoadCsv.Text = "CSVファイルのロード";
			this.buttonLoadCsv.UseVisualStyleBackColor = true;
			this.buttonLoadCsv.Click += new System.EventHandler(this.buttonLoadCsv_Click);
			// 
			// textBox
			// 
			this.textBox.Location = new System.Drawing.Point(12, 41);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(654, 474);
			this.textBox.TabIndex = 1;
			this.textBox.WordWrap = false;
			// 
			// buttonLoadJson
			// 
			this.buttonLoadJson.Location = new System.Drawing.Point(148, 12);
			this.buttonLoadJson.Name = "buttonLoadJson";
			this.buttonLoadJson.Size = new System.Drawing.Size(130, 23);
			this.buttonLoadJson.TabIndex = 2;
			this.buttonLoadJson.Text = "JSONファイルのロード";
			this.buttonLoadJson.UseVisualStyleBackColor = true;
			this.buttonLoadJson.Click += new System.EventHandler(this.buttonLoadJson_Click);
			// 
			// textBoxTime
			// 
			this.textBoxTime.Location = new System.Drawing.Point(566, 16);
			this.textBoxTime.Name = "textBoxTime";
			this.textBoxTime.Size = new System.Drawing.Size(100, 19);
			this.textBoxTime.TabIndex = 3;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(678, 527);
			this.Controls.Add(this.textBoxTime);
			this.Controls.Add(this.buttonLoadJson);
			this.Controls.Add(this.textBox);
			this.Controls.Add(this.buttonLoadCsv);
			this.Name = "Form1";
			this.Text = "Form1";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button buttonLoadCsv;
		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Button buttonLoadJson;
		private System.Windows.Forms.TextBox textBoxTime;
	}
}

