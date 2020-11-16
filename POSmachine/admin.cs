﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace POSmachine
{
    public partial class admin : Form
    {
        //values
        private int idPotition = -60;//id location
        private int idDot=0;
        private int totalCostDot = 0;
        int[] idItemList;
        //myControl
        ComboBox myIdDotCbb = new ComboBox();
        ComboBox myRankItemsCbb = new ComboBox();
        Label myTotalCostLb = new Label();
        Label myGrowthRateLb = new Label();
        Label myDateTimeLb = new Label() { Name = "myDateTimeLabel", Font = new Font("Microsoft Sans Serif", 10) };
        Label myRankNumberLb;
        Label myRankItemsNameLb;
        Label myRankItemsSellLb;
        Label myRankItemsCostLb;
        Panel myRankItemsBarPn;
        //connect database
        static string conString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Excercise\WINDOWSPROGRAM\BTcuoiky\GitHub\POSmachine\POSmachine\QLCUAHANG.mdf;Integrated Security=True";
        SqlConnection myconn = new SqlConnection(conString);
        SqlCommand cmd;
        SqlDataAdapter da;
        DataTable dt;
        DataSet ds;
        SqlDataReader rd;
        public admin()
        {
            myconn.Open();//open connect database
            InitializeComponent();
            createMyIdDotCbb();
            showDgvdanhsachmon();
            createGroupLabel();
            showTotalCost();
            showGrowthRate();
            createGroupRankLabelAndPanel();
        }
        private void showDgvdanhsachmon()
        {
            string filterQuery = "Select Itemname,Soluong,Price,Khuyenmai,Idhoadon from Itemorder,Items"+
            " where Itemorder.Iditem = Items.Iditem and Idngay = '"+ idDot +"';";
            cmd = new SqlCommand(filterQuery, myconn);
            da = new SqlDataAdapter(filterQuery, myconn);
            dt = new DataTable();
            da.Fill(ds);
            da.Fill(dt);
            dgvdanhsachmon.DataSource = dt;
            dgvdanhsachmon.Columns["Itemname"].HeaderText = "Tên món";
            dgvdanhsachmon.Columns["Soluong"].HeaderText = "Số lượng";
            dgvdanhsachmon.Columns["Price"].HeaderText = "Giá";
            dgvdanhsachmon.Columns["Khuyenmai"].HeaderText = "Khuyến mãi";
            dgvdanhsachmon.Columns["Idhoadon"].HeaderText = "Mã hóa đơn";
            dgvdanhsachmon.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }
        private void showTotalCost()
        {
            string getMoneyQuery = "Select Tongtien from Hoadon" +
            " where Iddot = '" + idDot + "';";
            cmd = new SqlCommand(getMoneyQuery, myconn);
            da = new SqlDataAdapter(getMoneyQuery, myconn);
            ds = new DataSet();
            da.Fill(ds);
            totalCostDot = 0;int price = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                price = Int32.Parse(ds.Tables[0].Rows[i][0].ToString());
                totalCostDot += price;
            }
            String totalCostString = String.Format("{0:n0}", totalCostDot); ;
            myTotalCostLb.Text = totalCostString;
        }
        private void showGrowthRate()
        {
            int previousIdDot = idDot - 1;
            string getMoneyQuery = "Select Tongtien from Hoadon" +
            " where Iddot = '" + previousIdDot + "';";
            cmd = new SqlCommand(getMoneyQuery, myconn);
            da = new SqlDataAdapter(getMoneyQuery, myconn);
            ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count == 0)
            {
                myGrowthRateLb.Text = "Chưa xác định.";
                return;
            }
            int PreviousTotalCost = 0, price = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                price = Int32.Parse(ds.Tables[0].Rows[i][0].ToString());
                PreviousTotalCost += price;
            }
            int growthRate =((totalCostDot * 100) / PreviousTotalCost) - 100;
            myGrowthRateLb.Text = growthRate+"%";
        }
        private void createMyIdDotCbb()
        {
            myIdDotCbb = new ComboBox() { Name = "myIdDotCbb" };
            myIdDotCbb.Location = new Point(580, 6);
            myIdDotCbb.Size = new System.Drawing.Size(65, 21);
            //add data
            myIdDotCbb.DisplayMember = "Text";
            myIdDotCbb.ValueMember = "Value";
            string idDotString="";
            string getDotQuery = "select Iddot from Dot order by Iddot desc;";
            cmd = new SqlCommand(getDotQuery, myconn);
            da = new SqlDataAdapter(getDotQuery, myconn);
            dt = new DataTable();
            ds = new DataSet();
            da.Fill(ds);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                if (i == 0)
                {
                    idDotString = ds.Tables[0].Rows[i][0].ToString();
                    idDot = Int32.Parse(ds.Tables[0].Rows[i][0].ToString());
                }
                else
                {
                    idDotString = ds.Tables[0].Rows[i][0].ToString();
                }
                int idcbb = i + 1;
                myIdDotCbb.Items.Add(new { Value = idcbb, Text = idDotString });
            }
            myIdDotCbb.SelectedIndex = 0;
            tabpdanhsachmonorder.Controls.Add(myIdDotCbb);
            // SelectedIndexChanged event.
            this.myIdDotCbb.SelectedIndexChanged +=
                new System.EventHandler(myIdDotCbb_SelectedIndexChanged);
        }
        private void createGroupLabel()
        {
            //create myTotalCostLb
            myTotalCostLb = new Label() { Name = "myTotalCostLb", Font = new Font(Label.DefaultFont, FontStyle.Bold) };
            myTotalCostLb.Location = new Point(576, 61);
            myTotalCostLb.Size = new System.Drawing.Size(69, 21);
            myTotalCostLb.TextAlign = ContentAlignment.TopRight;
            tabpdanhsachmonorder.Controls.Add(myTotalCostLb);
            //create myGrowthRateLb
            myGrowthRateLb = new Label() { Name = "myGrowthRateLb", Font = new Font(Label.DefaultFont, FontStyle.Bold) };
            myGrowthRateLb.TextAlign = ContentAlignment.TopRight;
            myGrowthRateLb.Location = new Point(577, 86);
            myGrowthRateLb.Size = new System.Drawing.Size(69, 42);
            tabpdanhsachmonorder.Controls.Add(myGrowthRateLb);
            myGrowthRateLb.Text = "0";
            myTotalCostLb.Text = "0";
        }
        private void createGroupRankLabelAndPanel()
        {
            //create myRankItemsBarPn
            myRankItemsBarPn = new Panel() { Name = "myRankItemsBarPn" };
            myRankItemsBarPn.Location = new Point(3, 24);
            myRankItemsBarPn.Size = new System.Drawing.Size(396, 72);
            pnGroupRankItem.Controls.Add(myRankItemsBarPn);
            //create myRankItemsNameLb
            myRankItemsNameLb = new Label() { Name = "myRankItemsNameLb", Font = new Font(Label.DefaultFont, FontStyle.Bold) };
            myRankItemsNameLb.Location = new Point(93, 34);
            myRankItemsNameLb.Text = "0";
            //myRankItemsNameLb.Size = new System.Drawing.Size(69, 42);
            myRankItemsBarPn.Controls.Add(myRankItemsNameLb);
            //create myRankNumberLb
            myRankNumberLb = new Label() { Name = "myRankNumberLb", Font = new Font(Label.DefaultFont, FontStyle.Bold) };
            myRankNumberLb.Location = new Point(3, 34);
            myRankNumberLb.Text = "0";
            //myRankNumberLb.Size = new System.Drawing.Size(69, 42);
            myRankItemsBarPn.Controls.Add(myRankNumberLb);
            //create myRankItemsSellLb
            myRankItemsSellLb = new Label() { Name = "myRankItemsSellLb", Font = new Font(Label.DefaultFont, FontStyle.Bold) };
            myRankItemsSellLb.Location = new Point(326, 34);
            //myRankItemsSellLb.Size = new System.Drawing.Size(69, 42);
            myRankItemsBarPn.Controls.Add(myRankItemsSellLb);
            //create myRankItemsCostLb
            myRankItemsCostLb = new Label() { Name = "myRankItemsCostLb", Font = new Font(Label.DefaultFont, FontStyle.Bold) };
            myRankItemsCostLb.TextAlign = ContentAlignment.TopRight;
            myRankItemsCostLb.Location = new Point(236, 34);
            //myRankItemsCostLb.Size = new System.Drawing.Size(69, 42);
            myRankItemsBarPn.Controls.Add(myRankItemsCostLb);
        }
        private void myIdDotCbb_SelectedIndexChanged(object sender,EventArgs e)
        {
            ComboBox mycbb = (ComboBox)sender;
            idDot = Int32.Parse(mycbb.GetItemText(mycbb.SelectedItem));
            showDgvdanhsachmon();
            showTotalCost();
            showGrowthRate();
        }
        private void btnthoat_Click(object sender, EventArgs e)
        {
            this.Close();
            Form1 login = new Form1();
            login.Show();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void tabpdanhsachmon_Click(object sender, EventArgs e)
        {

        }

        private void btnclickme_Click(object sender, EventArgs e)
        {
            string getIdItemQuery = "select Iditem from Items;";
            cmd = new SqlCommand(getIdItemQuery, myconn);
            da = new SqlDataAdapter(getIdItemQuery, myconn);
            ds = new DataSet();
            da.Fill(ds);
            idItemList = new int[ds.Tables[0].Rows.Count*2];
            int idItem = 0;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                idItem = Int32.Parse(ds.Tables[0].Rows[i][0].ToString());
            }
            //
            string getTotalSellQuery = "select * from Itemorder,Items where Itemorder.Iditem = Items.Iditem order by Items.Iditem; ";
            cmd = new SqlCommand(getIdItemQuery, myconn);
            da = new SqlDataAdapter(getIdItemQuery, myconn);
            ds = new DataSet();
            da.Fill(ds);
            int sl = 0, cost = 0,distance = -2;
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                idItem = Int32.Parse(ds.Tables[0].Rows[i][1].ToString());
                sl = Int32.Parse(ds.Tables[0].Rows[i][4].ToString());
                cost = Int32.Parse(ds.Tables[0].Rows[i][8].ToString());
                int pos = findValue(idItem);
                if (pos >= 0)
                {
                    idItemList[pos + 1] += sl;
                }
            }
            MessageBox.Show(string.Join(",",idItemList));
        }

        private void btnenddot_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Bạn muốn kết thúc đợt làm việc này, bắt đầu đợt làm việc mới?", "Thông báo", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                string insertBillQuery = "insert into Dot(TongTien) values(0);";
                cmd = new SqlCommand(insertBillQuery, myconn);
                cmd.ExecuteNonQuery();
                //refesh
                tabpdanhsachmonorder.Controls.Remove(myIdDotCbb);
                tabpdanhsachmonorder.Controls.Remove(myTotalCostLb);
                tabpdanhsachmonorder.Controls.Remove(myGrowthRateLb);
                createGroupLabel();
                createMyIdDotCbb();
                showDgvdanhsachmon();
            }
        }
        private int findValue(int id)
        {
            for(int i = 0; i < idItemList.Length; i+=3)
            {
                if (idItemList[i] == id)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
