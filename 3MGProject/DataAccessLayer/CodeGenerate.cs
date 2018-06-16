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
            return string.Format("{0:D10}", number);
        }

        public static string SMU(int number)
        {
            return string.Format("{0:D10}", number);
        }

        public static string Manifest(int number)
        {
            return string.Format("{0:D10}", null);
        }

        public static Task<int> GetNewPTINumber()
        {
            using (var db = new OcphDbContext())
            {
                var cmd = db.CreateCommand();
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.CommandText = "NewPTINumber";
                var data = cmd.ExecuteScalar();
                var result = Int32.Parse(data.ToString());
                result++;
                return Task.FromResult(result);
            }
        }

    }
}
