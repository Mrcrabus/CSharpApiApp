using Microsoft.EntityFrameworkCore;
using WebApplication1.Dto;
using WebApplication1.Helpers;
using WebApplication1.Model;

namespace WebApplication1.Services;

public class UserService
{
    private readonly DataContext _db;

    public UserService(DataContext context)
    {
        _db = context;
    }

    public async Task<User[]> GetUsers()
    {
        User[] users = await _db.Users.ToArrayAsync();

        return users;
    }

    public async Task<User> GetUserById(int id)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return user;
    }

    public async Task<User> GetUserByEmail(string email)
    {
        User? user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

        if (user == null) return null;

        return user;
    }

    public async Task<User> CreateUser(User user)
    {
        User? userExist = await this.GetUserByEmail(user.Email);

        if (userExist != null) return null;
        
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync();
        User newUser = await this.GetUserByEmail(user.Email);

        return newUser;
    }
}