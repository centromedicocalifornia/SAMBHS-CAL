using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Infragistics.Documents.Excel;
using System.Drawing;
using Globals = SAMBHS.Common.Resource.Globals;
using SAMBHS.Common.Resource;

namespace SAMBHS.Windows.WinClient.UI.Reportes
{
    #region ExcelReport
    /// <summary>
    /// Reporte en Excel
    /// </summary>
    internal class ExcelReport
    {
        #region Fields
        protected readonly Workbook Wbook;
        protected readonly DataTable Source;
        protected int PosRow;
        private int _maxLevelHeader;
        private string[] _cols;
        private readonly Dictionary<int, Queue<int>> _indices = new Dictionary<int, Queue<int>>();
        private Point _start;
        private int _numGroups;
        private int _withHeader;
        #endregion

        #region Events
        /// <summary>
        /// Ocurre cuando finaliza la Escritura de una Seccion.
        /// </summary>
        public event EventHandler<ExcelReportSectionEventArgs> EndSection;
        /// <summary>
        /// Ocurre cuando finaliza la Escritura de un Groupo.
        /// </summary>
        public event EventHandler<ExcelReportSectionGroupEventArgs> EndSectionGroup;

        #region Trigger
        /// <summary>
        /// Dispara el evento <see cref="EndSection"/>
        /// </summary>
        /// <param name="args">Argumento para el Evento</param>
        protected void OnEndSection(ExcelReportSectionEventArgs args)
        {
            if (EndSection != null)
                EndSection(this, args);
        }
        /// <summary>
        /// Dispara el evento <see cref="EndSectionGroup"/>
        /// </summary>
        /// <param name="args">Argumento para el Evento</param>
        protected void OnEndSectionGroup(ExcelReportSectionGroupEventArgs args)
        {
            if (EndSectionGroup != null)
                EndSectionGroup(this, args);
        }
        #endregion
        #endregion

        #region Construct

        /// <summary>
        /// Crea una Instancia de <see cref="ExcelReport"/>
        /// </summary>
        /// <param name="source">Data para el Excel</param>
        /// <param name="nameSheet">Nombre de la Hoja de Excel</param>
        /// <param name="impresionFactura"></param>
        public ExcelReport(DataTable source, string nameSheet = "Reporte", bool impresionFactura = false)
        {
            if (source == null)
                throw new ArgumentNullException("source", @"Source is null");
            Source = source;
            Wbook = new Workbook(WorkbookFormat.Excel2007);
            Wbook.Worksheets.Add(nameSheet);
            Wbook.Styles.NormalStyle.StyleFormat.Font.Name = "Courier New";
            if (impresionFactura)
            {
                PosRow = 10;
            }
            _start = new Point(1, 1);
        }
        #endregion

        #region Properties
        /// <summary>
        /// Current Row Position in WorkSheet
        /// </summary>
        public int CurrentPosition
        {
            get { return PosRow; }
            set
            {
                if (value > 0)
                    PosRow = value;
            }
        }

        /// <summary>
        /// Excel Headers.
        /// </summary>
        public ExcelHeader[] Headers;

        /// <summary>
        /// Apply format to Header Cell.
        /// </summary>
        public Action<IWorksheetCellFormat> FormaterHeader;

        /// <summary>
        /// Posicion en la que inicia la Insercion de los Datos, incluyendo el Title.
        /// <para> Based Zero</para>
        /// </summary>
        public Point StartSheet
        {
            get { return _start; }
            set
            {
                if (value.X >= 0 || value.Y >= 0) _start = value;
            }
        }

