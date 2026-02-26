using Microsoft.EntityFrameworkCore;
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

using Microsoft.Office.Interop.Excel;
using System.IO;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace QuanLyBanHang.Forms
{
    public partial class frmHoaDon : Form
    {
        public frmHoaDon()
        {
            InitializeComponent();
        }

        QLBHDbContext context = new QLBHDbContext(); // Khởi tạo biến ngữ cảnh CSDL
        int id;

        private void frmHoaDon_Load(object sender, EventArgs e)
        {
            dataGridView.AutoGenerateColumns = false;
            List<DanhSachHoaDon> hd = new List<DanhSachHoaDon>();
            hd = context.HoaDon.Select(r => new DanhSachHoaDon
            {
                ID = r.ID,
                NhanVienID = r.NhanVienID,
                HoVaTenNhanVien = r.NhanVien.HoVaTen,
                KhachHangID = r.KhachHangID,
                HoVaTenKhachHang = r.KhachHang.HoVaTen,
                NgayLap = r.NgayLap,
                GhiChuHoaDon = r.GhiChuHoaDon,
                TongTienHoaDon = r.HoaDon_ChiTiet.Sum(r => r.SoLuongBan * r.DonGiaBan),
                XemChiTiet = "Xem chi tiết"
            }).ToList();
            dataGridView.DataSource = hd;

            cboTimKiem.Items.Clear();
            foreach (DataGridViewColumn col in dataGridView.Columns)
            {
                if (col.Visible && !string.IsNullOrEmpty(col.HeaderText))
                {
                    cboTimKiem.Items.Add(col.HeaderText);
                }
            }

            if (cboTimKiem.Items.Count > 0)
            {
                cboTimKiem.SelectedIndex = 0;
            }
        }
        private void btnLapHoaDon_Click(object sender, EventArgs e)
        {
            using (frmHoaDon_ChiTiet chiTiet = new frmHoaDon_ChiTiet())
            {
                chiTiet.ShowDialog();
            }
        }

        private void btnSua_Click(object sender, EventArgs e)
        {
            id = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value.ToString());
            using (frmHoaDon_ChiTiet chiTiet = new frmHoaDon_ChiTiet(id))
            {
                chiTiet.ShowDialog();
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            if (dataGridView.CurrentRow != null)
            {
                int idHoaDon = Convert.ToInt32(dataGridView.CurrentRow.Cells["ID"].Value);

                if (MessageBox.Show($"Bạn có chắc chắn muốn xóa hóa đơn mã số {idHoaDon} không?",
                    "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        var hoaDon = context.HoaDon.Find(idHoaDon);

                        if (hoaDon != null)
                        {
                            context.HoaDon.Remove(hoaDon);
                            context.SaveChanges();

                            MessageBox.Show("Xóa hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            frmHoaDon_Load(sender, e);
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy hóa đơn cần xóa.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi khi xóa: " + ex.Message, "Lỗi hệ thống", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn một hóa đơn trong danh sách để xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnThoat_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void ExportToExcel(string path)
        {
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add(Type.Missing);
            Microsoft.Office.Interop.Excel._Worksheet worksheet = workbook.ActiveSheet;
            try
            {
                for (int i = 1; i <= dataGridView.Columns.Count; i++)
                {
                    worksheet.Cells[1, i] = dataGridView.Columns[i - 1].HeaderText;
                }
                for (int i = 0; i < dataGridView.Rows.Count; i++)
                {
                    for (int j = 0; j < dataGridView.Columns.Count; j++)
                    {
                        worksheet.Cells[i + 2, j + 1] = dataGridView.Rows[i].Cells[j].Value?.ToString();
                    }
                }
                workbook.SaveAs2(path);
                MessageBox.Show("Lưu file thành công!", "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Có lỗi xảy ra:" + ex.Message);
            }
            finally
            {
                workbook.Close();
                excelApp.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
            }
        }
        private void btnXuat_Click(object sender, EventArgs e)
        {
            if (dataGridView.Rows.Count > 0)
            {
                //Khởi tạo savefiledialog để chọn nơi lưu
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "Excel Workbook (*.xlsx)|*.xlsx|All Files (*.*)|*.*";
                //gợi ý tên mặc định (người dùng có thể sửa lại tên này trong hộp thoại
                sfd.FileName = "HoaDon_" + DateTime.Now.ToString("ddMMyyyy_HHmm");
                //thiết lập tiêu đề cho cửa sổ chọn file
                sfd.Title = "Chọn nơi lưu và đặt tên cho file Excel của bạn";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string filePath = sfd.FileName;
                    ExportToExcel(filePath);
                }
                /*Excel.Application excelApp = new Excel.Application();
                excelApp.Application.Workbooks.Add(Type.Missing);
                for (int i = 1; i < dataGridView.Rows.Count + 1; i++)
                {
                    excelApp.Cells[1, i] = dataGridView.Columns[i - 1].HeaderText;
                    excelApp.Cells[1, i].Font.Bold = true;
                }
                for (int i =0; i < dataGridView.Rows.Count; i++)
                {
                    for (int j=0;j<dataGridView.Columns.Count;j++)
                    {
                        excelApp.Cells[i+2,j+1] = dataGridView.Rows[i].Cells[j].Value?.ToString();
                    }
                }
                excelApp.Columns.AutoFit();
                excelApp.Visible = true;
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);*/
            }
            else
            {
                MessageBox.Show("Không có dữ liệu để xuất!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string danhMuc = cboTimKiem.Text;
            string tuKhoa = txtTimKiem.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(tuKhoa))
            {
                return;
            }
            var query = context.HoaDon.AsQueryable();
            if (danhMuc == "ID")
            {
                query = query.Where(hd => hd.ID.ToString().Contains(tuKhoa));
            }
            else if (danhMuc == "Nhân viên")
            {
                query = query.Where(hd => hd.NhanVien.HoVaTen.ToLower().Contains(tuKhoa));
            }
            else if (danhMuc == "Khách hàng")
            {
                query = query.Where(hd => hd.KhachHang.HoVaTen.ToLower().Contains(tuKhoa));
            }

            var ketQua = query.Select(hd => new
            {
                ID = hd.ID,
                NhanVien = hd.NhanVien.HoVaTen,
                KhachHang = hd.KhachHang.HoVaTen,
                NgayLap = hd.NgayLap,
                TongTien = hd.HoaDon_ChiTiet.Sum(ct => ct.SoLuongBan * ct.DonGiaBan)

            }).ToList();
            dataGridView.DataSource = ketQua;

            if (ketQua.Count == 0)
            {
                MessageBox.Show("Không tìm thấy kết quả phù hợp.","Thông báo",MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
