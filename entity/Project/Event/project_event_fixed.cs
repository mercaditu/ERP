using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Linq;
using entity.Brillo;

namespace entity
{
    public class project_event_fixed : Audit, IDataErrorInfo
    {
        public project_event_fixed()
        {
            id_user =  CurrentSession.Id_User;
           
            id_company = CurrentSession.Id_Company;
         
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int id_project_event_fixed { get; set; }
        public int id_project_event { get; set; }

        public int? id_tag { get; set; }
        [Required]
        [CustomValidation(typeof(Class.EntityValidation), "CheckId")]
        public int id_item
        {
            get { return _id_item; }
            set
            {
                if (value > 0 && value != _id_item)
                {
                    _id_item = value;
                    RaisePropertyChanged("id_item");

                  
                        using (db db = new db())
                        {
                            item _item = db.items.Where(x => x.id_item == _id_item).FirstOrDefault();

                        
                        }

                        update_UnitPrice();
                    
                }
            }
        }
        private int _id_item;
        public decimal consumption { get; set; }
        public bool is_included { get; set; }
        [NotMapped]
        public decimal unit_price { get; set; }
        public virtual project_event project_event { get; set; }
        public virtual item_tag item_tag { get; set; }
        public virtual item item { get; set; }
        private void update_UnitPrice()
        {
            if (item != null)
            {
                unit_price = get_Price(project_event.contact, project_event.id_currencyfx, item, App.Modules.Sales);
                RaisePropertyChanged("unit_price");

            }
        }
        public decimal get_Price(contact contact, int id_currencyfx, item item, entity.App.Modules Module)
        {
            if (item != null && contact != null)
            {
                //Step 1. Get Price List.
                int id_priceList = 0;
                if (contact.id_price_list != null)
                {
                    id_priceList = (int)contact.id_price_list; //Get Price List from Contact.
                }
                else
                {
                    id_priceList = get_Default(item.id_company).id_price_list; //Get Price List from Default, because Contact has no values.
                }

                //Step 1 1/2. Check if Quantity gets us a better Price List.
                //Step 2. Get Price in Currency.
                int id_currency = 0;
                using (db db = new db())
                {
                    if (db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault() != null)
                    {
                        id_currency = db.app_currencyfx.Where(x => x.id_currencyfx == id_currencyfx).FirstOrDefault().id_currency;
                    }
                }

                item_price item_price = item.item_price.Where(x => x.id_currency == id_currency
                                    && x.id_price_list == id_priceList).FirstOrDefault();

                if (item_price != null)
                {
                    return item_price.valuewithVAT;
                    //  return Currency.convert_Value(item_price_value, id_currencyfx, entity.App.Modules.Sales);
                }
                else
                {
                    decimal item_price_value = 0;
                    //decimal currencyfx = Currency.get_specificRate(id_currencyfx, application);
                    if (item.item_price.Where(x => x.id_price_list == id_priceList) != null && item.item_price.Where(x => x.id_price_list == id_priceList).Count() > 0)
                    {
                        item_price = item.item_price.Where(y => y.id_price_list == id_priceList).FirstOrDefault();
                        item_price_value = item_price.valuewithVAT;
                        using (db db = new db())
                        {

                            if (db.app_currency.Where(x => x.id_currency == item_price.id_currency).FirstOrDefault() != null)
                            {
                                if (db.app_currency.Where(x => x.id_currency == item_price.id_currency).FirstOrDefault().app_currencyfx.Where(x => x.is_active).FirstOrDefault() != null)
                                {
                                    return Currency.convert_Values(item_price_value, db.app_currency.Where(x => x.id_currency == item_price.id_currency).FirstOrDefault().app_currencyfx.Where(x => x.is_active).FirstOrDefault().id_currencyfx, id_currencyfx, Module);

                                }
                            }
                        }

                    }
                    //return Currency.convert_Value(item_price_value, id_currencyfx, Module);            

                }
            }
            return 0;
        }

        public item_price get_Default(int? id_company)
        {
            item_price item_price = new item_price();
            using (db db = new db())
            {
                if (db.item_price.Where(x => x.item_price_list.is_active == true && x.item_price_list.id_company == id_company) != null)
                {
                    item_price = db.item_price.Where(x => x.item_price_list.is_active == true
                                                            && x.item_price_list.id_company == id_company).FirstOrDefault();
                }
                else
                {
                    item_price = db.item_price.Where(x => x.item_price_list.id_company == id_company).FirstOrDefault();
                }
            }
            return item_price;
        }
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
                    string propertyError = this[prop.Name];
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
                if (columnName == "id_tag")
                {
                    if (id_tag == 0)
                        return "Tag needs to be selected";
                }
                if (columnName == "id_item")
                {
                    if (id_item == 0)
                        return "Item needs to be selected";
                }
                return "";
            }
        }
    }
}