        /// <summary>
        /// Return Row in Current WorkSheet.
        /// </summary>
        /// <param name="index">Index Based Zero.</param>
        /// <returns></returns>
        public WorksheetRow this[int index]
        {
            get { return Wbook.Worksheets[0].Rows[index]; }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Inserta el Title del Reporte.
        /// </summary>
        /// <param name="title">titulo a mostrar.</param>
        /// <param name="subheader">subtitulo a mostrar</param>
        public void SetTitle(string title, string subheader = "")
        {
            var sheet = Wbook.Worksheets[0];

            MergeCellRow(0, 0, 2, "EMPRESA : " + Globals.ClientSession.v_CurrentExecutionNodeName);
            MergeCellRow(1, 0, 2, "R.U.C. : " + Globals.ClientSession.v_RucEmpresa);
            MergeCellRow(2, 0, 2, "FECHA DEL REPORTE: " + DateTime.Now.ToString("dd/MM/yyyy"));
            // MergeCellRow(2, 2, Fecha);
            //sheet.Rows[0].Cells[0].Value = ;
            //sheet.Rows[1].Cells[0].Value = ;
            //sheet.Rows[2].Cells[0].Value = ;
            _start.Y += 3;
            _withHeader = Headers != null ? Headers.Sum(h => h.Size) : 2;
            var mergedCell = sheet.MergedCellsRegions.Add(_start.Y, _start.X, _start.Y, _start.X - 1 + _withHeader);
            mergedCell.Value = title;
            mergedCell.CellFormat.Alignment = HorizontalCellAlignment.Center;
            mergedCell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            mergedCell.CellFormat.Font.Height = 320;
            mergedCell.CellFormat.Fill = GetDefaultColor();

            if (!string.IsNullOrEmpty(subheader))
                MergeCellRow(_start.Y + 1, _start.X, _withHeader, subheader).CellFormat.Alignment = HorizontalCellAlignment.Center;
            PosRow = _start.Y + 2;
        }

        /// <summary>
        /// Inserta los Headers de las Columnas.
        /// </summary>
        public void SetHeaders()
        {
            if (Headers == null) return;
            ShowHeaders(new ExcelHeader { Headers = Headers }, PosRow, _start.X);
            PosRow = _maxLevelHeader;
        }

        /// <summary>
        /// Inserta los datos del Source a la Hoja de Excel.
        /// </summary>
        /// <param name="columnas">Columnas del <see cref="Source"/> a insertar.</param>
        /// <param name="groups">Columnas del <see cref="Source"/> que serviran de agrupamiento.</param>
        public void SetData(ref  OperationResult objOperationResult, string[] columnas, params string[] groups)
        {

            try
            {
                objOperationResult.Success = 1;
                if (groups.Length == 0)
                {
                    var r = PosRow;
                    var rows = Source.Select();
                    InsertRows(ref objOperationResult, rows, columnas);
                    OnEndSection(new ExcelReportSectionEventArgs(Wbook.Worksheets[0], r, PosRow, rows.FirstOrDefault()));
                    return;
                }
                _numGroups = groups.Length;
                _cols = columnas;
                var result = Source.DefaultView.ToTable(true, groups);
                for (var i = 1; i <= _numGroups; i++)
                    _indices.Add(i, new Queue<int>());
                ShowHierarchy(ref objOperationResult, result.Select(), groups);
                OnEndSectionGroup(new ExcelReportSectionGroupEventArgs(Wbook.Worksheets[0], _indices[_numGroups].ToArray(), PosRow));
            }
            catch (Exception ex)
            {

                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ExcelReports.SetData()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);

            }
        }

        /// <summary>
        /// Resize columns in currente WorkSheet
        /// </summary>
        /// <param name="startCol">start of columna</param>
        /// <param name="widths">widths of columns</param>
        public void AutoSizeColumns(int startCol, params int[] widths)
        {
            var wksheet = Wbook.Worksheets[0];
            foreach (var col in widths)
                wksheet.Columns[startCol++].SetWidth(col, WorksheetColumnWidthUnit.Character);
        }

        /// <summary>
        /// Inserta Formula al Excel.
        /// </summary>
        /// <param name="startColumn">Index Column (Based Zero)</param>
        /// <param name="ctitle">Title para la formula</param>
        /// <param name="values">Formulas</param>
        public void SetFormulas(int startColumn, string ctitle, params string[] values)
        {
            var row = Wbook.Worksheets[0].Rows[PosRow];
            var cell = row.Cells[startColumn++];
            cell.Value = ctitle;
            cell.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            //cell.CellFormat.Font.ColorInfo = new WorkbookColorInfo(Color.White);
            //cell.CellFormat.Fill = GetDefaultColor();
            foreach (var valor in values)
            {
                cell = row.Cells[startColumn];
                if (!string.IsNullOrEmpty(valor)) cell.ApplyFormula(valor);
                cell.CellFormat.TopBorderStyle = CellBorderLineStyle.Thin;
                //cell.CellFormat.BottomBorderStyle = CellBorderLineStyle.Thin;
                //cell.CellFormat.RightBorderStyle = CellBorderLineStyle.Thin;
                startColumn++;
            }
        }

