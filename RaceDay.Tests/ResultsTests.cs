
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using RaceDay.Api.DTOs;
using Xunit;

namespace RaceDay.Tests
{
    public class ResultsTests
    {
        [Fact]
        public void ResultDto_WithValidData_PassesValidation()
        {
            var dto = new ResultDto
            {
                EnrolmentID = 1,
                FinishPosition = 1,
                FinishTime = new TimeSpan(0, 45, 30),
                ResultStatus = "Finished"
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(dto);

            Validator.TryValidateObject(
                dto,
                context,
                results,
                validateAllProperties: true);

            Assert.Empty(results);
        }

        [Fact]
        public void ResultDto_WithZeroEnrolmentID_FailsValidation()
        {
            var dto = new ResultDto
            {
                EnrolmentID = 0,
                FinishPosition = 1,
                FinishTime = new TimeSpan(0, 45, 30),
                ResultStatus = "Finished"
            };

            var results = new List<ValidationResult>();
            var context = new ValidationContext(dto);

            Validator.TryValidateObject(
                dto,
                context,
                results,
                validateAllProperties: true);

            Assert.NotEmpty(results);
        }
    }
}
