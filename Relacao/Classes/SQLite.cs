using Relacao.Classes;
using System;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Windows;

namespace Relacao
{
    class SQLite
    {
        private SQLiteConnection sqliteConn;
        private SQLiteCommand sqliteComm;
        private string pathDB;
        private SQLiteTransaction sqliteTrans;

        public SQLite()
        {
            pathDB = ConfigurationManager.AppSettings["PathDB"];

            if (pathDB.Trim() == "")
            {
                pathDB = System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        internal bool Connect()
        {
            bool retorno = false;
            string strConn = @"Data Source=" + pathDB + "Relacao.s3db";

            sqliteConn = new SQLiteConnection();

            try
            {
                sqliteConn.ConnectionString = strConn;
                sqliteConn.Open();
                retorno = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar no Banco de Dados\n" + ex.ToString());
                retorno = false;
            }

            return retorno;
        }

        internal bool Disconnect()
        {
            bool retorno = false;

            if (sqliteConn != null)
            {
                if (sqliteConn.State == System.Data.ConnectionState.Open)
                {
                    sqliteConn.Close();

                    while (sqliteConn.State != ConnectionState.Closed)
                    {
                    }

                    if (sqliteConn.State == ConnectionState.Closed)
                        retorno = true;
                    else
                        retorno = false;
                }
            }

            return retorno;
        }

        internal DataTable GetTable(string query)
        {
            DataTable table = new DataTable();
            SQLiteDataReader reader;
            string stringCommand = query;

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = stringCommand;

            try
            {
                reader = sqliteComm.ExecuteReader();
                table.Load(reader);

                reader.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao conectar no Banco de Dados\n" + ex.ToString(),
                    "Erro de Conexão", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return table;
        }

        internal DataTable GetTableConnect(string query)
        {
            DataTable table = new DataTable();
            string stringCommand = query;

            if (Connect())
            {
                table = GetTable(query);

                Disconnect();
            }

            return table;
        }

        internal long GetLastID(string tabela)
        {
            long retorno = 0;
            string stringCommand = "SELECT LAST_INSERT_ROWID() FROM " + tabela;

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = stringCommand;

            try
            {
                retorno = (long)sqliteComm.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar buscar o último ID inserido\n" + ex.ToString(),
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return retorno;
        }

        internal long InsertQuery(string query, string tabela)
        {
            long retorno = 0;

            sqliteTrans = sqliteConn.BeginTransaction();

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = query;

            try
            {
                sqliteComm.ExecuteNonQuery();
                sqliteTrans.Commit();

                retorno = GetLastID(tabela);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inserir no Banco de Dados\n" + ex.ToString(),
                    "Erro de Inserção", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return retorno;
        }

        internal void InsertSingleQuery(string query)
        {
            sqliteTrans = sqliteConn.BeginTransaction();

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = query;

            try
            {
                sqliteComm.ExecuteNonQuery();
                sqliteTrans.Commit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao inserir no Banco de Dados\n" + ex.ToString(),
                    "Erro de Inserção", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal bool UpdateQuery(string query)
        {
            bool retorno = false;

            sqliteTrans = sqliteConn.BeginTransaction();

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = query;

            try
            {
                sqliteComm.ExecuteNonQuery();
                sqliteTrans.Commit();

                retorno = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao atualizar item no Banco de Dados\n" + ex.ToString(),
                    "Erro de Atualização", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return retorno;
        }

        internal bool DeleteQuery(string query)
        {
            bool retorno = false;

            sqliteTrans = sqliteConn.BeginTransaction();

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = query;

            try
            {
                sqliteComm.ExecuteNonQuery();
                sqliteTrans.Commit();

                retorno = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao excluir item do Banco de Dados\n" + ex.ToString(),
                    "Erro de Exclusão", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return retorno;
        }

        internal bool DeleteQueryConnect(string query)
        {
            bool retorno = false;

            if (Connect())
            {
                retorno = DeleteQuery(query);

                Disconnect();
            }

            return retorno;
        }

        internal TipoMateriaPrima GetTipoMateriaPrima(long tipoID)
        {
            TipoMateriaPrima retorno = new TipoMateriaPrima();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO,UNIDADE FROM TIPOMATERIAPRIMA WHERE ID=" + tipoID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                        retorno.Unidade = row[2].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal SubTipoMateriaPrima GetSubTipoMateriaPrima(long subtipoID)
        {
            SubTipoMateriaPrima retorno = new SubTipoMateriaPrima();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO FROM SUBTIPOMATERIAPRIMA WHERE ID=" + subtipoID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal Particularidade GetParticularidade(long particularidadeID)
        {
            Particularidade retorno = new Particularidade();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO FROM PARTICULARIDADE WHERE ID=" + particularidadeID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal Linha GetLinha(long linhaID)
        {
            Linha retorno = new Linha();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO FROM LINHA WHERE ID=" + linhaID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal TipoProduto GetTipoProduto(long tipoID)
        {
            TipoProduto retorno = new TipoProduto();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO FROM TIPOPRODUTO WHERE ID=" + tipoID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal TipoComponente GetTipoComponente(long tipoID)
        {
            TipoComponente retorno = new TipoComponente();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO FROM TIPOCOMPONENTE WHERE ID=" + tipoID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal MateriaPrima GetMateriaPrima(long materiaprimaID)
        {
            MateriaPrima retorno = new MateriaPrima();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO FROM MATERIAPRIMA WHERE ID=" + materiaprimaID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistLinha(string descricao)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM LINHA WHERE DESCRICAO='" + descricao + "'";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistTipoProduto(string descricao)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM TIPOPRODUTO WHERE DESCRICAO='" + descricao + "'";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistTipoComponente(string descricao)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM TIPOCOMPONENTE WHERE DESCRICAO='" + descricao + "'";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistTipoMateriaPrima(string descricao)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM TIPOMATERIAPRIMA WHERE DESCRICAO='" + descricao + "'";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistParticularidade(Particularidade particularidade)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM PARTICULARIDADE WHERE " +
                "IDTIPOPRODUTO=" + particularidade.Tipo.ID + " AND " +
                "DESCRICAO='" + particularidade.Descricao + "'";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistSubTipoMateriaPrima(SubTipoMateriaPrima subtipo)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM SUBTIPOMATERIAPRIMA WHERE " +
                "IDTIPOMATERIAPRIMA=" + subtipo.Tipo.ID + " AND " +
                "DESCRICAO='" + subtipo.Descricao + "'";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistMateriaPrima(MateriaPrima materiaprima)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM MATERIAPRIMA WHERE " +
                "IDTIPOMATERIAPRIMA=" + materiaprima.SubTipo.Tipo.ID + " AND " +
                "IDSUBTIPOMATERIAPRIMA=" + materiaprima.SubTipo.ID + " AND " +
                "BITOLA='" + materiaprima.Bitola + "'";

            if (this.Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Disconnect();
            }

            return retorno;
        }

        internal bool ExistProduto(Produto produto)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM PRODUTO WHERE " +
                "IDTIPOPRODUTO=" + produto.Particularidade.Tipo.ID + " AND " +
                "IDPARTICULARIDADE=" + produto.Particularidade.ID + " AND " +
                "IDLINHA=" + produto.Linha.ID + " AND " +
                "REFERENCIA='" + produto.Referencia + "'";

            if (this.Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Disconnect();
            }

            return retorno;
        }

        internal bool ExistComponente(Componente componente)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM COMPONENTE WHERE " +
                "IDTIPOCOMPONENTE=" + componente.Tipo.ID + " AND " +
                "IDMATERIAPRIMA=" + componente.MateriaPrima.ID + " AND " +
                "COMPRIMENTO=" + componente.Comprimento + " AND " +
                "LARGURA=" + componente.Largura + " AND " +
                "ESPESSURA=" + componente.Espessura + " AND " +
                "IFNULL(CODIGO,'')='" + componente.Codigo + "'";

            if (this.Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Disconnect();
            }

            return retorno;
        }

        internal int GetIDByReferencia(string referencia)
        {
            int retorno = 0;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM PRODUTO WHERE " +
                "REFERENCIA='" + referencia + "'";

            if (this.Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        retorno = reader.GetInt32(0);
                    }

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Disconnect();
            }

            return retorno;
        }

        internal long GetComponenteIDByComponente(Componente componente)
        {
            long retorno = 0;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM COMPONENTE WHERE " +
                "IDTIPOCOMPONENTE=" + componente.Tipo.ID + " AND " +
                "IDMATERIAPRIMA=" + componente.MateriaPrima.ID + " AND " +
                "COMPRIMENTO=" + componente.Comprimento + " AND " +
                "LARGURA=" + componente.Largura + " AND " +
                "ESPESSURA=" + componente.Espessura;

            if (this.Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        retorno = reader.GetInt64(0);
                    }

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao buscar ID do Componente\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Disconnect();
            }

            return retorno;
        }

        internal string GetReferenciaByID(long produtoID)
        {
            string retorno = "";
            SQLiteDataReader reader;

            string stringCommand = "SELECT REFERENCIA FROM PRODUTO WHERE ID=" + produtoID.ToString();

            if (this.Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        retorno = reader.GetString(0);
                    }

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao buscar Referência do Produto\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Disconnect();
            }

            return retorno;
        }

        internal Produto GetProdutoByID(long produtoID)
        {
            Produto retorno = new Produto();
            DataTable table = new DataTable();

            string query = "SELECT ID,REFERENCIA,DESCRICAO FROM PRODUTO WHERE ID=" + produtoID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Referencia = row[1].ToString();
                        retorno.Descricao = row[2].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistReferenciaProduto(Produto produto)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM PRODUTO WHERE " +
                "REFERENCIA='" + produto.Referencia + "'";

            if (this.Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                this.Disconnect();
            }

            return retorno;
        }

        internal int ComponenteIsUsed(long componenteID)
        {
            int retorno = 0;
            DataTable table = new DataTable();

            string query =
                "SELECT PRODUTO.ID, " +
                "       PRODUTO.REFERENCIA, " +
                "       PRODUTO.DESCRICAO " +
                "  FROM COMPONENTE, " +
                "       FICHATECNICA, " +
                "       PRODUTO " +
                " WHERE FICHATECNICA.IDCOMPONENTE = COMPONENTE.ID AND " +
                "       FICHATECNICA.IDPRODUTO = PRODUTO.ID AND " +
                "       COMPONENTE.ID = " + componenteID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    retorno = table.Rows.Count;
                }

                table.Dispose();
                table = null;

                Disconnect();
            }

            return retorno;
        }

        internal FichaTecnicaAgrupada GetFichaAgrupada(string querySearch)
        {
            FichaTecnicaAgrupada retorno = new FichaTecnicaAgrupada();
            SQLiteDataReader reader;

            string stringCommand = querySearch;

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        retorno.Produto.ID = reader.GetInt64(0);
                        retorno.Quantidade = reader.GetInt32(1);
                    }

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal RelatorioFichaTecnicaAgrupada GetRelatorioFichaAgrupada(string querySearch)
        {
            RelatorioFichaTecnicaAgrupada retorno = new RelatorioFichaTecnicaAgrupada();
            SQLiteDataReader reader;

            string stringCommand = querySearch;

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = stringCommand;

            try
            {
                reader = sqliteComm.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    retorno.Relatorio.ID = reader.GetInt64(0);
                    retorno.MateriaPrima.ID = reader.GetInt64(1);
                    retorno.Quantidade = reader.GetInt32(2);
                    retorno.Medidas = reader.GetString(3);
                    retorno.Metragem = reader.GetDecimal(4);
                }

                reader.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return retorno;
        }

        internal RelatorioFichaTecnicaAgrupada GetRelatorioFichaAgrupadaConnect(string querySearch)
        {
            RelatorioFichaTecnicaAgrupada retorno = new RelatorioFichaTecnicaAgrupada();
            SQLiteDataReader reader;

            string stringCommand = querySearch;

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        retorno.Relatorio.ID = reader.GetInt64(0);
                        retorno.MateriaPrima.ID = reader.GetInt64(1);
                        retorno.Quantidade = reader.GetInt32(2);
                        retorno.Medidas = reader.GetString(3);
                        retorno.Metragem = reader.GetDecimal(4);
                    }

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal Relatorio GetRelatorioByID(long relatorioID)
        {
            Relatorio retorno = new Relatorio();
            DataTable table = new DataTable();

            string query = "SELECT ID,DESCRICAO FROM RELATORIO WHERE ID=" + relatorioID.ToString();

            if (Connect())
            {
                table = GetTable(query);

                if (table.Rows.Count > 0)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        retorno.ID = Convert.ToInt64(row[0]);
                        retorno.Descricao = row[1].ToString();
                    }
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistRelatorio(string descricao)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM RELATORIO WHERE DESCRICAO='" + descricao + "'";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool ExistProdutoNoRelatorio(FichaTecnicaAgrupada fichatecnica)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT IDPRODUTO FROM FICHATECNICAAGRUPADA WHERE " +
                "IDPRODUTO=" + fichatecnica.Produto.ID + " AND " +
                "IDRELATORIO=" + fichatecnica.Relatorio.ID + "";

            if (Connect())
            {
                sqliteComm = new SQLiteCommand();
                sqliteComm.Connection = sqliteConn;
                sqliteComm.CommandType = CommandType.Text;
                sqliteComm.CommandText = stringCommand;

                try
                {
                    reader = sqliteComm.ExecuteReader();

                    if (reader.HasRows)
                        retorno = true;

                    reader.Dispose();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao verificar se o item já existe\n" + ex.ToString(),
                        "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                Disconnect();
            }

            return retorno;
        }

        internal bool HasFichaTecnica(Produto produto)
        {
            bool retorno = false;
            SQLiteDataReader reader;

            string stringCommand = "SELECT ID FROM FICHATECNICA WHERE IDPRODUTO=" + produto.ID;

            sqliteComm = new SQLiteCommand();
            sqliteComm.Connection = sqliteConn;
            sqliteComm.CommandType = CommandType.Text;
            sqliteComm.CommandText = stringCommand;

            try
            {
                reader = sqliteComm.ExecuteReader();

                if (reader.HasRows)
                    retorno = true;

                reader.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao verificar se já existe Ficha Técnica\n" + ex.ToString(),
                    "Erro de Busca de Dados", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            return retorno;
        }

        internal void SaveDataTableOnSQLite(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                string query = 
                    "INSERT INTO RELATORIOFICHATECNICAAGRUPADA ( " +
                    "       IDRELATORIO, " +
                    "       IDMATERIAPRIMA, " +
                    "       QTDPECAS, " +
                    "       MEDIDAS, " +
                    "       METRAGEM, " +
                    "       AGRUPAMENTO) " +
                    "VALUES ( " +
                    row["IDRELATORIO"].ToString() + ", " +
                    row["IDMATERIAPRIMA"].ToString() + ", " +
                    row["QTDPECAS"].ToString().Replace(',', '.') + ", '" +
                    row["MEDIDAS"].ToString() + "', " +
                    row["METRAGEM"].ToString().Replace(',', '.') + ", '" +
                    row["AGRUPAMENTO"] + "')";

                InsertSingleQuery(query);
            }
        }

    }
}
