using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class CoachController : Controller
    {
        TrungTamTheThaoEntities db = new TrungTamTheThaoEntities();
        // GET: Coach
        public ActionResult Coach()
        {
            List<HuanLuyenVien> hlv = db.HuanLuyenViens.ToList();
            return View(hlv);
        }
        public ActionResult GymCoach(int? maCT , int? maKH)
        {
            if(Session["LoggedInUserId"] == null)
            {
                ViewBag.ErrorMessage = "Vui lòng đăng nhập trước";
                return RedirectToAction("Login", "LoginSignUp");
            }
            if(maCT == null)
            {
                ViewBag.ErrorMessage = "Vui lòng chọn gói dịch vụ trước";
                return RedirectToAction("Index","TrangChu");
            }
            var danhSachGoiDangKy = db.ThongTinDangKies
                                    .Where(ttdk => ttdk.MAHLV != null) 
                                    .Select(ttdk => ttdk.MAHLV) 
                                    .Distinct()
                                    .ToList();
            ViewBag.IsGymCoach = true;
            ViewBag.MaCT = maCT;
            ViewBag.MaKH = maKH;
            List<HuanLuyenVien> hlv_gym = db.HuanLuyenViens
                                        .Where(gym => gym.MaDV == "GYM" && !danhSachGoiDangKy.Contains(gym.MaHLV))
                                        .ToList();  
            return View("Coach",hlv_gym);
        }  
        
        [HttpPost]
        public ActionResult DangKyHLV(int? maCT, int? maKH, string maHLV)
        {
            try
            {
                if (string.IsNullOrEmpty(maHLV))
                {
                    // Xử lý nếu không có mã HLV được truyền vào
                    // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng về trang Coach
                    return RedirectToAction("Coach");
                }

                // Chuyển maHLV từ string sang int bằng int.TryParse()
                if (!int.TryParse(maHLV, out int maHLVInt))
                {
                    // Xử lý khi giá trị maHLV không hợp lệ
                    // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng về trang Coach
                    return RedirectToAction("Coach");
                }

                var hlv = db.HuanLuyenViens.FirstOrDefault(mahlv => mahlv.MaHLV == maHLVInt);
                if (hlv == null || maCT == null || maKH == null)
                {
                    // Xử lý nếu không tìm thấy thông tin HLV hoặc mã CT hoặc mã KH không hợp lệ
                    // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng về trang Coach
                    return RedirectToAction("Coach");
                }

                var khachhang = db.KhachHangs.FirstOrDefault(makh => makh.MaKH == maKH);
                if (khachhang == null)
                {
                    // Xử lý nếu không tìm thấy thông tin khách hàng
                    // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng về trang Coach
                    return RedirectToAction("Coach");
                }

                var chitietDichVu = db.ChiTietDichVus.FirstOrDefault(mact => mact.MaCT == maCT);
                if (chitietDichVu == null)
                {
                    // Xử lý nếu không tìm thấy thông tin chi tiết dịch vụ
                    // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng về trang Coach
                    return RedirectToAction("Coach");
                }

                // Kiểm tra nếu số dư tài khoản không đủ để đăng ký gói dịch vụ
                if (khachhang.SoTienTK < chitietDichVu.GiaTien + hlv.GiaTien)
                {
                    ViewBag.ErrorMessage = "Số dư tài khoản không đủ để đăng ký gói dịch vụ.";
                    return RedirectToAction("Coach");
                }

                // Truyền mã CT và mã HLV sang trang Gym.cshtml để xử lý đăng ký
                var thongTinDangKy = new ThongTinDangKy
                {
                    MaCT = maCT,
                    MaKH = maKH,
                    MAHLV = maHLVInt,
                    ThoiGianBatDau = DateTime.Now,
                    ThoiGianKetThuc = DateTime.Now.AddMonths((int)chitietDichVu.GoiDichVu.ThoiGian),
                    GiaTienDK = chitietDichVu.GiaTien + hlv.GiaTien,
                };

                db.ThongTinDangKies.Add(thongTinDangKy);         
                if (khachhang.MaLoai == "TT")
                {
                    khachhang.SoTienTK -= 0.85 * thongTinDangKy.GiaTienDK;
                    Session["SoTienCuaKH"] = khachhang.SoTienTK;
                    khachhang.ChiTieu += 0.85 * thongTinDangKy.GiaTienDK;
                }
                else
                {
                    khachhang.SoTienTK -= thongTinDangKy.GiaTienDK;
                    Session["SoTienCuaKH"] = khachhang.SoTienTK;
                    khachhang.ChiTieu += thongTinDangKy.GiaTienDK;
                }
                if (DemSoLanSuDungDichVu(khachhang.MaKH) >= 3)
                {
                    khachhang.MaLoai = "TT"; // Nâng cấp thành tài khoản thân thiết
                    ViewBag.ChucMungMessage = "Chúc mừng tài khoản của bạn đã được nâng cấp thành tài khoản thân thiết!";
                }

                db.SaveChanges();
                ThemLichTap(thongTinDangKy.MaDK);
                return RedirectToAction("Coach");
            }
            catch (Exception ex)
            {
                // Xử lý nếu có lỗi xảy ra
                // Ví dụ: Hiển thị thông báo lỗi hoặc chuyển hướng về trang Coach
                return RedirectToAction("Coach");
            }
        }
        private int DemSoLanSuDungDichVu(int maKH)
        {
            return db.ThongTinDangKies.Count(ttdk => ttdk.MaKH == maKH);
        }
        [HttpPost]
        public ActionResult ThemLichTap(int maDK)
        {
            var thongTinDangKy = db.ThongTinDangKies.FirstOrDefault(ttdk => ttdk.MaDK == maDK);

            if (thongTinDangKy != null)
            {
                for (int i = 0; i < 3; i++)
                {
                    var lichTap = new LichTap
                    {
                        MaDK = thongTinDangKy.MaDK,
                        Ngay = null, 
                        GioBatDau = new TimeSpan(0, 0, 0), 
                        TenPhong = "Chỉnh sửa",
                        MoTa = "Hãy chỉnh sửa lại ngày, giờ bắt đầu và giờ kết thúc",
                    };
                    db.LichTaps.Add(lichTap);
                }

                db.SaveChanges();
            }

            return RedirectToAction("Coach");
        }
    }
}