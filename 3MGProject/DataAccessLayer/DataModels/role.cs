using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.DataModels
{
    [TableName("roles")]
    public class role :  BaseNotify
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

        public virtual bool Selected {
            get { return _selected; }
            set { SetProperty(ref _selected, value); }
        }

        private int _id;
        private string _name;
        private bool _selected;
    }

}
