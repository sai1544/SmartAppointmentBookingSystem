using NUnit.Framework;
using SmartAppointmentBookingSystem.Models;
using SmartAppointmentBookingSystem.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartAppointmentBookingSystem.Tests
{
    #region Member 7

    [TestFixture]
    public class UnitTest1
    {
        private AppointmentManager _manager;
        private List<Appointment> _testAppointments;

        [SetUp]
        public void Setup()
        {
            _manager = new AppointmentManager();

            var client1 = new User(1, "Alice", "alice@mail.com");
            var client2 = new User(2, "Bob", "bob@mail.com");

            var pro1 = new Professional(1, "Dr. Smith", "smith@mail.com", "Cardiologist");
            var pro2 = new Professional(2, "Dr. Adams", "adams@mail.com", "Dentist");

            _testAppointments = new List<Appointment>
            {
                new Appointment(1, client1, pro1, DateTime.Today.AddDays(1)),
                new Appointment(2, client2, pro1, DateTime.Today.AddDays(2)),
                new Appointment(3, client1, pro2, DateTime.Today.AddDays(1)),
                new Appointment(4, client2, pro2, DateTime.Today.AddDays(3))
            };

            // Set internal _appointments field using reflection
            typeof(AppointmentManager)
                .GetField("_appointments", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                ?.SetValue(_manager, _testAppointments);
        }

        [Test]
        public void GetAppointmentsByDate_ShouldReturnAppointmentsForSpecificDate()
        {
            var dateToSearch = DateTime.Today.AddDays(1);
            var results = _manager.GetAppointmentsByDate(dateToSearch, _testAppointments).ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(a => a.AppointmentDate.Date == dateToSearch.Date));
        }

        

        [Test]
        public void GetAppointmentsByProfessionalName_ShouldReturnCorrectAppointments()
        {
            var results = _manager.GetAppointmentsByProfessionalName("Dr. Adams").ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(a => a.Professional.Name == "Dr. Adams"));
        }

        [Test]
        public void GetAppointmentsByProfessionalEmail_ShouldReturnCorrectAppointments()
        {
            var results = _manager.GetAppointmentsByProfessionalEmail("smith@mail.com").ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(a => a.Professional.Email == "smith@mail.com"));
        }

        [Test]
        public void SearchAppointmentsByClientName_ShouldReturnMatches()
        {
            var results = _manager.SearchAppointmentsByClientName("Alice").ToList();

            Assert.AreEqual(2, results.Count);
            Assert.IsTrue(results.All(a => a.Client.Name.Contains("Alice")));
        }


   

 
        [Test]
        public void SortAppointmentsByClientName_Ascending_ShouldWork()
        {
            var sorted = _manager.SortAppointmentsByClientName(true).ToList();

            for (int i = 1; i < sorted.Count; i++)
            {
                Assert.LessOrEqual(
                    string.Compare(sorted[i - 1].Client.Name, sorted[i].Client.Name, StringComparison.Ordinal),
                    0);
            }
        }

        [Test]
        public void SortAppointmentsByClientName_Descending_ShouldWork()
        {
            var sorted = _manager.SortAppointmentsByClientName(false).ToList();

            for (int i = 1; i < sorted.Count; i++)
            {
                Assert.GreaterOrEqual(
                    string.Compare(sorted[i - 1].Client.Name, sorted[i].Client.Name, StringComparison.Ordinal),
                    0);
            }
        }
    }


    #endregion
}
