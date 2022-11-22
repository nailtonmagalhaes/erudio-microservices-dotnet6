﻿using GeekShopping.Email.Model.Context;
using GeekShopping.Email.Messages;
using Microsoft.EntityFrameworkCore;
using GeekShopping.Email.Model;

namespace GeekShopping.Email.Repository;

public class EmailRepository : IEmailRepository
{
    private readonly DbContextOptions<MySQLContext> _context;

    public EmailRepository(DbContextOptions<MySQLContext> context)
    {
        _context = context;
    }

    public async Task LogEmail(UpdatePaymentResultMessage message)
    {
        EmailLog emailLog = new()
        {
            Email = message.Email,
            SentDate = DateTime.Now,
            Log = $"Order - {message.OrderId} has been created successfuly!",
        };
        await using var _db = new MySQLContext(_context);
        _db.Emails.Add(emailLog);
        await _db.SaveChangesAsync();
    }
}
