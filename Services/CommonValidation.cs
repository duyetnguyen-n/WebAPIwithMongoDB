using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebAPIwithMongoDB.Services
{
    public class CommonValidation
    {
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            
            return Regex.IsMatch(phoneNumber, @"^\d{10}$");
        }

        public static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}