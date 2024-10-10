using AppointmentAPI.Data;
using AppointmentAPI.Dtos;
using AppointmentAPI.Model;
using Microsoft.EntityFrameworkCore;

namespace AppointmentAPI.Services
{
    public class AppointmentService
    {
        private readonly DbContextClass _dbContext;
        private readonly IConfiguration _configuration;

        public AppointmentService(DbContextClass dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Get all appointments
        public async Task<IEnumerable<AppointmentDto>> GetAllAppointments()
        {
            // Fetch all appointments from the database
            var appointments = await _dbContext.Appointments.ToListAsync();

            // Map the Appointment entities to AppointmentDto
            return appointments.Select(appointment => new AppointmentDto
            {
                PatientName = appointment.PatientName,
                PatientId = appointment.PatientId,
                PhoneNumber = appointment.PhoneNumber,
                Date = appointment.Date,
                Speciality = appointment.Speciality,
                DoctorId = appointment.DoctorId,
                SelectedSlot = appointment.SelectedSlot,
                AppointmentId=appointment.AppointmentId,
                DoctorStatus=appointment.DoctorStatus,
            }).ToList();
        }

        // Get appointment by ID
        public async Task<Appointment> GetAppointmentById(string id)
        {
            return await _dbContext.Appointments.FindAsync(id);
        }

        // Create a new appointment
        public async Task<Appointment> CreateAppointment(Appointment appointment)
        {
            try
            {
                // Check if AppointmentId is provided; if not, generate a new one.
                if (string.IsNullOrEmpty(appointment.AppointmentId))
                {
                    appointment.AppointmentId = GenerateNewAppointmentId();
                }

                //Create a new BookedAppointment
                var bookedAppointment = new BookedAppointment
                {
                    BookedId = GenerateNewBookedId(), // Generate a new unique ID
                    AvailabilityId = appointment.AvailabilityId,  // Use the correct availability ID
                    Time = appointment.SelectedSlot,           // Set the booked time
                    StatusBooked = true  ,    
                    AppointmentId=appointment.AppointmentId// Set the status to 'Booked'
                };

                // Add BookedAppointment to the database
                _dbContext.BookedAppointments.Add(bookedAppointment);
                await _dbContext.SaveChangesAsync();

                appointment.PatientStatus = "Scheduled";
                appointment.DoctorStatus = "Pending";
                // Add the Appointment to the database
                _dbContext.Appointments.Add(appointment);
                await _dbContext.SaveChangesAsync();

                return appointment;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error while creating appointment", ex);
            }
        }

        // Update an existing appointment
        public async Task<bool> UpdateAppointment(string id, Appointment updatedAppointment)
        {
            var appointment = await _dbContext.Appointments.FindAsync(id);
            if (appointment == null) return false;

            // Update properties
            appointment.PatientName = updatedAppointment.PatientName;
            appointment.PatientId = updatedAppointment.PatientId;
            appointment.PhoneNumber = updatedAppointment.PhoneNumber;
            appointment.Date = updatedAppointment.Date;
            appointment.Speciality = updatedAppointment.Speciality;
            appointment.DoctorId = updatedAppointment.DoctorId;
            appointment.SelectedSlot = updatedAppointment.SelectedSlot;

            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Delete an appointment
        public async Task<bool> DeleteAppointment(string id)
        {
            // Find the appointment to delete
            var appointment = await _dbContext.Appointments.FindAsync(id);
            if (appointment == null) return false;

            // Remove related bookings from the BookedAppointments table
            var bookedAppointments = await _dbContext.BookedAppointments
                .Where(booking => booking.AppointmentId == id) // Adjust the condition based on your foreign key relationship
                .ToListAsync();

            if (bookedAppointments.Any())
            {
                _dbContext.BookedAppointments.RemoveRange(bookedAppointments); // Remove all related bookings
            }

            // Remove the appointment
            _dbContext.Appointments.Remove(appointment);
            await _dbContext.SaveChangesAsync();
            return true;
        }


        // Generate a new unique appointment ID
        private string GenerateNewAppointmentId()
        {
            var lastAppointment = _dbContext.Appointments
                .OrderByDescending(a => a.AppointmentId)
                .FirstOrDefault();

            if (lastAppointment == null)
            {
                return "AP001";
            }

            try
            {
                var lastIdNumber = int.Parse(lastAppointment.AppointmentId.Substring(2)); // Skip "AP"
                var newIdNumber = lastIdNumber + 1;
                return $"AP{newIdNumber:D3}"; // Format as AP001, AP002, etc.
            }
            catch (FormatException ex)
            {
                throw new Exception("Error while parsing the last appointment ID", ex);
            }
        }

        // Generate a new unique booked appointment ID
        private string GenerateNewBookedId()
        {
            var lastBooked = _dbContext.BookedAppointments
                .OrderByDescending(b => b.BookedId)
                .FirstOrDefault();

            if (lastBooked == null)
            {
                return "BK001";
            }

            try
            {
                var lastIdNumber = int.Parse(lastBooked.BookedId.Substring(2)); // Skip "BK"
                var newIdNumber = lastIdNumber + 1;
                return $"BK{newIdNumber:D3}"; // Format as BK001, BK002, etc.
            }
            catch (FormatException ex)
            {
                throw new Exception("Error while parsing the last booked ID", ex);
            }
        }
        public async Task<IEnumerable<BookedAppointment>> GetAllBookedAppointments()
        {
            // Logic to retrieve all booked appointments from the database
            return await _dbContext.BookedAppointments.ToListAsync(); // Assuming you're using Entity Framework Core
        }

    }
}
