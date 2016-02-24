using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cognitivo.Class
{
    class itemmovement
    {
        public itemmovement()
        {
            this._transdate = DateTime.Now;
        }

        private string _code;
        public string code
        {
            get { return _code; }
            set { _code = value; }
        }

        private int _id_item;
        public int itemid { get { return _id_item; } set { _id_item = value; } }

        private int _id_module;
        public int moduleid { get { return _id_module; } set { _id_module = value; } }

        private int _tagid;
        public int tagid { get { return _tagid; } set { _tagid = value; } }

        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _locationname;
        public string locationname
        {
            get { return _locationname; }
            set { _locationname = value; }
        }

        private int? _locationid;
        public int? locationid
        {
            get { return _locationid; }
            set { _locationid = value; }
        }

        private string _measurement;
        public string measurement
        {
            get { return _measurement; }
            set { _measurement = value; }

        }

        private decimal? _quantity;
        public decimal? quantity
        {
            get { return _quantity; }
            set { _quantity = value; }

        }

        private string _comment;
        public string comments { get { return _comment; } set { _comment = value; } }

        private DateTime _transdate;
        public DateTime transdate { get { return _transdate; } set { _transdate = value; } }
    }
}
