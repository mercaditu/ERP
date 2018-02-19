namespace entity
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.DataAnnotations;
	using System.ComponentModel.DataAnnotations.Schema;
	using System.Text;

	public partial class item_transfer_detail : Audit, IDataErrorInfo
	{
		public item_transfer_detail()
		{
			id_company = CurrentSession.Id_Company;
			id_user = CurrentSession.Id_User;
			is_head = true;
			item_movement = new List<item_movement>();
            item_movement_archive = new List<item_movement_archive>();
            item_transfer_dimension = new List<item_transfer_dimension>();
		}

		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int id_transfer_detail { get; set; }

		public Status.Documents_General status { get; set; }
		public Status.Documents_General status_dest { get; set; }
		public int id_transfer { get; set; }
		public int? id_project_task { get; set; }
		public int id_item_product { get; set; }
		public int? movement_id { get; set; }
		public decimal? gross_weight { get; set; }
		public decimal? net_weight { get; set; }
		public decimal? volume { get; set; }

		[CustomValidation(typeof(Class.EntityValidation), "CheckIddecimal")]
		public decimal quantity_origin
		{
			get { return _quantity_origin; }
			set
			{
				if (_quantity_origin != value)
				{
					_quantity_origin = value;
					RaisePropertyChanged("quantity_origin");

					//Updates the Destination automatically.
					quantity_destination = _quantity_origin;
				}
			}
		}

		private decimal _quantity_origin;

		[NotMapped]
		public decimal Quantity_InStock
		{
			get { return _Quantity_InStock; }
			set
			{
				if (_Quantity_InStock != value)
				{
					_Quantity_InStock = value;
					RaisePropertyChanged("Quantity_InStock");

					InStock = quantity_origin <= _Quantity_InStock ? true : false;
					RaisePropertyChanged("InStock");
				}
			}
		}

		private decimal _Quantity_InStock;

		[NotMapped]
		public bool InStock { get; set; }
		[NotMapped]
		public decimal? Quantity_InStockLot { get; set; }

		public decimal quantity_destination
		{
			get { return _quantity_destination; }
			set
			{
				if (_quantity_destination != value)
				{
					_quantity_destination = value;
					RaisePropertyChanged("quantity_destination");
				}
			}
		}

		private decimal _quantity_destination;

		public decimal? verified_by
		{
			get { return _verified_by; }
			set { _verified_by = CurrentSession.Id_User; RaisePropertyChanged("verified_by"); }
		}

		private decimal? _verified_by;

		public DateTime? expire_date { get; set; }
		public string batch_code { get; set; }

		public virtual item_transfer item_transfer { get; set; }
		public virtual item_product item_product { get; set; }
		public virtual project_task project_task { get; set; }
		public virtual app_measurement measurement_weight { get; set; }
		public virtual app_measurement measurement_volume { get; set; }
		public virtual ICollection<item_movement> item_movement { get; set; }
        public virtual ICollection<item_movement_archive> item_movement_archive { get; set; }
        public virtual ICollection<item_transfer_dimension> item_transfer_dimension { get; set; }
		#region "Validation"

		public string Error
		{
			get
			{
				StringBuilder error = new StringBuilder();
				// iterate over all of the properties
				// of this object - aggregating any validation errors
				PropertyDescriptorCollection props = TypeDescriptor.GetProperties(this);
				foreach (PropertyDescriptor prop in props)
				{
					String propertyError = this[prop.Name];
					if (propertyError != string.Empty)
					{
						error.Append((error.Length != 0 ? ", " : "") + propertyError);
					}
				}
				return error.Length == 0 ? null : error.ToString();
			}
		}

		public string this[string columnName]
		{
			get
			{
				// apply property level validation rules

				if (columnName == "quantity_destination")
				{
					if (quantity_destination == 0)
					{
						return "Quantity can not be zero";
					}
				}
				else if (columnName == "quantity_origin")
				{
					if (Quantity_InStockLot != null)
					{
						if (Quantity_InStockLot < quantity_origin)
						{
							return "Stock Exceeded";
						}
					}
				}
				return "";
			}
		}

		#endregion "Validation"
		public void Calulate_Stock()
		{
			//entity.Brillo.
		}
	}
}