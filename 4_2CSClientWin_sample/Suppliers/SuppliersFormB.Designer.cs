namespace _2CSClientWin_sample.Suppliers
{
    partial class SuppliersFormB
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
            this.components = new System.ComponentModel.Container();
            this.lblGuide = new System.Windows.Forms.Label();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.btnDeleteRow = new System.Windows.Forms.Button();
            this.dgvSuppliers = new System.Windows.Forms.DataGridView();
            this.bsSuppliers = new System.Windows.Forms.BindingSource(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSuppliers)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsSuppliers)).BeginInit();
            this.pnlBody.SuspendLayout();
            this.SuspendLayout();
            //
            // lblGuide
            //
            this.lblGuide.AutoSize = true;
            this.lblGuide.Location = new System.Drawing.Point(16, 12);
            this.lblGuide.Name = "lblGuide";
            this.lblGuide.Size = new System.Drawing.Size(0, 12);
            this.lblGuide.TabIndex = 0;
            this.lblGuide.Text = "［一覧取得］で一覧を表示し、セルを直接編集できます。［行追加］／［削除］で行を増減し、［更新］でまとめて反映します。";
            //
            // btnAddRow （グリッド外の追加ボタン。フッタ部ではない）
            //
            this.btnAddRow.Location = new System.Drawing.Point(16, 34);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(100, 26);
            this.btnAddRow.TabIndex = 1;
            this.btnAddRow.Text = "行追加";
            this.btnAddRow.UseVisualStyleBackColor = true;
            //
            // btnDeleteRow （選択行の削除）
            //
            this.btnDeleteRow.Location = new System.Drawing.Point(122, 34);
            this.btnDeleteRow.Name = "btnDeleteRow";
            this.btnDeleteRow.Size = new System.Drawing.Size(100, 26);
            this.btnDeleteRow.TabIndex = 2;
            this.btnDeleteRow.Text = "削除";
            this.btnDeleteRow.UseVisualStyleBackColor = true;
            //
            // dgvSuppliers
            //
            this.dgvSuppliers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top
                | System.Windows.Forms.AnchorStyles.Bottom)
                | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSuppliers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSuppliers.Location = new System.Drawing.Point(16, 66);
            this.dgvSuppliers.Name = "dgvSuppliers";
            this.dgvSuppliers.RowTemplate.Height = 21;
            this.dgvSuppliers.Size = new System.Drawing.Size(852, 340);
            this.dgvSuppliers.TabIndex = 3;
            this.dgvSuppliers.AllowUserToAddRows = false;
            this.dgvSuppliers.AllowUserToDeleteRows = false;
            //
            // SuppliersFormB
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 470);
            this.Name = "SuppliersFormB";
            this.Text = "Suppliers 画面Ｂ";
            this.pnlBody.Controls.Add(this.dgvSuppliers);
            this.pnlBody.Controls.Add(this.btnDeleteRow);
            this.pnlBody.Controls.Add(this.btnAddRow);
            this.pnlBody.Controls.Add(this.lblGuide);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSuppliers)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsSuppliers)).EndInit();
            this.pnlBody.ResumeLayout(false);
            this.pnlBody.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        /// <summary>案内ラベル</summary>
        private System.Windows.Forms.Label lblGuide;
        /// <summary>行追加ボタン（グリッド外）</summary>
        private System.Windows.Forms.Button btnAddRow;
        /// <summary>行削除ボタン</summary>
        private System.Windows.Forms.Button btnDeleteRow;
        /// <summary>一覧グリッド</summary>
        private System.Windows.Forms.DataGridView dgvSuppliers;
        /// <summary>バインディング ソース</summary>
        private System.Windows.Forms.BindingSource bsSuppliers;
    }
}