        /// <summary>
        /// Generate File .xlsx.
        /// </summary>
        /// <param name="saveFilename">SaveFilename</param>
        /// <exception cref="ArgumentException">The saveFilename's extension not match with .xslx</exception>
        public void Generate(string saveFilename)
        {
            if (saveFilename == null ||
                !Path.GetExtension(saveFilename).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException(@"The saveFilename's extension not match with .xslx", "saveFilename");
            Wbook.Save(saveFilename);
        }
        #endregion

        #region Private Methods
        private void ShowHeaders(ExcelHeader head, int level, int size)
        {
            if (head.Headers.Length > 0)
            {
                var acum = size;
                var wsheet = Wbook.Worksheets[0];
                Array.ForEach(head.Headers, h =>
                {
                    IWorksheetCellFormat cellformat;
                    if (h.Size > 1)
                    {
                        var meg = wsheet.MergedCellsRegions.Add(level, size, level, size + h.Size - 1);
                        meg.Value = h.Title;
                        cellformat = meg.CellFormat;
                    }
                    else
                    {
                        var cell = wsheet.Rows[level].Cells[size];
                        cell.Value = h.Title;
                        cellformat = cell.CellFormat;
                    }
                    SetCellFormat(cellformat);
                    if (FormaterHeader != null)
                        FormaterHeader(cellformat);
                    size += h.Size;
                });
                foreach (var header in head.Headers)
                {
                    ShowHeaders(header, level + 1, acum);
                    acum += header.Size;
                }
            }
            else
                _maxLevelHeader = level;
        }

        private void ShowHierarchy(ref OperationResult objOperationResult, IEnumerable<DataRow> rows, IList<string> args, string query = "")
        {

            var items = rows.GroupBy(r => r[args[0]].ToString(), r => r);
            query += args[0] + "='";
            foreach (var item in items)
            {
                SetHeaderGroup(item.Key, args.Count);
                if (args.Count == 1)
                {
                    var r = PosRow;
                    var res = Source.Select(query + item.Key.Replace("'", "''") + "'");
                    InsertRows(ref objOperationResult, res, _cols);
                    var end = PosRow;
                    _indices[1].Enqueue(end + 1);
                    OnEndSection(new ExcelReportSectionEventArgs(Wbook.Worksheets[0], r, end, res.FirstOrDefault()));
                    PosRow++;
                    for (var i = end; i < PosRow; i++)
                        Wbook.Worksheets[0].Rows[i].OutlineLevel = _numGroups;
                }
                else
                {
                    var q = query + item.Key.Replace("'", "''") + "' AND ";
                    var ars = args.ToList();
                    ars.RemoveAt(0);
                    ShowHierarchy(ref objOperationResult, item.ToArray(), ars.ToArray(), q);
                    _indices[args.Count].Enqueue(++PosRow + 1);
                    OnEndSectionGroup(new ExcelReportSectionGroupEventArgs(Wbook.Worksheets[0], _indices[ars.Count].ToArray(), PosRow) { Group = ars.Count });
                    PosRow++;
                    _indices[ars.Count].Clear();
                }
            }
        }

        private void SetHeaderGroup(string title, int width)
        {
            var c = Wbook.Worksheets[0].MergedCellsRegions.Add(PosRow, _start.X, PosRow, _start.X - 1 + _withHeader);
            c.CellFormat.Font.Bold = ExcelDefaultableBoolean.True;
            c.Value = title;
            Wbook.Worksheets[0].Rows[PosRow].OutlineLevel = _numGroups - width;
            c.CellFormat.Fill = GetDefaultColor();
            PosRow++;
        }

        private static void SetCellFormat(IWorksheetCellFormat format)
        {
            format.Font.Bold = ExcelDefaultableBoolean.True;
            format.WrapText = ExcelDefaultableBoolean.True;
            format.Alignment = HorizontalCellAlignment.Center;
            format.VerticalAlignment = VerticalCellAlignment.Center;
            format.TopBorderStyle = CellBorderLineStyle.Thin;
            format.RightBorderStyle = CellBorderLineStyle.Thin;
            format.LeftBorderStyle = CellBorderLineStyle.Thin;
            format.BottomBorderStyle = CellBorderLineStyle.Thin;
        }

        private void InsertRows(ref OperationResult objOperationResult, IEnumerable<DataRow> dataRows, IList<string> cols)
        {
            try
            {
                objOperationResult.Success = 1;
                var hoja = Wbook.Worksheets[0];
                foreach (var fila in dataRows)
                {
                    var row = hoja.Rows[PosRow++];
                    row.OutlineLevel = _numGroups;
                    short j = 0;
                    for (; j < cols.Count; j++)
                        row.Cells[j + _start.X].Value = fila[cols[j]];
                }
            }
            catch (Exception ex)
            {
                objOperationResult.Success = 0;
                objOperationResult.AdditionalInformation = "ExcelReports.InsertRows()";
                objOperationResult.ErrorMessage = ex.Message;
                objOperationResult.ExceptionMessage = ex.InnerException != null ? ex.InnerException.Message : string.Empty;
                Utils.ExceptionToLog(Globals.ClientSession.i_SystemUserId, objOperationResult);

            }
        }

        private CellFillPattern GetDefaultColor()
        {
            return CellFill.CreateSolidFill(Color.FromArgb(220, 230, 241));
        }

        private WorksheetMergedCellsRegion MergeCellRow(int row, int startX, int width, string value)
        {
            var sheet = Wbook.Worksheets[0];
            var mergedCell = sheet.MergedCellsRegions.Add(row, startX, row, startX + width);
            mergedCell.Value = value;
            return mergedCell;
        }
        #endregion
    }
    #endregion

