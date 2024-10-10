using DoctorAPI.Models;
using DoctorAPI.Dtos;
using DoctorAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

namespace DoctorAPI.Services
{
    public class DoctorService
    {
        private readonly DbContextClass _dbContext;
        private readonly IConfiguration _configuration;

        public DoctorService(DbContextClass dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Convert Base64 to byte array
        // Convert Base64 to byte array with validation
        private byte[] ConvertBase64ToBytes(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            // Strip the metadata if it exists (e.g., data:image/png;base64,)
            var base64Data = base64String.Contains(",")
                ? base64String.Substring(base64String.IndexOf(",") + 1)
                : base64String;

            // Regular expression to check if the string is valid Base64
            if (!IsValidBase64(base64Data))
                throw new Exception("Invalid Base64 string format for image.");

            try
            {
                return Convert.FromBase64String(base64Data);
            }
            catch (FormatException ex)
            {
                throw new Exception("Invalid Base64 string format for image.", ex);
            }
        }


        // Helper method to validate Base64 string
        private bool IsValidBase64(string base64String)
        {
            // Remove any spaces or padding characters first
            base64String = base64String.Trim();
            // Validate the Base64 string using regular expression
            return (base64String.Length % 4 == 0) &&
                System.Text.RegularExpressions.Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,2}$", System.Text.RegularExpressions.RegexOptions.None);
        }


        // Convert byte array to Base64 string
        private string ConvertBytesToBase64(byte[] bytes)
        {
            return bytes == null ? null : Convert.ToBase64String(bytes);
        }

        // Get all doctors
        // Get all doctors
        public async Task<IEnumerable<DoctorDto>> GetAllDoctors()
        {
            var doctors = await _dbContext.Doctors.ToListAsync();

            return doctors.Select(doctor => new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber,
                ImageBase64 = ConvertBytesToBase64(doctor.Image) // Include Base64 image string
            });
        }

