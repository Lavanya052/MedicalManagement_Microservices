using PatientAPI.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using PatientAPI.Dto;
using PatientAPI.Model;

namespace PatientAPI.Services
{
    public class PatientService
    {
        private readonly DbContextClass _dbContext;
        private readonly IConfiguration _configuration;

        public PatientService(DbContextClass dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        // Convert Base64 to byte array with validation
        private byte[] ConvertBase64ToBytes(string base64String)
        {
            if (string.IsNullOrEmpty(base64String))
                return null;

            var base64Data = base64String.Contains(",")
                ? base64String.Substring(base64String.IndexOf(",") + 1)
                : base64String;

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

        private bool IsValidBase64(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                System.Text.RegularExpressions.Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,2}$", System.Text.RegularExpressions.RegexOptions.None);
        }

        private string ConvertBytesToBase64(byte[] bytes)
        {
            return bytes == null ? null : Convert.ToBase64String(bytes);
        }

        // Get all patients
        public async Task<IEnumerable<PatientDto>> GetAllPatients()
        {
            var patients = await _dbContext.Patients.ToListAsync();

            return patients.Select(patient => new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                BloodGroup = patient.BloodGroup,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                ImageBase64 = ConvertBytesToBase64(patient.Image),
                Address =patient.Address,
                Gender =patient.Gender,
                DOB =patient.DOB,
            });
        }

        // Get patient by ID
        public async Task<PatientDto> GetPatientById(string id)
        {
            var patient = await _dbContext.Patients.FindAsync(id);
            if (patient == null) return null;

            return new PatientDto
            {
                PatientId = patient.PatientId,
                Name = patient.Name,
                BloodGroup = patient.BloodGroup,
                Email = patient.Email,
                PhoneNumber = patient.PhoneNumber,
                ImageBase64 = ConvertBytesToBase64(patient.Image),
                Address = patient.Address,
                Gender = patient.Gender,
                DOB = patient.DOB,
            };
        }

        // Get patient by Username or Email
        public async Task<PatientInternalDto> GetPatientByUsername(string id)
        {
            var patient = await _dbContext.Patients
                .Where(p => p.PatientId == id || p.Email == id)
                .Select(p => new PatientInternalDto
                {
                    PatientId = p.PatientId,
                    Name = p.Name,
                    Email = p.Email,
                    PhoneNumber = p.PhoneNumber,
                    Password = p.Password,  // Be cautious when exposing this data
                    Address = p.Address,
                    DOB = p.DOB,
                    Gender = p.Gender,
                    ImageBase64 = ConvertBytesToBase64(p.Image),
                    CreatedAt = p.CreatedAt,
                    UpdatedAt = p.UpdatedAt,
                    IsActive = p.IsActive,
                    LastLogin = p.LastLogin
                })
                .FirstOrDefaultAsync();

            return patient;
        }

        // Create a new patient
        public async Task<Patient> CreatePatient(Patient patient)
        {
            try
            {
                if (!string.IsNullOrEmpty(patient.ImageBase64))
                {
                    patient.Image = ConvertBase64ToBytes(patient.ImageBase64);
                }

                // Check if email already exists
                if (await _dbContext.Patients.AnyAsync(p => p.Email == patient.Email))
                {
                    throw new Exception("A patient with this email already exists.");
                }

                var newId = GenerateNewId();
                while (await _dbContext.Patients.AnyAsync(p => p.PatientId == newId))
                {
                    newId = GenerateNewId();
                }
                patient.PatientId = newId;

                string unHasedPassword = patient.Password; // Store the unhashed password for email
                patient.Password = BCrypt.Net.BCrypt.HashPassword(patient.Password); // Hash the password

                _dbContext.Patients.Add(patient);
                await _dbContext.SaveChangesAsync();

                // Generate a reset token (this is a placeholder; you should implement token generation logic)
                string resetToken = Guid.NewGuid().ToString();

                // Send Email Notification
                await SendEmailNotificationAsync(patient.Email, patient.Name, patient.PatientId, unHasedPassword, resetToken);

                return patient;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Error while creating patient", ex);
            }
        }

        // Send email notification to the patient
        private async Task SendEmailNotificationAsync(string email, string name, string patientId, string unHasedPassword, string resetToken)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            if (string.IsNullOrWhiteSpace(patientId))
                throw new ArgumentException("Patient ID cannot be null or empty.", nameof(patientId));
            if (string.IsNullOrWhiteSpace(unHasedPassword))
                throw new ArgumentException("Password cannot be null or empty.", nameof(unHasedPassword));
            if (string.IsNullOrWhiteSpace(resetToken))
                throw new ArgumentException("Reset token cannot be null or empty.", nameof(resetToken));

            string subject = "Welcome to Healify!";
            string resetLink = $"http://localhost:4200/reset-password?token={resetToken}&patientId={patientId}";

            var body = $@"
    <p>Hello {name},</p>
    <p>Your Patient ID is: {patientId}</p>
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
                    var smtpPort = int.Parse(_configuration["Smtp:Port"]);
                    var smtpUsername = _configuration["Smtp:Username"];
                    var smtpPassword = _configuration["Smtp:Password"];
                    var enableSsl = bool.Parse(_configuration["Smtp:EnableSSL"]);

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




        // Update a patient
        public async Task<Patient> UpdatePatient(string id, Patient updatedPatient)
        {
            var patient = await _dbContext.Patients.FindAsync(id);
            if (patient == null) return null;

            patient.Name = updatedPatient.Name;
            patient.BloodGroup = updatedPatient.BloodGroup;
            patient.Email = updatedPatient.Email;
            patient.PhoneNumber = updatedPatient.PhoneNumber;

            if (!string.IsNullOrEmpty(updatedPatient.ImageBase64))
            {
                patient.Image = ConvertBase64ToBytes(updatedPatient.ImageBase64);
            }

            await _dbContext.SaveChangesAsync();
            return patient;
        }

        // Delete a patient
        public async Task<bool> DeletePatient(string id)
        {
            var patient = await _dbContext.Patients.FindAsync(id);
            if (patient == null) return false;

            _dbContext.Patients.Remove(patient);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private string GenerateNewId()
        {
            var lastPatient = _dbContext.Patients
                .OrderByDescending(p => p.PatientId)
                .FirstOrDefault();

            if (lastPatient == null)
            {
                return "P001";
            }

            try
            {
                var lastIdNumber = int.Parse(lastPatient.PatientId.Substring(1));
                var newIdNumber = lastIdNumber + 1;
                return $"P{newIdNumber:D3}";
            }
            catch (FormatException ex)
            {
                throw new Exception("Error while parsing the last patient ID", ex);
            }
        }
    }
}
