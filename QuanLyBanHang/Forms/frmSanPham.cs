using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using QuanLyBanHang.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyBanHang;


namespace QuanLyBanHang.Forms
{
    public partial class frmSanPham : Form
    {
        QLBHDbContext context = new QLBHDbContext();
        bool xulyThem = false;
        int id;
        string imagesFolder = Application.StartupPath.Replace("bin\\Debug\\net8.0-windows", "Images");

        public frmSanPham()
        {
            InitializeComponent();
        }

        private void BatTatChucNang(bool giatri)
        {
            btnLuu.Enabled = giatri;
            btnHuyBo.Enabled = giatri;
            cboHangSanXuat.Enabled = giatri;
            cboLoaiSanPham.Enabled = giatri;
            txtTenSanPham.Enabled = giatri;
            numSoLuong.Enabled = giatri;
            numDonGia.Enabled = giatri;
            txtMoTa.Enabled = giatri;
            picHinhAnh.Enabled = giatri;

            btnThem.Enabled = !giatri;
            btnDoiAnh.Enabled = !giatri;
            btnSua.Enabled = !giatri;
            btnXoa.Enabled = !giatri;
            btnTimKiem.Enabled = !giatri;
            btnNhap.Enabled = !giatri;
            btnXuat.Enabled = !giatri;
        }
        public void LayLoaiSanPhamVaoComboBox()
        {
            cboLoaiSanPham.DataSource = context.LoaiSanPham.ToList();
            cboLoaiSanPham.ValueMember = "ID";
            cboLoaiSanPham.DisplayMember = "TenLoai";
        }