        // Get doctor by ID
        public async Task<DoctorDto> GetDoctorById(string id)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor == null) return null;

            return new DoctorDto
            {
                DoctorId = doctor.DoctorId,
                Name = doctor.Name,
                Specialization = doctor.Specialization,
                Email = doctor.Email,
                PhoneNumber = doctor.PhoneNumber,
                ImageBase64 = ConvertBytesToBase64(doctor.Image) // Include Base64 image string
            };
        }


        public async Task<DoctorInternalDto> GetDoctorByUsername(string id)
        {
            var doctor = await _dbContext.Doctors
                .Where(d => d.DoctorId == id || d.Email == id)
                .Select(d => new DoctorInternalDto
                {
                    DoctorId = d.DoctorId,
                    Name = d.Name,
                    Specialization = d.Specialization,
                    Email = d.Email,
                    PhoneNumber = d.PhoneNumber,
                    Password = d.Password,
                    Address=d.Address,
                    DOB=d.DOB,
                    Gender=d.Gender,
                    ImageBase64=d.ImageBase64,
                })
                .FirstOrDefaultAsync();

            return doctor;
        }

        // Create a new doctor
        public async Task<Doctor> CreateDoctor(Doctor doctor)
        {
            try
            {
                // Convert Base64 image to byte array
                if (!string.IsNullOrEmpty(doctor.ImageBase64))
                {
                    doctor.Image = ConvertBase64ToBytes(doctor.ImageBase64);
                }

                if (await _dbContext.Doctors.AnyAsync(d => d.Email == doctor.Email))
                {
                    throw new Exception("A Doctor with this email already exists.");
                }

                var newId = GenerateNewId();
                while (await _dbContext.Doctors.AnyAsync(d => d.DoctorId == newId))
                {
                    newId = GenerateNewId();
                }
                doctor.DoctorId = newId;
                string unHasedPassword = doctor.Password;
                doctor.Password = BCrypt.Net.BCrypt.HashPassword(doctor.Password);
                _dbContext.Doctors.Add(doctor);
                await _dbContext.SaveChangesAsync();

                string resetToken = Guid.NewGuid().ToString();

                // Send Email Notification
                await SendEmailNotificationAsync(doctor.Email, doctor.Name, doctor.DoctorId, unHasedPassword, resetToken);
                return doctor;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error while creating doctor", ex);
            }
        }

        private async Task SendEmailNotificationAsync(string email, string name, string DoctorId, string unHasedPassword, string resetToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(DoctorId))
                throw new ArgumentException("Patient ID cannot be null or empty.", nameof(DoctorId));
            if (string.IsNullOrWhiteSpace(unHasedPassword))
                throw new ArgumentException("Password cannot be null or empty.", nameof(unHasedPassword));
            if (string.IsNullOrWhiteSpace(resetToken))
                throw new ArgumentException("Reset token cannot be null or empty.", nameof(resetToken));

            string subject = "Welcome to Healify!";
            string resetLink = $"http://localhost:4200/reset-password?token={resetToken}&patientId={DoctorId}";

            var body = $@"
    <p>Hello Dr.{name},</p>
    <p>Your Doctor ID is: {DoctorId}</p>
    <p>Your Temporary Password is: {unHasedPassword}</p>
    <p>To reset your password. You can reset it by clicking the link below:</p>
    <p><a href='{resetLink}'>Click here to reset your password</a></p>
    <p>Best regards,<br>Healify :)</p>";

            try
            {
                using (var message = new MailMessage("medicalportal01@gmail.com", email)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // Set to true to send HTML content
                })
                {
                    var smtpHost = _configuration["Smtp:Host"];
                    var smtpPort = int.Parse(_configuration["Smtp:Port"]); // Corrected the syntax
                    var smtpUsername = _configuration["Smtp:Username"];
                    var smtpPassword = _configuration["Smtp:Password"];
                    var enableSsl = bool.Parse(_configuration["Smtp:EnableSSL"]); // Corrected the syntax


                    using (var client = new SmtpClient(smtpHost, smtpPort))
                    {
                        client.Credentials = new System.Net.NetworkCredential(smtpUsername, smtpPassword);
                        client.EnableSsl = enableSsl;
                        await client.SendMailAsync(message);
                    }
                }
            }
            catch (FormatException ex)
            {
                throw new Exception("Error while parsing SMTP configuration.", ex);
            }
            catch (SmtpException ex)
            {
                throw new Exception("SMTP error occurred while sending email.", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sending email", ex);
            }
        }



        // Update an existing doctor
        public async Task<Doctor> UpdateDoctor(string id, Doctor updatedDoctor)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor == null) return null;

            doctor.Name = updatedDoctor.Name;
            doctor.Specialization = updatedDoctor.Specialization;
            doctor.Email = updatedDoctor.Email;
            doctor.PhoneNumber = updatedDoctor.PhoneNumber;

            if (!string.IsNullOrEmpty(updatedDoctor.ImageBase64))
            {
                doctor.Image = ConvertBase64ToBytes(updatedDoctor.ImageBase64);
            }

            await _dbContext.SaveChangesAsync();
            return doctor;
        }

        // Delete a doctor
        public async Task<bool> DeleteDoctor(string id)
        {
            var doctor = await _dbContext.Doctors.FindAsync(id);
            if (doctor == null) return false;

            _dbContext.Doctors.Remove(doctor);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Generate a new unique ID
        private string GenerateNewId()
        {
            var lastDoctor = _dbContext.Doctors
                .OrderByDescending(d => d.DoctorId)
                .FirstOrDefault();

            if (lastDoctor == null)
            {
                return "D001";
            }

            try
            {
                var lastIdNumber = int.Parse(lastDoctor.DoctorId.Substring(1));
                var newIdNumber = lastIdNumber + 1;
                return $"D{newIdNumber:D3}";
            }
            catch (FormatException ex)
            {
                throw new Exception("Error while parsing the last doctor ID", ex);
            }
        }

        // Get distinct specializations
        public async Task<IEnumerable<string>> GetDistinctSpecializations()
        {
            var specializations = await _dbContext.Doctors
                .Select(d => d.Specialization)
                .Distinct()
                .ToListAsync();

            return specializations;
        }

    }
}
