// <copyright file="ChangeDataType.xaml.cs" company="McLaren Applied Ltd.">
// Copyright (c) McLaren Applied Ltd.</copyright>

using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SystemMonitorProtobuf;
using DataType = SystemMonitorProtobuf.DataType;

namespace SystemMonitorConfigurationTest.Dialogs
{
    public partial class ChangeDataType
    {
        public ChangeDataType(MainWindow window)
        {
            this.InitializeComponent();

            window.PopulateParams(ParameterType.Virtual, ref this.vList);

            var dt2 = new DataTable();
            dt2.Columns.Add("Value", typeof(ParameterType));
            dt2.Columns.Add("Type", typeof(string));

            dt2.Rows.Add(DataType.Ubyte, DataType.Ubyte.ToString());
            dt2.Rows.Add(DataType.Byte, DataType.Byte.ToString());
            dt2.Rows.Add(DataType.Uword, DataType.Uword.ToString());
            dt2.Rows.Add(DataType.Word, DataType.Word.ToString());
            dt2.Rows.Add(DataType.Ulong, DataType.Ulong.ToString());
            dt2.Rows.Add(DataType.Long, DataType.Long.ToString());
            dt2.Rows.Add(DataType.Float, DataType.Float.ToString());

            var binding2 = new Binding
            {
                Source = dt2
            };

            this.typeList.DisplayMemberPath = "Type";
            this.typeList.SetBinding(ItemsControl.ItemsSourceProperty, binding2);
            this.typeList.SelectedIndex = 0;

        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

    }
}
