using CNPM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CNPM.Controllers
{
    public class ScheduleController : Controller
    {
        private TrungTamTheThaoEntities db = new TrungTamTheThaoEntities();
        // GET: Schedule
        public ActionResult Schedule()
        {
            int? loggedInUserId = Session["LoggedInUserId"] as int?;
            if (loggedInUserId != null)
            {
                // Lấy danh sách các gói đăng ký của khách hàng
                List<ThongTinDangKy> danhSachGoiDangKy = db.ThongTinDangKies
                    .Where(ttdk => ttdk.MaKH == loggedInUserId && ttdk.MAHLV != null) 
                    .ToList();

                // Lấy danh sách lịch tập tương ứng với các gói đăng ký
                List<LichTap> danhSachLichTap = new List<LichTap>();
                foreach (var goiDangKy in danhSachGoiDangKy)
                {
                    List<LichTap> lichTap = db.LichTaps.Where(lt => lt.MaDK == goiDangKy.MaDK && lt.Ngay != null).ToList();
                    if (lichTap != null && lichTap.Any())
                    {
                        danhSachLichTap.AddRange(lichTap);
                    }
                }

                return View(danhSachLichTap);
            }
            else
            {
                // Người dùng chưa đăng nhập, thực hiện xử lý tương ứng
                return View();
            }           
        }
    }
}