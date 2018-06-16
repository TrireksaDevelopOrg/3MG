using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataModels
{
    [TableName("users")]
    public class user : BaseNotify
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

        [DbColumn("Name")]
        public string Name
        {
            get { return _name; }
            set
            {

                SetProperty(ref _name, value);
            }
        }

        [DbColumn("Gender")]
        public Gender Gender
        {
            get { return _gender; }
            set
            {

                SetProperty(ref _gender, value);
            }
        }

        [DbColumn("UserName")]
        public string UserName
        {
            get { return _username; }
            set
            {

                SetProperty(ref _username, value);
            }
        }

        [DbColumn("Password")]
        public string Password
        {
            get { return _password; }
            set
            {

                SetProperty(ref _password, value);
            }
        }

        [DbColumn("Address")]
        public string Address
        {
            get { return _address; }
            set
            {

                SetProperty(ref _address, value);
            }
        }

        [DbColumn("Phone")]
        public string Phone
        {
            get { return _phone; }
            set
            {

                SetProperty(ref _phone, value);
            }
        }

        [DbColumn("Actived")]
        public string Actived
        {
            get { return _actived; }
            set
            {

                SetProperty(ref _actived, value);
            }
        }

        public virtual string ConfirmPassword { get; set; }


        private int _id;
        private string _name;
        private Gender _gender;
        private string _username;
        private string _password;
        private string _address;
        private string _phone;
        private string _actived;
    }
}

