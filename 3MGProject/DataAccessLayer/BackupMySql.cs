using MySql.Data.MySqlClient;
using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public delegate void delMessage (string message);
    public delegate void delOnComplete(string message);

   public class BackupMySqlBase:BaseNotify
    {
        protected MySqlBackup Backup { get; set; }

        public event delMessage OnError;
        public event  delOnComplete OnComplete;

        public BackupMySqlBase()
        {
            Backup = new MySqlBackup();
            Backup.ExportProgressChanged += Backup_ExportProgressChanged;
            Backup.ExportCompleted += Backup_ExportCompleted;
        }

        private void Backup_ExportCompleted(object sender, ExportCompleteArgs e)
        {
            if (OnComplete != null)
                OnComplete("Backup Selesai");
        }

        private async void Backup_ExportProgressChanged(object sender, ExportProgressArgs e)
        {
            
   
            ProgressValue = e.CurrentRowIndexInAllTables;
            TotalData = e.TotalRowsInAllTables;
            TableName = e.CurrentTableName;
     
            await Task.Delay(1000);
        }

        public void BackupAction( string file)
        {
            try
            {
                using (var db = new OcphDbContext())
                {
                    var cmd = db.CreateCommand() as MySqlCommand;
                    Backup.Command = cmd;
                    Backup.ExportToFile(file);
                }
            }
            catch (Exception ex)
            {
                if (OnError != null)
                    OnError(ex.Message);
            }
        }

        private long data;

        public long ProgressValue
        {
            get { return data; }
            set {SetProperty(ref data ,value); }
        }


        private double totalData;
        private string tableName;

        public double TotalData
        {
            get { return totalData; }
            set { SetProperty(ref totalData ,value); }
        }



        public string TableName
        {
            get { return tableName; }
            set { SetProperty(ref tableName, value); }
        }


        public Task<bool> NormalizationCollySender()
        {
            using (var db = new OcphDbContext())
            {
                try
                {
                    var cmd = db.CreateCommand();
                    cmd.CommandText = "NormalisasiCollies";
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    var reader = cmd.ExecuteNonQuery();
                    if (reader > 0)
                        return Task.FromResult(true);
                    else
                        return Task.FromResult(false);
                }
                catch (Exception ex)
                {
                    OnError?.Invoke(ex.Message);
                    return Task.FromResult(false);
                }
            }
        }


    }
}
