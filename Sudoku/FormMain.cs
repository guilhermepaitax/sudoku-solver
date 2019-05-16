using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace Sudoku
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();
        }

        private int[,] MatrizSudoku = new int[9, 9];

        private void btnResolver_Click(object sender, EventArgs e)
        {
            //Limpa a matriz
            MatrizSudoku = new int[9, 9];
            //Valida as informações do grid
            if (!ValidaGrid())
            {
                MessageBox.Show("Formatação do quebra cabeça inválida", "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Chama função para resolver o Sudoku
            if (Resolver(0, 0)) MessageBox.Show("Sudoku resolvido com sucesso!", "Sucesso!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            else MessageBox.Show("Impossivel resolver Sudoku!", "Alerta!", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        }

        //Valida e monta Matriz com base nos valores do Grid
        private bool ValidaGrid()
        {
            int num;
            for (int i = 0; i < DataGridView1.RowCount; i++)
            {
                for (int j = 0; j < DataGridView1.ColumnCount; j++)
                {

                    if (DataGridView1.Rows[i].Cells[j].Value == null || DataGridView1.Rows[i].Cells[j].Value == "")
                    {
                        MatrizSudoku[i, j] = 0;
                    }
                    else if (Int32.TryParse(DataGridView1.Rows[i].Cells[j].Value.ToString(), out num))
                    {
                        if (!ValidaNum(i, j, num)) return false;
                        else MatrizSudoku[i, j] = num;
                    }
                    else return false;

                }
            }
            return true;
        }

        private bool Resolver(int l, int c)
        {

            int coluna = c, linha = l;

            //Condição de parada
            if (c == 9) return true;
            //Pega próxima posição
            if (linha < 8) linha++;
            else
            {
                linha = 0;
                coluna++;
            }

            //Verifica se a posição do quebra cabeça esta vazia
            if (MatrizSudoku[l, c] != 0) return Resolver(linha, coluna);
            else
            {
                //Loop nos possíveis elementos a serem preenchidos
                for (int num = 1; num < 10; num++)
                {
                    //Chama função para validar se elemento é válido
                    if (ValidaNum(l, c, num))
                    {
                        MatrizSudoku[l, c] = num;
                        DataGridView1.Rows[l].Cells[c].Value = num;
                        DataGridView1.Rows[l].Cells[c].Style.ForeColor = Color.LightSeaGreen;
                        if (chkProcesso.Checked)
                        {
                            //Delay para mostrar processo de construção do quebra cabeça
                            var t = Task.Run(async delegate
                            {
                                await Task.Delay(100);
                            });
                            t.Wait();
                        }
                        //Chama função com proxíma posição
                        if (Resolver(linha, coluna)) return true;
                        //Caso não seja possível preencher casa, zera o valor para tentar novamente
                        MatrizSudoku[l, c] = 0;
                        DataGridView1.Rows[l].Cells[c].Value = "";
                    }
                }
            }
            return false;
        }

        //Retorna se um determinado número é valido no quebra cabeça
        private bool ValidaNum(int l, int c, int num)
        {
            //Verifica se ele repete nas linhas e colunas
            for (int i = 0; i < 9; i++)
            {
                if (MatrizSudoku[l, i] == num)
                    return false;
                if (MatrizSudoku[i, c] == num)
                    return false;
            }

            //Pega o quadrante do número
            int linha = (l / 3) * 3;
            int coluna = (c / 3) * 3;

            //Verifica se ele repete no quadrante
            for (int j = linha; j < linha + 3; j++)
            {
                for (int k = coluna; k < coluna + 3; k++)
                {
                    if (MatrizSudoku[j, k] == num) return false;
                }
            }

            return true;
        }

        //Limpa todas as células do Grid
        private void LimparGrid()
        {
            for (int i = 0; i < DataGridView1.RowCount; i++)
            {
                for (int j = 0; j < DataGridView1.ColumnCount; j++)
                {
                    DataGridView1.Rows[i].Cells[j].Value = null;
                    DataGridView1.Rows[i].Cells[j].Style.ForeColor = Color.FromArgb(64, 64, 64);
                }
            }
        }

        //Abre arquivo txt
        private void btnImportar_Click(object sender, EventArgs e)
        {
            openFileDialog1.FileName = "";
            openFileDialog1.Filter = "Text Files (*txt)|*.txt";
            openFileDialog1.Title = "Sudoku TXT";

            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {

                if (File.Exists(openFileDialog1.FileName))
                {
                    LimparGrid();
                    LerArquivo(openFileDialog1.FileName);
                }
            }
        }

        //Faz a leitura do arquivo txt e monta o Grid com base nele
        private void LerArquivo(string arquivo)
        {
            StreamReader leitor = new StreamReader(arquivo);
            string linha = leitor.ReadLine();

            while (linha != null)
            {

                for (int i = 0; i < 9; i++)
                {
                    int index = 0;
                    for (int j = 0; j < 9; j++)
                    {
                        try
                        {
                            if (linha.Substring(index, 1) != "0")
                            {
                                DataGridView1.Rows[i].Cells[j].Value = linha.Substring(index, 1);
                                index += 1;
                            }
                            else
                            {
                                DataGridView1.Rows[i].Cells[j].Value = null;
                                index += 1;
                            }
                        }
                        catch (Exception)
                        {
                            LimparGrid();
                            MessageBox.Show("Formatação de arquivo inválida", "Erro!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;

                        }
                    }
                    linha = leitor.ReadLine();
                }

            }
        }

        //Chama função para limpar o Grid
        private void btnLimpar_Click(object sender, EventArgs e)
        {
            LimparGrid();
        }

        //Monta Grid que representa o quebra cabeça Sudoku
        private void FormMain_Load(object sender, EventArgs e)
        {
            DataGridView1.Rows.Add(9);
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    DataGridView1.Rows[i].Cells[j].Style.BackColor = Color.LightCyan;
                    DataGridView1.Rows[i].Cells[j + 6].Style.BackColor = Color.LightCyan;
                    DataGridView1.Rows[i + 3].Cells[j + 3].Style.BackColor = Color.LightCyan;
                    DataGridView1.Rows[i + 6].Cells[j].Style.BackColor = Color.LightCyan;
                    DataGridView1.Rows[i + 6].Cells[j + 6].Style.BackColor = Color.LightCyan;
                }
            }
        }
    }
}