        public void LayHangSanXuatVaoComboBox()
        {
            cboHangSanXuat.DataSource = context.HangSanXuat.ToList();
            cboHangSanXuat.ValueMember = "ID";
            cboHangSanXuat.DisplayMember = "TenHangSanXuat";
        }
        private void frmSanPham_Load(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            LayLoaiSanPhamVaoComboBox();
            LayHangSanXuatVaoComboBox();

            dataGridView.AutoGenerateColumns = false;

            List<DanhSachSanPham> sp = new List<DanhSachSanPham>();
            sp = context.SanPham.Select(r => new DanhSachSanPham
            {
                ID = r.ID,
                LoaiSanPhamID = r.LoaiSanPhamID,
                TenLoai = r.LoaiSanPham.TenLoai,
                HangSanXuatID = r.HangSanXuatID,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                TenSanPham = r.TenSanPham,
                DonGia = r.DonGia,
                SoLuong = r.SoLuong,
                HinhAnh = r.HinhAnh,
                MoTa = r.MoTa
            }).ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = sp;

            cboLoaiSanPham.DataBindings.Clear();
            cboLoaiSanPham.DataBindings.Add("SelectedValue", bindingSource, "LoaiSanPhamID", false, DataSourceUpdateMode.Never);

            cboHangSanXuat.DataBindings.Clear();
            cboHangSanXuat.DataBindings.Add("SelectedValue", bindingSource, "HangSanXuatID", false, DataSourceUpdateMode.Never);

            txtTenSanPham.DataBindings.Clear();
            txtTenSanPham.DataBindings.Add("Text", bindingSource, "TenSanPham", false, DataSourceUpdateMode.Never);

            txtMoTa.DataBindings.Clear();
            txtMoTa.DataBindings.Add("Text", bindingSource, "MoTa", false, DataSourceUpdateMode.Never);

            numSoLuong.DataBindings.Clear();
            numSoLuong.DataBindings.Add("Value", bindingSource, "SoLuong", false, DataSourceUpdateMode.Never);

            numDonGia.DataBindings.Clear();
            numDonGia.DataBindings.Add("Value", bindingSource, "DonGia", false, DataSourceUpdateMode.Never);

            picHinhAnh.DataBindings.Clear();
            Binding hinhAnh = new Binding("ImageLocation", bindingSource, "HinhAnh");
            hinhAnh.Format += (s, e) =>
            {
                e.Value = Path.Combine(imagesFolder, e.Value.ToString());
            };
            picHinhAnh.DataBindings.Add(hinhAnh);

            dataGridView.DataSource = bindingSource;
        }
        private void dataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dataGridView.Columns[e.ColumnIndex].Name == "HinhAnh")
            {
                Image image = Image.FromFile(Path.Combine(imagesFolder, e.Value.ToString()));
                image = new Bitmap(image, 24, 24);

                e.Value = image;
            }
        }

        private void btnThem_Click(object sender, EventArgs e)
        {
            xulyThem = true;
            BatTatChucNang(true);
            cboLoaiSanPham.Text = "";
            cboHangSanXuat.Text = "";
            txtTenSanPham.Clear();
            txtMoTa.Clear();
            numSoLuong.Value = 0;
            numDonGia.Value = 0;
            picHinhAnh.Image = null;
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            xulyThem = false;
            BatTatChucNang(true);
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
        }

        private void btnLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(cboLoaiSanPham.Text))
            {
                MessageBox.Show("Vui lòng chọn loại sản phẩm", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrWhiteSpace(cboHangSanXuat.Text))
            {
                MessageBox.Show("Vui lòng chọn hãng sản xuất", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (string.IsNullOrWhiteSpace(txtTenSanPham.Text))
            {
                MessageBox.Show("Vui lòng nhập tên sản phẩm", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numSoLuong.Value <= 0)
            {
                MessageBox.Show("Số lượng phải lớn hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (numDonGia.Value <= 0)
            {
                MessageBox.Show("Đơn giá sản phẩm phải lớn hơn 0", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (xulyThem)
                {
                    SanPham sp = new SanPham();
                    sp.LoaiSanPhamID = Convert.ToInt32(cboLoaiSanPham.Text);
                    sp.HangSanXuatID = Convert.ToInt32(cboHangSanXuat.Text);
                    sp.TenSanPham = txtTenSanPham.Text;
                    sp.MoTa = txtMoTa.Text;
                    sp.SoLuong = Convert.ToInt32(numSoLuong.Value);
                    sp.DonGia = Convert.ToInt32(numDonGia.Value);
                    sp.HinhAnh = "";
                    context.SanPham.Add(sp);


                    context.SaveChanges();

                }
                else
                {
                    SanPham sp = context.SanPham.Find(id);
                    if (sp != null)
                    {
                        sp.TenLoai = cboLoaiSanPham.Text;
                        sp.TenHangSanXuat = cboHangSanXuat.Text;
                        sp.TenSanPham = txtTenSanPham.Text;
                        sp.MoTa = txtMoTa.Text;
                        sp.SoLuong = Convert.ToInt32(numSoLuong.Value);
                        sp.DonGia = Convert.ToInt32(numDonGia.Value);
                        sp.HinhAnh = "";
                        context.SanPham.Update(sp);

                        context.SaveChanges();
                    }
                }
            }
            frmSanPham_Load(sender, e);
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Xác nhận xóa sản phẩm" + txtTenSanPham.Text + "?", "Xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                SanPham sp = context.SanPham.Find(id);
                if (sp != null)
                {
                    context.SanPham.Remove(sp);
                }
                context.SaveChanges();

                frmSanPham_Load(sender, e);
            }
        }

        private void btnHuyBo_Click(object sender, EventArgs e)
        {
            frmSanPham_Load(sender, e);
        }

        private void btnDoiAnh_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Cập nhật hình ảnh sản phẩm";
            openFileDialog.Filter = "Tập tin hình ảnh | *.jpg;*.jpeg;*.png;*.bmp";
            openFileDialog.Multiselect = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                string ext = Path.GetExtension(openFileDialog.FileName);
                string fileSavePath = Path.Combine(imagesFolder, fileName.GenerateSlug() + ext);
                File.Copy(openFileDialog.FileName, fileSavePath, true);

                id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
                SanPham sp = context.SanPham.Find(id);
                sp.HinhAnh = fileName.GenerateSlug() + ext;
                context.SanPham.Update(sp);

                context.SaveChanges();
                frmSanPham_Load(sender, e);
            }
        }
        //bổ sung thêm ngoài bài hướng dẫn
        /*private void LoadData()
        {
            var listSanPham = context.SanPham.ToList();

            dataGridView.DataSource = null;
            dataGridView.DataSource = listSanPham;

            dataGridView.Columns["LoaiSanPhamID"].Visible = false;
            using (var newContext = new QLBHDbContext())
            {
                dataGridView.DataSource = newContext.SanPham.ToList();
            }
        }*/

        private void frmSanPham_Load_1(object sender, EventArgs e)
        {
            BatTatChucNang(false);
            LayLoaiSanPhamVaoComboBox();
            LayHangSanXuatVaoComboBox();

            dataGridView.AutoGenerateColumns = false;

            List<DanhSachSanPham> sp = new List<DanhSachSanPham>();
            sp = context.SanPham.Select(r => new DanhSachSanPham
            {
                ID = r.ID,
                LoaiSanPhamID = r.LoaiSanPhamID,
                TenLoai = r.LoaiSanPham.TenLoai,
                HangSanXuatID = r.HangSanXuatID,
                TenHangSanXuat = r.HangSanXuat.TenHangSanXuat,
                TenSanPham = r.TenSanPham,
                DonGia = r.DonGia,
                SoLuong = r.SoLuong,
                HinhAnh = r.HinhAnh,
                MoTa = r.MoTa
            }).ToList();

            BindingSource bindingSource = new BindingSource();
            bindingSource.DataSource = sp;

            cboLoaiSanPham.DataBindings.Clear();
            cboLoaiSanPham.DataBindings.Add("SelectedValue", bindingSource, "LoaiSanPhamID", false, DataSourceUpdateMode.Never);

            cboHangSanXuat.DataBindings.Clear();
            cboHangSanXuat.DataBindings.Add("SelectedValue", bindingSource, "HangSanXuatID", false, DataSourceUpdateMode.Never);

            txtTenSanPham.DataBindings.Clear();
            txtTenSanPham.DataBindings.Add("Text", bindingSource, "TenSanPham", false, DataSourceUpdateMode.Never);

            txtMoTa.DataBindings.Clear();
            txtMoTa.DataBindings.Add("Text", bindingSource, "MoTa", false, DataSourceUpdateMode.Never);

            numSoLuong.DataBindings.Clear();
            numSoLuong.DataBindings.Add("Value", bindingSource, "SoLuong", false, DataSourceUpdateMode.Never);

            numDonGia.DataBindings.Clear();
            numDonGia.DataBindings.Add("Value", bindingSource, "DonGia", false, DataSourceUpdateMode.Never);

            picHinhAnh.DataBindings.Clear();
            Binding hinhAnh = new Binding("ImageLocation", bindingSource, "HinhAnh");
            hinhAnh.Format += (s, e) =>
            {
                e.Value = Path.Combine(imagesFolder, e.Value.ToString());
            };
            picHinhAnh.DataBindings.Add(hinhAnh);

            dataGridView.DataSource = bindingSource;
        }

        private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string phanLoai = cboLoaiSanPham.Text;
            string hangSanXuat = cboHangSanXuat.Text;
            string tenSanPham = txtTenSanPham.Text.Trim();

            var query = context.SanPham.AsEnumerable();
            if (!string.IsNullOrEmpty(phanLoai))
            {
                query = query.Where(sp => sp.TenLoai.Contains(phanLoai));
            }
            if (!string.IsNullOrEmpty (hangSanXuat))
            {
                query = query.Where(sp => sp.TenHangSanXuat.Contains(hangSanXuat));
            }
            if (!string.IsNullOrEmpty(tenSanPham))
            {
                query = query.Where(sp => sp.TenSanPham.Contains(tenSanPham));
            }
            var ketQua = query.ToList();
            dataGridView.DataSource = ketQua;
            if (ketQua.Count == 0)
            {
                MessageBox.Show("Không tìm thấy sản phẩm nào khớp với thông tin!","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
        }
    }
}
