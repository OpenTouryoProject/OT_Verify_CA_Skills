namespace _2CSClientWin_sample.Suppliers
{
    partial class SuppliersFormA
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
            this.lblGuide = new System.Windows.Forms.Label();
            this.pnlBody.SuspendLayout();
            this.SuspendLayout();
            //
            // lblGuide
            //
            this.lblGuide.AutoSize = true;
            this.lblGuide.Location = new System.Drawing.Point(16, 20);
            this.lblGuide.Name = "lblGuide";
            this.lblGuide.Size = new System.Drawing.Size(0, 12);
            this.lblGuide.TabIndex = 0;
            this.lblGuide.Text = "［件数確認］で Suppliers のデータ件数を表示します。［画面遷移］で画面Ｂを開きます。";
            //
            // SuppliersFormA
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 470);
            this.Name = "SuppliersFormA";
            this.Text = "Suppliers 画面Ａ";
            this.pnlBody.Controls.Add(this.lblGuide);
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>案内ラベル</summary>
        private System.Windows.Forms.Label lblGuide;
    }
}
