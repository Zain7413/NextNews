using Microsoft.EntityFrameworkCore;
using NextNews.Data;
using NextNews.Models.Database;
using System;

namespace NextNews.Services
{
    public class ContactService : IContactService
    {
        private readonly ApplicationDbContext _context;

        public ContactService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void SaveContactMessage(ContactFormMessage obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            try
            {
                _context.ContactFormMessages.Add(obj);
                _context.SaveChanges();
            }
            catch (DbUpdateException ex)
            {
                // Log or handle the exception
                // You can also log the details of the exception for debugging purposes
                Console.WriteLine($"Error saving contact message: {ex.Message}");
                // Optionally rethrow the exception if needed
                throw;
            }
        }
    }
}
