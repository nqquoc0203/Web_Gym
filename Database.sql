use master
go
CREATE DATABASE TrungTamTheThao
go
use TrungTamTheThao
go
CREATE TABLE LoaiKhachHang(
	MaLoai char(2) PRIMARY KEY,
	TenLoai nvarchar(30)
)
CREATE TABLE KhachHang(
	MaKH int IDENTITY(1,1) PRIMARY KEY,
	HoKH nvarchar(30),
	TenKH nvarchar(30),
	DiaChi nvarchar(100),
	Email varchar(50),
	MatKhau varchar(50),
	ChiTieu float ,
	SoTienTK float,
	SDT char(10),
	MaLoai char(2),
	FOREIGN KEY (MaLoai) REFERENCES LoaiKhachHang(MaLoai)
)

CREATE TABLE LoaiDichVu(
	MaDV varchar(6)  PRIMARY KEY,
	TenDV nvarchar(50)
)
CREATE TABLE GoiDichVu(
	MaGoi varchar(7)  PRIMARY KEY,
	TenGoi nvarchar(20),
	ThoiGian tinyint
)

CREATE TABLE ThongTinDangKy(
	MaDK int IDENTITY(1,1)  PRIMARY KEY,
	MaCT int ,
	MAHLV int,
	MaKH int,
	ThoiGianBatDau datetime,
	ThoiGianKetThuc datetime,
	GiaTienDK float,
	FOREIGN KEY (MaHLV) REFERENCES HuanLuyenVien(MaHLV),
	FOREIGN KEY (MaCT) REFERENCES ChiTietDichVu(MaCT),
	FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH)
)


ALTER TABLE HuanLuyenVien
ADD HinhAnh	varchar(100)


CREATE TABLE ChiTietDichVu(
	MaCT int  PRIMARY KEY,
	TenCT nvarchar(50),
	MaGoi varchar(7),
	MaDV varchar(6),
	GiaTien float ,
	MoTa nvarchar(500),
	HinhAnh varchar(100),
	FOREIGN KEY (MaGoi) REFERENCES GoiDichVu(MaGoi),
	FOREIGN KEY (MaDV) REFERENCES LoaiDichVu(MaDV)
)
CREATE TABLE GoiDichVu(
	MaGoi varchar(7)  PRIMARY KEY,
	TenGoi nvarchar(20),
	ThoiGian tinyint
)
CREATE TABLE LichTap(
	MaLT int IDENTITY(1,1) PRIMARY KEY, 
	MaDK int,
	Ngay date ,
	GioBatDau time not null,
	 GioKetThuc AS DATEADD(HOUR, 3, GioBatDau) persisted,
	TenPhong nvarchar(50),
	MoTa nvarchar(100),
	FOREIGN KEY (MaDK) REFERENCES ThongTinDangKy(MaDK)
) 


CREATE TABLE HuanLuyenVien(
	MaHLV int PRIMARY KEY,
	TenHLV nvarchar(50),
	MoTa nvarchar(100),
	GiaTien float,
	MaDV varchar(6),
	FOREIGN KEY (MaDV) REFERENCES LoaiDichVu(MaDV)
)

INSERT INTO LoaiKhachHang
	VALUES('BT',N'Bình thường'),
		('TT',N'Thân thiết')

INSERT INTO GoiDichVu
VALUES('1month',N'Gói 1 tháng',1),
('3month',N'Gói 3 tháng',3),
('6month',N'Gói 6 tháng',6),
('12month',N'Gói 12 tháng',12)

INSERT INTO LoaiDichVu
VALUES('GYM',N'Tập GYM cùng QDN CENTER'),
('BOXING',N'Tập BOXING cùng QDN CENTER'),
('ARB',N'Tập AEROBIC cùng QDN CENTER'),
('YOGA',N'Tập YOGA cùng QDN CENTER')


INSERT INTO ChiTietDichVu
VALUES(1,N'GYM 1 Tháng','1month','GYM',500000,N'Gói dịch vụ có thời hạn 1 tháng cho khách hàng. Bạn sẽ có quyền sử dụng tất cả các tiện ích và trang thiết bị thể thao trong suốt 1 tháng. Gói này thích hợp cho những người muốn thử nghiệm và tận hưởng trung tâm thể thao trong thời gian ngắn.','gym1.png'),
(2,N'GYM 3 Tháng','3month','GYM',800000,N'Gói dịch vụ kéo dài trong 3 tháng, cung cấp cơ hội để tận dụng môi trường tập luyện và các chương trình thể thao lâu hơn. Gói này phù hợp cho những người muốn duy trì sự thể thao và lối sống tích cực trong thời gian trung bình.','gym3.png'),
(3,N'GYM 6 Tháng','6month','GYM',1400000,N'Gói dịch vụ dài hạn kéo dài trong 6 tháng, cho phép bạn tiếp tục sử dụng trung tâm thể thao và tham gia các hoạt động trong suốt khoảng thời gian khá dài. Gói này thích hợp cho những người có quyết tâm duy trì lối sống thể thao và muốn tiết kiệm hơn so với việc mua gói 1 tháng hoặc 3 tháng.','gym6.png'),
(4,N'GYM 12 Tháng','12month','GYM',2000000,N'Gói dịch vụ dài nhất với thời hạn 12 tháng, cung cấp cơ hội tận hưởng trung tâm thể thao và các hoạt động trong suốt một năm. Gói này dành cho những người cam kết lâu dài và mong muốn duy trì một lối sống thể thao và khỏe mạnh trong thời gian dài nhất. Đây là gói tiết kiệm cao nhất so với các gói ngắn hạn.','gym12.png'),


INSERT INTO HuanLuyenVien
VALUES (1,'TRANCY',N'Chứng chỉ HLV quốc tế: Body Jam, Body Combat, Body Balance, Body Pump, RPM và Zumba',300000,'GYM','rectangle-5427.png'),
(2,'RACHEL', N'Có bằng cấp quốc tế: Body Combat, Body Pump, BodyJam, Body Balance',400000,'GYM','rectangle-5428.png'),
(3,'BELLA', N'Hiện tại đang giảng dạy các môn Les Mills: Bodyjam, BodyCombat, BodyBalance, Les Mills Core',200000,'GYM','rectangle-5429.png'),
(4,'TONYO', N'Đang giảng dạy các môn Les Mills — RPM, BodyPump, BodyCombat, BodyBalance, Les Mills Core',250000,'GYM','rectangle-5430.png')


