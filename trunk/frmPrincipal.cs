using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Net;
using Microsoft.Win32;
using ConsultaCotacaoClient.br.com.riscozeroprojetos.consultacotacao;
using System.Threading;
using System.Globalization;

namespace ConsultaCotacaoClient
{
    public partial class frmPrincipal : Form
    {
        private CotacaoService webservice;

        /// <summary>
        /// Inicializa o formulário principal
        /// </summary>
        public frmPrincipal()
        {
            InitializeComponent();
            webservice = new CotacaoService();
            try
            {
                webservice.UseDefaultCredentials = true;
                webservice.Proxy = WebRequest.DefaultWebProxy;
                webservice.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
            } 
            catch (Exception e) {
                MessageBox.Show("Erro ao configurar o Proxy: " + e.Message);
            }
        }

        /// <summary>
        /// Quando der dois cliques em uma célula preenchida, consulta a cotação
        /// </summary>
        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            //Não atualiza cotação se o clique duplo for na primeira coluna (onde digita o papel)
            if (e.ColumnIndex == 0 || e.RowIndex == -1)
                return;

            //Se a linha estir preenchida, atualiza
            if (dataGridView1.Rows[e.RowIndex].Cells["CodNeg"].Value != null && ((string)dataGridView1.Rows[e.RowIndex].Cells["CodNeg"].Value).Trim() != "")
                atualizarCotacao((string)dataGridView1.Rows[e.RowIndex].Cells["CodNeg"].Value, e.RowIndex);
            else
                limparLinha(e.RowIndex);
        }

