namespace entity
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Drawing;

    public partial class item_inventory_detail : Audit
    {
        public item_inventory_detail()
        {
            id_company = CurrentSession.Id_Company;
            id_user = CurrentSession.Id_User;
            is_head = true;
            item_inventory_dimension = new List<item_inventory_dimension>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_inventory_detail { get; set; }

        public int id_inventory { get; set; }
        public int id_location { get; set; }

        public int id_item_product
        {
            get
            {
                return _id_item_product;
            }
            set
            {
                _id_item_product = value;
            }
        }

        private int _id_item_product;

        [NotMapped]
        public string currency { get; set; }

        public Status.Documents status { get; set; }

        public decimal value_system
        {
            get
            {
                return _value_system;
            }
            set
            {
                _value_system = value;
            }
        }

        private decimal _value_system = 0;

        public decimal? value_counted
        {
            get
            {
                return _value_counted;
            }
            set
            {

                if (_value_counted != value)
                {
                    _value_counted = value;
                    RaisePropertyChanged("value_counted");
                    decimal _delta = this.Delta;
                    if (item_product != null)
                    {
                        if (item_product.item != null)
                        {
                            if (value_counted != null)
                            {
                                _Quantity_Factored = Brillo.ConversionFactor.Factor_Quantity(item_product.item, Convert.ToDecimal(value_counted), GetDimensionValue());
                                RaisePropertyChanged("Quantity_Factored");

                            }
                        }
                    }
                }

                RaisePropertyChanged("Delta");
                RaisePropertyChanged("Foreground");

            }
        }

        private decimal? _value_counted;

        [NotMapped]
        public decimal InternalValue_Counted
        {
            get
            {
                if (value_counted==null)
                {
                    _InternalValue_Counted = value_system;

                }
                else
                {
                    _InternalValue_Counted = Convert.ToDecimal(value_counted);
                }

                return _InternalValue_Counted;
            }
        }
        decimal _InternalValue_Counted;

        [NotMapped]
        public decimal Delta
        {
            get
            {
                decimal _delta = Convert.ToDecimal(InternalValue_Counted) - value_system;
                if (_delta < 0)
                {
                    IsEnabled = true;
                    RaisePropertyChanged("IsEnabled");
                }
                else
                {
                    IsEnabled = false;
                    RaisePropertyChanged("IsEnabled");
                }
                return _delta;
            }
        }

        [NotMapped]
        public bool IsEnabled { get; set; }

        [NotMapped]

        public Brush Foreground
        {
            get
            {
                if (Delta>0)
                {
                    return Brushes.Green;
                }
                else if (Delta < 0)
                {
                    return Brushes.Crimson;
                }
                else
                {
                    return Brushes.Black;
                }
            }
           
        }

        [NotMapped]
        public decimal Quantity_Factored
        {
            get { return _Quantity_Factored; }
            set
            {
                if (_Quantity_Factored != value)
                {
                    _Quantity_Factored = value;
                    RaisePropertyChanged("Quantity_Factored");

                    if (item_product != null)
                    {
                        if (item_product.item != null)
                        {
                            _value_counted = Brillo.ConversionFactor.Factor_Quantity_Back(item_product.item, Quantity_Factored, GetDimensionValue());
                            RaisePropertyChanged("value_counted");
                        }
                    }
                }
            }
        }

        private decimal _Quantity_Factored;

        public string comment
        {
            get { return _comment; }
            set { _comment = value; RaisePropertyChanged("comment"); }
        }

        private string _comment;

        public int id_currencyfx { get; set; }

        public decimal unit_value
        {
            get { return _unit_value; }
            set { _unit_value = value; RaisePropertyChanged("unit_value"); }
        }

        private decimal _unit_value;

        public DateTime? expire_date { get; set; }

        public string batch_code
        {
            get { return _batch_code; }
            set { _batch_code = value; RaisePropertyChanged("batch_code"); }
        }

        private string _batch_code;

        public virtual item_inventory item_inventory { get; set; }
        public virtual app_location app_location { get; set; }
        public virtual item_product item_product { get; set; }
        public virtual ICollection<item_inventory_dimension> item_inventory_dimension { get; set; }
        public int? movement_id { get; set; }

        private decimal GetDimensionValue()
        {
            decimal Dimension = 1M;
            if (item_inventory_dimension != null)
            {
                foreach (item_inventory_dimension _item_inventory_dimension in item_inventory_dimension)
                {
                    Dimension = Dimension * _item_inventory_dimension.value;
                }
            }
            return Dimension;
        }
    }
}