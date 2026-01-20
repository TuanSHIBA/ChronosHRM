using Chronos.Application.DTOs.Attendance;
using Chronos.Application.IServices;
using Chronos.Domain.Entity;
using Chronos.Domain.Interfaces;
using System;
using System.Threading.Tasks;

namespace Chronos.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;

        // Chỉ Inject IUnitOfWork, không Inject DbContext nữa
        public AttendanceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. Lấy thông tin chấm công hôm nay
        public async Task<AttendanceDto?> GetTodayAttendance(Guid employeeId)
        {
            var today = DateTime.Now.Date;

            var entity = (await _unitOfWork.Attendance.GetAllAsync(includeProperties: "Employee")).FirstOrDefault(a => a.EmployeeId == employeeId && a.Date == today);
            var attendanceDto = new AttendanceDto
            {
                Id = entity.Id,
                EmployeeId = entity.EmployeeId,
                // 👇 Map dữ liệu từ bảng Employee sang
                EmployeeName = entity.Employee != null ? entity.Employee.FullName : "N/A",
                EmployeeCode = entity.Employee != null ? entity.Employee.EmployeeCode : "N/A",

                Date = entity.Date,
                // Format giờ cho đẹp
                CheckInTime = entity.CheckInTime?.ToString(@"hh\:mm"),
                CheckOutTime = entity.CheckOutTime?.ToString(@"hh\:mm"),
                WorkingHours = entity.WorkingHours
            };
            return attendanceDto;
        }

        // 2. Xử lý Check In
        public async Task<string> CheckIn(Guid employeeId)
        {
            var today = DateTime.Now.Date;

            // Kiểm tra đã check-in chưa
            var existingRecord = (await _unitOfWork.Attendance.GetAllAsync(includeProperties: "Employee")).FirstOrDefault(a => a.EmployeeId == employeeId && a.Date == today);

            if (existingRecord != null)
            {
                return "Bạn đã Check-in ngày hôm nay rồi!";
            }

            var newRecord = new Attendance
            {
                EmployeeId = employeeId,
                Date = today,
                CheckInTime = DateTime.Now.TimeOfDay,
                CheckOutTime = null,
                WorkingHours = 0
            };

            // Thêm vào qua Repository
            await _unitOfWork.Attendance.AddAsync(newRecord);

            // Lưu xuống DB qua UnitOfWork
            await _unitOfWork.SaveChangesAsync();

            return "Check-in thành công!";
        }

        // 3. Xử lý Check Out
        public async Task<string> CheckOut(Guid employeeId)
        {
            var today = DateTime.Now.Date;

            // Tìm bản ghi qua Repository
            var record = (await _unitOfWork.Attendance.GetAllAsync(includeProperties: "Employee")).FirstOrDefault(a => a.EmployeeId == employeeId && a.Date == today);

            if (record == null)
            {
                return "Bạn chưa Check-in, không thể Check-out!";
            }

            // Cập nhật thông tin
            record.CheckOutTime = DateTime.Now.TimeOfDay;

            if (record.CheckInTime.HasValue)
            {
                var duration = record.CheckOutTime.Value - record.CheckInTime.Value;
                record.WorkingHours = duration.TotalHours;
            }

            // Một số Generic Repository cần gọi hàm Update, một số thì EF tự track.
            // Để chắc chắn, bạn cứ gọi Update
            _unitOfWork.Attendance.Update(record);

            // Lưu thay đổi
            await _unitOfWork.SaveChangesAsync();

            return "Check-out thành công!";
        }
    }
}