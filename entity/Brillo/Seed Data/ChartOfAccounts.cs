using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entity.Brillo.Seed_Data
{
    public class ChartOfAccounts
    {
        private accounting_chart CreateChart(string ChartCode, string ChartName, accounting_chart ChartParent)
        {
            accounting_chart chart = new accounting_chart();
            
            if(ChartParent != null)
            {
                chart.parent = ChartParent;
            }

            chart.name = ChartName;
            chart.code = ChartCode;

            chart.id_company = CurrentSession.Id_Company;
            chart.id_user = CurrentSession.Id_User;
            chart.is_active = true;

            return chart;
        }

        public void Paraguay()
        {
            using (AccountingChartDB db = new AccountingChartDB())
            {
                db.accounting_chart.Add(CreateChart("1", "ACTIVO", null));
                db.accounting_chart.Add(CreateChart("1.01", "ACTIVO CORRIENTE", db.accounting_chart.Where(x => x.code == "1").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.01", "DISPONIBILIDADES", db.accounting_chart.Where(x => x.code == "1.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.01.01", "RECAUDACIONES A DEPOSITAR", db.accounting_chart.Where(x => x.code == "1.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.01.02", "CAJA", db.accounting_chart.Where(x => x.code == "1.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.01.03", "FONDOS FIJOS ", db.accounting_chart.Where(x => x.code == "1.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.01.04", "BANCOS", db.accounting_chart.Where(x => x.code == "1.01.01").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.01.02", "INVERSIONES TEMPORARIAS", db.accounting_chart.Where(x => x.code == "1.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.02.01", "INVERSIONES FINANCIERAS", db.accounting_chart.Where(x => x.code == "1.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.02.02", "INVERSIONES EN ENTIDADES VINCULADAS", db.accounting_chart.Where(x => x.code == "1.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.02.03", "OTRAS INVERSIONES", db.accounting_chart.Where(x => x.code == "1.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.02.04", "INTERESES, REGALÍAS Y OTROS RENDIMIENTOS DE INVERSIONES", db.accounting_chart.Where(x => x.code == "1.01.02").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.01.03", "CRÉDITOS", db.accounting_chart.Where(x => x.code == "1.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.01", "DEUDORES POR VENTAS", db.accounting_chart.Where(x => x.code == "1.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.02", "DEUDORES POR PRÉSTAMOS", db.accounting_chart.Where(x => x.code == "1.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.03", "CUENTAS A COBRAR A DIRECTORES Y FUNCIONARIOS", db.accounting_chart.Where(x => x.code == "1.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.04", "CUENTAS A COBRAR A SOCIOS O A ENTIDADES VINCULADAS", db.accounting_chart.Where(x => x.code == "1.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.05", "CRÉDITOS POR IMPUESTOS CORRIENTES", db.accounting_chart.Where(x => x.code == "1.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.05.01", "ANTICIPOS Y RETENCIONES DE IMPUESTO A LA RENTA", db.accounting_chart.Where(x => x.code == "1.01.03.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.05.02", "RETENCIONES DE IVA", db.accounting_chart.Where(x => x.code == "1.01.03.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.05.03", "IVA - CRÉDITO FISCAL", db.accounting_chart.Where(x => x.code == "1.01.03.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.05.04", "IVA - CRÉDITO FISCAL - RÉGIMEN DE TURISMO", db.accounting_chart.Where(x => x.code == "1.01.03.05").FirstOrDefault()));
                
                db.accounting_chart.Add(CreateChart("1.01.03.06", "ANTICIPO A PROVEEDORES", db.accounting_chart.Where(x => x.code == "1.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.06.01", "ANTICIPOS A PROVEEDORES LOCALES", db.accounting_chart.Where(x => x.code == "1.01.03.06").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.03.06.02", "ANTICIPOS A PROVEEDORES DEL EXTERIOR", db.accounting_chart.Where(x => x.code == "1.01.03.06").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.01.04", "INVENTARIOS", db.accounting_chart.Where(x => x.code == "1.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.01", "MERCADERÍAS", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.01.01", "MERCADERÍAS GRAVADAS POR EL IVA AL 10%", db.accounting_chart.Where(x => x.code == "1.01.04.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.01.02", "MERCADERÍAS GRAVADAS POR EL IVA AL 5%", db.accounting_chart.Where(x => x.code == "1.01.04.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.01.03", "MERCADERÍAS EXENTAS DEL IVA", db.accounting_chart.Where(x => x.code == "1.01.04.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.02", "MERCADERÍAS REGÍMENES ESPECIALES", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.02.01", "MERCADERÍAS RÉGIMEN DE TURISMO", db.accounting_chart.Where(x => x.code == "1.01.04.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.02.02", "MERCADERÍAS ZONA FRANCA", db.accounting_chart.Where(x => x.code == "1.01.04.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.02.03", "MERCADERÍAS RÉGIMEN DE MAQUILA", db.accounting_chart.Where(x => x.code == "1.01.04.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.02.04", "MERCADERÍAS OTROS REGÍMENES ESPECIALES", db.accounting_chart.Where(x => x.code == "1.01.04.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.03", "PRODUCTOS TERMINADOS", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.03.01", "PRODUCTOS TERMINADOS GRAVADOS POR EL IVA AL 10%", db.accounting_chart.Where(x => x.code == "1.01.04.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.03.02", "PRODUCTOS TERMINADOS GRAVADOS POR EL IVA AL 5%", db.accounting_chart.Where(x => x.code == "1.01.04.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.03.03", "PRODUCTOS TERMINADOS EXENTOS DEL IVA", db.accounting_chart.Where(x => x.code == "1.01.04.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.04", "PRODUCTOS EN PROCESO", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.05", "MATERIAS PRIMAS", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.06", "MATERIALES, SUMINISTROS Y REPUESTOS", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.07", "PRODUCTOS AGRÍCOLAS", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.08", "PRODUCTOS FORESTALES", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.09", "ACTIVOS BIOLÓGICOS EN PRODUCCIÓN", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.10", "ACTIVOS BIOLÓGICOS EN DESARROLLO", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.04.11", "IMPORTACIONES EN CURSO", db.accounting_chart.Where(x => x.code == "1.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.05", "GASTOS PAGADOS POR ADELANTADO", db.accounting_chart.Where(x => x.code == "1.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.05.01", "ALQUILERES PAGADOS POR ADELANTADO", db.accounting_chart.Where(x => x.code == "1.01.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.05.01", "SEGUROS A DEVENGAR", db.accounting_chart.Where(x => x.code == "1.01.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.01.06", "OTROS ACTIVOS", db.accounting_chart.Where(x => x.code == "1.01").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.02", "ACTIVO NO CORRIENTE", db.accounting_chart.Where(x => x.code == "1").FirstOrDefault()));
                
                db.accounting_chart.Add(CreateChart("1.02.01", "CRÉDITOS A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "1.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.01", "DEUDORES POR VENTAS", db.accounting_chart.Where(x => x.code == "1.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.02", "DEUDORES POR PRÉSTAMOS", db.accounting_chart.Where(x => x.code == "1.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.03", "CUENTAS A COBRAR A DIRECTORES Y FUNCIONARIOS", db.accounting_chart.Where(x => x.code == "1.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.04", "CUENTAS A COBRAR A SOCIOS O ENTIDADES VINCULADAS", db.accounting_chart.Where(x => x.code == "1.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.05", "DEUDORES EN GESTIÓN DE COBRO – MOROSOS O SIMILARES", db.accounting_chart.Where(x => x.code == "1.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.06", "ANTICIPOS A PROVEEDORES", db.accounting_chart.Where(x => x.code == "1.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.06.01", "ANTICIPOS A PROVEEDORES LOCALES", db.accounting_chart.Where(x => x.code == "1.02.01.06").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.06.02", "ANTICIPOS A PROVEEDORES DEL EXTERIOR", db.accounting_chart.Where(x => x.code == "1.02.01.06").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.01.07", "CRÉDITOS POR IMPUESTOS DIFERIDOS", db.accounting_chart.Where(x => x.code == "1.02.01").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.02.02", "INVENTARIOS A REALIZAR A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "1.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.02.01", "ACTIVOS BIOLÓGICOS EN PRODUCCIÓN", db.accounting_chart.Where(x => x.code == "1.02.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.02.02", "ACTIVOS BIOLÓGICOS EN DESARROLLO", db.accounting_chart.Where(x => x.code == "1.02.02").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.02.03", "INVERSIONES A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "1.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.03.01", "INVERSIONES FINANCIERAS", db.accounting_chart.Where(x => x.code == "1.02.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.03.02", "INVERSIONES EN ENTIDADES VINCULADAS", db.accounting_chart.Where(x => x.code == "1.02.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.03.03", "INVERSIONES INMOBILIARIAS", db.accounting_chart.Where(x => x.code == "1.02.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.03.04", "OTRAS INVERSIONES", db.accounting_chart.Where(x => x.code == "1.02.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.03.05", "INTERESES, REGALÍAS Y OTROS RENDIMIENTOS DE INVERSIONES", db.accounting_chart.Where(x => x.code == "1.02.03").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.02.04", "PROPIEDAD, PLANTA Y EQUIPO", db.accounting_chart.Where(x => x.code == "1.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.01", "INMUEBLES", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.02", "RODADOS /TRANSPORTES", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.03", "MUEBLES, ÚTILES Y ENSERES", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.04", "MAQUINARIAS", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.05", "EQUIPOS", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.06", "HERRAMIENTAS", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.07", "BIENES FUERA DE OPERACIÓN", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.08", "MEJORAS EN PREDIO AJENO", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.97", "BIENES INCORPORADOS AL AMPARO DE LA LEY N° 60/90", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.04.98", "BIENES EN ARRENDAMIENTO FINANCIERO", db.accounting_chart.Where(x => x.code == "1.02.04").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.02.05", "OTROS ACTIVOS A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "1.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.05.01", "DERECHOS FIDUCIARIOS", db.accounting_chart.Where(x => x.code == "1.02.05").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.02.06", "CARGOS DIFERIDOS", db.accounting_chart.Where(x => x.code == "1.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.06.01", "GASTOS DE CONSTITUCIÓN", db.accounting_chart.Where(x => x.code == "1.02.06").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.06.02", "GASTOS DE ORGANIZACIÓN", db.accounting_chart.Where(x => x.code == "1.02.06").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.06.03", "GASTOS DE DESARROLLO", db.accounting_chart.Where(x => x.code == "1.02.06").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.06.04", "GASTOS DE PROYECTOS DE INVERSIÓN", db.accounting_chart.Where(x => x.code == "1.02.06").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("1.02.07", "ACTIVOS INTANGIBLES", db.accounting_chart.Where(x => x.code == "1.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.07.01", "LICENCIAS, MARCAS Y PATENTES", db.accounting_chart.Where(x => x.code == "1.02.07").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("1.02.07.02", "FRANQUICIAS", db.accounting_chart.Where(x => x.code == "1.02.07").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("2", "PASIVO", null));
                db.accounting_chart.Add(CreateChart("2.01", "PASIVO CORRIENTE", db.accounting_chart.Where(x => x.code == "2").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.01", "ACREEDORES COMERCIALES", db.accounting_chart.Where(x => x.code == "2.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.01.01", "PROVEEDORES LOCALES", db.accounting_chart.Where(x => x.code == "2.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.01.02", "PROVEEDORES DEL EXTERIOR", db.accounting_chart.Where(x => x.code == "2.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.01.03", "INTERESES A PAGAR A PROVEEDORES", db.accounting_chart.Where(x => x.code == "2.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.01.04", "OTROS ACREEDORES", db.accounting_chart.Where(x => x.code == "2.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.02", "DEUDAS FINANCIERAS", db.accounting_chart.Where(x => x.code == "2.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.02.01", "PRÉSTAMOS DE BANCOS Y OTRAS ENTIDADES FINANCIERAS", db.accounting_chart.Where(x => x.code == "2.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.02.02", "PRÉSTAMOS DEL DUEÑO, SOCIOS O  ENTIDADES VINCULADAS", db.accounting_chart.Where(x => x.code == "2.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.02.03", "ARRENDAMIENTOS FINANCIEROS", db.accounting_chart.Where(x => x.code == "2.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.02.04", "OTROS PRÉSTAMOS A PAGAR", db.accounting_chart.Where(x => x.code == "2.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.02.05", "INTERESES A PAGAR", db.accounting_chart.Where(x => x.code == "2.01.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03", "OTRAS CUENTAS POR PAGAR", db.accounting_chart.Where(x => x.code == "2.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03.01", "PRÉSTAMOS DE BANCOS Y OTRAS ENTIDADES FINANCIERAS", db.accounting_chart.Where(x => x.code == "2.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03.01.01", "IMPUESTO A LA RENTA A PAGAR", db.accounting_chart.Where(x => x.code == "2.01.03.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03.01.02", "IVA A PAGAR", db.accounting_chart.Where(x => x.code == "2.01.03.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03.01.03", "RETENCIONES DE IMPUESTOS A INGRESAR", db.accounting_chart.Where(x => x.code == "2.01.03.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03.02", "OBLIGACIONES LABORALES Y CARGAS SOCIALES", db.accounting_chart.Where(x => x.code == "2.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03.03", "DIVIDENDOS A PAGAR", db.accounting_chart.Where(x => x.code == "2.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.03.04", "PASIVOS FIDUCIARIOS", db.accounting_chart.Where(x => x.code == "2.01.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.04", "PROVISIONES", db.accounting_chart.Where(x => x.code == "2.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.04.01", "OBLIGACIONES POR GARANTÍAS OTORGADAS", db.accounting_chart.Where(x => x.code == "2.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.04.02", "OBLIGACIONES LEGALES POR LITIGIOS", db.accounting_chart.Where(x => x.code == "2.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.04.03", "PROVISIÓN PARA BENEFICIOS A EMPLEADOS", db.accounting_chart.Where(x => x.code == "2.01.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.05", "INGRESOS DIFERIDOS", db.accounting_chart.Where(x => x.code == "2.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.05.01", "ANTICIPOS DE CLIENTES", db.accounting_chart.Where(x => x.code == "2.01.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.05.02", "SUBVENCIONES", db.accounting_chart.Where(x => x.code == "2.01.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.05.03", "DERECHOS DE FIDELIZACIÓN DE CLIENTES", db.accounting_chart.Where(x => x.code == "2.01.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.01.05.04", "ALQUILERES COBRADOS POR ADELANTADO", db.accounting_chart.Where(x => x.code == "2.01.05").FirstOrDefault()));
                
                db.accounting_chart.Add(CreateChart("2.02", "PASIVO NO CORRIENTE", db.accounting_chart.Where(x => x.code == "2").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.01", "ACREEDORES COMERCIALES A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "2.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.01.01", "PROVEEDORES LOCALES", db.accounting_chart.Where(x => x.code == "2.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.01.02", "PROVEEDORES DEL EXTERIOR", db.accounting_chart.Where(x => x.code == "2.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.01.03", "INTERESES A  PAGAR A PROVEEDORES", db.accounting_chart.Where(x => x.code == "2.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.01.04", "OTROS ACREEDORES", db.accounting_chart.Where(x => x.code == "2.02.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.02", "DEUDAS FINANCIERAS A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "2.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.02.01", "PRÉSTAMOS DE BANCOS Y OTRAS ENTIDADES FINANCIERAS", db.accounting_chart.Where(x => x.code == "2.02.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.02.02", "PRÉSTAMOS DEL DUEÑO, SOCIOS O  ENTIDADES VINCULADAS", db.accounting_chart.Where(x => x.code == "2.02.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.02.03", "ARRENDAMIENTOS FINANCIERO", db.accounting_chart.Where(x => x.code == "2.02.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.02.04", "OTROS PRÉSTAMOS", db.accounting_chart.Where(x => x.code == "2.02.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.02.05", "INTERESES A PAGAR", db.accounting_chart.Where(x => x.code == "2.02.02").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("2.02.03", "OTRAS CUENTAS POR PAGAR", db.accounting_chart.Where(x => x.code == "2.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.03.01", "PASIVOS POR IMPUESTOS DIFERIDOS", db.accounting_chart.Where(x => x.code == "2.02.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.03.02", "PASIVOS FIDUCIARIOS", db.accounting_chart.Where(x => x.code == "2.02.03").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("2.02.04", "PROVISIONES PARA OBLIGACIONES A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "2.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.04.01", "OBLIGACIONES POR GARANTÍAS OTORGADAS", db.accounting_chart.Where(x => x.code == "2.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.04.02", "OBLIGACIONES LEGALES POR LITIGIOS PENDIENTES", db.accounting_chart.Where(x => x.code == "2.02.04").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.04.03", "PROVISIONES PARA BENEFICIOS A EMPLEADOS", db.accounting_chart.Where(x => x.code == "2.02.04").FirstOrDefault()));

                db.accounting_chart.Add(CreateChart("2.02.05", "INGRESOS DIFERIDOS A LARGO PLAZO", db.accounting_chart.Where(x => x.code == "2.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.05.01", "ANTICIPOS DE CLIENTES", db.accounting_chart.Where(x => x.code == "2.02.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.05.02", "SUBVENCIONES", db.accounting_chart.Where(x => x.code == "2.02.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.05.03", "DERECHO DE FIDELIZACIÓN DE CLIENTES", db.accounting_chart.Where(x => x.code == "2.02.05").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("2.02.05.04", "ALQUILERES COBRADOS POR ADELANTADO", db.accounting_chart.Where(x => x.code == "2.02.05").FirstOrDefault()));
                
                db.accounting_chart.Add(CreateChart("3", "PATRIMONIO NETO", null));
                db.accounting_chart.Add(CreateChart("3.01", "CAPITAL", db.accounting_chart.Where(x => x.code == "3").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.01.01", "CAPITAL INTEGRADO", db.accounting_chart.Where(x => x.code == "3.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.01.01.01", "CAPITAL SUSCRIPTO", db.accounting_chart.Where(x => x.code == "3.01.01").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.02", "RESERVAS", db.accounting_chart.Where(x => x.code == "3").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.02.01", "RESERVA LEGAL", db.accounting_chart.Where(x => x.code == "3.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.02.02", "RESERVA DE REVALÚO", db.accounting_chart.Where(x => x.code == "3.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.02.02.01", "RESERVA DE REVALÚO FISCAL", db.accounting_chart.Where(x => x.code == "3.02.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.02.02.02", "RESERVA DE REVALÚO TÉCNICO", db.accounting_chart.Where(x => x.code == "3.02.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.02.03", "OTRAS RESERVAS", db.accounting_chart.Where(x => x.code == "3.02").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.03", "RESULTADOS", db.accounting_chart.Where(x => x.code == "3").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.03.01", "RESULTADOS ACUMULADOS", db.accounting_chart.Where(x => x.code == "3.03").FirstOrDefault()));
                db.accounting_chart.Add(CreateChart("3.03.02", "RESULTADO DEL EJERCICIO", db.accounting_chart.Where(x => x.code == "3.03").FirstOrDefault()));
                db.SaveChangesAsync();
            }											
        }

        public void India()
        {

        }
    }
}
