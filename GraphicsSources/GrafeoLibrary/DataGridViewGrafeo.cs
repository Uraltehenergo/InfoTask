using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace GrafeoLibrary
{
    internal class DataGridViewGrafeo
    {
        private readonly DataGridView _dataGridView;

        //установка DoubleBuffered для DataGridView
        private void SetDoubleBuffered()
        {
            //set instance non-public property with name "DoubleBuffered" to true
            typeof(Control).InvokeMember("DoubleBuffered",
                                          BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                                          null, _dataGridView, new object[] { true });
        }

        internal DataGridViewGrafeo(DataGridView dataGridView)
        {
            _dataGridView = dataGridView;

            //Для того, чтобы датагрид отображался быстрее
            SetDoubleBuffered();
            
            InitColumns();
        }

        private void InitColumns()
        {
            _dataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _dataGridView.ScrollBars = ScrollBars.Both;
            _dataGridView.DefaultCellStyle.SelectionBackColor = SystemColors.GradientActiveCaption;

            _dataGridView.RowHeadersVisible = false;
            _dataGridView.MultiSelect = false;
            _dataGridView.AllowUserToAddRows = false;

            int i = _dataGridView.Columns.Add("№ в таблице", "№ в таблице");
            //Columns["№ в таблице"].Width = 22;
            //Columns["№ в таблице"].Frozen = true;
            //Columns["№ в таблице"].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold,
            //                                                        GraphicsUnit.Point, ((204)));
            //Columns["№ в таблице"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            //Columns["№ в таблице"].ReadOnly = true;
            //Columns["№ в таблице"].DefaultCellStyle.ForeColor = Color.White;
            //Columns["№ в таблице"].DefaultCellStyle.SelectionForeColor = Color.White;
            _dataGridView.Columns[i].Width = 22;
            _dataGridView.Columns[i].Frozen = true;
            _dataGridView.Columns[i].DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Bold,
                                                                      GraphicsUnit.Point, ((204)));
            _dataGridView.Columns[i].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            _dataGridView.Columns[i].ReadOnly = true;
            _dataGridView.Columns[i].DefaultCellStyle.ForeColor = Color.White;
            _dataGridView.Columns[i].DefaultCellStyle.SelectionForeColor = Color.White;

            i = _dataGridView.Columns.Add("Код", "Код");
            //Columns["Код"].Width = 130;
            //Columns["Код"].ReadOnly = true;
            _dataGridView.Columns[i].Width = 130;
            _dataGridView.Columns[i].ReadOnly = true;

            i = _dataGridView.Columns.Add("Наименование", "Наименование");
            //Columns["Наименование"].Width = 310;
            //Columns["Наименование"].ReadOnly = true;
            _dataGridView.Columns[i].Width = 310;
            _dataGridView.Columns[i].ReadOnly = true;

            i = _dataGridView.Columns.Add("Визир", "Визир");
            //Columns["Визир"].Width = 70;
            //Columns["Визир"].ReadOnly = true;
            _dataGridView.Columns[i].Width = 70;
            _dataGridView.Columns[i].ReadOnly = true;

            i = _dataGridView.Columns.Add("Недост.", "Недост.");
            //Columns["Недост."].Width = 50;
            //Columns["Недост."].ReadOnly = true;
            _dataGridView.Columns[i].Width = 50;
            _dataGridView.Columns[i].ReadOnly = true;

            //Columns.Add("Последнее", "Последнее");
            //Columns["Последнее"].Width = 70;
            //Columns["Последнее"].ReadOnly = true;

            i = _dataGridView.Columns.Add("Ед. измер.", "Ед. измер.");
            //Columns["Ед. измер."].Width = 80;
            //Columns["Ед. измер."].ReadOnly = true;
            _dataGridView.Columns[i].Width = 80;
            _dataGridView.Columns[i].ReadOnly = true;

            i = _dataGridView.Columns.Add("Мин.", "Мин.");
            //Columns["Мин."].Width = 50;
            //Columns["Мин."].ReadOnly = true;
            _dataGridView.Columns[i].Width = 50;
            _dataGridView.Columns[i].ReadOnly = true;

            i = _dataGridView.Columns.Add("Макс.", "Макс.");
            //Columns["Макс."].Width = 50;
            //Columns["Макс."].ReadOnly = true;
            _dataGridView.Columns[i].Width = 50;
            _dataGridView.Columns[i].ReadOnly = true;

            i = _dataGridView.Columns.Add("Тип данных", "Тип данных");
            //Columns["Тип данных"].Width = 80;
            //Columns["Тип данных"].ReadOnly = true;
            _dataGridView.Columns[i].Width = 80;
            _dataGridView.Columns[i].ReadOnly = true;

            //Columns.Add("Доп. инфо", "Доп. инфо");
            //Columns["Доп. инфо"].Width = 95;
            //Columns["Доп. инфо"].ReadOnly = true;
            //Columns.Add("А мин", "А мин");
            //Columns["А мин"].Width = 49;
            //Columns["А мин"].ReadOnly = true;
            //Columns.Add("П мин", "П мин");
            //Columns["П мин"].Width = 49;
            //Columns["П мин"].ReadOnly = true;
            //Columns.Add("П макс", "П макс");
            //Columns["П макс"].Width = 50;
            //Columns["П макс"].ReadOnly = true;
            //Columns.Add("А макс", "А макс");
            //Columns["А макс"].Width = 49;
            //Columns["А макс"].ReadOnly = true;

            i = _dataGridView.Columns.Add("Group", "Group");
            //Columns["Group"].Visible = false;
            _dataGridView.Columns[i].Visible = false;

            var yColumn = new DataGridViewCheckBoxColumn
            {
                HeaderText = @"Y",
                Width = 22,
                Frozen = true,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                TrueValue = true,
                FalseValue = false
            };
            _dataGridView.Columns.Insert(0, yColumn);
            _dataGridView.Columns[0].SortMode = DataGridViewColumnSortMode.Programmatic;

            //var colorColumn = new DataGridViewButtonColumn()
            //                      {
            //                          Name = "Color",
            //                          HeaderText = "Цвет",
            //                          Width = 50,
            //                          Frozen = false,
            //                          AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
            //                          Text = "Color",
            //                          UseColumnTextForButtonValue = true
            //                      };
            //_dataGridView.Columns.Add(colorColumn);

            //a CellClick += DataGrid_CellClick;
            //a CellValueChanged += DataGridCheckChange;
            //a CurrentCellDirtyStateChanged += DataGridView_CurrentCellDirtyStateChanged;
            //a SelectionChanged += DataGridSelectionChanged;
            //~ UserDeletingRow += DatagridRowAnnihilate;
        }

        //Заполнение таблицы инфой
        internal void DataGridAddTestRow()
        {
            var num = _dataGridView.Rows.Count + 1;
            int row = _dataGridView.Rows.Add(new IComparable[]
                                                {
                                                    true, num, "TEST" + num, "Тест " + num, "", "", "", "","", ""
                                                });

            if(_testColors == null ) _testColors = new OuterColorUseList();
            var color = _testColors.GetColor();

            _dataGridView.Rows[row].Cells["№ в таблице"].Style.BackColor = color;
            _dataGridView.Rows[row].Cells["№ в таблице"].Style.SelectionBackColor = color;
            _dataGridView.Rows[row].Cells[0].Style.BackColor = color;
            _dataGridView.Rows[row].Cells[0].Style.SelectionBackColor = color;
            _dataGridView.Rows[row].Cells["Group"].Value = num;
        }

        private OuterColorUseList _testColors;
    }

    //comparer сортировки датагрида по цвету
    public class DatagridColorSort : IComparer<DataGridViewRow>
    {
        public int Compare(DataGridViewRow row1, DataGridViewRow row2)
        {
            try
            {
                if ((int)row1.Cells["Group"].Value > (int)row2.Cells["Group"].Value) return 1;
                if ((int)row1.Cells["Group"].Value < (int)row2.Cells["Group"].Value) return -1;
                if (int.Parse(row1.Cells[1].Value.ToString()) > int.Parse(row2.Cells[1].Value.ToString())) return 1;
                if (int.Parse(row1.Cells[1].Value.ToString()) < int.Parse(row2.Cells[1].Value.ToString())) return -1;
            }
            catch { }
            return 0;
        }
    }
}
