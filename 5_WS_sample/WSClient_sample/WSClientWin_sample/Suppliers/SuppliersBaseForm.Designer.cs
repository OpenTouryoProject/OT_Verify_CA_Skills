namespace WSClientWin_sample.Suppliers
{
    partial class SuppliersBaseForm
    {
        /// <summary>必要なデザイナ変数です。</summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>使用中のリソースをすべてクリーンアップします。</summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.btnMain5 = new System.Windows.Forms.Button();
            this.btnMain4 = new System.Windows.Forms.Button();
            this.btnMain3 = new System.Windows.Forms.Button();
            this.btnMain2 = new System.Windows.Forms.Button();
            this.btnMain1 = new System.Windows.Forms.Button();
            this.pnlBody = new System.Windows.Forms.Panel();
            this.pnlFooter.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlFooter
            //
            this.pnlFooter.Controls.Add(this.btnMain5);
            this.pnlFooter.Controls.Add(this.btnMain4);
            this.pnlFooter.Controls.Add(this.btnMain3);
            this.pnlFooter.Controls.Add(this.btnMain2);
            this.pnlFooter.Controls.Add(this.btnMain1);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 420);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(884, 50);
            this.pnlFooter.TabIndex = 0;
            //
            // btnMain1
            //
            this.btnMain1.Location = new System.Drawing.Point(12, 10);
            this.btnMain1.Name = "btnMain1";
            this.btnMain1.Size = new System.Drawing.Size(150, 30);
            this.btnMain1.TabIndex = 0;
            this.btnMain1.Text = "－";
            this.btnMain1.UseVisualStyleBackColor = true;
            //
            // btnMain2
            //
            this.btnMain2.Location = new System.Drawing.Point(168, 10);
            this.btnMain2.Name = "btnMain2";
            this.btnMain2.Size = new System.Drawing.Size(150, 30);
            this.btnMain2.TabIndex = 1;
            this.btnMain2.Text = "－";
            this.btnMain2.UseVisualStyleBackColor = true;
            //
            // btnMain3
            //
            this.btnMain3.Location = new System.Drawing.Point(324, 10);
            this.btnMain3.Name = "btnMain3";
            this.btnMain3.Size = new System.Drawing.Size(150, 30);
            this.btnMain3.TabIndex = 2;
            this.btnMain3.Text = "－";
            this.btnMain3.UseVisualStyleBackColor = true;
            //
            // btnMain4
            //
            this.btnMain4.Location = new System.Drawing.Point(480, 10);
            this.btnMain4.Name = "btnMain4";
            this.btnMain4.Size = new System.Drawing.Size(150, 30);
            this.btnMain4.TabIndex = 3;
            this.btnMain4.Text = "－";
            this.btnMain4.UseVisualStyleBackColor = true;
            //
            // btnMain5
            //
            this.btnMain5.Location = new System.Drawing.Point(636, 10);
            this.btnMain5.Name = "btnMain5";
            this.btnMain5.Size = new System.Drawing.Size(150, 30);
            this.btnMain5.TabIndex = 4;
            this.btnMain5.Text = "－";
            this.btnMain5.UseVisualStyleBackColor = true;
            //
            // pnlBody
            //
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Location = new System.Drawing.Point(0, 0);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(884, 420);
            this.pnlBody.TabIndex = 1;
            //
            // SuppliersBaseForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 470);
            this.Controls.Add(this.pnlBody);
            this.Controls.Add(this.pnlFooter);
            this.Name = "SuppliersBaseForm";
            this.Text = "業務画面";
            this.pnlFooter.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>フッタ パネル</summary>
        protected System.Windows.Forms.Panel pnlFooter;
        /// <summary>本体パネル（各画面のコントロールを載せる）</summary>
        protected System.Windows.Forms.Panel pnlBody;
        /// <summary>メイン ボタン1</summary>
        protected System.Windows.Forms.Button btnMain1;
        /// <summary>メイン ボタン2</summary>
        protected System.Windows.Forms.Button btnMain2;
        /// <summary>メイン ボタン3</summary>
        protected System.Windows.Forms.Button btnMain3;
        /// <summary>メイン ボタン4</summary>
        protected System.Windows.Forms.Button btnMain4;
        /// <summary>メイン ボタン5</summary>
        protected System.Windows.Forms.Button btnMain5;
    }
}
