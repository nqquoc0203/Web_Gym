
using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Razor.Tokenizer;

namespace CNPM.Controllers
{
    public class LoginSignUpController : Controller
    {
        private TrungTamTheThaoEntities db = new TrungTamTheThaoEntities();
  
        // GET: LoginSignUp
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string email, string pass)
        {
            try
            {
                
                var khachHang = db.KhachHangs.FirstOrDefault(kh => kh.Email == email);

                if (khachHang == null)
                {
                    ViewBag.ErrorMessage = "Tài khoản không tồn tại.";
                    return View();
                }
                if (khachHang.MatKhau != pass)
                {
                    ViewBag.ErrorMessage = "Mật khẩu không đúng.";
                    return View();
                }
                Session["LoggedInUserId"] = khachHang.MaKH;
                Session["NameUser"] = khachHang.HoKH +" "+ khachHang.TenKH;
                Session["EmailUser"] = khachHang.Email;
                Session["SoTienCuaKH"] = khachHang.SoTienTK;
                return RedirectToAction("Index", "TrangChu"); 
            }
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình đăng nhập. Vui lòng thử lại sau.";
                return View();
            }
        }
        public ActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignUp(FormCollection form)
        {
            try
            {
               
                string ho = form["Ho"];
                string ten = form["Ten"];
                string email = form["Email"];
                string matKhau = form["MatKhau"];
                string repassword = form["repassword"];
                string soDienThoai = form["SoDienThoai"];
                string diaChi = form["DiaChi"];

                if (string.IsNullOrEmpty(ho) || string.IsNullOrEmpty(ten)
                   || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(matKhau)
                   || string.IsNullOrEmpty(repassword) || string.IsNullOrEmpty(soDienThoai)
                   || string.IsNullOrEmpty(diaChi))
                {
                    ViewBag.ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
                    return View();
                }

                if (matKhau != repassword)
                {
                    ViewBag.ErrorMessage = "Mật khẩu xác nhận không khớp";
                    return View();
                }
                if (db.KhachHangs.Any(kh => kh.Email == email))
                {
                    ViewBag.ErrorMessage = "Email đã tồn tại trong hệ thống.";
                    return View();
                }

                
                var KhachHang = new KhachHang
                {
                    HoKH = ho,
                    TenKH = ten,
                    Email = email,
                    MatKhau = matKhau,
                    SDT = soDienThoai,
                    DiaChi = diaChi,
                    ChiTieu = 0,
                    MaLoai = "BT",
                    SoTienTK = 3000000
                };
                
                db.KhachHangs.Add(KhachHang);
                db.SaveChanges();

                
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                
                ViewBag.ErrorMessage = "Đã xảy ra lỗi trong quá trình đăng ký. Vui lòng thử lại sau.";
                return View();
            }
        }
        public ActionResult Logout()
        {    
            Session["LoggedInUserId"] = null;
            Session["NameUser"] = null;
            Session["EmailUser"] = null;
            Session["SoTienCuaKH"] = null;
            return RedirectToAction("Index", "TrangChu");
        }
    }
}