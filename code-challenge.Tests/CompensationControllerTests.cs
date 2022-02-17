using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;
using System;
using System.Net;
using System.Net.Http;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;
        private static DateTime _utcNow;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
            _utcNow = DateTime.UtcNow;
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            double salary = 1000.50;

            var employee = new Employee()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
            };

            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = salary,
                EffectiveDate = _utcNow
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            
            Assert.IsNotNull(newCompensation);
            Assert.AreEqual(newCompensation.Employee.EmployeeId, employee.EmployeeId);
            Assert.AreEqual(compensation.EffectiveDate, _utcNow);
            Assert.AreEqual(compensation.Salary, salary);
        }

        [TestMethod]
        public void CreateCompensation_InvalidEmployeeID_Returns_Error()
        {
            // Arrange
            double salary = 1000;

            var employee = new Employee()
            {
                EmployeeId = "some_invalid_id"
            };

            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = salary,
                EffectiveDate = _utcNow
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Negative_Salary_Returns_Error()
        {
            // Arrange
            double salary = -500;

            var employee = new Employee()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
            };

            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = salary,
                EffectiveDate = _utcNow
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Zero_Salary_Returns_Error()
        {
            // Can a salary be 0? Might not need this if that's valid

            // Arrange
            double salary = 0;

            var employee = new Employee()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
            };

            var compensation = new Compensation()
            {
                Employee = employee,
                Salary = salary,
                EffectiveDate = _utcNow
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [TestMethod]
        public void GeCompensationByEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedSalary = 1000.50;
            var expectedEffectiveDate = _utcNow;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            var compensation = response.DeserializeContent<Compensation>();

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);    
            Assert.AreEqual(employeeId, compensation.Employee.EmployeeId);
            Assert.AreEqual(expectedSalary, compensation.Salary);
            Assert.AreEqual(expectedEffectiveDate, _utcNow);
        }

        [TestMethod]
        public void GetCompensation_Returns_NotFound()
        {
            // Arrange
            var employeeId = "some_invalid_id";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        //[TestMethod]
        // May not may not need this, depending on requirements
        //public void GetCompensation_AlreadyExists_Errors()
        //{
        //    // Arrange
        //    double salary = 1000.50;

        //    var employee = new Employee()
        //    {
        //        EmployeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f"
        //    };

        //    var compensation = new Compensation()
        //    {
        //        Employee = employee,
        //        Salary = salary,
        //        EffectiveDate = _utcNow
        //    };

        //    var requestContent = new JsonSerialization().ToJson(employee);

        //    // Execute
        //    var postRequestTask = _httpClient.PostAsync("api/compensation",
        //       new StringContent(requestContent, Encoding.UTF8, "application/json"));
        //    var response = postRequestTask.Result;

        //    // Assert
        //    Assert.AreEqual(HttpStatusCode.InternalServerError, response.StatusCode);    
        //}
    }
}
