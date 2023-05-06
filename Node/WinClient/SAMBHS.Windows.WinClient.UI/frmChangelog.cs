using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Linq;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI
{
    public partial class frmChangelog : Form
    {
        private static readonly Image Normal = Image.FromFile(@"img\down-arrow-in-small-circle.png");
        private static readonly Image Medio = Image.FromFile(@"img\right-arrow.png");
        private static readonly Image Importante = Image.FromFile(@"img\up-arrow.png");

        private static List<Changelog> ObtenerLog
        {
            get
            {
                return
                    new List<Changelog>
                    {
                        new Changelog { Version = "1.0.9.202", Descripcion = "Implementación del log de cambios.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,4)},
                        new Changelog { Version = "1.0.9.202", Descripcion = "Optimización Registro Clientes-Proveedor, se desabilitó la edición del documento de Identidad , para mantener integridad de transacciones", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,5)},
                        new Changelog { Version = "1.0.9.207", Descripcion = "Se habilitó en la Compra Administrativa  la opción de Percepción.", ChangeScope = ChangeScope.Alta, Fecha = new DateTime(2017,7,6)},
                        new Changelog { Version = "1.0.9.207", Descripcion = "Optimización del login.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,6)},
                        new Changelog { Version = "1.0.9.208", Descripcion = "Se habilitó la opción Rectificación - PLE en el registro de Compras.", ChangeScope = ChangeScope.Alta, Fecha = new DateTime(2017,7,7)},
                        new Changelog { Version = "1.0.9.208", Descripcion = "Modificación del  PLE Compras para considerar la Rectificación PLE.", ChangeScope = ChangeScope.Alta, Fecha = new DateTime(2017,7,7)},
                        new Changelog { Version = "1.0.9.208", Descripcion = "Desarrollo del Reporte : Estados de Resultados por Función.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,7)},
                        new Changelog { Version = "1.0.9.208", Descripcion = "Desarrollo del Reporte : Estados de Resultados por Naturaleza.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,7)},
                        new Changelog { Version = "1.0.9.208", Descripcion = "Desarrollo del Reporte de Flujo Efectivo", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,7)},
                        new Changelog { Version = "1.0.9.208", Descripcion = "Modicación del Reporte Documentos Ingresados , en adelante se  visualizarán los Importes en Dolares del Debe y Haber", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,6,7)},
                        new Changelog { Version = "1.0.9.208", Descripcion = "Se agregó una nueva gama de anexos contables al sistema.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,7)},
                        new Changelog { Version = "1.0.9.211", Descripcion = "Implementación del Reporte : Estados de cambios en el patrimonio", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,8)},
                        new Changelog { Version = "1.0.9.215", Descripcion = "Desarrollo del Reporte Notas a los Estados Financieros.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,12)},
                        new Changelog { Version = "1.0.9.215", Descripcion = "Se habilitó el máximo número de items que deben contener los pedidos y cotizaciones", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,12)},
                        new Changelog { Version = "1.0.9.215", Descripcion = "Los pedidos que ya han sido facturados total o parcialmente no podrán ser editados.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,12)},
                        new Changelog { Version = "1.0.9.215", Descripcion = "Se desabilitó el botón Agregar Detalle de Venta ,cuando los detalles son extraidos desde un pedido", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,12)},
                        new Changelog { Version = "1.0.9.215", Descripcion = "Los titulos de los  Reporte  Libro Inventarios y Balances se modificaron de acuerdo al formato de la Sunat. ", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,12)},
                        new Changelog { Version = "1.0.9.227", Descripcion = "Desarrollo del reporte : Diario Simplificado", ChangeScope = ChangeScope.Alta, Fecha = new DateTime(2017,7,13)},
                        new Changelog { Version = "1.0.9.228", Descripcion = "Implementación: Motivos de eliminación para las ventas.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,13)},
                        new Changelog { Version = "1.0.9.228", Descripcion = "Implementación: Motivos de eliminación para las ventas.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,13)},
                        new Changelog { Version = "1.0.9.229", Descripcion = "Corrección: Ajuste de diferencia de cambio omitía las NCR a causa de una modificación anterior.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,14)},
                        new Changelog { Version = "1.0.9.230", Descripcion = "Corrección: Al editar un diario o tesorería mantiene el valor de el cambio editado manualmente.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,14)},
                        new Changelog { Version = "1.0.9.235", Descripcion = "Optimización: Consulta de pendientes por cobrar en cobranza mejorada tiempo de respuesta.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,17)},
                        new Changelog { Version = "1.0.9.235", Descripcion = "Modificación: Se modificó la interfaz de activo fijo ,ahora se puede asociar la foto del activo fijo  y se especificará el tipo de documento de compra", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,17)},
                        new Changelog { Version = "1.0.9.235", Descripcion = "Corrección:En el reporte de Ingresos Almacén analítico la +'Fecha del ' debe ser menor a la ' Fecha Al'", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,17)},
                        new Changelog { Version = "1.0.9.303", Descripcion = "Corrección:En Impresión Asiento Libro Diario , se cambió el nombre del encabezado de Detalle a Anexo", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,18)},
                        new Changelog { Version = "1.0.9.303", Descripcion = "Corrección:Las Notas de Crédito en Registro de Compra tomarán como referencia la fecha de Emisión  y el tipo de cambio del documento al que hacen referencia ", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,18)},
                        new Changelog { Version = "1.0.9.303", Descripcion = "Implementación :Se podrán adjuntar imagenes de los documentos asociados a los activos fijos", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,18)},
                        new Changelog { Version = "1.0.9.309", Descripcion = "Corrección: Al insertar una nueva fila en diario o tesoreria salía un error.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,20)},
                        new Changelog { Version = "1.0.9.310", Descripcion = "Corrección: Al buscar en registros de retención salia error.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,20)},
                        new Changelog { Version = "1.0.9.351", Descripcion = "Optimización: Al editar o eliminar una cobranza administrativa con letras en descuento, ahora notifica cúales son las letras..", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,21)},
                        new Changelog { Version = "1.0.9.355", Descripcion = "Implementación: Registro del ID de cada pc para una futura implementación de las licencias.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,24)},
                        new Changelog { Version = "1.0.9.355", Descripcion = "Optimización: En el Reporte Resumen de Almacén se agregó un filtro para visualizar solo Productos con Movimiento. ", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,24)},
                        new Changelog { Version = "1.0.9.371", Descripcion = "Desarrollo del Reporte: Análisis de Gastos por Función Analítico.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,25)},
                        new Changelog { Version = "1.0.9.372", Descripcion = "Optimización: Lista Precios Utilidades y Descuentos filtra por Descripción o código.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,25)},
                        new Changelog { Version = "1.0.9.372", Descripcion = "Optimización: En el  Reporte Pedidos Pedidos , se agregó filtro de Agrupamiento.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,7,25)},
                        new Changelog { Version = "1.0.9.373", Descripcion = "Optimización: Se ajustó de manera más precisa la conversión a moneda extranjera en las compras al pasar a contabilidad.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,7,25)},
                        new Changelog { Version = "1.0.10.14", Descripcion = "Desarrollo del Reporte: Análisis de Gastos por Función Resumen ", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,1)},
                        new Changelog { Version = "1.0.10.14", Descripcion = "Desarrollo del Reporte: Libro Balance", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,1)},
                        new Changelog { Version = "1.0.10.17", Descripcion = "Corrección: Asiento contable autogenerado de venta con anticipo corregido.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,2)},
                        new Changelog { Version = "1.0.10.17", Descripcion = "Corrección: Extracción de venta con distintos destinos en el detalle ahora se converva en la venta nueva.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,2)},
                        new Changelog { Version = "1.0.10.18", Descripcion = "Corrección: Al digitar manualmente una cuenta con centro de costos en la compra no habilitaba la casilla para ingresar el centro de costo.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,3)},
                        new Changelog { Version = "1.0.10.34", Descripcion = "Modificación: Se agregaron algunos Puertos Destinos para la Venta", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,4)},
                        new Changelog { Version = "1.0.10.42", Descripcion = "Modificación: Se agregaron algunos Puertos Destinos para la Venta", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,8)},
                        new Changelog { Version = "1.0.10.42", Descripcion = "Modificación: Reporte Resumen de Almacen , los Valores se reducieron a dos decimales", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,8)},
                        new Changelog { Version = "1.0.10.44", Descripcion = "Desarrollo del Reporte: Libro de Inventarios y Balance : 50 Capital", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,8)},
                        new Changelog { Version = "1.0.10.44", Descripcion = "Modificación: Registro de Compra Contable - Administrativa , Cambio en la secuencia del cursor al momento de registrar una NCR/NDB", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,8)},
                        new Changelog { Version = "1.0.10.44", Descripcion = "Optimización: del Reporte Planilla Oficial", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,8)},
                        new Changelog { Version = "1.0.10.48", Descripcion = "Corrección: Corrección del recalculo de Stocks, tomaba en cuenta pedidos anulados.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,9)},
                        new Changelog{ Version = "1.0.10.49", Descripcion = "Optimización:Se disminuió el tiempo de  Carga de  una Nota de Salida  ", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,9)},
                        new Changelog{ Version = "1.0.10.50", Descripcion = "Correción:Reporte Resumen de Almacén .. La opción 'Solo productos con movimiento' también tomará los productos que tienen saldo.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,9)},
                        new Changelog{ Version = "1.0.10.59", Descripcion = "Optimización : Del proceso Separación de Stock", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,11)},
                        new Changelog{ Version = "1.0.10.59", Descripcion = "Optimización : Del Reporte Pedidos Pedientes,se agregó la opción de Formato de Cantidad", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,11)},
                        new Changelog{ Version = "1.0.10.63", Descripcion = "Optimización : Se mejoró la actualización por internet del tipo de cambio de acuerdo al mes seleccionado.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,12)},
                        new Changelog{ Version = "1.0.10.65", Descripcion = "Corrección : Se corrigió el inconveniente de que algunas veces se generaba letras de un canje con tipo de cambio 0.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,14)},
                        new Changelog{ Version = "1.0.10.65", Descripcion = "Optimización :Se agilizó el procedimiento para imprimir los Documentos de Venta", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,14)},
                        new Changelog{ Version = "1.0.10.78", Descripcion = "Modificación :Se agregó al Reporte de Compras Sunat la opción de Formato de Impresión de la Estrtura", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,17)},
                        new Changelog{ Version = "1.0.10.99", Descripcion = "Corrección:Cuando se convierte una Factura en Guia de Remision , si existe el mismo producto n veces en la factura , este se sumariza en la Guia", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,17)},
                        
                        new Changelog{ Version = "1.0.10.107", Descripcion = "Modificación:Se agregó la opción para liberar la separación del producto", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.101", Descripcion = "Mejora: La importación de ventas por excel fue reestructurada y ahora importa más rapido.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.101", Descripcion = "Implementación: Las observaciones de los detalles de las ventas se ingresan por un formulario para ayudar si ingreso.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.102", Descripcion = "Corrección: Se cambio el método transaccional de las operaciones para evitar registros con números duplicados en las ventas cuando guardan en el mismo segundo.", ChangeScope = ChangeScope.Alta, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.102", Descripcion = "Implementación: Se implementó el borrado de ventas masivas por mes y criterio.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.103", Descripcion = "Implementación: Regeneración de asientos de ventas por mes.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.103", Descripcion = "Implementación: Se agregó la opción para poner un Anexo en el detalle de una compra para poder analizarlo en contabilidad.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.103", Descripcion = "Corrección: Se adaptó la serie de las ventas y compras para que puedan aceptar el nro de máquina registradora de los tickets.", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.107", Descripcion = "Modificación:Se agregó la opción para liberar la separación del producto", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,8,24)},
                        new Changelog{ Version = "1.0.10.117", Descripcion = "Modificación:En el Reporte de Kardex se tomarán en  cuentas las guia de remision de los detalles o  de las cabeceras", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,25)},
                        new Changelog{ Version = "1.0.10.140", Descripcion = "Modificación:En el Reporte Registro de Compras Sunat se agregó el filtro de Establecimiento", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,8,25)},
                        new Changelog{ Version = "1.0.10.210", Descripcion = "Mejora : Se habilitó la fecha vencimiento para todos los documentos de Registro de Compra", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,9,20)},
                        new Changelog{ Version = "1.0.10.210", Descripcion = "Optimización : Optimización del Reporte Kardex y Stock (Almacén)", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,9,20)},
                        new Changelog{ Version = "1.0.11.11", Descripcion = "Mejora :Se agregó la opción de estado (Activo-Inactivo) en el formulario Vendedor", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,9,20)},
                        new Changelog{ Version = "1.0.11.16", Descripcion = "Mejora :Se agregó la opción de Lotes  para el ingresos y salidas del almacén.", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,9,20)},
                        new Changelog{ Version = "1.0.11.16", Descripcion = "Mejora :Se agregó la extracción de Importación desde una Orden Compra", ChangeScope = ChangeScope.Baja, Fecha = new DateTime(2017,9,20)},
                        new Changelog{ Version = "1.0.11.25", Descripcion = "Mejora:En el reporte de Libro Diario se agregó la opción para poder visualizar de forma resumida algunos asientos", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,10,4)},
                        new Changelog{ Version = "1.0.11.47", Descripcion = "Mejora:En el reporte de Libro Mayor se agregó la opción para poder visualizar de forma resumida algunos asientos", ChangeScope = ChangeScope.Media, Fecha = new DateTime(2017,10,12)},
                    };
            }
        }

        public frmChangelog()
        {
            InitializeComponent();
            BackColor = new GlobalFormColors().FormColor;
        }

        private void frmChangelog_Load(object sender, EventArgs e)
        {
            ultraGrid1.DataSource = ObtenerLog.OrderByDescending(o => o.Fecha).ThenBy(o => o.Version).ToList();
        }

        private class Changelog
        {
            public string Version { get; set; }
            public string Descripcion { get; set; }
            public DateTime Fecha { get; set; }
            public ChangeScope ChangeScope { private get; set; }
            public Image Icono
            {
                get
                {
                    switch (ChangeScope)
                    {
                        case ChangeScope.Baja:
                            return Normal;

                        case ChangeScope.Media:
                            return Medio;

                        case ChangeScope.Alta:
                            return Importante;

                        default:
                            return null;
                    }
                }
            }
        }

        private enum ChangeScope
        {
            Baja,
            Media,
            Alta
        }
    }
}
