using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cognitivo.Project.PrintingPress;
using entity;

namespace Cognitivo.Project.PrintingPress
{
    public class calc_Cost
    {
        public Paper _paper { get; set; }
        public Page _page { get; set; }
        public Product _product { get; set; }
        public Printer _printer { get; set; }
        public Toner _toner { get; set; }
        public Cut _cut { get; set; }

        public decimal cost_Accessory { get; set; }
        public decimal cost_Paper { get; set; }
        public decimal cost_Printer { get; set; }
        public decimal cost_Toner { get; set; }
        public decimal cost_Ink { get; set; }
        public decimal cost_Cut { get; set; }
        public decimal cost { get; set; }

        public bool _individual_Change { get; set; }
        public bool _individual_Double { get; set; }

        public void calc_Automatic( ref List<PrintingPress.Ink> _ink, 
                                    ref List<PrintingPress.Accessory> _accessory,
                                    bool individual_Change, bool individual_Double )
        {
            //Storing Values
            _individual_Change = individual_Change;
            _individual_Double = individual_Double;

            int _straight_Qty
                = get_Qty(_page.Short - 1.5M, _page.Long, _product.Long + 0.3M, _product.Short + 0.3M);
            int _cross_Qty
                = get_Qty(_page.Long, _page.Short - 1.5M, _product.Long + 0.3M, _product.Short + 0.3M);
            int _best_Qty;

            if (_straight_Qty > _cross_Qty)
            {
                _best_Qty = _straight_Qty;
            }
            else
            {
                _best_Qty = _straight_Qty;
            }

            if (_best_Qty > 0)
            {

                _page.Qty_Fit = _best_Qty;

                SByte _decimal = 1;
                if (_product.Double) { _decimal = 2; }

                if (individual_Change)
                {
                    _printer.Runs = _product.Change * _decimal;
                }
                else
                {
                    if (_straight_Qty > 0)
                    {
                        _printer.Runs = (int)Math.Ceiling((decimal)(_product.Change * _decimal) / _straight_Qty);
                    }
                    else
                    {
                        _printer.Runs = (int)Math.Ceiling((decimal)(_product.Change * _decimal));
                    }
                }

                //Calculate times Printer will Run based on Color Limits * Printer Runs (based on Design)
                _printer.Runs = (int)Math.Ceiling((decimal)_product.Color / Convert.ToInt16(_printer.Color_Limit) * _printer.Runs);

                //Calculates Page Wastage based on 200 pages per Runs.
                if (individual_Double)
                {
                    //Note> Printer Runs + Wastage does not make sense. 
                    _page.Qty_Waste = _printer.Runs + 200;
                    //Single cost of Paper
                    //Single cost of Toner
                }
                else
                {
                    _page.Qty_Waste = _printer.Runs * 200;
                    //Multiple cost of paper
                    //Multiple cost of Toner
                }

                calc_Paper();
                calc_Printer(ref _ink);
                //calc_Color();
                calc_Ink(ref _ink);
                calc_Toner(_toner);
                calc_Accessory(ref _accessory);
            }
        }

        public void calc_Paper()
        {
            if (_page.Qty_Fit != 0) 
            {
                //Find out amount of Pages needed for this work.
                _page.Qty = (int)Math.Ceiling((decimal)_product.Qty / _page.Qty_Fit);
                //Add wastage of about 200 units per work.
                _page.Qty += _page.Qty_Waste;

                if (_paper.Long != 0 || _paper.Short != 0 ||
                    _page.Long != 0 || _page.Short != 0)
                {
                    //Calculate how many Pages FIT into a Paper.
                    int _paperFit_Straight = get_Qty(_paper.Short, _paper.Long, _page.Short, _page.Long);
                    int _paperFit_Cross = get_Qty(_paper.Long, _paper.Short, _page.Short, _page.Long);

                    //Add check to see if its max. if max, try to get exact consumtion without wastage.
                    if (_paperFit_Straight > _paperFit_Cross)
                    {
                        _paper.Qty_Fit = _paperFit_Straight;
                    } else {
                        _paper.Qty_Fit = _paperFit_Cross;
                    }

                    //Calculate how many Papers we will need.
                    _paper.Qty = (int)Math.Ceiling((decimal)_page.Qty / _paper.Qty_Fit);
                    //Multiply Paper Qty with Cost.
                    cost_Paper = _paper.Qty * (decimal)_paper.Cost;
                    cost += cost_Paper;
                    //_paper.Calc_Cost = cost_Paper;
                }
            }
        }

