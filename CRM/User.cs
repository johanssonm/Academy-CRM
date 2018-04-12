using System;
using System.Collections.Generic;

namespace CRM
{
    class User
    {
        protected string firstName { get; set; }
        protected string lastName { get; set; }
        protected string email { get; set; }

        public User()
        {
            
        }

        public User(string firstname, string lastname, string getmail)
        {

            firstName = firstname;
            lastName = lastname;
            email = getmail;

        }
    }

    class Customer : User
    {
        public enum Type
        {
            Lead,
            Prospect,
            Customer
        }

        public Type CustomerType { get; set; } 
        public List<string> PhoneNumber { get; set; }
        public string FirstName { get => firstName; set => firstName = value; }
        public string LastName { get => lastName; set => lastName = value; }
        public string Email { get => email; set => email = value; }

        public Customer()
        {
            
        }

        public Customer(string firstname, string lastname, string email, List<string> phonenumber, Type type) : base(firstname,lastname,email)
        {
            PhoneNumber = phonenumber;
            CustomerType = Type.Lead;

        }


            
        

    }
}