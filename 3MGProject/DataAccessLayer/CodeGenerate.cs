using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer
{
    public class CodeGenerate
    {
        public static string PTI(int number)
        {
            return string.Format("{0:D6}", number);
        }

        public static string SMU(int number)
        {
            return string.Format("T{0:D9}", number);
        }

        public static string Manifest(int number)
        {
            return string.Format("MT{0:D8}", number);
        }

        public static Task<int> GetNewPTINumber()
        {
            using (var db = new OcphDbContext())
            {
                var cmd = db.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "NewPTINumber";
                var data = cmd.ExecuteScalar();
                int result = 0;
                if (data != null)
                    result = Int32.Parse(data.ToString());
                result++;
                return Task.FromResult(result);
            }
        }


        public static Task<int> GetNewSMUNumber()
        {
            using (var db = new OcphDbContext())
            {
                var cmd = db.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "NewSMUNumber";
                var data = cmd.ExecuteScalar();
                int result = 0;
                if (data != null)
                    result = Int32.Parse(data.ToString());
                result++;
                return Task.FromResult(result);
            }
        }

        public static Task<int> GetNewManifestNumber()
        {
            using (var db = new OcphDbContext())
            {
                var cmd = db.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "NewManifestNumber";
                var data = cmd.ExecuteScalar();
                int result = 0;
                if (data != null)
                    result = Int32.Parse(data.ToString());
                result++;
                return Task.FromResult(result);
            }
        }
    }
}
