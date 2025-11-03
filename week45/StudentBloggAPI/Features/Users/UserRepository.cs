using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentBloggAPI.Data;
using StudentBloggAPI.Features.Users.Interfaces;

namespace StudentBloggAPI.Features.Users;

public class UserRepository(
    StudentBloggDbContext dbContext, 
    ILogger<UserRepository> logger) : IUserRepository
{
    private readonly StudentBloggDbContext _dbContext = dbContext;
    private readonly ILogger<UserRepository> _logger = logger;
    
    public async Task<User?> AddAsync(User entity)
    {
       _dbContext.Users.Add(entity);
       await _dbContext.SaveChangesAsync();
       return entity;
    }

    public async Task<User?> UpdateAsync(Guid id, User entity)
    {
        // sjekk om user finnes
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) return null;
        
        user.FirstName = string.IsNullOrEmpty(user.FirstName) ? entity.FirstName: user.FirstName;
        user.LastName = string.IsNullOrEmpty(user.LastName) ? entity.LastName: user.LastName;
        user.Email = string.IsNullOrEmpty(user.Email) ? entity.Email: user.Email;
        user.UserName = string.IsNullOrEmpty(user.UserName) ? entity.UserName: user.UserName;
        user.UpdatedAt = DateTime.UtcNow;
        
        // oppdater user
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<User?> GetByIdAsync(Guid id)
    {
        return await _dbContext.Users.FindAsync(id);
    }

    public async Task<User?> DeleteByIdAsync(Guid id)
    {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) return null;
        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return user;
    }

    public async Task<IEnumerable<User>> GetPagedAsync(int pageNumber, int pageSize)
    {
        int skip = (pageNumber - 1) * pageSize;
        
        // få total count av users
        int totalUsers = await _dbContext.Users.CountAsync();
        
        if (totalUsers == 0) return [];
        
        return await _dbContext.Users
            .OrderBy(u => u.Id)
            .Skip(skip) // start
            .Take(pageSize) // antall fra start
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        return await _dbContext.Users.Where(predicate).ToListAsync();
    }
}