        public void calc_Printer(ref List<PrintingPress.Ink> _ink)
        {
            for (int i = 0; i <= _printer.Runs - 1; i++)
            {
                //Calculates the Time needed to print Work
                _printer.Time = (_page.Qty / Convert.ToDecimal(_printer.Speed) + (0.5M)) * _printer.Runs;
                // + (0.5M)
                //Calculates Cost by * DietSet cost by Runs. then Add Printer Cost * Time. Added to original value.
                cost_Printer += (Convert.ToDecimal(_printer.Cost_DieSet * _ink.Count()) * _printer.Runs)
                                + (Convert.ToDecimal(_printer.Cost)
                                * _printer.Time);
                cost += cost_Printer;
                //_printer.Calc_Cost = cost_Printer;
            }
        }

        public void calc_Ink(ref List<PrintingPress.Ink> _ink)
        {
            cost_Ink = 0;

            foreach(Ink ink in _ink)
            {
                if(ink.Id != 0 && ink.Consumption != 0 )
                {
                    decimal consumption = (decimal)ink.Consumption / 100;
                    cost_Ink += Convert.ToDecimal(ink.Cost * (_product.Long * _product.Short) * _product.Qty) * (decimal)consumption;
                    cost += cost_Ink;
                }
            }
        }

        public void calc_Toner(Toner _toner)
        {
            cost_Toner = 0;
            if (_toner!=null)
            {
                cost_Toner = Convert.ToDecimal((_toner.Cost * (_product.Long * _product.Short) * _page.Qty_Fit));
                cost += cost_Toner;
            }
        }

        public void calc_Accessory(ref List<PrintingPress.Accessory> _accessory)
        {
            cost_Accessory = 0;
            foreach (Accessory accessory in _accessory)
            {
                if(accessory.Id != 0 && accessory.Consumption != 0)
                {
                    cost_Accessory += Convert.ToDecimal(accessory.Cost * accessory.Consumption);
                    cost += cost_Accessory;
                }
            }
        }

        #region "Helper Methods"

        /// <summary>
        /// Calculates how many items can fit inside parent. This includes Straight with Crossed Fitted.
        /// </summary>
        /// <param name="parent_Short">Height of Parent</param>
        /// <param name="parent_Long">Width of Parent</param>
        /// <param name="child_Short">Height of Child</param>
        /// <param name="child_Long">Width of Child</param>
        /// <returns>Number of Items</returns>
        private int get_Qty(decimal parent_Short, decimal parent_Long,
                            decimal child_Short, decimal child_Long)
        { 
            int QtyStraight = (int)(get_Consumption(parent_Short, child_Short) * get_Consumption(parent_Long, child_Long));
            int QtyCross = 0;

            decimal leftover_Short = parent_Short - (get_Consumption(parent_Short, child_Short) * child_Short);
            decimal leftover_Long = parent_Long - (get_Consumption(parent_Long, child_Long) * child_Long);

            if (leftover_Long > 0 || leftover_Short > 0)
            {
                if (leftover_Short > leftover_Long)
                {
                    //Short is Bigger
                    QtyCross = (int)(get_Consumption(leftover_Short, child_Long) * get_Consumption(parent_Long, child_Short));
                }
                else if (leftover_Long > leftover_Short)
                {
                    //Long is Bigger
                    QtyCross = (int)(get_Consumption(leftover_Long, child_Short) * get_Consumption(parent_Short, child_Long));
                }
            }

            return QtyStraight + QtyCross;
        }

        private int get_Consumption(decimal parent, decimal child)
        {
            decimal result = parent / child;
            return (int)Math.Floor(result);
        }
        #endregion

    }
}
