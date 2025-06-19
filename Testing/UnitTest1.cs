using SmartAppointmentBookingSystem.Models;
using SmartAppointmentBookingSystem.Services;

namespace Testing
{
    #region Member 5

    public class Tests
    {
        private AppointmentManager _manager;
        [SetUp]
        public void Setup()
        {
            _manager = new AppointmentManager();
        }

        [Test]
        public async Task CreateAppointmentAsync_ValidData_ReturnsAppointment()
        {
            
            var client = new User(1, "John Doe", "john.doe@example.com");
            var professional = new Professional(2, "Dr. Smith", "dr.smith@example.com", "Dentist");
            var appointmentDate = DateTime.Now.AddDays(1);

          
            var appointment = await _manager.CreateAppointmentAsync(client, professional, appointmentDate);

           
            Assert.IsNotNull(appointment);
            Assert.AreEqual("Scheduled", appointment.Status);
        }
        [Test]
        public async Task CancelAppointmentAsync_ValidId_ChangesStatusToCancelled()
        {
            
            var client = new User(1, "John Doe", "john.doe@example.com");
            var professional = new Professional(2, "Dr. Smith", "dr.smith@example.com", "Dentist");
            var appointmentDate = DateTime.Now.AddDays(1);

            var appointment = await _manager.CreateAppointmentAsync(client, professional, appointmentDate);

            await _manager.CancelAppointmentAsync(appointment.Id);

            
            Assert.AreEqual("Scheduled", appointment.Status);
        }
        [Test]
        public async Task LoadAppointmentsFromFileAsync_FileExists_ReturnsAppointments()
        {
          
            var appointments = await _manager.LoadAppointmentsFromFileAsync();

            Assert.IsNotNull(appointments);
            Assert.IsTrue(appointments.Count > 0);
        }
    }


    #endregion
}