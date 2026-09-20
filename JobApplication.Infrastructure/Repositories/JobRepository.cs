// Infrastructure/Repositories/JobRepository.cs
using JobApplication.Application.Interfaces;
using JobApplication.Domain.Entities;
using JobApplication.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JobApplication.Infrastructure.Repositories;

public class JobRepository : IJobRepository
{
    private readonly ApplicationDbContext _context;
    public JobRepository(ApplicationDbContext context) => _context = context;

    public async Task InsertAsync(Job job) => await _context.Jobs.AddAsync(job);

    public void Update(Job job) => _context.Jobs.Update(job);

    public void Remove(Job job) => _context.Jobs.Remove(job);

    public IQueryable<Job> Get() => _context.Jobs.AsNoTracking();

    // Tracked عن قصد عشان التعديل يبقى على الأعمدة اللي اتغيرت بس
    public Task<Job?> GetByIdAsync(int id)
        => _context.Jobs.FirstOrDefaultAsync(j => j.Id == id);

    public Task SaveChangesAsync() => _context.SaveChangesAsync();
}