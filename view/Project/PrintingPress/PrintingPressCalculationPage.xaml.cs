using entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Cognitivo.Project
{
    /// <summary>
    /// Interaction logic for PrintingPressCalculationPage.xaml
    /// </summary>
    public partial class PrintingPressCalculationPage : Page
    {
        private entity.dbContext entity = new entity.dbContext();

        public PrintingPress.calc_Cost _calc_Cost_BestPrinter;
        private List<PrintingPress.calc_Cost> _calc_CostList = new List<PrintingPress.calc_Cost>();

        private List<PrintingPress.Accessory> _accessoryList = new List<PrintingPress.Accessory>();
        private List<PrintingPress.Product> _productList = new List<PrintingPress.Product>();
        private List<PrintingPress.Printer> _printerList = new List<PrintingPress.Printer>();
        private List<PrintingPress.Paper> _paperList = new List<PrintingPress.Paper>();
        private List<PrintingPress.Ink> _inkList = new List<PrintingPress.Ink>();

        private int id_project = 0;
        public int _id_project { get { return id_project; } set { id_project = value; project_Changed(); } }
        public string pagename { get; set; }

        //Form Variables
        public string _project_Name { get; set; }

        public int _project_Type { get; set; }
        public int _change_Qty { get; set; }
        public int _color_Qty { get; set; }
        public int _product_Long { get; set; }
        public int _product_Short { get; set; }
        public int _product_Qty { get; set; }

        private List<int> ink_Percentage = new List<int>();

        private PrintingPress.Toner _toner = new PrintingPress.Toner();

        public int _printerTag { get; set; }

        //Preference of Height, Width, and Weight.
        private int id_Long;

        private int id_Short;
        private int id_Weight;

        public PrintingPressCalculationPage()
        {
            InitializeComponent();

            id_Long = Project.PrintingPress.Costing.Default.Long;
            id_Short = Project.PrintingPress.Costing.Default.Short;
            id_Weight = Project.PrintingPress.Costing.Default.Weight;

            ink_Percentage.Add(10);
            ink_Percentage.Add(20);
            ink_Percentage.Add(30);
            ink_Percentage.Add(40);
            ink_Percentage.Add(50);
            ink_Percentage.Add(75);
            ink_Percentage.Add(100);

            _toner.Cost = 22;

            cmbweight.SelectedValuePath = "value";
            cmbweight.DisplayMemberPath = "value";
            cmbweight.ItemsSource = (from p in entity.db.item_dimension
                                     where p.id_app_dimension == id_Weight
                                     group p by new { p.value }
                                         into mygroup
                                     select mygroup.FirstOrDefault()).ToList();
        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (id_Long == 0 && id_Short == 0 && id_Weight == 0)
            {
                MessageBox.Show("Asignar relacion de Largo, Ancho, y Peso", "Cognitivo");
                return;
            }

            _calc_CostList.Clear();

            Task taskAuth = Task.Factory.StartNew(() => thread_Calculate_Cost());
        }

        private void thread_Calculate_Cost()
        {
            //PrintingPress.Cut _cut = _cut;
            PrintingPress.Product _product = new PrintingPress.Product();
            _product.Long = _product_Long;
            _product.Short = _product_Short;
            _product.Qty = _product_Qty;
            _product.Change = _change_Qty;
            _product.Color = _color_Qty;

            _product.Weight = Convert.ToInt16(cmbweight.SelectedValue);

            List<PrintingPress.calc_Cost> _calc_Cost_PrinterList = new List<PrintingPress.calc_Cost>();
            foreach (PrintingPress.Printer printer in _printerList)
            {
                List<PrintingPress.calc_Cost> _calc_Cost_PaperList = new List<PrintingPress.calc_Cost>();
                foreach (PrintingPress.Paper _paper in _paperList.Where(i => i.Weight == Convert.ToDecimal(_product.Weight)))
                {
                    List<PrintingPress.calc_Cost> _calc_Cost_WorkList = new List<PrintingPress.calc_Cost>();

                    //Minimum
                    PrintingPress.calc_Cost calc_Cost_Min = new PrintingPress.calc_Cost();
                    calc_Cost_Min = return_Cost(_product, _paper, printer,
                                       printer.Min_Long,
                                       printer.Min_Short);
                    if (calc_Cost_Min != null && calc_Cost_Min._paper.Qty_Fit >= 1)
                    {
                        _calc_Cost_WorkList.Add(calc_Cost_Min);
                    }

                    //Maximum
                    PrintingPress.calc_Cost calc_Cost_Max = new PrintingPress.calc_Cost();
                    calc_Cost_Max = return_Cost(_product, _paper, printer,
                                                printer.Max_Long,
                                                printer.Max_Short);
                    if (calc_Cost_Max != null && calc_Cost_Max._paper.Qty_Fit >= 1)
                    {
                        _calc_Cost_WorkList.Add(calc_Cost_Max);
                    }

                    //Optimal based on Integer Min >= Increments >= Max.
                    for (decimal _page_Long = printer.Min_Long; _page_Long < printer.Max_Long; _page_Long += 0.10M)
                    {
                        for (decimal _page_Short = printer.Min_Short; _page_Short < printer.Max_Short; _page_Short += 0.10M)
                        {
                            PrintingPress.calc_Cost calc_Cost_Optimal = new PrintingPress.calc_Cost();
                            calc_Cost_Optimal = return_Cost(_product, _paper, printer,
                                                            _page_Long,
                                                            _page_Short);
                            if (calc_Cost_Optimal != null && calc_Cost_Optimal._paper.Qty_Fit >= 1 && calc_Cost_Optimal._page.Qty_Fit >= 1)
                            {
                                _calc_Cost_WorkList.Add(calc_Cost_Optimal);
                            }
                            calc_Cost_Optimal = null;
                        }
                    }

                    //Gets cheapest Iteration of this Printer and Paper selection.
                    if (_calc_Cost_WorkList.Count != 0)
                    {
                        PrintingPress.calc_Cost _calc_Cost_BestWork = _calc_Cost_WorkList.OrderBy(x => x.cost).First();
                        //_calc_Cost_WorkList.Clear();
                        _calc_Cost_PaperList.Add(_calc_Cost_BestWork);
                    }
                }
                //Gets cheapest Paper in the List
                if (_calc_Cost_PaperList.Count != 0)
                {
                    PrintingPress.calc_Cost _calc_Cost_BestPaper = _calc_Cost_PaperList.OrderBy(x => x.cost).First();
                    //_calc_Cost_PaperList.Clear();
                    _calc_Cost_PrinterList.Add(_calc_Cost_BestPaper);
                }
            }
            //Gets cheapest Printer in the List
            if (_calc_Cost_PrinterList.Count != 0)
            {
                _calc_Cost_BestPrinter = _calc_Cost_PrinterList.OrderBy(x => x.cost).First();
                CollectionViewSource cost_calviewsource = ((CollectionViewSource)(FindResource("cost_calviewsource")));
                cost_calviewsource.Source = _calc_Cost_PrinterList;
            }
            //Task calcCost = Task.Factory.StartNew(() => calculate_Cost());
        }

        private PrintingPress.calc_Cost return_Cost
                                        (PrintingPress.Product _product,
                                         PrintingPress.Paper _paper,
                                         PrintingPress.Printer printer,
                                         decimal _page_Long,
                                         decimal _page_Short)
        {
            List<PrintingPress.calc_Cost> _calc_CostList = new List<PrintingPress.calc_Cost>();
            PrintingPress.Paper papertt = new PrintingPress.Paper();
            papertt.Cost = _paper.Cost;
            papertt.Id = _paper.Id;
            papertt.IsSelected = _paper.IsSelected;
            papertt.Long = _paper.Long;
            papertt.Name = _paper.Name;
            papertt.Qty = _paper.Qty;
            papertt.Qty_Fit = _paper.Qty_Fit;
            papertt.Short = _paper.Short;
            papertt.Weight = _paper.Weight;

            PrintingPress.Printer Printertt = new PrintingPress.Printer();
            Printertt.Color_Limit = printer.Color_Limit;
            Printertt.Cost = printer.Cost;
            Printertt.Cost_DieSet = printer.Cost_DieSet;
            Printertt.Id = printer.Id;
            Printertt.IsSelected = printer.IsSelected;
            Printertt.Max_Long = printer.Max_Long;
            Printertt.Max_Short = printer.Max_Short;
            Printertt.Min_Long = printer.Min_Long;
            Printertt.Min_Short = printer.Min_Short;
            Printertt.Name = printer.Name;
            Printertt.Runs = printer.Runs;
            Printertt.Speed = printer.Speed;
            Printertt.Time = printer.Time;

            //True True
            PrintingPress.calc_Cost calc_Cost_TT = new PrintingPress.calc_Cost();
            calc_Cost_TT._printer = Printertt;
            calc_Cost_TT._product = _product;
            calc_Cost_TT._toner = _toner;

            calc_Cost_TT._paper = papertt; //_paperList.Where(x => x.IsSelected).FirstOrDefault();
            calc_Cost_TT._page = new PrintingPress.Page();
            calc_Cost_TT._page.Long = _page_Long;
            calc_Cost_TT._page.Short = _page_Short;
            calc_Cost_TT.calc_Automatic(ref _inkList, ref _accessoryList, true, true);
            if (calc_Cost_TT._paper.Qty_Fit >= 1)
            {
                _calc_CostList.Add(calc_Cost_TT);
            }
            PrintingPress.Paper papertf = new PrintingPress.Paper();
            papertf.Cost = _paper.Cost;
            papertf.Id = _paper.Id;
            papertf.IsSelected = _paper.IsSelected;
            papertf.Long = _paper.Long;
            papertf.Name = _paper.Name;
            papertf.Qty = _paper.Qty;
            papertf.Qty_Fit = _paper.Qty_Fit;
            papertf.Short = _paper.Short;
            papertf.Weight = _paper.Weight;

            PrintingPress.Printer Printertf = new PrintingPress.Printer();
            Printertf.Color_Limit = printer.Color_Limit;
            Printertf.Cost = printer.Cost;
            Printertf.Cost_DieSet = printer.Cost_DieSet;
            Printertf.Id = printer.Id;
            Printertf.IsSelected = printer.IsSelected;
            Printertf.Max_Long = printer.Max_Long;
            Printertf.Max_Short = printer.Max_Short;
            Printertf.Min_Long = printer.Min_Long;
            Printertf.Min_Short = printer.Min_Short;
            Printertf.Name = printer.Name;
            Printertf.Runs = printer.Runs;
            Printertf.Speed = printer.Speed;
            Printertf.Time = printer.Time;

            //True False
            PrintingPress.calc_Cost calc_Cost_TF = new PrintingPress.calc_Cost();
            calc_Cost_TF._printer = Printertf;
            calc_Cost_TF._product = _product;
            calc_Cost_TF._toner = _toner;

            calc_Cost_TF._paper = papertf; //_paperList.Where(x => x.IsSelected).FirstOrDefault();
            calc_Cost_TF._page = new PrintingPress.Page();
            calc_Cost_TF._page.Long = _page_Long;
            calc_Cost_TF._page.Short = _page_Short;
            calc_Cost_TF.calc_Automatic(ref _inkList, ref _accessoryList, true, false);
            if (calc_Cost_TF._paper.Qty_Fit >= 1)
            {
                _calc_CostList.Add(calc_Cost_TF);
            }
            PrintingPress.Paper paperft = new PrintingPress.Paper();
            paperft.Cost = _paper.Cost;
            paperft.Id = _paper.Id;
            paperft.IsSelected = _paper.IsSelected;
            paperft.Long = _paper.Long;
            paperft.Name = _paper.Name;
            paperft.Qty = _paper.Qty;
            paperft.Qty_Fit = _paper.Qty_Fit;
            paperft.Short = _paper.Short;
            paperft.Weight = _paper.Weight;

            PrintingPress.Printer Printerft = new PrintingPress.Printer();
            Printerft.Color_Limit = printer.Color_Limit;
            Printerft.Cost = printer.Cost;
            Printerft.Cost_DieSet = printer.Cost_DieSet;
            Printerft.Id = printer.Id;
            Printerft.IsSelected = printer.IsSelected;
            Printerft.Max_Long = printer.Max_Long;
            Printerft.Max_Short = printer.Max_Short;
            Printerft.Min_Long = printer.Min_Long;
            Printerft.Min_Short = printer.Min_Short;
            Printerft.Name = printer.Name;
            Printerft.Runs = printer.Runs;
            Printerft.Speed = printer.Speed;
            Printerft.Time = printer.Time;

            //False True
            PrintingPress.calc_Cost calc_Cost_FT = new PrintingPress.calc_Cost();
            calc_Cost_FT._printer = Printerft;
            calc_Cost_FT._product = _product;
            calc_Cost_FT._toner = _toner;

            calc_Cost_FT._paper = paperft; //_paperList.Where(x => x.IsSelected).FirstOrDefault();
            calc_Cost_FT._page = new PrintingPress.Page();
            calc_Cost_FT._page.Long = _page_Long;
            calc_Cost_FT._page.Short = _page_Short;
            calc_Cost_FT.calc_Automatic(ref _inkList, ref _accessoryList, false, true);
            if (calc_Cost_FT._paper.Qty_Fit >= 1)
            {
                _calc_CostList.Add(calc_Cost_FT);
            }
            PrintingPress.Paper paperff = new PrintingPress.Paper();
            paperff.Cost = _paper.Cost;
            paperff.Id = _paper.Id;
            paperff.IsSelected = _paper.IsSelected;
            paperff.Long = _paper.Long;
            paperff.Name = _paper.Name;
            paperff.Qty = _paper.Qty;
            paperff.Qty_Fit = _paper.Qty_Fit;
            paperff.Short = _paper.Short;
            paperff.Weight = _paper.Weight;

            PrintingPress.Printer Printerff = new PrintingPress.Printer();
            Printerff.Color_Limit = printer.Color_Limit;
            Printerff.Cost = printer.Cost;
            Printerff.Cost_DieSet = printer.Cost_DieSet;
            Printerff.Id = printer.Id;
            Printerff.IsSelected = printer.IsSelected;
            Printerff.Max_Long = printer.Max_Long;
            Printerff.Max_Short = printer.Max_Short;
            Printerff.Min_Long = printer.Min_Long;
            Printerff.Min_Short = printer.Min_Short;
            Printerff.Name = printer.Name;
            Printerff.Runs = printer.Runs;
            Printerff.Speed = printer.Speed;
            Printerff.Time = printer.Time;

            //False False
            PrintingPress.calc_Cost calc_Cost_FF = new PrintingPress.calc_Cost();
            calc_Cost_FF._printer = Printerff;
            calc_Cost_FF._product = _product;
            calc_Cost_FF._toner = _toner;

            calc_Cost_FF._paper = paperff; //_paperList.Where(x => x.IsSelected).FirstOrDefault();
            calc_Cost_FF._page = new PrintingPress.Page();
            calc_Cost_FF._page.Long = _page_Long;
            calc_Cost_FF._page.Short = _page_Short;
            calc_Cost_FF.calc_Automatic(ref _inkList, ref _accessoryList, false, false);
            if (calc_Cost_FF._paper.Qty_Fit >= 1)
            {
                _calc_CostList.Add(calc_Cost_FF);
            }

            //Returning Cheapest
            if (_calc_CostList.Count > 0)
            {
                return _calc_CostList.OrderBy(x => x.cost).First();
            }
            else
            {
                return null;
            }
        }

        #region "Helper Methods"

        private int get_Qty(decimal parent_Short, decimal parent_Long,
                            decimal child_Short, decimal child_Long)
        { return (int)(get_Consumption(parent_Short, child_Short) * get_Consumption(parent_Long, child_Long)); }

        private int get_Consumption(decimal parent, decimal child)
        {
            decimal result = parent / child;
            return (int)Math.Floor(result);
        }

        #endregion "Helper Methods"

        #region "Events"

        private void project_Changed()
        {
            List<project_template_detail> _projectTemplate = new List<project_template_detail>();
            int selectid = Convert.ToInt32(id_project);
            _projectTemplate = entity.db.project_template_detail.Where(x => x.id_project_template == selectid).ToList();
            project_template_detail accesories_template = _projectTemplate.Where(x => x.logic == "Accessory").FirstOrDefault();
            project_template_detail accesories_x_qty_template = _projectTemplate.Where(x => x.logic == "Accessory Per Quantity").FirstOrDefault();
            project_template_detail ink_template = _projectTemplate.Where(x => x.logic == "ink").FirstOrDefault();
            project_template_detail printer_template = _projectTemplate.Where(x => x.logic == "printer").FirstOrDefault();
            project_template_detail toner_template = _projectTemplate.Where(x => x.logic == "Toner").FirstOrDefault();
            project_template_detail paper_template = _projectTemplate.Where(x => x.logic == "paper").FirstOrDefault();
            project_template_detail cut_template = _projectTemplate.Where(x => x.logic == "Cut").FirstOrDefault();
            //project_template_detail accesory_abc = _projectTemplate.Where(x => x.logic == "accesory_abc").FirstOrDefault();

            if (ink_template != null)
            {
                int _inkTag = Convert.ToInt16(ink_template.id_tag);

                cmbink.SelectedValuePath = "id_item";
                cmbink.DisplayMemberPath = "name";
                cmbink.ItemsSource = entity.db.items
                    .Where(x => entity.db.item_tag_detail.Where(y => y.id_tag == _inkTag)
                        .Select(z => z.id_item)
                        .Contains(x.id_item)).ToList();
                cmbvalue.ItemsSource = ink_Percentage;
            }

            if (printer_template != null)
            {
                _printerTag = Convert.ToInt16(printer_template.id_tag);

                using (db db = new db())
                {
                    List<item> printerlist = entity.db.items.Where(x => entity.db.item_tag_detail
                        .Where(y => y.id_tag == _printerTag)
                            .Select(z => z.id_item)
                            .Contains(x.id_item)).ToList();

                    foreach (item _item in printerlist)
                    {
                        PrintingPress.Printer _printer = new PrintingPress.Printer();
                        _printer.Id = _item.id_item;
                        _printer.Name = _item.name;
                        _printer.Min_Long = (decimal)_item.item_asset.FirstOrDefault().min_width;
                        _printer.Min_Short = (decimal)_item.item_asset.FirstOrDefault().min_length;
                        _printer.Max_Long = (decimal)_item.item_asset.FirstOrDefault().max_width;
                        _printer.Max_Short = (decimal)_item.item_asset.FirstOrDefault().max_length;
                        _printer.Speed = (int)_item.item_asset.FirstOrDefault().speed;
                        _printer.Color_Limit = (int)_item.item_property.FirstOrDefault().value;
                        _printer.Cost = (decimal)_item.unit_cost;
                        _printer.Cost_DieSet = (decimal)_item.item_asset.FirstOrDefault().dieset_price;
                        //Adding to List
                        _printerList.Add(_printer);
                        //stpprinter.ItemsSource = _printerList;
                    }
                }
            }

            if (accesories_template != null)
            {
                int _accessoryTag = Convert.ToInt16(accesories_template.id_tag);
                List<item> _itemList = entity.db.items.Where(x => entity.db.item_tag_detail
                    .Where(y => y.id_tag == _accessoryTag)
                    .Select(z => z.id_item).Contains(x.id_item)).ToList();

                foreach (item item in _itemList)
                {
                    int id_item = Convert.ToInt32(item.id_item);
                    if (id_item > 0)
                    {
                        PrintingPress.Accessory accessory = new PrintingPress.Accessory();
                        accessory.Id = (int)item.id_item;
                        accessory.Cost = (decimal)item.unit_cost;
                        accessory.Name = item.name;
                        accessory.Consumption = 1;
                        accessory.Calc_Cost = accessory.Cost * accessory.Consumption;
                        _accessoryList.Add(accessory);
                    }
                }
                itemaccesories.ItemsSource = _accessoryList.ToList();
            }

            if (toner_template != null)
            {
                int _tonerTag = Convert.ToInt16(toner_template.id_tag);
                item _toner = entity.db.items
                    .Where(x => entity.db.item_tag_detail
                        .Where(y => y.id_tag == _tonerTag)
                        .Select(z => z.id_item).Contains(x.id_item)).FirstOrDefault();
            }

            if (cut_template != null)
            {
                int _cutTag = Convert.ToInt16(cut_template.id_tag);
                item _cut = entity.db.items
                    .Where(x => entity.db.item_tag_detail
                        .Where(y => y.id_tag == _cutTag)
                        .Select(z => z.id_item).Contains(x.id_item)).FirstOrDefault();
            }

            if (paper_template != null)
            {
                int _paperTag = Convert.ToInt16(paper_template.id_tag);
                using (db db = new db())
                {
                    List<item> paperlist = entity.db.items
                        .Where(x => entity.db.item_tag_detail.Where(y => y.id_tag == _paperTag)
                            .Select(z => z.id_item)
                            .Contains(x.id_item)).ToList();

                    foreach (item _item in paperlist)
                    {
                        PrintingPress.Paper _paper = new PrintingPress.Paper();
                        _paper.Id = _item.id_item;

                        if (_item.item_dimension.Where(x => x.id_app_dimension == id_Weight).FirstOrDefault() != null)
                        {
                            _paper.Weight = (int)_item.item_dimension.Where(x => x.id_app_dimension == id_Weight).FirstOrDefault().value;
                        }
                        else
                        { _paper.Weight = 0; }

                        _paper.Cost = (decimal)_item.unit_cost;

                        if (_item.item_dimension.Where(x => x.id_app_dimension == id_Long).FirstOrDefault() != null)
                        {
                            _paper.Long = (decimal)_item.item_dimension.Where(x => x.id_app_dimension == id_Long).FirstOrDefault().value;
                        }
                        else
                        { _paper.Long = 0; }

                        if (_item.item_dimension.Where(x => x.id_app_dimension == id_Short).FirstOrDefault() != null)
                        {
                            _paper.Short = (decimal)_item.item_dimension.Where(x => x.id_app_dimension == id_Short).FirstOrDefault().value;
                        }
                        else
                        { _paper.Short = 0; }

                        _paper.Name = _paper.Long.ToString() + " x " + _paper.Short.ToString();
                        //Adding Paper to PaperList
                        _paperList.Add(_paper);
                    }
                }
            }
        }

        //private void txtcolorsTextChanged(object sender, TextChangedEventArgs e)
        //{
        //    if (_inkList != null)
        //    {
        //        _inkList.Clear();
        //        if (_color_Qty == 0)
        //        {
        //            _color_Qty = 1;
        //        }
        //        int TotalInk = Convert.ToInt16(_color_Qty);
        //        for (int i = 0; i < TotalInk; i++)
        //        {
        //            PrintingPress.Ink _ink = new PrintingPress.Ink();
        //            _inkList.Add(_ink);
        //        }
        //        if (itemink != null)
        //        {
        //            itemink.ItemsSource = _inkList;
        //        }
        //    }
        //}

        private void itemink_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            PrintingPress.Ink _ink = (PrintingPress.Ink)itemink.SelectedItem;
            if (_ink != null)
            {
                ((PrintingPress.Ink)itemink.SelectedItem).Cost = (decimal)entity.db.items.Where(x => x.id_item == _ink.Id).FirstOrDefault().unit_cost;
                ((PrintingPress.Ink)itemink.SelectedItem).RaisePropertyChanged("Cost");
            }
        }

        private void itemink_CurrentCellChanged(object sender, EventArgs e)
        {
            itemink.CommitEdit();
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            _printerList.Clear();
            _productList.Clear();
            _paperList.Clear();

            cmbweight.SelectedIndex = -1;
        }

        #endregion "Events"

        private void cmbweight_SelectionChanged(object sender, EventArgs e)
        {
            int weight = 0;
            if (cmbweight != null)
            {
                weight = Convert.ToInt16(cmbweight.SelectedValue);
            }

            List<PrintingPress.Paper> _filterpaper;
            _filterpaper = _paperList.Where(x => x.Long > _product_Long
                                            && x.Short > _product_Short
                                            && x.Weight == weight).ToList();
            if (_filterpaper != null && _filterpaper.Count() > 0)
            {
                stppaper.ItemsSource = _filterpaper;
            }
        }

        private void itemaccesories_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            PrintingPress.Accessory _Accessory = (PrintingPress.Accessory)itemaccesories.SelectedItem;
            if (_Accessory != null)
            {
                ((PrintingPress.Accessory)itemaccesories.SelectedItem).Calc_Cost = ((PrintingPress.Accessory)itemaccesories.SelectedItem).Cost * ((PrintingPress.Accessory)itemaccesories.SelectedItem).Consumption;
                ((PrintingPress.Accessory)itemaccesories.SelectedItem).RaisePropertyChanged("Calc_Cost");
            }
        }
    }
}