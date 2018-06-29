using Ocph.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Models
{
 public   class Schedule:BaseNotify
    {
        public int Id
        {
            get { return _id; }
            set
            {

                SetProperty(ref _id, value);
            }
        }

        public DateTime Tanggal
        {
            get { return _tanggal; }
            set
            {

                SetProperty(ref _tanggal, value);
            }
        }

        public TimeSpan Start
        {
            get { return _start; }
            set
            {

                SetProperty(ref _start, value);
            }
        }

        public TimeSpan End
        {
            get { return _end; }
            set
            {

                SetProperty(ref _end, value);
            }
        }

        public int PlaneId
        {
            get { return _planeid; }
            set
            {

                SetProperty(ref _planeid, value);
            }
        }

        public bool Complete
        {
            get { return _complete; }
            set
            {

                SetProperty(ref _complete, value);
            }
        }

        public int PortFrom
        {
            get { return _portid; }
            set
            {

                SetProperty(ref _portid, value);
            }
        }

        public int PortTo
        {
            get { return _portTo; }
            set
            {

                SetProperty(ref _portTo, value);
            }
        }

        public double Capasities
        {
            get { return _capacities; }
            set { SetProperty(ref _capacities, value); }
        }

        public DateTime CreatedDate
        {
            get { return _createddate; }
            set
            {

                SetProperty(ref _createddate, value);
            }
        }

        public string OriginPortName { get; set; }
        public string OriginPortCode { get; set; }
        public string OriginCityName { get; set; }
        public string OriginCityCode { get; set; }

        public string DestinationPortName { get; set; }
        public string DestinationPortCode { get; set; }
        public string DestinationCityName { get; set; }
        public string DestinationCityCode { get; set; }
        public string PlaneName { get; set; }
        public string PlaneCode { get; set; }
        public string FlightNumber { get; set; }

        private int _id;
        private DateTime _tanggal;
        private TimeSpan _start;
        private TimeSpan _end;
        private int _planeid;
        private bool _complete;
        private int _portid;
        private DateTime _createddate;
        private int _portTo;
        private double _capacities;
    }
}
