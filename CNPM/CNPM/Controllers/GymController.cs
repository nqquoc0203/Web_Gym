using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class GymController : Controller
    {
        TrungTamTheThaoEntities db  = new TrungTamTheThaoEntities();
        public ActionResult Index()
        {
            List<ChiTietDichVu> danhSachChiTietDichVu = db.ChiTietDichVus
                .Where(ctdv => ctdv.MaDV == "GYM")
                .ToList();
            return View(danhSachChiTietDichVu);
        }
        public ActionResult Details(int? maCT)
        {
            if (maCT == null)
            {
                
                return RedirectToAction("Index");
            }

            var chiTietDichVu = db.ChiTietDichVus.FirstOrDefault(ctdv => ctdv.MaCT == maCT);

            if (chiTietDichVu == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.TenCT = chiTietDichVu.TenCT;
            ViewBag.MoTa = chiTietDichVu.MoTa;
            ViewBag.MaCT = chiTietDichVu.MaCT;
            return View("Index", db.ChiTietDichVus.Where(ctdv => ctdv.MaDV == "GYM").ToList());
        }
        [HttpPost]
        public ActionResult DangKy(int? maCT, bool isGanHLV, string maHLV)
        {
            try
            {               
                string hoTen = Session["NameUser"] as string;
                string email = Session["EmailUser"] as string;                              

                if (maCT == null)
                {
                    ViewBag.ErrorMessage = "Vui lòng chọn gói dịch vụ trước khi đăng ký.";
                    //ViewBag.ShowButton = true;
                    return View("Index", db.ChiTietDichVus.Where(ctdv => ctdv.MaDV == "GYM").ToList());
                }             
                if (string.IsNullOrEmpty(hoTen) || string.IsNullOrEmpty(email))
                {
                    ViewBag.ErrorMessage = "Vui lòng đăng nhập trước khi đăng ký gói dịch vụ.";
                    //ViewBag.ShowButton = true;
                    return View("Index", db.ChiTietDichVus.Where(ctdv => ctdv.MaDV == "GYM").ToList());
                }  
                
                var khachhang = db.KhachHangs.FirstOrDefault(makh => makh.Email == email);
                //if (DaDangKyGym(khachhang.MaKH))
                //{
                //    ViewBag.ErrorMessage = "Bạn đã đăng ký gói dịch vụ GYM rồi, không thể đăng ký thêm.";
                //    ViewBag.ShowButton = false;
                //    return View("Index", db.ChiTietDichVus.Where(ctdv => ctdv.MaDV == "GYM").ToList());
                //}
                var chitietDichVu = db.ChiTietDichVus.FirstOrDefault(mact => mact.MaCT == maCT);
                var goiDichVu = db.GoiDichVus.FirstOrDefault(gdv => gdv.MaGoi == chitietDichVu.MaGoi);
                DateTime thoiGianKetThuc = DateTime.Now.AddMonths((int)goiDichVu.ThoiGian);
                if (khachhang.SoTienTK < chitietDichVu.GiaTien)
                {
                    ViewBag.ErrorMessage = "Số dư tài khoản không đủ để đăng ký gói dịch vụ.";
                    //ViewBag.ShowButton = true; 
                    return View("Index", db.ChiTietDichVus.Where(ctdv => ctdv.MaDV == "GYM").ToList());
                }               
                    
                var thongTinDangKy = new ThongTinDangKy
                {
                    MaCT = maCT,
                    MaKH = khachhang.MaKH,                  
                    ThoiGianBatDau = DateTime.Now,
                    ThoiGianKetThuc = thoiGianKetThuc ,
                    GiaTienDK = chitietDichVu.GiaTien,


                };
                if (isGanHLV)
                {
                    //ViewBag.ShowButton = false; 
                    return RedirectToAction("GymCoach", "Coach", new { maCT = maCT , maKH = khachhang.MaKH });
                }
                else
                {
                    if (khachhang.MaLoai == "TT")
                    {
                        khachhang.SoTienTK -= 0.85 * chitietDichVu.GiaTien;
                        Session["SoTienCuaKH"] = khachhang.SoTienTK;
                        khachhang.ChiTieu += 0.85 * chitietDichVu.GiaTien;
                    }
                    else
                    {
                        khachhang.SoTienTK -= chitietDichVu.GiaTien;
                        Session["SoTienCuaKH"] = khachhang.SoTienTK;
                        khachhang.ChiTieu += chitietDichVu.GiaTien;
                    }
                    if (DemSoLanSuDungDichVu(khachhang.MaKH) >= 3)
                    {
                        khachhang.MaLoai = "TT"; // Nâng cấp thành tài khoản thân thiết
                        ViewBag.ChucMungMessage = "Chúc mừng tài khoản của bạn đã được nâng cấp thành tài khoản thân thiết!";
                    }

                    thongTinDangKy.MAHLV = null;
                    db.ThongTinDangKies.Add(thongTinDangKy);
                    db.SaveChanges();
                }           
                //ViewBag.ShowButton = false;               
                return View("Index", db.ChiTietDichVus.Where(ctdv => ctdv.MaDV == "GYM").ToList());
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại sau.";                
                return View("Index", db.ChiTietDichVus.Where(ctdv => ctdv.MaDV == "GYM").ToList());
            }
        }
        private int DemSoLanSuDungDichVu(int maKH)
        {
            return db.ThongTinDangKies.Count(ttdk => ttdk.MaKH == maKH);
        }
        public ActionResult ChonHLV(int maCT)
        {
            // Lưu maCT vào Session để sử dụng ở trang HLV (nếu cần)
            Session["MaCT"] = maCT;

            // Chuyển hướng đến trang HLV
            return RedirectToAction("Index", "HLV");
        }
        private bool DaDangKyGym(int maKH)
        {
            // Kiểm tra trong bảng ThongTinDangKy xem có bất kỳ bản ghi nào của khách hàng với MaKH và MaDV là "GYM" hay không
            return db.ThongTinDangKies.Any(tt => tt.MaKH == maKH && tt.MaCT == db.ChiTietDichVus.FirstOrDefault(ctdv => ctdv.MaDV == "GYM").MaCT);
        }
    }
}