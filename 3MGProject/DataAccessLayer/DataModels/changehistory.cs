using System; 
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ocph.DAL;

namespace DataAccessLayer.DataModels
{
    [TableName("changehistory")]
    public class changehistory : BaseNotify
    {
        [PrimaryKey("Id")]
        [DbColumn("Id")]
        public int Id
        {
            get { return _id; }
            set
            {

                SetProperty(ref _id, value);
            }
        }

        [DbColumn("BussinesType")]
        public BussinesType BussinesType
        {
            get { return _bussinestype; }
            set
            {

                SetProperty(ref _bussinestype, value);
            }
        }

        [DbColumn("Note")]
        public string Note
        {
            get { return _note; }
            set
            {

                SetProperty(ref _note, value);
            }
        }

        [DbColumn("BussinessId")]
        public int BussinessId
        {
            get { return _bussinessid; }
            set
            {

                SetProperty(ref _bussinessid, value);
            }
        }

        [DbColumn("ChangeType")]
        public ChangeType ChangeType
        {
            get { return _changetype; }
            set
            {

                SetProperty(ref _changetype, value);
            }
        }

        [DbColumn("CreatedDate")]
        public DateTime CreatedDate
        {
            get { return _createddate; }
            set
            {

                SetProperty(ref _createddate, value);
            }
        }

        [DbColumn("UserId")]
        public int UsersId
        {
            get { return _usersid; }
            set
            {

                SetProperty(ref _usersid, value);
            }
        }

        private int _id;
        private BussinesType _bussinestype;
        private string _note;
        private int _bussinessid;
        private ChangeType _changetype;
        private DateTime _createddate;
        private int _usersid;
    }
}