        /// <summary>
        /// Consulta uma cotação no webservice
        /// </summary>
        /// <param name="papel">Código de Negociação da Ação</param>
        /// <param name="linha">Linha no DataGrid</param>
        private void atualizarCotacao(String papel, int linha)
        {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            DateTime startTime = DateTime.Now;

            DadosCotacao Cotacao = webservice.ObterCotacao(papel.Trim());

            atualizarDataGrid(linha, Cotacao);
            
            lblSegundos.Text = (DateTime.Now - startTime).TotalSeconds.ToString("N2") + 's';
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Limpa todos os dados da linha - exceto CodNeg e formatações
        /// </summary>
        /// <param name="linha"></param>
        private void limparLinha(int linha)
        {
            dataGridView1.Rows[linha].Cells["CodNeg"].ToolTipText = "";
            dataGridView1.Rows[linha].Cells["CodNeg"].Style.ForeColor = Color.Black;
            dataGridView1.Rows[linha].Cells["Hora"].Value = "";
            dataGridView1.Rows[linha].Cells["PerVariacao"].Value = "";
            dataGridView1.Rows[linha].Cells["QtdNegocios"].ToolTipText = "";
            dataGridView1.Rows[linha].Cells["QtdNegocios"].Value = "";
            dataGridView1.Rows[linha].Cells["ValAbertura"].ToolTipText = "";
            dataGridView1.Rows[linha].Cells["ValAbertura"].Value = "";
            dataGridView1.Rows[linha].Cells["ValAtualCompra"].ToolTipText = "";
            dataGridView1.Rows[linha].Cells["ValAtualCompra"].Value = "";
            dataGridView1.Rows[linha].Cells["ValAtualVenda"].ToolTipText = "";
            dataGridView1.Rows[linha].Cells["ValAtualVenda"].Value = "";
            dataGridView1.Rows[linha].Cells["ValFechamento"].Value = "";
            dataGridView1.Rows[linha].Cells["ValMaximo"].ToolTipText = "";
            dataGridView1.Rows[linha].Cells["ValMaximo"].Value = "";
            dataGridView1.Rows[linha].Cells["ValMinimo"].ToolTipText = "";
            dataGridView1.Rows[linha].Cells["ValMinimo"].Value = "";
        }

        /// <summary>
        /// Atualiza os dados de uma linha do DataGrid com a cotação
        /// </summary>
        /// <param name="linha">Linha do DataGrid que será atualizada</param>
        /// <param name="Cotacao">Dados da cotação da ação</param>
        private void atualizarDataGrid(int linha, DadosCotacao Cotacao)
        {
            //Se não achou a ação...
            if (Cotacao.descAtivo == "")
            {
                limparLinha(linha);
                return;
            }

            dataGridView1.Rows[linha].Cells["CodNeg"].Value = Cotacao.codAtivo;
            dataGridView1.Rows[linha].Cells["CodNeg"].ToolTipText = Cotacao.descAtivo;
            dataGridView1.Rows[linha].Cells["ValFechamento"].Value = Cotacao.valFechamento;
            dataGridView1.Rows[linha].Cells["QtdNegocios"].Value = Cotacao.numNegocios;
            dataGridView1.Rows[linha].Cells["QtdNegocios"].ToolTipText = "R$ " + Cotacao.qtdVolume;
            dataGridView1.Rows[linha].Cells["Hora"].Value = Cotacao.datPregao + ' ' + Cotacao.horCotacao;

            dataGridView1.Rows[linha].Cells["ValAbertura"].Value = Cotacao.valAbertura;
            dataGridView1.Rows[linha].Cells["ValAbertura"].Style.ForeColor = (Cotacao.valAbertura < Cotacao.valFechamento ? Color.Red : (Cotacao.valAbertura > Cotacao.valFechamento ? Color.Blue : Color.Black));

            dataGridView1.Rows[linha].Cells["ValMinimo"].Value = Cotacao.valMinimo;
            dataGridView1.Rows[linha].Cells["ValMinimo"].Style.ForeColor = (Cotacao.valMinimo < Cotacao.valFechamento ? Color.Red : (Cotacao.valMinimo > Cotacao.valFechamento ? Color.Blue : Color.Black));

            dataGridView1.Rows[linha].Cells["ValMaximo"].Value = Cotacao.valMaximo;
            dataGridView1.Rows[linha].Cells["ValMaximo"].Style.ForeColor = (Cotacao.valMaximo < Cotacao.valFechamento ? Color.Red : (Cotacao.valMaximo > Cotacao.valFechamento ? Color.Blue : Color.Black));

            dataGridView1.Rows[linha].Cells["ValAtualCompra"].Value = Cotacao.valOfertaCompra[0];
            dataGridView1.Rows[linha].Cells["ValAtualCompra"].Style.ForeColor = (Cotacao.valOfertaCompra[0] < Cotacao.valFechamento ? Color.Red : (Cotacao.valOfertaCompra[0] > Cotacao.valFechamento ? Color.Blue : Color.Black));

            dataGridView1.Rows[linha].Cells["ValAtualVenda"].Value = Cotacao.valOfertaVenda[0];
            dataGridView1.Rows[linha].Cells["ValAtualVenda"].Style.ForeColor = (Cotacao.valOfertaVenda[0] < Cotacao.valFechamento ? Color.Red : (Cotacao.valOfertaVenda[0] > Cotacao.valFechamento ? Color.Blue : Color.Black));

            //Se tiver preço atual e de fechamento, calcula a porcentagem em tempo real
            if (Cotacao.valFechamento > 0 && Cotacao.valOfertaVenda[0] != 0)
            {
                dataGridView1.Rows[linha].Cells["PerVariacao"].Value = Convert.ToDecimal(((Cotacao.valOfertaCompra[0] / Cotacao.valFechamento - 1) ));
                dataGridView1.Rows[linha].Cells["PerVariacao"].Style.ForeColor = (Cotacao.valOfertaCompra[0] < Cotacao.valFechamento ? Color.Red : (Cotacao.valOfertaCompra[0] > Cotacao.valFechamento ? Color.Blue : Color.Black));
            }
            else
            {
                dataGridView1.Rows[linha].Cells["PerVariacao"].Value = decimal.Divide(Cotacao.qtdVariacao, 100);
                dataGridView1.Rows[linha].Cells["PerVariacao"].Style.ForeColor = (Cotacao.qtdVariacao < 0 ? Color.Red : (Cotacao.qtdVariacao > 0 ? Color.Blue : Color.Black));
            }
            dataGridView1.Rows[linha].Cells["CodNeg"].Style.ForeColor = dataGridView1.Rows[linha].Cells["PerVariacao"].Style.ForeColor;

            if (Cotacao.valFechamento > 0)
            {
                dataGridView1.Rows[linha].Cells["ValAbertura"].ToolTipText = Convert.ToDecimal(((Cotacao.valAbertura / Cotacao.valFechamento - 1))).ToString("P2");
                dataGridView1.Rows[linha].Cells["ValMinimo"].ToolTipText = Convert.ToDecimal(((Cotacao.valMinimo / Cotacao.valFechamento - 1))).ToString("P2");
                dataGridView1.Rows[linha].Cells["ValMaximo"].ToolTipText = Convert.ToDecimal(((Cotacao.valMaximo / Cotacao.valFechamento - 1))).ToString("P2");
                dataGridView1.Rows[linha].Cells["ValAtualCompra"].ToolTipText = Convert.ToDecimal(((Cotacao.valOfertaCompra[0] / Cotacao.valFechamento - 1))).ToString("P2");
                dataGridView1.Rows[linha].Cells["ValAtualVenda"].ToolTipText = Convert.ToDecimal(((Cotacao.valOfertaVenda[0] / Cotacao.valFechamento - 1))).ToString("P2");
            } else {
                dataGridView1.Rows[linha].Cells["ValAbertura"].ToolTipText = "";
                dataGridView1.Rows[linha].Cells["ValMinimo"].ToolTipText = "";
                dataGridView1.Rows[linha].Cells["ValMaximo"].ToolTipText = "";
                dataGridView1.Rows[linha].Cells["ValAtualCompra"].ToolTipText = "";
                dataGridView1.Rows[linha].Cells["ValAtualVenda"].ToolTipText = "";
            }
        }

        /// <summary>
        /// Consulta cotação de todos os papeis no webservice
        /// </summary>
        private void atualizarCotacoes() {
            System.Windows.Forms.Cursor.Current = Cursors.WaitCursor;
            DateTime startTime = DateTime.Now;

            string[] cotacoes = new string[dataGridView1.Rows.Count-1]; //a última linha está em branco
            int numRegistrosValidos = 0;

            for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
            {
                cotacoes[i] = (string)dataGridView1.Rows[i].Cells["CodNeg"].Value;
                if (dataGridView1.Rows[i].Cells["CodNeg"].Value != null && ((string)dataGridView1.Rows[i].Cells["CodNeg"].Value).Trim() != "")
                    numRegistrosValidos++;
            }

            if (numRegistrosValidos > 0)
            {
                DadosCotacao[] arrayCotacao = webservice.ObterCotacoes(cotacoes);

                for (int i = 0; i < arrayCotacao.Length; i++)
                    atualizarDataGrid(i, arrayCotacao[i]);
            }
            else
            {
                dataGridView1.Rows.Clear();
            }

            lblSegundos.Text = (DateTime.Now - startTime).TotalSeconds.ToString("N2") + 's';
            System.Windows.Forms.Cursor.Current = Cursors.Default;
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            atualizarCotacoes();
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("pt-BR");
            
            int numRegistrosValidos = 0;

            //Abre a chave do registro e, se não encontrar, cria
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Cintra\\ConsultaCotacao");
            if (key == null)
            {
                key = Registry.CurrentUser.CreateSubKey("Software\\Cintra\\ConsultaCotacao");
            }

            //Atualiza a posição do form
            try
            {
                //Verifica se não mudou a resolução - se mudou não atualiza
                if (Screen.PrimaryScreen.Bounds.Height == (int)key.GetValue(this.Name + "_screenHeight") &&
                   Screen.PrimaryScreen.Bounds.Width == (int)key.GetValue(this.Name + "_screenWidth"))
                {
                    this.SuspendLayout();
                    if ((int)key.GetValue(this.Name + "_top") >= 0)
                        this.Top = (int)key.GetValue(this.Name + "_top");
                    if ((int)key.GetValue(this.Name + "_left") >= 0)
                        this.Left = (int)key.GetValue(this.Name + "_left");
                    if ((int)key.GetValue(this.Name + "_width") >= 0)
                        this.Width = (int)key.GetValue(this.Name + "_width");
                    if ((int)key.GetValue(this.Name + "_height") >= 0)
                        this.Height = (int)key.GetValue(this.Name + "_height");
                    this.ResumeLayout();
                }
            }
            catch (NullReferenceException) { }

            //Se encontrou valor e ele for um array de string (não foi alterado na mão)
            if (key.GetValue("ListaAcoes") != null && key.GetValue("ListaAcoes").GetType() == typeof(string[]))
            {
                string[] ListaAcoes = (string[])key.GetValue("ListaAcoes");
                for (int i = 0; i < ListaAcoes.Length; i++)
                {
                    //Se o conteúdo não estiver vazio
                    if ( ListaAcoes[i] != "" ) 
                    {
                        dataGridView1.Rows.Add(1);
                        numRegistrosValidos++;
                        dataGridView1.Rows[numRegistrosValidos-1].Cells["CodNeg"].Value = ListaAcoes[i];
                    }
                }
                atualizarCotacoes();
            }
        }

        private void frmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            //Abre o registro pra gravar as ações
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Software\\Cintra\\ConsultaCotacao", true);

            //Se tiver mais que a última linha em branco
            if (dataGridView1.Rows.Count > 1)
            {
                string[] ListaAcoes = new string[dataGridView1.Rows.Count - 1];
                for (int i = 0; i < dataGridView1.Rows.Count-1; i++)
                {
                    ListaAcoes[i] = (string)dataGridView1.Rows[i].Cells["CodNeg"].Value == null ? "" : ((string)dataGridView1.Rows[i].Cells["CodNeg"].Value).Trim();
                }
                key.SetValue("ListaAcoes", ListaAcoes);
            }
            else
            {
                key.SetValue("ListaAcoes", "");
            }

            //Grava posição e tamanho
            key.SetValue(this.Name + "_screenHeight", Screen.PrimaryScreen.Bounds.Height);
            key.SetValue(this.Name + "_screenWidth", Screen.PrimaryScreen.Bounds.Width);
            if (this.Top >= 0)
                key.SetValue(this.Name + "_top", this.Top);
            if (this.Left >= 0)
                key.SetValue(this.Name + "_left", this.Left);
            if (this.Width >= 0)
                key.SetValue(this.Name + "_width", this.Width);
            if (this.Height >= 0)
                key.SetValue(this.Name + "_height", this.Height);
        }

        private void frmPrincipal_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                Show();
                WindowState = FormWindowState.Normal;
                Activate();
            }
            else
            {
                Hide();
                WindowState = FormWindowState.Minimized;
            }
        }
    }
}