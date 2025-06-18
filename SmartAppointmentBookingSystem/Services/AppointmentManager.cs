using SmartAppointmentBookingSystem.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartAppointmentBookingSystem.Services
{
    public class AppointmentManager
    {
        // Created by Sarath
        private List<Appointment> _appointments;

        
        public IEnumerable<Appointment> GetAppointmentsByDate(DateTime date, List<Appointment> allAppointments)
        {
            if (allAppointments == null)
                throw new ArgumentNullException(nameof(allAppointments), "Appointment list cannot be null.");

            return allAppointments.Where(a => a.AppointmentDate.Date == date.Date);
        }

        public IEnumerable<Appointment> GetAppointmentsByProfessionalName(string professionalName)
        {
            return _appointments.Where(a => a.Professional.Name.Equals(professionalName, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Appointment> GetAppointmentsByProfessionalEmail(string professionalEmail)
        {
            return _appointments.Where(a => a.Professional.Email.Equals(professionalEmail, StringComparison.OrdinalIgnoreCase));
        }
        private readonly string _appointmentsFilePath = "appointments.txt";

        //Created by Simran
        public async Task InitializeAsync()
        {
            _appointments = await LoadAppointmentsFromFileAsync();
        }

        public IEnumerable<Appointment> SearchAppointmentsByClientName(string keyword)
        {
            return _appointments
                .Where(a => a.Client.Name.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public IEnumerable<Appointment> FilterAppointmentsByStatus(string status)
        {
            return _appointments
                .Where(a => a.Status.Equals(status, StringComparison.OrdinalIgnoreCase));
        }

        public IEnumerable<Appointment> SortAppointmentsByDate(bool ascending = true)
        {
            return ascending
                ? _appointments.OrderBy(a => a.AppointmentDate)
                : _appointments.OrderByDescending(a => a.AppointmentDate);
        }

        public IEnumerable<Appointment> SortAppointmentsByClientName(bool ascending = true)
        {
            return ascending
                ? _appointments.OrderBy(a => a.Client.Name)
                : _appointments.OrderByDescending(a => a.Client.Name);
        }


        // Methods Are Created by Zamber sayali // Async And Await Are Created by Hemanth kumar reddy
        public async Task<Appointment> CreateAppointmentAsync(User client, Professional professional, DateTime appointmentDate)
        {
            try
            {
                if (client == null || professional == null)
                    throw new ArgumentNullException("Client and Professional must not be null.");

                if (appointmentDate <= DateTime.Now)
                    throw new ArgumentException("Appointment date must be in the future.");


                var newAppointment = new Appointment(
                    id: new Random().Next(1, 1000),
                    client: client,
                    professional: professional,
                    appointmentDate: appointmentDate
                );

                await SaveAppointmentToFileAsync(newAppointment);
                return newAppointment;
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        public async Task CancelAppointmentAsync(int appointmentId)
        {
            try
            {
                var appointments = await LoadAppointmentsFromFileAsync();
                var appointment = appointments.FirstOrDefault(a => a.Id == appointmentId);

                if (appointment == null)
                    throw new KeyNotFoundException("Appointment not found.");

                appointment.Cancel();
                await SaveAppointmentsToFileAsync(appointments);
            }
            catch (Exception ex)
            {
                LogError(ex);
                throw;
            }
        }

        private async Task SaveAppointmentToFileAsync(Appointment appointment)
        {
            try
            {
                var appointments = await LoadAppointmentsFromFileAsync();
                appointments.Add(appointment);
                await SaveAppointmentsToFileAsync(appointments);
            }
            catch (IOException ex)
            {
                LogError(ex);
                throw new ApplicationException("Failed to save appointment data.", ex);
            }
        }

        private async Task SaveAppointmentsToFileAsync(List<Appointment> appointments)
        {
            try
            {
                using (var writer = new StreamWriter(_appointmentsFilePath, false, Encoding.UTF8))
                {
                    foreach (var appointment in appointments)
                    {
                        await writer.WriteLineAsync($"{appointment.Id},{appointment.Client.Name},{appointment.Professional.Name},{appointment.AppointmentDate},{appointment.Status}");
                    }
                }
            }
            catch (IOException ex)
            {
                LogError(ex);
                throw new ApplicationException("Failed to write appointments to file.", ex);
            }
        }

        public async Task<List<Appointment>> LoadAppointmentsFromFileAsync()
        {
            try
            {
                if (!File.Exists(_appointmentsFilePath))
                    return new List<Appointment>();

                var appointments = new List<Appointment>();
                using (var reader = new StreamReader(_appointmentsFilePath, Encoding.UTF8))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        var parts = line.Split(',');

                        if (parts.Length == 5)
                        {
                            appointments.Add(new Appointment(
                                id: int.Parse(parts[0]),
                                client: new User(0, parts[1], ""),
                                professional: new Professional(0, parts[2], "", ""),
                                appointmentDate: DateTime.Parse(parts[3])
                            ));
                        }
                    }
                }
                return appointments;
            }
            catch (IOException ex)
            {
                LogError(ex);
                throw new ApplicationException("Failed to read appointments from file.", ex);
            }
        }

        private void LogError(Exception ex)
        {

            Console.WriteLine($"Error: {ex.Message}");
        }

        internal void CreateAppointmentAsync()
        {
            throw new NotImplementedException();
        }

    }
}