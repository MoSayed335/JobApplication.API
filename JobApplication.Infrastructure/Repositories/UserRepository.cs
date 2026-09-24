using JobApplication.Application.Interfaces.Auth;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;
    public UserRepository(ApplicationDbContext context) => _context = context;

    public Task<User?> GetByEmailAsync(string email)
        => _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<bool> EmailExistsAsync(string email)
        => _context.Users.AnyAsync(u => u.Email == email);

    public async Task InsertAsync(User user) => await _context.Users.AddAsync(user);
    public async Task InsertCandidateAsync(Candidate c) => await _context.Candidates.AddAsync(c);
    public async Task InsertRecruiterAsync(Recruiter r) => await _context.Recruiters.AddAsync(r);
    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}