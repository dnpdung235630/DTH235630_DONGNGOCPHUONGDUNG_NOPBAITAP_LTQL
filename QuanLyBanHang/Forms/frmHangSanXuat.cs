using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyBanHang.Forms
{
    public partial class frmHangSanXuat : Form
    {
        public frmHangSanXuat()
        {
            InitializeComponent();
        }
        QLBHDbContext context = new QLBHDbContext(); // Khởi tạo biến ngữ cảnh CSDL
        bool xuLyThem = false; // Kiểm tra có nhấn vào nút Thêm hay không?
        int id; // Lấy mã loại sản phẩm (dùng cho Sửa và Xóa)
        private void BatTatChucNang(bool giaTri)
        {
            btnLuu.Enabled = giaTri;
            btnHuyBo.Enabled = giaTri;
            txtTenHangSanXuat.Enabled = giaTri;
            btnThem.Enabled = !giaTri;
            btnSua.Enabled = !giaTri;
            btnXoa.Enabled = !giaTri;
        }
        private void frmHangSanXuat_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);

            List<HangSanXuat> hsx = new List<HangSanXuat>();
            hsx = context.HangSanXuat.ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = hsx;

            txtTenHangSanXuat.DataBindings.Clear();
            txtTenHangSanXuat.DataBindings.Add("Text", bindingSource, "TenHangSanXuat", false, DataSourceUpdateMode.Never);

            dataGridView.DataSource = bindingSource;

        }
        private void btnThem_Click(object sender, EventArgs e)
        {
            xuLyThem = true;
            BatTatChucNang(true);
            txtTenHangSanXuat.Clear();
        }
        private void btnSua_Click(object sender, EventArgs e)
        {
            xuLyThem = false;
            BatTatChucNang(true);

        }
        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (xuLyThem)
            {
                HangSanXuat hsx = new HangSanXuat();
                hsx.TenHangSanXuat = txtTenHangSanXuat.Text;
                context.HangSanXuat.Add(hsx);
            }
            else
            {
                id = (int)dataGridView.CurrentRow.Cells[0].Value;
                HangSanXuat hsx = context.HangSanXuat.FirstOrDefault(x => x.Id == id);
                hsx.TenHangSanXuat = txtTenHangSanXuat.Text;
            }
            context.SaveChanges();
            frmHangSanXuat_Load(sender, e);
            BatTatChucNang(false);
        }
        private void btnXoa_Click(object sender, EventArgs e)
        {

            id = (int)dataGridView.CurrentRow.Cells[0].Value;
            HangSanXuat hsx = context.HangSanXuat.FirstOrDefault(x => x.Id == id);
            context.HangSanXuat.Remove(hsx);
            context.SaveChanges();
            frmHangSanXuat_Load(sender, e);
            BatTatChucNang(false);

        }
        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            frmHangSanXuat_Load(sender, e);
        }
        
    }
}