    #region ExcelHeader
    /// <summary>
    /// Header for Excel Report.
    /// </summary>
    public class ExcelHeader
    {
        #region Construct
        public ExcelHeader()
        {
            Headers = new ExcelHeader[0];
        }
        #endregion

        #region Properties
        /// <summary>
        /// Size from child headers.
        /// </summary>
        public int Size
        {
            get
            {
                return Headers.Length == 0 ? 1 : Headers.Sum(h => h.Size);
            }
        }

        /// <summary>
        /// Header's Title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Child Headers.
        /// </summary>
        public ExcelHeader[] Headers;
        #endregion

        #region Operators
        public static implicit operator ExcelHeader(string value)
        {
            return new ExcelHeader
            {
                Title = value
            };
        }
        #endregion
    }
    #endregion

    #region EventArgs
    /// <summary>
    /// ExcelReportEventArgs class
    /// </summary>
    public class ExcelReportEventArgs : EventArgs
    {
        /// <summary>
        /// Current WorkSheet
        /// </summary>
        public Worksheet Wsheet { get; private set; }

        /// <summary>
        /// Posicion a partir de la que se empezo a Insertar las Filas.
        /// </summary>
        public int StartPosition { get; set; }



        public ExcelReportEventArgs(Worksheet sheet, int position)
        {
            Wsheet = sheet;
            StartPosition = position;
        }
    }

    public class ExcelReportSectionEventArgs : EventArgs
    {
        /// <summary>
        /// Current WorkSheet
        /// </summary>
        public Worksheet Wsheet { get; private set; }

        /// <summary>
        /// First DataRow from Source
        /// </summary>
        public DataRow FirsRow { get; private set; }

        /// <summary>
        /// Posicion a partir de la que se empezo a Insertar las Filas.
        /// </summary>
        public int StartPosition { get; set; }

        /// <summary>
        /// Ultima posicion en la que se inserto las filas.
        /// </summary>
        public int EndPosition { get; set; }

        public ExcelReportSectionEventArgs(Worksheet sheet, int start, int end, DataRow row)
        {
            Wsheet = sheet;
            StartPosition = start;
            EndPosition = end;
            FirsRow = row;
        }
    }

    public class ExcelReportSectionGroupEventArgs : EventArgs
    {
        /// <summary>
        /// Current WorkSheet
        /// </summary>
        public Worksheet Wsheet { get; private set; }

        /// <summary>
        /// Posiciones que representan la linea posterior de escribir los datos de cada subgrupo.
        /// </summary>
        public int[] EndSections;

        /// <summary>
        /// Indica la actual posicion, que se situa despues de escribir los subgrupos 
        /// </summary>
        public int Position { get; set; }

        /// <summary>
        /// Gets or sets the group.
        /// </summary>
        /// <value>The group.</value>
        public int Group { get; set; }

        public ExcelReportSectionGroupEventArgs(Worksheet sheet, int[] posInts, int end)
        {
            Wsheet = sheet;
            EndSections = posInts;
            Position = end;
        }
    }
    #endregion
}